namespace OltreIlTempo.Engine;

using OltreIlTempo.Domain;

/// <summary>
/// Implementazione del valutatore di condizioni (Interpreter Pattern).
/// DADDA: implementa la valutazione di condizioni complesse dal JSON.
/// 
/// Supporta condizioni come:
/// - "flag_true:name_of_flag"
/// - "flag_false:name_of_flag"
/// - "trust_at_least:npc_id:50"
/// - "clue_found:clue_id"
/// - "suspicion_at_most:75"
/// - "inventory_has:item_id"
/// - "current_room:room_id"
/// </summary>
public class ConditionEvaluator : IConditionEvaluator
{
    public bool Evaluate(string conditionDefinition, GameState state)
    {
        if (string.IsNullOrWhiteSpace(conditionDefinition))
        {
            return true;  // Condizione vuota = sempre vera
        }

        var parts = conditionDefinition.Split(':');
        if (parts.Length == 0)
        {
            return false;
        }

        string conditionType = parts[0].ToLower().Trim();

        return conditionType switch
        {
            "flag_true" => EvaluateFlagTrue(parts, state),
            "flag_false" => EvaluateFlagFalse(parts, state),
            "trust_at_least" => EvaluateTrustAtLeast(parts, state),
            "trust_at_most" => EvaluateTrustAtMost(parts, state),
            "suspicion_at_least" => EvaluateSuspicionAtLeast(parts, state),
            "suspicion_at_most" => EvaluateSuspicionAtMost(parts, state),
            "clue_found" => EvaluateClueFound(parts, state),
            "inventory_has" => EvaluateInventoryHas(parts, state),
            "current_room" => EvaluateCurrentRoom(parts, state),
            "achievement_unlocked" => EvaluateAchievementUnlocked(parts, state),
            "visited_room" => EvaluateVisitedRoom(parts, state),
            "npc_trust_range" => EvaluateNpcTrustRange(parts, state),
            _ => false
        };
    }

    private bool EvaluateFlagTrue(string[] parts, GameState state)
    {
        if (parts.Length < 2)
        {
            return false;
        }

        string flagName = parts[1].Trim();
        return state.Flags.GetValueOrDefault(flagName, false);
    }

    private bool EvaluateFlagFalse(string[] parts, GameState state)
    {
        if (parts.Length < 2)
        {
            return false;
        }

        string flagName = parts[1].Trim();
        return !state.Flags.GetValueOrDefault(flagName, false);
    }

    private bool EvaluateTrustAtLeast(string[] parts, GameState state)
    {
        if (parts.Length < 3)
        {
            return false;
        }

        string npcId = parts[1].Trim();
        if (!int.TryParse(parts[2].Trim(), out int minTrust))
        {
            return false;
        }

        int currentTrust = state.NpcTrust.GetValueOrDefault(npcId, 0);
        return currentTrust >= minTrust;
    }

    private bool EvaluateTrustAtMost(string[] parts, GameState state)
    {
        if (parts.Length < 3)
        {
            return false;
        }

        string npcId = parts[1].Trim();
        if (!int.TryParse(parts[2].Trim(), out int maxTrust))
        {
            return false;
        }

        int currentTrust = state.NpcTrust.GetValueOrDefault(npcId, 0);
        return currentTrust <= maxTrust;
    }

    private bool EvaluateSuspicionAtLeast(string[] parts, GameState state)
    {
        if (parts.Length < 2)
        {
            return false;
        }

        if (!int.TryParse(parts[1].Trim(), out int minSuspicion))
        {
            return false;
        }

        return state.Suspicion >= minSuspicion;
    }

    private bool EvaluateSuspicionAtMost(string[] parts, GameState state)
    {
        if (parts.Length < 2)
        {
            return false;
        }

        if (!int.TryParse(parts[1].Trim(), out int maxSuspicion))
        {
            return false;
        }

        return state.Suspicion <= maxSuspicion;
    }

    private bool EvaluateClueFound(string[] parts, GameState state)
    {
        if (parts.Length < 2)
        {
            return false;
        }

        string clueId = parts[1].Trim();
        return state.FoundClues.GetValueOrDefault(clueId, false);
    }

    private bool EvaluateInventoryHas(string[] parts, GameState state)
    {
        if (parts.Length < 2)
        {
            return false;
        }

        string itemId = parts[1].Trim();
        return state.InventoryItemIds.Contains(itemId);
    }

    private bool EvaluateCurrentRoom(string[] parts, GameState state)
    {
        if (parts.Length < 2)
        {
            return false;
        }

        string roomId = parts[1].Trim();
        return state.CurrentRoomId == roomId;
    }

    private bool EvaluateAchievementUnlocked(string[] parts, GameState state)
    {
        if (parts.Length < 2)
        {
            return false;
        }

        string achievementId = parts[1].Trim();
        return state.UnlockedAchievements.ContainsKey(achievementId);
    }

    private bool EvaluateVisitedRoom(string[] parts, GameState state)
    {
        if (parts.Length < 2)
        {
            return false;
        }

        string roomId = parts[1].Trim();
        return state.UnlockedRooms.GetValueOrDefault(roomId, false);
    }

    private bool EvaluateNpcTrustRange(string[] parts, GameState state)
    {
        if (parts.Length < 4)
        {
            return false;
        }

        string npcId = parts[1].Trim();
        if (!int.TryParse(parts[2].Trim(), out int minTrust))
        {
            return false;
        }

        if (!int.TryParse(parts[3].Trim(), out int maxTrust))
        {
            return false;
        }

        int currentTrust = state.NpcTrust.GetValueOrDefault(npcId, 0);
        return currentTrust >= minTrust && currentTrust <= maxTrust;
    }
}
