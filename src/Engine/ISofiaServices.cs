namespace OltreIlTempo.Engine;

using OltreIlTempo.Domain;

/// <summary>
/// Interfaccia per il sistema di achievement/trofei.
/// Sofia implementerà questa interfaccia.
/// </summary>
public interface IAchievementService
{
    /// <summary>
    /// Valuta i nuovi trofei sbloccabili e restituisce le notifiche di sblocco.
    /// Chiamato ogni turno da GameEngine.
    /// </summary>
    List<string> EvaluateNewUnlocks(GameState state, GameContent content);
    
    /// <summary>
    /// Restituisce la descrizione di tutti i trofei (sbloccati e non).
    /// </summary>
    string Describe(GameState state, GameContent content);
}

/// <summary>
/// Interfaccia per il sistema di deduzioni e bacheca indizi.
/// Sofia implementerà questa interfaccia.
/// </summary>
public interface IDeductionService
{
    /// <summary>
    /// Mostra la bacheca degli indizi trovati.
    /// </summary>
    string DescribeBoard(GameState state, GameContent content);
    
    /// <summary>
    /// Prova a sbloccare una deduzione basata sugli indizi trovati.
    /// Restituisce il messaggio di risultato.
    /// </summary>
    string Run(GameState state, GameContent content);
}
