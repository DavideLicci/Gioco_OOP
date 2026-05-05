namespace OltreIlTempo.Domain;

/// <summary>
/// Interfaccia per il sistema di dialogo.
/// Valkiria implementerà questa interfaccia.
/// </summary>
public interface IDialogueSystem
{
    /// <summary>
    /// Inizia un dialogo con un NPC. Restituisce le opzioni disponibili.
    /// </summary>
    List<DialogueOptionDefinition> StartDialogue(NpcDefinition npc, GameState state);
    
    /// <summary>
    /// Processa la scelta dell'utente e aggiorna lo stato di gioco.
    /// </summary>
    void ChooseOption(string optionId, GameState state, NpcDefinition npc);
}
