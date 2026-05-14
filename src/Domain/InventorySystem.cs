namespace OltreIlTempo.Domain;

using OltreIlTempo.Engine;

/// <summary>
/// Implementazione del sistema di inventario.
/// ALICE: gestisce gli oggetti e l'inventario del giocatore.
/// </summary>
public class InventorySystem : IInventorySystem
{
    public bool Add(string itemId, GameState state, GameContent content)
    {
        // TODO: Implementare l'aggiunta di un oggetto
        // 1. Verifica che l'oggetto esista
        // 2. Controlla il peso massimo
        // 3. Aggiorna l'inventario
        
        if (!content.Items.TryGetValue(itemId, out var item))
        {
            return false;
        }
        
        double totalWeight = GetTotalWeight(state, content) + item.Weight;
        
        if (totalWeight > content.Config.MaxInventoryWeight)
        {
            return false;  // Inventario pieno
        }
        
        state.InventoryItemIds.Add(itemId);
        
        // Aggiorna la location dell'item
        if (state.ItemStates.TryGetValue(itemId, out var itemState))
        {
            itemState.Location = "inventory";
        }
        
        return true;
    }
    
    public bool Remove(string itemId, GameState state)
    {
        // TODO: Implementare la rimozione di un oggetto
        // 1. Rimuovi dall'inventario
        // 2. Depositalo nella stanza attuale
        
        if (!state.InventoryItemIds.Remove(itemId))
        {
            return false;
        }
        
        // Aggiorna la location dell'item
        if (state.ItemStates.TryGetValue(itemId, out var itemState))
        {
            itemState.Location = $"room_{state.CurrentRoomId}";
        }
        
        return true;
    }
    
    public List<ItemDefinition> List(GameState state, GameContent content, string? filter = null)
    {
        // TODO: Implementare la lista di oggetti
        // Supporta filtri: "documents", "keys", "evidence", "all"
        
        var items = new List<ItemDefinition>();
        
        foreach (var itemId in state.InventoryItemIds)
        {
            if (content.Items.TryGetValue(itemId, out var item))
            {
                if (MatchesFilter(item, filter))
                {
                    items.Add(item);
                }
            }
        }
        
        return items;
    }
    
    public bool Use(string itemId, string? targetItemId, GameState state)
    {
        // TODO: Implementare l'uso di un oggetto
        // Delega a GameEngine tramite flag
        
        if (!state.InventoryItemIds.Contains(itemId))
        {
            return false;
        }
        
        // Placeholder: marca come usato
        return true;
    }
    
    public double GetTotalWeight(GameState state, GameContent content)
    {
        double totalWeight = 0.0;
        
        foreach (var itemId in state.InventoryItemIds)
        {
            if (content.Items.TryGetValue(itemId, out var item))
            {
                totalWeight += item.Weight;
            }
        }
        
        return totalWeight;
    }
    
    private bool MatchesFilter(ItemDefinition item, string? filter)
    {
        if (string.IsNullOrEmpty(filter) || filter == "all")
        {
            return true;
        }
        
        return filter.ToLower() switch
        {
            "documents" => item.Type == ItemType.Document,
            "keys" => item.Type == ItemType.Key,
            "evidence" => item.Type == ItemType.Evidence,
            "future" => item.Type == ItemType.Future,
            _ => true
        };
    }
}
