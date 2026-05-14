namespace OltreIlTempo.Engine;

using OltreIlTempo.Domain;

public partial class GameEngine
{
    // ==================== Comandi di Azione ====================
    
    private CommandResult ExecuteCollect(List<string> args)
    {
        var result = new CommandResult();
        
        if (args.Count == 0)
        {
            result.AddError("Cosa vuoi raccogliere?");
            return result;
        }
        
        string itemName = string.Join(" ", args).ToLower();
        
        if (!_content.Rooms.TryGetValue(_state.CurrentRoomId, out var room))
        {
            result.AddError("Stanza attuale non trovata.");
            return result;
        }
        
        // Trova l'oggetto nella stanza
        var itemId = room.ItemIds.FirstOrDefault(id =>
            _content.Items.TryGetValue(id, out var item) &&
            item.Name.ToLower().Contains(itemName)
        );
        
        if (string.IsNullOrEmpty(itemId))
        {
            result.AddLine("Non vedi questo oggetto qui.", OutputLineType.Warning);
            return result;
        }
        
        if (_inventorySystem == null)
        {
            result.AddError("Sistema di inventario non disponibile.");
            return result;
        }
        
        if (_inventorySystem.Add(itemId, _state, _content))
        {
            var item = _content.Items[itemId];
            result.AddLine($"Hai raccolto: {item.Name}", OutputLineType.Success);
            room.ItemIds.Remove(itemId);
        }
        else
        {
            result.AddLine("L'inventario è troppo pieno.", OutputLineType.Warning);
        }
        
        return result;
    }
    
    private CommandResult ExecuteDrop(List<string> args)
    {
        var result = new CommandResult();
        
        if (args.Count == 0)
        {
            result.AddError("Cosa vuoi lasciare?");
            return result;
        }
        
        string itemName = string.Join(" ", args).ToLower();
        
        if (_inventorySystem == null)
        {
            result.AddError("Sistema di inventario non disponibile.");
            return result;
        }
        
        var itemId = _state.InventoryItemIds.FirstOrDefault(id =>
            _content.Items.TryGetValue(id, out var item) &&
            item.Name.ToLower().Contains(itemName)
        );
        
        if (string.IsNullOrEmpty(itemId))
        {
            result.AddLine("Non hai questo oggetto.", OutputLineType.Warning);
            return result;
        }
        
        if (_inventorySystem.Remove(itemId, _state))
        {
            var item = _content.Items[itemId];
            result.AddLine($"Hai lasciato: {item.Name}", OutputLineType.Success);
            
            if (_content.Rooms.TryGetValue(_state.CurrentRoomId, out var room))
            {
                room.ItemIds.Add(itemId);
            }
        }
        
        return result;
    }
    
