namespace OltreIlTempo.Domain;

/// <summary>
/// Rappresenta lo stato completo di una partita in corso.
/// Sofia — questo è il file critico condiviso da tutti.
/// </summary>
public class GameState
{
    public string CurrentRoomId { get; set; } = string.Empty;
    public int TurnNumber { get; set; } = 0;
    public int Suspicion { get; set; } = 0;
    public int CurrentLevel { get; set; } = 1;
    
    // Inventario
    public List<string> InventoryItemIds { get; set; } = new();
    
    // Flag dinamici (es. "visited_kitchen", "has_key_A")
    public Dictionary<string, bool> Flags { get; set; } = new();
    
    // Trust dei NPC (npc_id -> trust value 0-100)
    public Dictionary<string, int> NpcTrust { get; set; } = new();
    
    // NPC runtime state (npc_id -> state)
    public Dictionary<string, NpcRuntimeState> NpcStates { get; set; } = new();
    
    // Item runtime state (item_id -> state)
    public Dictionary<string, ItemRuntimeState> ItemStates { get; set; } = new();
    
    // Indizi trovati (clue_id -> true)
    public Dictionary<string, bool> FoundClues { get; set; } = new();
    
    // Achievement sbloccati (achievement_id -> unlock time in turn)
    public Dictionary<string, int> UnlockedAchievements { get; set; } = new();
    
    // Stanze sbloccate (room_id -> true)
    public Dictionary<string, bool> UnlockedRooms { get; set; } = new();
    
    // Deduzioni sbloccate (deduction_id -> true)
    public Dictionary<string, bool> ResolvedDeductions { get; set; } = new();
    
    // Numero di oggetti futuri esaminati
    public int FutureObjectsShown { get; set; } = 0;
}

/// <summary>
/// Stato runtime di un NPC durante la partita.
/// </summary>
public class NpcRuntimeState
{
    public string NpcId { get; set; } = string.Empty;
    public int Trust { get; set; } = 0;
    public string CurrentRoomId { get; set; } = string.Empty;
    
    // Opzioni dialogo già utilizzate (non ripetibili)
    public HashSet<string> UsedDialogueOptions { get; set; } = new();
}

/// <summary>
/// Stato runtime di un oggetto durante la partita.
/// </summary>
public class ItemRuntimeState
{
    public string ItemId { get; set; } = string.Empty;
    
    // Dove si trova: "room_{roomId}", "inventory", "container_{containerId}"
    public string Location { get; set; } = string.Empty;
    
    // Se è in un contenitore, se il contenitore è aperto
    public bool ContainerOpen { get; set; } = false;
    
    // Pagine lette per un documento (numero pagina -> true)
    public Dictionary<int, bool> PagesRead { get; set; } = new();
    
    // Se è un oggetto futuro, quante volte è stato esaminato
    public int TimesSeen { get; set; } = 0;
}

/// <summary>
/// Risultato di un comando eseguito.
/// Racchiude le righe di output con tipo e stile.
/// </summary>
public class CommandResult
{
    public List<OutputLine> Lines { get; set; } = new();
    public bool IsValid { get; set; } = true;
    public string ErrorMessage { get; set; } = string.Empty;
    
    public void AddLine(string text, OutputLineType type = OutputLineType.Normal)
    {
        Lines.Add(new OutputLine { Text = text, Type = type });
    }
    
    public void AddError(string message)
    {
        IsValid = false;
        ErrorMessage = message;
        AddLine(message, OutputLineType.Error);
    }
}

public class OutputLine
{
    public string Text { get; set; } = string.Empty;
    public OutputLineType Type { get; set; } = OutputLineType.Normal;
}

public enum OutputLineType
{
    Normal,
    Success,
    Warning,
    Error,
    Subtle  // Output meno importante, es. "...caricando..."
}

/// <summary>
/// Comando parsato e pronto per l'esecuzione.
/// </summary>
public class ParsedCommand
{
    public string Verb { get; set; } = string.Empty;
    public List<string> Args { get; set; } = new();
    public bool IsValid { get; set; } = true;
    
    /// <summary>
    /// Suggerimento di comando se il verb non è riconosciuto (distanza Levenshtein).
    /// </summary>
    public string? Suggestion { get; set; }
}

/// <summary>
/// Configurazione del gioco (da config.json).
/// </summary>
public class GameConfig
{
    public double MaxInventoryWeight { get; set; } = 15.0;
    public int MaxTrust { get; set; } = 100;
    public int MaxSuspicion { get; set; } = 100;
    public int AutosaveFrequency { get; set; } = 10; // ogni N turni
}
