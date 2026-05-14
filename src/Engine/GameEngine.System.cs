namespace OltreIlTempo.Engine;

using OltreIlTempo.Domain;

public partial class GameEngine
{
    // ==================== Comandi di Status ====================
    
    private CommandResult ExecuteInventory(List<string>? args)
    {
        var result = new CommandResult();
        
        if (_inventorySystem == null)
        {
            result.AddError("Sistema di inventario non disponibile.");
            return result;
        }
        
        var items = _inventorySystem.List(_state, _content);
        
        if (items.Count == 0)
        {
            result.AddLine("L'inventario è vuoto.", OutputLineType.Subtle);
            return result;
        }
        
        result.AddLine(AsciiArtProvider.GetInventoryHeader());
        result.AddLine(AsciiArtProvider.GetSeparator());
        
        foreach (var item in items)
        {
            result.AddLine($"    • {item.Name} (peso: {item.Weight}kg)");
        }
        
        var totalWeight = _inventorySystem.GetTotalWeight(_state, _content);
        result.AddLine(AsciiArtProvider.GetThinSeparator());
        result.AddLine($"  Peso totale: {totalWeight:F1}kg / {_config.MaxInventoryWeight}kg");
        
        return result;
    }
    
    private CommandResult ExecuteStatus()
    {
        var result = new CommandResult();
        
        result.AddLine(AsciiArtProvider.GetSeparator());
        result.AddLine("  [ STATUS AGENTE ]", OutputLineType.Success);
        result.AddLine(AsciiArtProvider.GetSeparator());
        result.AddLine($"  🕒 Turno:     {_state.TurnNumber}");
        result.AddLine($"  📈 Livello:   {_state.CurrentLevel}");
        result.AddLine($"  🕵️ Sospetto:  {_state.Suspicion}/{_config.MaxSuspicion}");
        result.AddLine($"  🔍 Indizi:    {_state.FoundClues.Count}");
        result.AddLine(AsciiArtProvider.GetSeparator());
        
        return result;
    }
    
    // ==================== Comandi di Sistema ====================
    
    private CommandResult ExecuteBoard()
    {
        var result = new CommandResult();
        
        if (_deductionService == null)
        {
            result.AddError("Sistema di deduzioni non disponibile.");
            return result;
        }
        
        result.AddLine("\n  ╔══════════════════════════════════════════════════════╗");
        result.AddLine("  ║             📌 BACHECA DEGLI INDIZI                ║");
        result.AddLine("  ╚══════════════════════════════════════════════════════╝\n");
        
        var board = _deductionService.DescribeBoard(_state, _content);
        result.AddLine(board);
        
        return result;
    }
    
    private CommandResult ExecuteDeduce()
    {
        var result = new CommandResult();
        
        if (_deductionService == null)
        {
            result.AddError("Sistema di deduzioni non disponibile.");
            return result;
        }
        
        result.AddLine(AsciiArtProvider.GetDeductionArt());
        result.AddLine(AsciiArtProvider.GetSeparator());
        
        var deduction = _deductionService.Run(_state, _content);
        result.AddLine(deduction);
        
        return result;
    }
    
    private CommandResult ExecuteAchievements()
    {
        var result = new CommandResult();
        
        if (_achievementService == null)
        {
            result.AddError("Sistema di achievement non disponibile.");
            return result;
        }
        
        result.AddLine(AsciiArtProvider.GetTrophy());
        result.AddLine(AsciiArtProvider.GetSeparator());
        
        var achievements = _achievementService.Describe(_state, _content);
        result.AddLine(achievements);
        
        return result;
    }
    
    private CommandResult ExecuteSave(List<string> args)
    {
        var result = new CommandResult();
        
        if (_saveService == null)
        {
            result.AddError("Sistema di salvataggio non disponibile.");
            return result;
        }
        
        string filePath = args.Count > 0 ? args[0] : "savegame.json";
        
        try
        {
            _saveService.Save(_state, filePath);
            result.AddLine($"Gioco salvato in {filePath}", OutputLineType.Success);
        }
        catch (Exception ex)
        {
            result.AddError($"Errore nel salvataggio: {ex.Message}");
        }
        
        return result;
    }
    
    private CommandResult ExecuteLoad(List<string> args)
    {
        var result = new CommandResult();
        
        if (_saveService == null)
        {
            result.AddError("Sistema di caricamento non disponibile.");
            return result;
        }
        
        string filePath = args.Count > 0 ? args[0] : "savegame.json";
        
        try
        {
            _state = _saveService.Load(filePath);
            result.AddLine($"Gioco caricato da {filePath}", OutputLineType.Success);
            result.AddLine(DescribeCurrentRoom());
        }
        catch (Exception ex)
        {
            result.AddError($"Errore nel caricamento: {ex.Message}");
        }
        
        return result;
    }
    
