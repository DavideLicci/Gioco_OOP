namespace OltreIlTempo.Engine;

using OltreIlTempo.Domain;

/// <summary>
/// Motore principale del gioco (Command Pattern).
/// DADDA: implementa la logica centrale del gioco.
/// Coordina tutti gli altri servizi e gestisce i comandi.
/// </summary>
public partial class GameEngine : IGameEngine
{
    private GameState _state = new();
    private GameContent _content = new();
    private GameConfig _config = new();
    private readonly ICommandParser _parser;
    private readonly IConditionEvaluator _evaluator;
    
    // Servizi opzionali (implementati da altri)
    private ISaveService? _saveService;
    private IContentLoader? _contentLoader;
    private IProgressionService? _progressionService;
    private IDialogueSystem? _dialogueSystem;
    private IInventorySystem? _inventorySystem;
    private IAchievementService? _achievementService;
    private IDeductionService? _deductionService;
    
    // Stato del dialogo attivo (per il flusso parla → scegli)
    private string? _activeDialogueNpcId;
    private List<DialogueOptionDefinition>? _activeDialogueOptions;
    
    // Mappe dei comandi (Command Pattern)
    private Dictionary<string, Func<List<string>, CommandResult>> _commandDispatcher = new();
    
    public GameEngine(
        GameContent content, 
        GameConfig config,
        ICommandParser parser,
        IConditionEvaluator evaluator)
    {
        _content = content;
        _config = config;
        _parser = parser;
        _evaluator = evaluator;
        _state = InitializeGameState(content, config);
        
        // Registra i comandi disponibili
        RegisterCommands();
    }
    
    /// <summary>
    /// Registra i servizi opzionali di altri moduli.
    /// </summary>
    public void RegisterServices(
        ISaveService? saveService = null,
        IContentLoader? contentLoader = null,
        IProgressionService? progressionService = null,
        IDialogueSystem? dialogueSystem = null,
        IInventorySystem? inventorySystem = null,
        IAchievementService? achievementService = null,
        IDeductionService? deductionService = null)
    {
        _saveService = saveService;
        _contentLoader = contentLoader;
        _progressionService = progressionService;
        _dialogueSystem = dialogueSystem;
        _inventorySystem = inventorySystem;
        _achievementService = achievementService;
        _deductionService = deductionService;
    }
    
    /// <summary>
    /// Registra i comandi disponibili nel dispatcher (Command Pattern).
    /// </summary>
    private void RegisterCommands()
    {
        _commandDispatcher = new Dictionary<string, Func<List<string>, CommandResult>>
        {
            // Movimento
            { "north", args => ExecuteMove("n") },
            { "south", args => ExecuteMove("s") },
            { "east", args => ExecuteMove("e") },
            { "west", args => ExecuteMove("o") },
            { "forward", args => ExecuteMove("n") },
            
            // Azioni
            { "take", ExecuteCollect },
            { "collect", ExecuteCollect },
            { "drop", ExecuteDrop },
            { "talk", ExecuteTalk },
            { "ask", ExecuteTalk },
            { "examine", ExecuteExamine },
            { "look", args => ExecuteLook() },
            { "read", ExecuteRead },
            { "use", ExecuteUse },
            { "open", ExecuteOpen },
            { "close", ExecuteClose },
            { "choose", ExecuteChoose },
            
            // Inventario e status
            { "inventory", args => ExecuteInventory(null) },
            { "status", args => ExecuteStatus() },
            
            // Sistema di gioco
            { "board", args => ExecuteBoard() },
            { "deduce", args => ExecuteDeduce() },
            { "achievements", args => ExecuteAchievements() },
            { "quest", args => ExecuteQuest() },
            { "map", args => ExecuteMap() },
            { "save", ExecuteSave },
            { "load", ExecuteLoad },
            { "help", args => ExecuteHelp() },
            { "quit", args => ExecuteQuit() },
            { "exit", args => ExecuteQuit() }
        };
    }
    
    public CommandResult Execute(string input)
    {
        var result = new CommandResult();
        
        if (string.IsNullOrWhiteSpace(input))
        {
            result.AddError("Comando vuoto.");
            return result;
        }
        
        var parsed = _parser.Parse(input);
        
        if (!parsed.IsValid)
        {
            if (!string.IsNullOrEmpty(parsed.Suggestion))
            {
                result.AddError($"Comando '{parsed.Verb}' non riconosciuto. Intendevi '{parsed.Suggestion}'?");
            }
            else
            {
                result.AddError($"Comando '{parsed.Verb}' non riconosciuto.");
            }
            result.AddLine("Digita 'aiuto' per una lista di comandi disponibili.", OutputLineType.Subtle);
            return result;
        }
        
        try
        {
            // Dispatch al comando appropriato
            if (_commandDispatcher.TryGetValue(parsed.Verb, out var commandHandler))
            {
                result = commandHandler(parsed.Args);
            }
            else
            {
                result.AddError($"Comando '{parsed.Verb}' non gestito.");
            }
            
            // Finalizza il turno (incrementa turno, progressione, achievement)
            if (result.IsValid && parsed.Verb != "help" && parsed.Verb != "status" && parsed.Verb != "inventory")
            {
                FinalizeTurn(result);
            }
        }
        catch (Exception ex)
        {
            result.AddError($"Errore durante l'esecuzione del comando: {ex.Message}");
        }
        
        return result;
    }
    
