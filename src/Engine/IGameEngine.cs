namespace OltreIlTempo.Engine;

using OltreIlTempo.Domain;

/// <summary>
/// Interfaccia pubblica per GameEngine.
/// Dadda implementerà questa interfaccia.
/// </summary>
public interface IGameEngine
{
    /// <summary>
    /// Esegue un comando e restituisce il risultato.
    /// </summary>
    CommandResult Execute(string input);
    
    /// <summary>
    /// Ottiene lo stato attuale del gioco.
    /// </summary>
    GameState GetGameState();
    
    /// <summary>
    /// Descrive la stanza attuale.
    /// </summary>
    string DescribeCurrentRoom();
}

/// <summary>
/// Interfaccia per il parsing dei comandi.
/// Dadda implementerà questa interfaccia.
/// </summary>
public interface ICommandParser
{
    /// <summary>
    /// Esegue il parsing di un comando di input.
    /// </summary>
    ParsedCommand Parse(string input);
    
    /// <summary>
    /// Imposta gli alias dinamici dal contenuto del gioco.
    /// </summary>
    void SetDynamicAliases(Dictionary<string, string> aliases);
}

/// <summary>
/// Interfaccia per la valutazione delle condizioni.
/// Dadda implementerà questa interfaccia.
/// </summary>
public interface IConditionEvaluator
{
    /// <summary>
    /// Valuta una condizione complessa basata sul GameState.
    /// </summary>
    bool Evaluate(string conditionDefinition, GameState state);
}
