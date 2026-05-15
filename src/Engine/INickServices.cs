namespace OltreIlTempo.Engine;

using OltreIlTempo.Domain;

/// <summary>
/// Interfaccia per il servizio di salvataggio e caricamento.
/// Nick implementerà questa interfaccia.
/// </summary>
public interface ISaveService
{
    /// <summary>
    /// Salva lo stato del gioco in un file JSON.
    /// </summary>
    void Save(GameState state, string filePath);
    
    /// <summary>
    /// Carica lo stato del gioco da un file JSON.
    /// </summary>
    GameState Load(string filePath);
    
    /// <summary>
    /// Controlla se un file di salvataggio esiste e è valido.
    /// </summary>
    bool IsValidSaveFile(string filePath);
}

/// <summary>
/// Interfaccia per il caricamento dei contenuti di gioco.
/// Nick implementerà questa interfaccia.
/// </summary>
public interface IContentLoader
{
    /// <summary>
    /// Carica il contenuto del gioco da game-content.json.
    /// </summary>
    GameContent Load(string filePath);
}

/// <summary>
/// Interfaccia per la gestione della progressione dei livelli.
/// Nick implementerà questa interfaccia.
/// </summary>
public interface IProgressionService
{
    /// <summary>
    /// Controlla se il livello corrente è completato e avanza al prossimo se necessario.
    /// </summary>
    bool CheckLevelComplete(GameState state, GameContent content);
    
    /// <summary>
    /// Ottiene la definizione del livello corrente.
    /// </summary>
    LevelDefinition? GetCurrentLevel(int levelNumber, GameContent content);
}
