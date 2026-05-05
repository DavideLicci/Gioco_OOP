namespace OltreIlTempo.Engine;

using OltreIlTempo.Domain;

/// <summary>
/// Rappresenta tutto il contenuto del gioco caricato dal JSON.
/// Usato da GameEngine e gli altri servizi.
/// </summary>
public class GameContent
{
    public string FormatVersion { get; set; } = "1.0";
    
    // Configurazione
    public GameConfig Config { get; set; } = new();
    
    // Definizioni di stanze
    public Dictionary<string, RoomDefinition> Rooms { get; set; } = new();
    
    // Definizioni di NPC
    public Dictionary<string, NpcDefinition> Npcs { get; set; } = new();
    
    // Definizioni di item
    public Dictionary<string, ItemDefinition> Items { get; set; } = new();
    
    // Definizioni di indizi
    public Dictionary<string, ClueDefinition> Clues { get; set; } = new();
    
    // Definizioni di deduzioni
    public Dictionary<string, DeductionDefinition> Deductions { get; set; } = new();
    
    // Definizioni di achievement
    public Dictionary<string, AchievementDefinition> Achievements { get; set; } = new();
    
    // Definizioni di livelli
    public List<LevelDefinition> Levels { get; set; } = new();
    
    // Stanza iniziale
    public string StartingRoomId { get; set; } = string.Empty;
}

/// <summary>
/// Definizione di un indizio.
/// Sofia creerà questa classe.
/// </summary>
public class ClueDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ClueQuality Quality { get; set; } = ClueQuality.Medium;
    public int Confidence { get; set; } = 50;  // 0-100
}

public enum ClueQuality
{
    Low,
    Medium,
    High
}

/// <summary>
/// Definizione di una deduzione.
/// Sofia creerà questa classe.
/// </summary>
public class DeductionDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Indizi richiesti per sbloccare questa deduzione
    public List<string> RequiredClueIds { get; set; } = new();
    
    // Punteggio minimo evidenza (somma delle confidence)
    public int MinEvidenceScore { get; set; } = 80;
    
    // Indizio risultante (il principale risultato della deduzione)
    public string? ResultingClueId { get; set; }
    
    // Flag da impostare quando la deduzione viene sbloccata
    public List<string> GrantsFlags { get; set; } = new();
}

/// <summary>
/// Definizione di un achievement/trofeo.
/// Sofia creerà questa classe.
/// </summary>
public class AchievementDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Hidden { get; set; } = false;
    
    // Condizioni per sbloccare il trofeo
    public List<AchievementCondition> Conditions { get; set; } = new();
}

public class AchievementCondition
{
    public string Type { get; set; } = string.Empty;  // es. "clue_count_at_least", "achievement_at_turn"
    public string? ClueId { get; set; }
    public int? Number { get; set; }
    public int? MaxTurns { get; set; }
}
