namespace OltreIlTempo.Domain;

/// <summary>
/// Definizione di un NPC dal JSON.
/// Valkiria creerà questa classe.
/// </summary>
public class NpcDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string RoomId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;  // es. "suspect", "witness", "expert"
    public bool IsRedHerring { get; set; } = false;
    
    // Estrategie di dialogo: "friendly", "suspicious", "neutral"
    public string DialogueStrategy { get; set; } = "neutral";
    
    public List<DialogueOptionDefinition> DialogueOptions { get; set; } = new();
}

/// <summary>
/// Definizione di un'opzione di dialogo.
/// </summary>
public class DialogueOptionDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;  // Domanda da porre
    public string Response { get; set; } = string.Empty;  // Risposta dell'NPC
    
    public int TrustDelta { get; set; } = 0;
    public int SuspicionDelta { get; set; } = 0;
    
    // Se false, può essere scelta solo una volta
    public bool Repeatable { get; set; } = false;
    
    // Flag richiesti per poter scegliere questa opzione
    public List<string> RequiredFlags { get; set; } = new();
    
    // Flag che vengono impostati quando si sceglie questa opzione
    public List<string> GrantsFlags { get; set; } = new();
    
    // Range di suspicion per cui questa opzione è disponibile
    public int MinSuspicion { get; set; } = 0;
    public int MaxSuspicion { get; set; } = 100;
    
    // Indizio sbloccato se si sceglie questa opzione
    public string? GrantsClueId { get; set; }
}

/// <summary>
/// Definizione di un elemento di gioco del JSON.
/// Alice userà questa classe per gli Item.
/// </summary>
public class ItemDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ItemType Type { get; set; } = ItemType.Normal;
    
    public double Weight { get; set; } = 0.0;
    public bool Collectible { get; set; } = true;
    
    // Per documenti: numero di pagine
    public int Pages { get; set; } = 0;
    
    // Per contenitori: quali item contiene (item IDs)
    public List<string> Contains { get; set; } = new();
    
    // Flag richiesto per aprire il contenitore
    public string? OpenRequiredFlag { get; set; }
    
    // Se è un oggetto futuro, in quale anno mostra una visione
    public int? FutureYear { get; set; }
    
    // Se è una prova, a quale indizio è collegato
    public string? LinkedClueId { get; set; }
}

public enum ItemType
{
    Normal,      // Oggetto generico
    Key,         // Chiave
    Document,    // Documento con pagine
    Evidence,    // Prova / indizio fisico
    Future,      // Oggetto che mostra il futuro
    Container    // Contenitore
}

/// <summary>
/// Definizione di una stanza.
/// </summary>
public class RoomDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Uscite disponibili
    public Dictionary<string, string> Exits { get; set; } = new();  // direction -> roomId
    
    // Item inizialmente in questa stanza
    public List<string> ItemIds { get; set; } = new();
    
    // NPC in questa stanza
    public List<string> NpcIds { get; set; } = new();
    
    // Flag richiesto per accedere
    public string? AccessRequiredFlag { get; set; }
}

/// <summary>
/// Definizione di un livello di gioco.
/// </summary>
public class LevelDefinition
{
    public int LevelNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Condizioni per completare il livello
    public List<LevelCondition> CompletionConditions { get; set; } = new();
}

public class LevelCondition
{
    public string Type { get; set; } = string.Empty;  // es. "flag_true", "clue_count_at_least"
    public string? FlagName { get; set; }
    public int? MinimumValue { get; set; }
}

/// <summary>
/// Definizione di un alias di comando.
/// DADDA: permette di configurare alias dinamici via JSON.
/// Es: "n" -> "north", "raccogli" -> "take"
/// </summary>
public class CommandAliasDefinition
{
    /// <summary>Alias shortened command (e.g., "n", "inv")</summary>
    public string Alias { get; set; } = string.Empty;
    
    /// <summary>Full command that the alias resolves to (e.g., "north", "inventory")</summary>
    public string ResolvedCommand { get; set; } = string.Empty;
}
