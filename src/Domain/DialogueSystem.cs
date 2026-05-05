namespace OltreIlTempo.Domain;

/// <summary>
/// Implementazione del sistema di dialogo.
/// VALKIRIA: gestisce gli NPC e le loro interazioni.
/// </summary>
public class DialogueSystem : IDialogueSystem
{
    public List<DialogueOptionDefinition> StartDialogue(NpcDefinition npc, GameState state)
    {
        // TODO: Implementare l'inizio del dialogo
        // 1. Ottieni lo stato runtime dell'NPC
        // 2. Filtra le opzioni disponibili in base a:
        //    - Flag richiesti
        //    - Livello di suspicion
        //    - Opzioni già usate (se non ripetibili)
        // 3. Restituisci le opzioni disponibili
        
        var npcState = state.NpcStates.GetValueOrDefault(npc.Id);
        if (npcState == null)
        {
            npcState = new NpcRuntimeState { NpcId = npc.Id };
            state.NpcStates[npc.Id] = npcState;
        }
        
        var availableOptions = npc.DialogueOptions
            .Where(opt => CanChooseOption(opt, state, npcState))
            .ToList();
        
        return availableOptions;
    }
    
    public void ChooseOption(string optionId, GameState state, NpcDefinition npc)
    {
        // TODO: Implementare la scelta di un'opzione
        // 1. Trova l'opzione nel dialogo dell'NPC
        // 2. Applica gli effetti:
        //    - Aggiorna il trust dell'NPC
        //    - Aggiorna la suspicion
        //    - Imposta i flag
        //    - Sblocca indizi
        // 3. Marca l'opzione come usata se non ripetibile
        
        var option = npc.DialogueOptions.FirstOrDefault(o => o.Id == optionId);
        if (option == null)
            return;
        
        var npcState = state.NpcStates.GetValueOrDefault(npc.Id);
        if (npcState == null)
            return;
        
        // Applica trust delta
        var newTrust = state.NpcTrust.GetValueOrDefault(npc.Id, 0) + option.TrustDelta;
        state.NpcTrust[npc.Id] = Math.Clamp(newTrust, 0, 100);
        
        // Applica suspicion delta
        state.Suspicion = Math.Clamp(state.Suspicion + option.SuspicionDelta, 0, 100);
        
        // Imposta i flag
        foreach (var flag in option.GrantsFlags)
        {
            state.Flags[flag] = true;
        }
        
        // Sblocca indizio se presente
        if (!string.IsNullOrEmpty(option.GrantsClueId))
        {
            state.FoundClues[option.GrantsClueId] = true;
        }
        
        // Marca come usata se non ripetibile
        if (!option.Repeatable)
        {
            npcState.UsedDialogueOptions.Add(optionId);
        }
    }
    
    private bool CanChooseOption(DialogueOptionDefinition option, GameState state, NpcRuntimeState npcState)
    {
        // Controlla se l'opzione è già stata usata (se non ripetibile)
        if (!option.Repeatable && npcState.UsedDialogueOptions.Contains(option.Id))
        {
            return false;
        }
        
        // Controlla i flag richiesti
        foreach (var requiredFlag in option.RequiredFlags)
        {
            if (!state.Flags.GetValueOrDefault(requiredFlag, false))
            {
                return false;
            }
        }
        
        // Controlla il range di suspicion
        if (state.Suspicion < option.MinSuspicion || state.Suspicion > option.MaxSuspicion)
        {
            return false;
        }
        
        return true;
    }
}