    private CommandResult ExecuteTalk(List<string> args)
    {
        var result = new CommandResult();
        
        if (args.Count == 0)
        {
            result.AddError("Con chi vuoi parlare?");
            return result;
        }
        
        string npcName = string.Join(" ", args).ToLower();
        
        if (!_content.Rooms.TryGetValue(_state.CurrentRoomId, out var room))
        {
            result.AddError("Stanza attuale non trovata.");
            return result;
        }
        
        var npcId = room.NpcIds.FirstOrDefault(id =>
            _content.Npcs.TryGetValue(id, out var npc) &&
            npc.Name.ToLower().Contains(npcName)
        );
        
        if (string.IsNullOrEmpty(npcId) || !_content.Npcs.TryGetValue(npcId, out var npc2))
        {
            result.AddLine("Questa persona non è qui.", OutputLineType.Warning);
            return result;
        }
        
        if (_dialogueSystem == null)
        {
            result.AddLine($"{npc2.Name} non parla.", OutputLineType.Subtle);
            return result;
        }
        
        var options = _dialogueSystem.StartDialogue(npc2, _state);
        
        if (options.Count == 0)
        {
            result.AddLine($"{npc2.Name}: (Non ha nient'altro da dire per ora.)", OutputLineType.Subtle);
            _activeDialogueNpcId = null;
            _activeDialogueOptions = null;
            return result;
        }
        
        // Salva lo stato del dialogo per il comando 'scegli'
        _activeDialogueNpcId = npcId;
        _activeDialogueOptions = options;
        
        // Mostra l'arte ASCII del NPC
        var npcArt = AsciiArtProvider.GetNpcArt(npcId);
        if (!string.IsNullOrEmpty(npcArt))
        {
            result.AddLine(npcArt);
        }
        
        result.AddLine(AsciiArtProvider.GetThinSeparator());
        result.AddLine($"  💬 {npc2.Name}:", OutputLineType.Success);
        result.AddLine(AsciiArtProvider.GetThinSeparator());
        result.AddLine("  Scegli un'opzione di dialogo:");
        for (int i = 0; i < options.Count; i++)
        {
            result.AddLine($"  [{i + 1}] {options[i].Prompt}");
        }
        result.AddLine("\n(Usa 'scegli [numero]' per rispondere)", OutputLineType.Subtle);
        
        return result;
    }

    private CommandResult ExecuteChoose(List<string> args)
    {
        var result = new CommandResult();
        
        if (_activeDialogueNpcId == null || _activeDialogueOptions == null || _activeDialogueOptions.Count == 0)
        {
            result.AddLine("Non c'è un dialogo attivo. Usa 'parla [persona]' prima.", OutputLineType.Warning);
            return result;
        }
        
        if (args.Count == 0 || !int.TryParse(args[0], out int choice))
        {
            result.AddError("Scegli un numero. Es: 'scegli 1'");
            return result;
        }
        
        if (choice < 1 || choice > _activeDialogueOptions.Count)
        {
            result.AddLine($"Scelta non valida. Scegli tra 1 e {_activeDialogueOptions.Count}.", OutputLineType.Warning);
            return result;
        }
        
        var selectedOption = _activeDialogueOptions[choice - 1];
        
        if (!_content.Npcs.TryGetValue(_activeDialogueNpcId, out var npc))
        {
            result.AddError("NPC del dialogo non trovato.");
            _activeDialogueNpcId = null;
            _activeDialogueOptions = null;
            return result;
        }
        
        // Mostra la risposta
        result.AddLine($"Tu: \"{selectedOption.Prompt}\"");
        result.AddLine($"{npc.Name}: \"{selectedOption.Response}\"", OutputLineType.Success);
        
        // Applica gli effetti tramite il DialogueSystem
        if (_dialogueSystem != null)
        {
            _dialogueSystem.ChooseOption(selectedOption.Id, _state, npc);
            
            // Mostra indicatori di cambiamento
            if (selectedOption.TrustDelta != 0)
            {
                string arrow = selectedOption.TrustDelta > 0 ? "↑" : "↓";
                result.AddLine($"  {arrow} Fiducia con {npc.Name}: {(selectedOption.TrustDelta > 0 ? "+" : "")}{selectedOption.TrustDelta}", OutputLineType.Subtle);
            }
            if (selectedOption.SuspicionDelta != 0)
            {
                string arrow = selectedOption.SuspicionDelta > 0 ? "↑" : "↓";
                result.AddLine($"  {arrow} Sospetto: {(selectedOption.SuspicionDelta > 0 ? "+" : "")}{selectedOption.SuspicionDelta}", OutputLineType.Subtle);
            }
            if (!string.IsNullOrEmpty(selectedOption.GrantsClueId))
            {
                if (_content.Clues.TryGetValue(selectedOption.GrantsClueId, out var clue))
                {
                    result.AddLine($"  🔍 Nuovo indizio: {clue.Title}", OutputLineType.Success);
                }
            }
        }
        
        // Resetta lo stato del dialogo
        _activeDialogueNpcId = null;
        _activeDialogueOptions = null;
        
        return result;
    }
    