    private CommandResult ExecuteHelp()
    {
        var result = new CommandResult();
        
        result.AddLine(AsciiArtProvider.GetSeparator());
        result.AddLine("  [Comandi Disponibili]", OutputLineType.Success);
        result.AddLine(AsciiArtProvider.GetSeparator());
        result.AddLine("\n  === Movimento ===");
        result.AddLine("    n/nord, s/sud, e/est, o/ovest  - Muoviti in una direzione");
        result.AddLine("\n  === Azioni ===");
        result.AddLine("    raccogli [oggetto]   - Raccogli un oggetto");
        result.AddLine("    lascia [oggetto]     - Lascia un oggetto");
        result.AddLine("    parla [persona]      - Parla con una persona");
        result.AddLine("    scegli [numero]      - Scegli un'opzione di dialogo");
        result.AddLine("    esamina [oggetto]    - Esamina un oggetto");
        result.AddLine("    leggi [doc] [pagina] - Leggi un documento");
        result.AddLine("    usa [oggetto]        - Usa un oggetto");
        result.AddLine("    usa [obj] su [obj]   - Usa un oggetto su un altro");
        result.AddLine("    apri [contenitore]   - Apri un contenitore");
        result.AddLine("    chiudi [contenitore] - Chiudi un contenitore");
        result.AddLine("    guarda               - Guarda intorno");
        result.AddLine("\n  === Sistema ===");
        result.AddLine("    inventario           - Mostra inventario");
        result.AddLine("    stato                - Mostra lo stato del gioco");
        result.AddLine("    missione             - Mostra la missione corrente");
        result.AddLine("    mappa                - Mostra la mappa della villa");
        result.AddLine("    bacheca              - Mostra gli indizi trovati");
        result.AddLine("    deduci               - Sblocca deduzioni");
        result.AddLine("    trofei               - Mostra i trofei");
        result.AddLine("    salva [file]         - Salva il gioco");
        result.AddLine("    carica [file]        - Carica il gioco");
        result.AddLine("    aiuto                - Mostra questo messaggio");
        result.AddLine(AsciiArtProvider.GetSeparator());
        
        return result;
    }
    
    private CommandResult ExecuteQuest()
    {
        var result = new CommandResult();
        
        result.AddLine("[Missione Corrente]", OutputLineType.Success);
        
        if (_progressionService == null)
        {
            result.AddLine($"Livello: {_state.CurrentLevel}");
            return result;
        }
        
        var level = _progressionService.GetCurrentLevel(_state.CurrentLevel, _content);
        
        if (level == null)
        {
            result.AddLine("🎉 Hai completato tutti i livelli!", OutputLineType.Success);
            return result;
        }
        
        result.AddLine($"📋 Livello {level.LevelNumber}: {level.Title}");
        result.AddLine($"   {level.Description}");
        result.AddLine("");
        result.AddLine("Condizioni da completare:");
        
        foreach (var condition in level.CompletionConditions)
        {
            string conditionStr = $"{condition.Type}:{condition.FlagName}";
            bool isMet = _evaluator.Evaluate(conditionStr, _state);
            string icon = isMet ? "✅" : "⬜";
            string condText = condition.Type switch
            {
                "flag_true" => $"Sblocca: {condition.FlagName}",
                "clue_found" => $"Trova indizio: {condition.FlagName}",
                "visited_room" => $"Visita: {condition.FlagName}",
                "trust_at_least" => $"Trust minimo: {condition.MinimumValue}",
                _ => $"{condition.Type}: {condition.FlagName}"
            };
            result.AddLine($"  {icon} {condText}");
        }
        
        return result;
    }
    
    private CommandResult ExecuteMap()
    {
        var result = new CommandResult();
        
        result.AddLine(AsciiArtProvider.GetVillaMap(_state.CurrentRoomId));
        
        // Mostra posizione attuale
        if (_content.Rooms.TryGetValue(_state.CurrentRoomId, out var room))
        {
            result.AddLine($"\n  📍 Sei qui: {room.Name.ToUpper()}", OutputLineType.Success);
        }
        
        return result;
    }
    
    private CommandResult ExecuteQuit()
    {
        var result = new CommandResult();
        result.AddLine("Grazie per aver giocato! Arrivederci.", OutputLineType.Success);
        return result;
    }
}
