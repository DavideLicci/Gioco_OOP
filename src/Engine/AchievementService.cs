namespace OltreIlTempo.Engine;

using OltreIlTempo.Domain;

/// <summary>
/// Implementazione del servizio di achievement/trofei.
/// SOFIA: gestisce i trofei e i loro sblocchi.
/// </summary>
public class AchievementService : IAchievementService
{
    private readonly IConditionEvaluator _evaluator;
    
    public AchievementService(IConditionEvaluator evaluator)
    {
        _evaluator = evaluator;
    }
    
    public List<string> EvaluateNewUnlocks(GameState state, GameContent content)
    {
        // TODO: Implementare la valutazione degli achievement
        // 1. Scansiona tutti gli achievement non ancora sbloccati
        // 2. Per ciascuno, controlla se le condizioni sono soddisfatte
        // 3. Se sì, sblocca e restituisci la notifica
        
        var unlocks = new List<string>();
        
        foreach (var achievement in content.Achievements)
        {
            if (state.UnlockedAchievements.ContainsKey(achievement.Id))
            {
                continue;  // Già sbloccato
            }
            
            if (AreConditionsMet(achievement.Conditions, state, content))
            {
                state.UnlockedAchievements[achievement.Id] = state.TurnNumber;
                unlocks.Add($"🏆 Trofeo sbloccato: {achievement.Title}");
            }
        }
        
        return unlocks;
    }
    
    public string Describe(GameState state, GameContent content)
    {
        // TODO: Implementare la descrizione dei trofei
        
        var description = "== Trofei ==\n";
        
        foreach (var achievement in content.Achievements)
        {
            var isUnlocked = state.UnlockedAchievements.ContainsKey(achievement.Id);
            
            if (isUnlocked)
            {
                description += $"✓ {achievement.Title}: {achievement.Description}\n";
            }
            else if (!achievement.Hidden)
            {
                description += $"○ {achievement.Title}: {achievement.Description}\n";
            }
            else
            {
                description += $"? Trofeo nascosto\n";
            }
        }
        
        return description;
    }
    
    private bool AreConditionsMet(List<AchievementCondition> conditions, GameState state, GameContent content)
    {
        // TODO: Valutare le condizioni
        // Tipi di condizioni:
        // - clue_count_at_least: numero di indizi trovati >= numero
        // - level_complete_at_turn: livello completato entro N turni
        // - ecc.
        
        return conditions.All(condition => EvaluateCondition(condition, state, content));
    }
    
    private bool EvaluateCondition(AchievementCondition condition, GameState state, GameContent content)
    {
        return condition.Type switch
        {
            "clue_count_at_least" => state.FoundClues.Count >= (condition.Number ?? 0),
            "achievement_at_turn" => state.TurnNumber <= (condition.MaxTurns ?? int.MaxValue),
            _ => false
        };
    }
}
