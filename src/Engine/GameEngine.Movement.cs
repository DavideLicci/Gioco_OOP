namespace OltreIlTempo.Engine;

using OltreIlTempo.Domain;

public partial class GameEngine
{
    // ==================== Comandi di Movimento ====================
    
    private CommandResult ExecuteMove(string direction)
    {
        var result = new CommandResult();
        
        if (!_content.Rooms.TryGetValue(_state.CurrentRoomId, out var currentRoom))
        {
            result.AddError("Stanza attuale non trovata.");
            return result;
        }
        
        if (!currentRoom.Exits.TryGetValue(direction, out var nextRoomId))
        {
            result.AddLine("Non puoi andare in quella direzione.", OutputLineType.Warning);
            return result;
        }
        
        if (!_content.Rooms.TryGetValue(nextRoomId, out var nextRoom))
        {
            result.AddError("Destinazione non trovata.");
            return result;
        }
        
        // Controlla se la stanza è bloccata
        if (!string.IsNullOrEmpty(nextRoom.AccessRequiredFlag))
        {
            if (!_state.Flags.GetValueOrDefault(nextRoom.AccessRequiredFlag, false))
            {
                result.AddLine("La stanza è bloccata. Ti serve qualcosa per entrarvi.", OutputLineType.Warning);
                return result;
            }
        }
        
        // Movimento riuscito
        _state.CurrentRoomId = nextRoomId;
        _state.UnlockedRooms[nextRoomId] = true;
        MarkRoomVisited(_state, nextRoomId);
        
        // Mostra la descrizione della nuova stanza
        result.AddLine($"Vai verso {GetDirectionFullName(direction)}.", OutputLineType.Success);
        result.AddLine(DescribeCurrentRoom());
        
        return result;
    }

    private string GetDirectionFullName(string direction) => direction.ToLower() switch
    {
        "n" or "north" => "Nord",
        "s" or "south" => "Sud",
        "e" or "east"  => "Est",
        "o" or "west" or "w" => "Ovest",
        _ => direction
    };
}
