namespace OltreIlTempo.Domain;

/// <summary>
/// Interfaccia per il sistema di inventario.
/// Alice implementerà questa interfaccia.
/// </summary>
public interface IInventorySystem
{
    /// <summary>
    /// Aggiunge un oggetto all'inventario. Restituisce true se riuscito.
    /// </summary>
    bool Add(string itemId, GameState state, GameContent content);
    
    /// <summary>
    /// Rimuove un oggetto dall'inventario e lo deposita nella stanza.
    /// </summary>
    bool Remove(string itemId, GameState state);
    
    /// <summary>
    /// Restituisce gli oggetti in inventario, opzionalmente filtrati.
    /// Filtri: "documents", "keys", "evidence", "all"
    /// </summary>
    List<ItemDefinition> List(GameState state, GameContent content, string? filter = null);
    
    /// <summary>
    /// Usa un oggetto. Delega la logica specifica a GameEngine tramite flag.
    /// </summary>
    bool Use(string itemId, string? targetItemId, GameState state);
    
    /// <summary>
    /// Ottiene il peso totale dell'inventario.
    /// </summary>
    double GetTotalWeight(GameState state, GameContent content);
}
