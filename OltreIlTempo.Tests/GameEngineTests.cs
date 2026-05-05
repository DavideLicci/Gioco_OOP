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
}
