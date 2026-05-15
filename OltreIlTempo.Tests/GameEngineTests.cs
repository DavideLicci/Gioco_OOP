using OltreIlTempo.Engine;
using OltreIlTempo.Domain;
using Xunit;

namespace OltreIlTempo.Tests;

public class GameEngineTests
{
    [Fact]
    public void GameEngine_InitialState_IsValid()
    {
        // Arrange
        var config = new GameConfig();
        var content = new GameContent
        {
            StartingRoomId = "room_kitchen",
            Rooms = new Dictionary<string, RoomDefinition>
            {
                { "room_kitchen", new RoomDefinition { Id = "room_kitchen", Name = "Kitchen" } }
            }
        };
        
        // Act
        var engine = new GameEngine(content, config);
        var state = engine.GetGameState();
        
        // Assert
        Assert.Equal("room_kitchen", state.CurrentRoomId);
        Assert.Equal(0, state.TurnNumber);
        Assert.Equal(1, state.CurrentLevel);
    }
    
    [Fact]
    public void CommandParser_ParsesSimpleCommand()
    {
        // Arrange
        var parser = new CommandParser();
        
        // Act
        var result = parser.Parse("north");
        
        // Assert
        Assert.True(result.IsValid);
        Assert.Equal("north", result.Verb);
    }

    [Fact]
    public void CommandParser_ParsesNaturalItalianMovement()
    {
        // Arrange
        var parser = new CommandParser();
        
        // Act
        var result = parser.Parse("vai a nord");
        
        // Assert
        Assert.True(result.IsValid);
        Assert.Equal("north", result.Verb);
        Assert.Empty(result.Args);
    }

    [Fact]
    public void CommandParser_RemovesNaturalItalianFillers()
    {
        // Arrange
        var parser = new CommandParser();
        
        // Act
        var talk = parser.Parse("parla con Chef Marco");
        var collect = parser.Parse("raccogli la nota");
        var use = parser.Parse("usa la chiave su la porta");
        
        // Assert
        Assert.Equal("talk", talk.Verb);
        Assert.Equal(new[] { "Chef", "Marco" }, talk.Args);
        Assert.Equal("collect", collect.Verb);
        Assert.Equal(new[] { "nota" }, collect.Args);
        Assert.Equal("use", use.Verb);
        Assert.Equal(new[] { "chiave", "su", "porta" }, use.Args);
    }

    [Fact]
    public void GameEngine_InfoCommands_DoNotAdvanceTurn()
    {
        // Arrange
        var engine = CreateTwoRoomEngine();
        
        // Act
        engine.Execute("mappa");
        engine.Execute("missione");
        engine.Execute("guarda");
        
        // Assert
        Assert.Equal(0, engine.GetGameState().TurnNumber);
    }

    [Fact]
    public void GameEngine_Movement_AdvancesTurnAndMarksVisitedRoom()
    {
        // Arrange
        var engine = CreateTwoRoomEngine();
        var evaluator = new ConditionEvaluator();
        
        // Act
        engine.Execute("vai a nord");
        var state = engine.GetGameState();
        
        // Assert
        Assert.Equal(1, state.TurnNumber);
        Assert.True(state.Flags.GetValueOrDefault("visited_hallway", false));
        Assert.True(state.Flags.GetValueOrDefault("visited_room_hallway", false));
        Assert.True(evaluator.Evaluate("visited_room:visited_hallway", state));
        Assert.True(evaluator.Evaluate("visited_room:room_hallway", state));
    }

    [Fact]
    public void GameEngine_FirstRoomDescription_IsNotMarkedAsReturn()
    {
        // Arrange
        var engine = CreateTwoRoomEngine();
        
        // Act
        var firstDescription = engine.DescribeCurrentRoom();
        var secondDescription = engine.DescribeCurrentRoom();
        
        // Assert
        Assert.DoesNotContain("Sei tornato qui.", firstDescription);
        Assert.Contains("Sei tornato qui.", secondDescription);
    }