    private CommandResult ExecuteExamine(List<string> args)
    {
        var result = new CommandResult();
        
        if (args.Count == 0)
        {
            result.AddError("Cosa vuoi esaminare?");
            return result;
        }
        
        string itemName = string.Join(" ", args).ToLower();
        
        if (!_content.Rooms.TryGetValue(_state.CurrentRoomId, out var room))
        {
            result.AddError("Stanza attuale non trovata.");
            return result;
        }
        
        // Cerca nella stanza
        var itemId = room.ItemIds.FirstOrDefault(id =>
            _content.Items.TryGetValue(id, out var item) &&
            item.Name.ToLower().Contains(itemName)
        );
        
        // Cerca nell'inventario
        if (string.IsNullOrEmpty(itemId))
        {
            itemId = _state.InventoryItemIds.FirstOrDefault(id =>
                _content.Items.TryGetValue(id, out var item) &&
                item.Name.ToLower().Contains(itemName)
            );
        }
        
        if (string.IsNullOrEmpty(itemId) || !_content.Items.TryGetValue(itemId, out var item2))
        {
            result.AddLine("Non vedi questo qui.", OutputLineType.Warning);
            return result;
        }
        
        result.AddLine(AsciiArtProvider.GetItemArt(item2.Type.ToString()));
        result.AddLine(AsciiArtProvider.GetThinSeparator());
        result.AddLine($"{item2.Name}", OutputLineType.Success);
        result.AddLine(item2.Description);
        
        return result;
    }
    
    private CommandResult ExecuteLook()
    {
        var result = new CommandResult();
        result.AddLine(DescribeCurrentRoom());
        return result;
    }
    
    private CommandResult ExecuteRead(List<string> args)
    {
        var result = new CommandResult();
        
        if (args.Count == 0)
        {
            result.AddError("Cosa vuoi leggere?");
            return result;
        }
        
        // Controlla se l'ultimo arg è un numero di pagina
        int? pageNumber = null;
        string itemName;
        
        if (args.Count >= 2 && int.TryParse(args[^1], out int parsedPage))
        {
            pageNumber = parsedPage;
            itemName = string.Join(" ", args[..^1]).ToLower();
        }
        else
        {
            itemName = string.Join(" ", args).ToLower();
        }
        
        var itemId = _state.InventoryItemIds.FirstOrDefault(id =>
            _content.Items.TryGetValue(id, out var item) &&
            item.Type == ItemType.Document &&
            item.Name.ToLower().Contains(itemName)
        );
        
        if (string.IsNullOrEmpty(itemId) || !_content.Items.TryGetValue(itemId, out var item2))
        {
            result.AddLine("Non hai niente da leggere con quel nome.", OutputLineType.Warning);
            return result;
        }
        
        result.AddLine($"📖 [{item2.Name}]", OutputLineType.Success);
        
        if (item2.Pages <= 0)
        {
            result.AddLine(item2.Description);
            return result;
        }
        
        if (pageNumber == null)
        {
            result.AddLine($"Documento con {item2.Pages} pagine.");
            result.AddLine($"Usa 'leggi {args[0]} [1-{item2.Pages}]' per leggere una pagina.");
            
            // Mostra quali pagine sono state lette
            if (_state.ItemStates.TryGetValue(itemId, out var itemState))
            {
                var readPages = itemState.PagesRead.Where(p => p.Value).Select(p => p.Key).OrderBy(p => p).ToList();
                if (readPages.Count > 0)
                {
                    result.AddLine($"Pagine già lette: {string.Join(", ", readPages)}", OutputLineType.Subtle);
                }
            }
            return result;
        }
        
        if (pageNumber < 1 || pageNumber > item2.Pages)
        {
            result.AddLine($"Pagina non valida. Scegli tra 1 e {item2.Pages}.", OutputLineType.Warning);
            return result;
        }
        
        // Marca la pagina come letta
        if (_state.ItemStates.TryGetValue(itemId, out var state))
        {
            state.PagesRead[pageNumber.Value] = true;
        }
        
        result.AddLine($"--- Pagina {pageNumber}/{item2.Pages} ---");
        result.AddLine(item2.Description);
        
        return result;
    }
    
