namespace OltreIlTempo.Engine;

using OltreIlTempo.Domain;
using System.Text.RegularExpressions;

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
            \"flag_true\" => EvaluateFlagTrue(parts, state),\n            \"flag_false\" => EvaluateFlagFalse(parts, state),\n            \"trust_at_least\" => EvaluateTrustAtLeast(parts, state),\n            \"trust_at_most\" => EvaluateTrustAtMost(parts, state),\n            \"suspicion_at_least\" => EvaluateSuspicionAtLeast(parts, state),\n            \"suspicion_at_most\" => EvaluateSuspicionAtMost(parts, state),\n            \"clue_found\" => EvaluateClueFound(parts, state),\n            \"inventory_has\" => EvaluateInventoryHas(parts, state),\n            \"current_room\" => EvaluateCurrentRoom(parts, state),\n            \"achievement_unlocked\" => EvaluateAchievementUnlocked(parts, state),\n            \"visited_room\" => EvaluateVisitedRoom(parts, state),\n            \"npc_trust_range\" => EvaluateNpcTrustRange(parts, state),\n            _ => false\n        };\n    }\n    \n    private bool EvaluateFlagTrue(string[] parts, GameState state)\n    {\n        if (parts.Length < 2)\n            return false;\n        \n        string flagName = parts[1].Trim();\n        return state.Flags.GetValueOrDefault(flagName, false);\n    }\n    \n    private bool EvaluateFlagFalse(string[] parts, GameState state)\n    {\n        if (parts.Length < 2)\n            return false;\n        \n        string flagName = parts[1].Trim();\n        return !state.Flags.GetValueOrDefault(flagName, false);\n    }\n    \n    private bool EvaluateTrustAtLeast(string[] parts, GameState state)\n    {\n        if (parts.Length < 3)\n            return false;\n        \n        string npcId = parts[1].Trim();\n        if (!int.TryParse(parts[2].Trim(), out int minTrust))\n            return false;\n        \n        int currentTrust = state.NpcTrust.GetValueOrDefault(npcId, 0);\n        return currentTrust >= minTrust;\n    }\n    \n    private bool EvaluateTrustAtMost(string[] parts, GameState state)\n    {\n        if (parts.Length < 3)\n            return false;\n        \n        string npcId = parts[1].Trim();\n        if (!int.TryParse(parts[2].Trim(), out int maxTrust))\n            return false;\n        \n        int currentTrust = state.NpcTrust.GetValueOrDefault(npcId, 0);\n        return currentTrust <= maxTrust;\n    }\n    \n    private bool EvaluateSuspicionAtLeast(string[] parts, GameState state)\n    {\n        if (parts.Length < 2)\n            return false;\n        \n        if (!int.TryParse(parts[1].Trim(), out int minSuspicion))\n            return false;\n        \n        return state.Suspicion >= minSuspicion;\n    }\n    \n    private bool EvaluateSuspicionAtMost(string[] parts, GameState state)\n    {\n        if (parts.Length < 2)\n            return false;\n        \n        if (!int.TryParse(parts[1].Trim(), out int maxSuspicion))\n            return false;\n        \n        return state.Suspicion <= maxSuspicion;\n    }\n    \n    private bool EvaluateClueFound(string[] parts, GameState state)\n    {\n        if (parts.Length < 2)\n            return false;\n        \n        string clueId = parts[1].Trim();\n        return state.FoundClues.GetValueOrDefault(clueId, false);\n    }\n    \n    private bool EvaluateInventoryHas(string[] parts, GameState state)\n    {\n        if (parts.Length < 2)\n            return false;\n        \n        string itemId = parts[1].Trim();\n        return state.InventoryItemIds.Contains(itemId);\n    }\n    \n    private bool EvaluateCurrentRoom(string[] parts, GameState state)\n    {\n        if (parts.Length < 2)\n            return false;\n        \n        string roomId = parts[1].Trim();\n        return state.CurrentRoomId == roomId;\n    }\n    \n    private bool EvaluateAchievementUnlocked(string[] parts, GameState state)\n    {\n        if (parts.Length < 2)\n            return false;\n        \n        string achievementId = parts[1].Trim();\n        return state.UnlockedAchievements.ContainsKey(achievementId);\n    }\n    \n    private bool EvaluateVisitedRoom(string[] parts, GameState state)\n    {\n        if (parts.Length < 2)\n            return false;\n        \n        string roomId = parts[1].Trim();\n        return state.UnlockedRooms.GetValueOrDefault(roomId, false);\n    }\n    \n    private bool EvaluateNpcTrustRange(string[] parts, GameState state)\n    {\n        if (parts.Length < 4)\n            return false;\n        \n        string npcId = parts[1].Trim();\n        if (!int.TryParse(parts[2].Trim(), out int minTrust))\n            return false;\n        if (!int.TryParse(parts[3].Trim(), out int maxTrust))\n            return false;\n        \n        int currentTrust = state.NpcTrust.GetValueOrDefault(npcId, 0);\n        return currentTrust >= minTrust && currentTrust <= maxTrust;\n    }\n}
