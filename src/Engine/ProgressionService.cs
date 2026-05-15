namespace OltreIlTempo.Engine;

using OltreIlTempo.Domain;

/// <summary>
/// Implementazione del servizio di progressione.
/// NICK: gestisce i livelli e il completamento degli obiettivi.
/// </summary>
public class ProgressionService : IProgressionService
{
    private readonly IConditionEvaluator _evaluator;
    
    public ProgressionService(IConditionEvaluator evaluator)
    {
        _evaluator = evaluator;
    }
    
    public bool CheckLevelComplete(GameState state, GameContent content)
    {
        // TODO: Implementare il controllo di completamento del livello
        // 1. Ottieni il livello corrente
        // 2. Controlla se tutte le condizioni sono soddisfatte
        // 3. Se sì, avanza al livello successivo
        // 4. Restituisci true se il livello è stato appena completato
        
        var level = GetCurrentLevel(state.CurrentLevel, content);
        
        if (level == null)
        {
            return false;
        }
        
        bool allConditionsMet = level.CompletionConditions.All(condition =>
            _evaluator.Evaluate($"{condition.Type}:{condition.FlagName}", state)
        );
        
        if (allConditionsMet && state.CurrentLevel < content.Levels.Count)
        {
            state.CurrentLevel++;
            return true;
        }
        
        return false;
    }
    
    public LevelDefinition? GetCurrentLevel(int levelNumber, GameContent content)
    {
        // TODO: Restituisci il livello corrispondente
        return content.Levels.FirstOrDefault(l => l.LevelNumber == levelNumber);
    }
}