    private CommandResult ExecuteUse(List<string> args)
    {
        var result = new CommandResult();
        
        if (args.Count == 0)
        {
            result.AddError("Cosa vuoi usare?");
            return result;
        }
        
        // Cerca "usa [item] su [target]" o "usa [item]"
        string itemName;
        string? targetName = null;
        int suIndex = args.IndexOf("su");
        
        if (suIndex > 0 && suIndex < args.Count - 1)
        {
            itemName = string.Join(" ", args.Take(suIndex)).ToLower();
            targetName = string.Join(" ", args.Skip(suIndex + 1)).ToLower();
        }
        else
        {
            itemName = string.Join(" ", args).ToLower();
        }
        
        // Trova l'item nell'inventario
        var itemId = _state.InventoryItemIds.FirstOrDefault(id =>
            _content.Items.TryGetValue(id, out var item) &&
            item.Name.ToLower().Contains(itemName)
        );
        
        if (string.IsNullOrEmpty(itemId) || !_content.Items.TryGetValue(itemId, out var usedItem))
        {
            result.AddLine("Non hai questo oggetto.", OutputLineType.Warning);
            return result;
        }
        
        // Se è una chiave, controlla se c'è una stanza bloccata accessibile
        if (usedItem.Type == ItemType.Key)
        {
            if (_content.Rooms.TryGetValue(_state.CurrentRoomId, out var room))
            {
                bool usedKey = false;
                foreach (var exit in room.Exits)
                {
                    if (_content.Rooms.TryGetValue(exit.Value, out var targetRoom) &&
                        !string.IsNullOrEmpty(targetRoom.AccessRequiredFlag) &&
                        !_state.Flags.GetValueOrDefault(targetRoom.AccessRequiredFlag, false))
                    {
                        _state.Flags[targetRoom.AccessRequiredFlag] = true;
                        result.AddLine($"🔑 Hai usato {usedItem.Name}! Una via si è sbloccata.", OutputLineType.Success);
                        usedKey = true;
                        break;
                    }
                }
                if (!usedKey)
                {
                    result.AddLine("Non c'è nulla da sbloccare qui con questa chiave.", OutputLineType.Warning);
                }
            }
            return result;
        }
        
        // Se è un oggetto collegato a un indizio
        if (!string.IsNullOrEmpty(usedItem.LinkedClueId))
        {
            if (!_state.FoundClues.ContainsKey(usedItem.LinkedClueId))
            {
                _state.FoundClues[usedItem.LinkedClueId] = true;
                if (_content.Clues.TryGetValue(usedItem.LinkedClueId, out var clue))
                {
                    result.AddLine($"🔍 Hai scoperto un indizio: {clue.Title}", OutputLineType.Success);
                    result.AddLine($"   {clue.Description}");
                }
                else
                {
                    result.AddLine($"Hai scoperto un indizio!", OutputLineType.Success);
                }
            }
            else
            {
                result.AddLine("Hai già trovato questo indizio.", OutputLineType.Subtle);
            }
            return result;
        }
        
        // Se è un oggetto futuro
        if (usedItem.Type == ItemType.Future && usedItem.FutureYear.HasValue)
        {
            if (_state.ItemStates.TryGetValue(itemId, out var itemState))
            {
                itemState.TimesSeen++;
            }
            _state.FutureObjectsShown++;
            result.AddLine($"⏳ Una visione dell'anno {usedItem.FutureYear}...", OutputLineType.Success);
            result.AddLine(usedItem.Description);
            return result;
        }
        
        result.AddLine($"Non sai come usare {usedItem.Name} in questo momento.", OutputLineType.Warning);
        return result;
    }
    