    [Fact]
    public void GameEngine_AppliesDynamicAliasesFromContent()
    {
        // Arrange
        var engine = CreateTwoRoomEngine(content =>
        {
            content.CommandAliases.Add(new CommandAliasDefinition
            {
                Alias = "settentrione",
                ResolvedCommand = "nord"
            });
        });
        
        // Act
        engine.Execute("settentrione");
        
        // Assert
        Assert.Equal("room_hallway", engine.GetGameState().CurrentRoomId);
        Assert.Equal(1, engine.GetGameState().TurnNumber);
    }

    [Fact]
    public void GameEngine_OtherCommandClearsActiveDialogue()
    {
        // Arrange
        var engine = CreateDialogueEngine();
        
        // Act
        engine.Execute("parla con Marco");
        engine.Execute("vai a nord");
        var chooseAfterMove = engine.Execute("scegli 1");
        
        // Assert
        Assert.Contains(
            chooseAfterMove.Lines,
            line => line.Text.Contains("Non c'è un dialogo attivo"));
    }
    
    [Fact]
    public void InventorySystem_Add_IncreaseWeight()
    {
        // Arrange
        var inventory = new InventorySystem();
        var state = new GameState();
        var content = new GameContent
        {
            Config = new GameConfig { MaxInventoryWeight = 10.0 },
            Items = new Dictionary<string, ItemDefinition>
            {
                { "item_knife", new ItemDefinition { Id = "item_knife", Name = "Knife", Weight = 0.5 } }
            }
        };
        
        // Act
        var result = inventory.Add("item_knife", state, content);
        var weight = inventory.GetTotalWeight(state, content);
        
        // Assert
        Assert.True(result);
        Assert.Equal(0.5, weight);
        Assert.Contains("item_knife", state.InventoryItemIds);
    }
    
    [Fact]
    public void DialogueSystem_OptionFiltering()
    {
        // Arrange
        var dialogue = new DialogueSystem();
        var state = new GameState { Suspicion = 50 };
        var npc = new NpcDefinition
        {
            Id = "npc_test",
            DialogueOptions = new List<DialogueOptionDefinition>
            {
                new DialogueOptionDefinition
                {
                    Id = "opt_1",
                    Prompt = "Hello",
                    Response = "Hi",
                    MinSuspicion = 0,
                    MaxSuspicion = 100,
                    Repeatable = true
                }
            }
        };
        
        // Act
        var options = dialogue.StartDialogue(npc, state);
        
        // Assert
        Assert.Single(options);
    }

    private static GameEngine CreateTwoRoomEngine(Action<GameContent>? configureContent = null)
    {
        var content = new GameContent
        {
            StartingRoomId = "room_kitchen",
            Rooms = new Dictionary<string, RoomDefinition>
            {
                {
                    "room_kitchen",
                    new RoomDefinition
                    {
                        Id = "room_kitchen",
                        Name = "Kitchen",
                        Description = "A small kitchen.",
                        Exits = new Dictionary<string, string> { { "n", "room_hallway" } }
                    }
                },
                {
                    "room_hallway",
                    new RoomDefinition
                    {
                        Id = "room_hallway",
                        Name = "Hallway",
                        Description = "A quiet hallway.",
                        Exits = new Dictionary<string, string> { { "s", "room_kitchen" } }
                    }
                }
            }
        };

        configureContent?.Invoke(content);

        return new GameEngine(content, content.Config);
    }

    private static GameEngine CreateDialogueEngine()
    {
        var engine = CreateTwoRoomEngine(content =>
        {
            content.Rooms["room_kitchen"].NpcIds.Add("npc_marco");
            content.Npcs["npc_marco"] = new NpcDefinition
            {
                Id = "npc_marco",
                Name = "Marco",
                RoomId = "room_kitchen",
                DialogueOptions = new List<DialogueOptionDefinition>
                {
                    new()
                    {
                        Id = "opt_marco_1",
                        Prompt = "Ciao Marco.",
                        Response = "Ciao.",
                        Repeatable = true
                    }
                }
            };
        });

        engine.RegisterServices(dialogueSystem: new DialogueSystem());
        return engine;
    }
}