    public GameState GetGameState() => _state;
    
    public string DescribeCurrentRoom()
    {
        if (!_content.Rooms.TryGetValue(_state.CurrentRoomId, out var room))
        {
            return "Sei in una stanza sconosciuta.";
        }
        
        bool alreadyVisited = _state.UnlockedRooms.GetValueOrDefault(_state.CurrentRoomId, false);
        
        var sb = new System.Text.StringBuilder();
        
        // ASCII Art della stanza
        sb.AppendLine(AsciiArtProvider.GetRoomArt(_state.CurrentRoomId));
        sb.AppendLine(AsciiArtProvider.GetThinSeparator());
        
        // Titolo della stanza
        sb.AppendLine(AsciiArtProvider.GetRoomTitle(room.Name));
        sb.AppendLine(AsciiArtProvider.GetThinSeparator());
        
        // Descrizione
        sb.AppendLine(alreadyVisited 
            ? $"  Sei tornato qui. {room.Description}" 
            : $"  {room.Description}");
        sb.AppendLine();
        
        // Aggiungi oggetti nella stanza
        var items = room.ItemIds
            .Where(id => _content.Items.ContainsKey(id))
            .Select(id => _content.Items[id].Name)
            .ToList();
            
        if (items.Any())
        {
            sb.AppendLine($"  🔍 Oggetti: {string.Join(", ", items)}");
        }
        
        // Aggiungi NPC nella stanza
        var npcs = room.NpcIds
            .Where(id => _content.Npcs.ContainsKey(id))
            .Select(id => _content.Npcs[id].Name)
            .ToList();
            
        if (npcs.Any())
        {
            sb.AppendLine($"  👤 Persone: {string.Join(", ", npcs)}");
        }
        
        // Bussola con uscite
        sb.AppendLine();
        sb.AppendLine(AsciiArtProvider.GetCompass(room.Exits));
        
        return sb.ToString();
    }
    
    // ==================== Logica di Turno ====================
    
    private void FinalizeTurn(CommandResult result)
    {
        _state.TurnNumber++;
        
        // Controlla la progressione del livello
        if (_progressionService != null)
        {
            if (_progressionService.CheckLevelComplete(_state, _content))
            {
                result.AddLine(AsciiArtProvider.GetLevelComplete(), OutputLineType.Success);
                result.AddLine($"🎉 Hai completato il livello {_state.CurrentLevel - 1}!", OutputLineType.Success);
                result.AddLine($"Sei passato al livello {_state.CurrentLevel}.", OutputLineType.Success);
            }
        }
        
        // Valuta nuovi achievement
        if (_achievementService != null)
        {
            var newUnlocks = _achievementService.EvaluateNewUnlocks(_state, _content);
            foreach (var unlock in newUnlocks)
            {
                result.AddLine(unlock, OutputLineType.Success);
            }
        }
        
        // Autosave
        if (_saveService != null && _state.TurnNumber % _config.AutosaveFrequency == 0)
        {
            try
            {
                _saveService.Save(_state, "autosave.json");
            }
            catch { /* Silenzio gli errori di autosave */ }
        }
    }
    
    private GameState InitializeGameState(GameContent content, GameConfig config)
    {
        var state = new GameState
        {
            CurrentRoomId = content.StartingRoomId,
            TurnNumber = 0,
            Suspicion = 0,
            CurrentLevel = 1
        };
        
        // Inizializza il trust dei NPC
        foreach (var npc in content.Npcs)
        {
            state.NpcTrust[npc.Key] = 0;
            state.NpcStates[npc.Key] = new NpcRuntimeState
            {
                NpcId = npc.Key,
                CurrentRoomId = npc.Value.RoomId,
                Trust = 0
            };
        }
        
        // Inizializza gli item
        foreach (var item in content.Items)
        {
            var room = content.Rooms.Values.FirstOrDefault(r => r.ItemIds.Contains(item.Key));
            state.ItemStates[item.Key] = new ItemRuntimeState
            {
                ItemId = item.Key,
                Location = room != null ? $"room_{room.Id}" : "unknown"
            };
        }
        
        // Inizializza le stanze sbloccate
        foreach (var room in content.Rooms)
        {
            state.UnlockedRooms[room.Key] = string.IsNullOrEmpty(room.Value.AccessRequiredFlag);
        }
        
        return state;
    }
}
