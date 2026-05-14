namespace OltreIlTempo.Engine;

using OltreIlTempo.Domain;

/// <summary>
/// Implementazione del servizio di deduzioni.
/// SOFIA: gestisce gli indizi e il sistema di deduzioni.
/// </summary>
public class DeductionService : IDeductionService
{
    public string DescribeBoard(GameState state, GameContent content)
    {
        // TODO: Implementare la descrizione della bacheca degli indizi
        
        var board = "== Bacheca Indizi ==\n";
        
        if (state.FoundClues.Count == 0)
        {
            board += "Nessun indizio trovato ancora.\n";
            return board;
        }
        
        int totalConfidence = 0;
        
        foreach (var clueId in state.FoundClues.Keys)
        {
            if (content.Clues.TryGetValue(clueId, out var clue))
            {
                board += $"\n📌 {clue.Title}\n";
                board += $"   {clue.Description}\n";
                board += $"   Qualità: {clue.Quality} | Confidenza: {clue.Confidence}%\n";
                
                totalConfidence += clue.Confidence;
            }
        }
        
        board += $"\n--- Confidenza totale: {totalConfidence} ---\n";
        
        return board;
    }
    
    public string Run(GameState state, GameContent content)
    {
        // TODO: Implementare il comando 'deduci'
        // 1. Scansiona le deduzioni disponibili
        // 2. Controlla se gli indizi richiesti sono stati trovati
        // 3. Calcola il punteggio totale di confidenza
        // 4. Se supera la soglia, sblocca la deduzione
        
        var result = "== Deduzioni ==\n";
        
        bool anyUnlocked = false;
        
        foreach (var deduction in content.Deductions.Values)
        {
            if (state.ResolvedDeductions.ContainsKey(deduction.Id))
            {
                continue;  // Già risolto
            }
            
            // Controlla se gli indizi richiesti sono stati trovati
            bool hasAllClues = deduction.RequiredClueIds.All(clueId =>
                state.FoundClues.ContainsKey(clueId)
            );
            
            if (!hasAllClues)
            {
                continue;
            }
            
            // Calcola il punteggio di confidenza
            int confidenceScore = 0;
            foreach (var clueId in deduction.RequiredClueIds)
            {
                if (content.Clues.TryGetValue(clueId, out var clue))
                {
                    confidenceScore += clue.Confidence;
                }
            }
            
            // Se supera la soglia, sblocca
            if (confidenceScore >= deduction.MinEvidenceScore)
            {
                state.ResolvedDeductions[deduction.Id] = true;
                result += $"✓ Deduzione sbloccata: {deduction.Title}\n";
                result += $"  {deduction.Description}\n";
                
                // Sblocca indizio risultante
                if (!string.IsNullOrEmpty(deduction.ResultingClueId))
                {
                    state.FoundClues[deduction.ResultingClueId] = true;
                }
                
                // Imposta i flag
                foreach (var flag in deduction.GrantsFlags)
                {
                    state.Flags[flag] = true;
                }
                
                anyUnlocked = true;
            }
        }
        
        if (!anyUnlocked)
        {
            result += "Nessuna deduzione sbloccabile al momento.\n";
        }
        
        return result;
    }
}