    private CommandResult ExecuteOpen(List<string> args)
    {
        var result = new CommandResult();
        
        if (args.Count == 0)
        {
            result.AddError("Cosa vuoi aprire?");
            return result;
        }
        
        string itemName = string.Join(" ", args).ToLower();
        
        if (!_content.Rooms.TryGetValue(_state.CurrentRoomId, out var room))
        {
            result.AddError("Stanza attuale non trovata.");
            return result;
        }
        
        // Cerca nella stanza o nell'inventario
        var itemId = room.ItemIds.Concat(_state.InventoryItemIds).FirstOrDefault(id =>
            _content.Items.TryGetValue(id, out var item) &&
            item.Type == ItemType.Container &&
            item.Name.ToLower().Contains(itemName)
        );
        
        if (string.IsNullOrEmpty(itemId) || !_content.Items.TryGetValue(itemId, out var container))
        {
            result.AddLine("Non vedi niente da aprire con quel nome.", OutputLineType.Warning);
            return result;
        }
        
        // Verifica se serve un flag per aprire
        if (!string.IsNullOrEmpty(container.OpenRequiredFlag) &&
            !_state.Flags.GetValueOrDefault(container.OpenRequiredFlag, false))
        {
            result.AddLine("È chiuso a chiave. Ti serve qualcosa per aprirlo.", OutputLineType.Warning);
            return result;
        }
        
        // Apri il contenitore
        if (_state.ItemStates.TryGetValue(itemId, out var itemState))
        {
            if (itemState.ContainerOpen)
            {
                result.AddLine("È già aperto.", OutputLineType.Subtle);
                return result;
            }
            itemState.ContainerOpen = true;
        }
        
        result.AddLine($"Hai aperto {container.Name}.", OutputLineType.Success);
        
        // Mostra il contenuto
        if (container.Contains.Count > 0)
        {
            result.AddLine("Contiene:");
            foreach (var containedId in container.Contains)
            {
                if (_content.Items.TryGetValue(containedId, out var contained))
                {
                    result.AddLine($"  • {contained.Name}");
                    // Rendi gli oggetti raccoglibili dalla stanza
                    if (!room.ItemIds.Contains(containedId))
                    {
                        room.ItemIds.Add(containedId);
                    }
                }
            }
        }
        else
        {
            result.AddLine("È vuoto.", OutputLineType.Subtle);
        }
        
        return result;
    }
    
    private CommandResult ExecuteClose(List<string> args)
    {
        var result = new CommandResult();
        
        if (args.Count == 0)
        {
            result.AddError("Cosa vuoi chiudere?");
            return result;
        }
        
        string itemName = string.Join(" ", args).ToLower();
        
        if (!_content.Rooms.TryGetValue(_state.CurrentRoomId, out var room))
        {
            result.AddError("Stanza attuale non trovata.");
            return result;
        }
        
        var itemId = room.ItemIds.Concat(_state.InventoryItemIds).FirstOrDefault(id =>
            _content.Items.TryGetValue(id, out var item) &&
            item.Type == ItemType.Container &&
            item.Name.ToLower().Contains(itemName)
        );
        
        if (string.IsNullOrEmpty(itemId) || !_content.Items.TryGetValue(itemId, out var container))
        {
            result.AddLine("Non vedi niente da chiudere con quel nome.", OutputLineType.Warning);
            return result;
        }
        
        if (_state.ItemStates.TryGetValue(itemId, out var itemState))
        {
            if (!itemState.ContainerOpen)
            {
                result.AddLine("È già chiuso.", OutputLineType.Subtle);
                return result;
            }
            itemState.ContainerOpen = false;
        }
        
        result.AddLine($"Hai chiuso {container.Name}.", OutputLineType.Success);
        return result;
    }
}
