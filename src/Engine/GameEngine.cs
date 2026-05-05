namespace OltreIlTempo.Engine;

using OltreIlTempo.Domain;

/// <summary>
/// Motore principale del gioco (Command Pattern).
/// DADDA: implementa la logica centrale del gioco.
/// Coordina tutti gli altri servizi e gestisce i comandi.
/// </summary>
public class GameEngine : IGameEngine
{
    private GameState _state = new();
    private GameContent _content = new();
    private GameConfig _config = new();
    private readonly ICommandParser _parser;
    private readonly IConditionEvaluator _evaluator;
    
    // Servizi opzionali (implementati da altri)
    private ISaveService? _saveService;
    private IContentLoader? _contentLoader;
    private IProgressionService? _progressionService;
    private IDialogueSystem? _dialogueSystem;
    private IInventorySystem? _inventorySystem;
    private IAchievementService? _achievementService;
    private IDeductionService? _deductionService;
    
    // Stato del dialogo attivo (per il flusso parla → scegli)
    private string? _activeDialogueNpcId;
    private List<DialogueOptionDefinition>? _activeDialogueOptions;
    
    // Mappe dei comandi (Command Pattern)
    private Dictionary<string, Func<List<string>, CommandResult>> _commandDispatcher = new();
    
    public GameEngine(GameContent content, GameConfig config)
    {
        _content = content;
        _config = config;
        _state = InitializeGameState(content, config);
        
        // Inizializza i parser e evaluator (sempre necessari)
        _parser = new CommandParser();
        _evaluator = new ConditionEvaluator();
        
        // Registra i comandi disponibili
        RegisterCommands();
    }
    
    /// <summary>
    /// Registra i servizi opzionali di altri moduli.
    /// </summary>
    public void RegisterServices(
        ISaveService? saveService = null,
        IContentLoader? contentLoader = null,
        IProgressionService? progressionService = null,
        IDialogueSystem? dialogueSystem = null,
        IInventorySystem? inventorySystem = null,
        IAchievementService? achievementService = null,
        IDeductionService? deductionService = null)
    {
        _saveService = saveService;
        _contentLoader = contentLoader;
        _progressionService = progressionService;
        _dialogueSystem = dialogueSystem;
        _inventorySystem = inventorySystem;
        _achievementService = achievementService;
        _deductionService = deductionService;
    }
    
    /// <summary>
    /// Registra i comandi disponibili nel dispatcher (Command Pattern).
    /// </summary>
    private void RegisterCommands()
    {
        _commandDispatcher = new Dictionary<string, Func<List<string>, CommandResult>>
        {
            // Movimento
            { "north", args => ExecuteMove("n") },
            { "south", args => ExecuteMove("s") },
            { "east", args => ExecuteMove("e") },
            { "west", args => ExecuteMove("o") },
            { "forward", args => ExecuteMove("n") },
            
            // Azioni
            { "take", ExecuteCollect },
            { "collect", ExecuteCollect },
            { "drop", ExecuteDrop },
            { "talk", ExecuteTalk },
            { "ask", ExecuteTalk },
            { "examine", ExecuteExamine },
            { "look", args => ExecuteLook() },
            { "read", ExecuteRead },
            { "use", ExecuteUse },
            { "open", ExecuteOpen },
            { "close", ExecuteClose },
            { "choose", ExecuteChoose },
            
            // Inventario e status
            { "inventory", args => ExecuteInventory(null) },
            { "status", args => ExecuteStatus() },
            
            // Sistema di gioco
            { "board", args => ExecuteBoard() },
            { "deduce", args => ExecuteDeduce() },
            { "achievements", args => ExecuteAchievements() },
            { "quest", args => ExecuteQuest() },
            { "save", ExecuteSave },
            { "load", ExecuteLoad },
            { "help", args => ExecuteHelp() },
            { "quit", args => ExecuteQuit() },
            { "exit", args => ExecuteQuit() }
        };
    }
    
    public CommandResult Execute(string input)
    {
        var result = new CommandResult();
        
        if (string.IsNullOrWhiteSpace(input))
        {
            result.AddError("Comando vuoto.");
            return result;
        }
        
        var parsed = _parser.Parse(input);
        
        if (!parsed.IsValid)
        {
            if (!string.IsNullOrEmpty(parsed.Suggestion))
            {
                result.AddError($"Comando '{parsed.Verb}' non riconosciuto. Intendevi '{parsed.Suggestion}'?");
            }
            else
            {
                result.AddError($"Comando '{parsed.Verb}' non riconosciuto.");
            }
            result.AddLine("Digita 'aiuto' per una lista di comandi disponibili.", OutputLineType.Subtle);
            return result;
        }
        
        try
        {
            // Dispatch al comando appropriato
            if (_commandDispatcher.TryGetValue(parsed.Verb, out var commandHandler))
            {
                result = commandHandler(parsed.Args);
            }
            else
            {
                result.AddError($"Comando '{parsed.Verb}' non gestito.");
            }
            
            // Finalizza il turno (incrementa turno, progressione, achievement)
            if (result.IsValid)
            {
                FinalizeTurn(result);
            }
        }
        catch (Exception ex)
        {
            result.AddError($"Errore durante l'esecuzione del comando: {ex.Message}");
        }
        
        return result;
    }
    
    public GameState GetGameState() => _state;
    
    public string DescribeCurrentRoom()
    {
        if (!_content.Rooms.TryGetValue(_state.CurrentRoomId, out var room))
        {
            return "Sei in una stanza sconosciuta.";
        }
        
        bool alreadyVisited = _state.UnlockedRooms.GetValueOrDefault(_state.CurrentRoomId, false);
        
        string description;
        if (alreadyVisited)
        {
            description = $"📍 {room.Name}\nSei tornato qui. {room.Description}\n";
        }
        else
        {
            description = $"📍 {room.Name}\n{room.Description}\n";
        }
        
        // Aggiungi oggetti nella stanza
        if (room.ItemIds.Count > 0)
        {
            var items = room.ItemIds
                .Where(itemId => _content.Items.ContainsKey(itemId))
                .Select(itemId => _content.Items[itemId].Name)
                .ToList();
            
            if (items.Count > 0)
            {
                description += $"\n🔍 Oggetti: {string.Join(", ", items)}";
            }
        }
        
        // Aggiungi NPC nella stanza
        if (room.NpcIds.Count > 0)
        {
            var npcs = room.NpcIds
                .Where(npcId => _content.Npcs.ContainsKey(npcId))
                .Select(npcId => _content.Npcs[npcId].Name)
                .ToList();
            
            if (npcs.Count > 0)
            {
                description += $"\n👤 Persone: {string.Join(", ", npcs)}";
            }
        }
        
        // Aggiungi uscite con nomi leggibili
        if (room.Exits.Count > 0)
        {
            var directionNames = room.Exits.Keys.Select(d => d switch
            {
                "n" => "Nord",
                "s" => "Sud",
                "e" => "Est",
                "o" => "Ovest",
                _ => d
            });
            description += $"\n🚪 Uscite: {string.Join(", ", directionNames)}";
        }
        
        return description;
    }
    
    // ==================== Comandi di Movimento ====================
    
    private CommandResult ExecuteMove(string direction)
    {
        var result = new CommandResult();
        
        if (!_content.Rooms.TryGetValue(_state.CurrentRoomId, out var currentRoom))
        {
            result.AddError("Stanza attuale non trovata.");
            return result;
        }
        
        if (!currentRoom.Exits.TryGetValue(direction, out var nextRoomId))
        {
            result.AddLine("Non puoi andare in quella direzione.", OutputLineType.Warning);
            return result;
        }
        
        if (!_content.Rooms.TryGetValue(nextRoomId, out var nextRoom))
        {
            result.AddError("Destinazione non trovata.");
            return result;
        }
        
        // Controlla se la stanza è bloccata
        if (!string.IsNullOrEmpty(nextRoom.AccessRequiredFlag))
        {
            if (!_state.Flags.GetValueOrDefault(nextRoom.AccessRequiredFlag, false))
            {
                result.AddLine("La stanza è bloccata. Ti serve qualcosa per entrarvi.", OutputLineType.Warning);
                return result;
            }
        }
        
        // Movimento riuscito
        _state.CurrentRoomId = nextRoomId;
        _state.UnlockedRooms[nextRoomId] = true;
        
        // Mostra la descrizione della nuova stanza
        result.AddLine($"Vai verso {direction}.", OutputLineType.Success);
        result.AddLine(DescribeCurrentRoom());
        
        return result;
    }
    
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
        
        result.AddLine($"💬 {npc2.Name}:", OutputLineType.Success);
        result.AddLine("Scegli un'opzione di dialogo:");
        for (int i = 0; i < options.Count; i++)
        {
            result.AddLine($"  [{i + 1}] {options[i].Prompt}");
        }
        result.AddLine("\n(Usa 'scegli [numero]' per rispondere)", OutputLineType.Subtle);
        
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
    
    // ==================== Comandi di Dialogo ====================
    
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
        
        result.AddLine("[Inventario]", OutputLineType.Success);
        foreach (var item in items)
        {
            result.AddLine($"  • {item.Name} (peso: {item.Weight})kg");
        }
        
        var totalWeight = _inventorySystem.GetTotalWeight(_state, _content);
        result.AddLine($"\nPeso totale: {totalWeight:F1}kg / {_config.MaxInventoryWeight}kg");
        
        return result;
    }
    
    private CommandResult ExecuteStatus()
    {
        var result = new CommandResult();
        
        result.AddLine("[Status di Gioco]", OutputLineType.Success);
        result.AddLine($"Turno: {_state.TurnNumber}");
        result.AddLine($"Livello: {_state.CurrentLevel}");
        result.AddLine($"Suspicion: {_state.Suspicion}/100");
        result.AddLine($"Indizi trovati: {_state.FoundClues.Count}");
        
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
        
        result.AddLine("[Comandi Disponibili]", OutputLineType.Success);
        result.AddLine("\n=== Movimento ===");
        result.AddLine("  n/nord, s/sud, e/est, o/ovest  - Muoviti in una direzione");
        result.AddLine("\n=== Azioni ===");
        result.AddLine("  raccogli [oggetto]   - Raccogli un oggetto");
        result.AddLine("  lascia [oggetto]     - Lascia un oggetto");
        result.AddLine("  parla [persona]      - Parla con una persona");
        result.AddLine("  scegli [numero]      - Scegli un'opzione di dialogo");
        result.AddLine("  esamina [oggetto]    - Esamina un oggetto");
        result.AddLine("  leggi [doc] [pagina] - Leggi un documento (pagina opzionale)");
        result.AddLine("  usa [oggetto]        - Usa un oggetto");
        result.AddLine("  usa [obj] su [obj]   - Usa un oggetto su un altro");
        result.AddLine("  apri [contenitore]   - Apri un contenitore");
        result.AddLine("  chiudi [contenitore] - Chiudi un contenitore");
        result.AddLine("  guarda               - Guarda intorno");
        result.AddLine("\n=== Sistema ===");
        result.AddLine("  inventario           - Mostra inventario");
        result.AddLine("  stato                - Mostra lo stato del gioco");
        result.AddLine("  missione             - Mostra la missione corrente");
        result.AddLine("  bacheca              - Mostra gli indizi trovati");
        result.AddLine("  deduci               - Sblocca deduzioni");
        result.AddLine("  trofei               - Mostra i trofei");
        result.AddLine("  salva [file]         - Salva il gioco");
        result.AddLine("  carica [file]        - Carica il gioco");
        result.AddLine("  aiuto                - Mostra questo messaggio");
        
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
    
    private CommandResult ExecuteQuit()
    {
        var result = new CommandResult();
        result.AddLine("Grazie per aver giocato! Arrivederci.", OutputLineType.Success);
        return result;
    }
    
    // ==================== Logica di Turno ====================
    
    private void FinalizeTurn(CommandResult result)
    {
        _state.TurnNumber++;
        
        // Controlla la progressione del livello
        if (_progressionService != null)
        {
            if (_progressionService.CheckLevelComplete(_state, _content))
            {
                result.AddLine($"🎉 Hai completato il livello {_state.CurrentLevel - 1}!", OutputLineType.Success);
                result.AddLine($"Sei passato al livello {_state.CurrentLevel}.", OutputLineType.Success);
            }
        }
        
        // Valuta nuovi achievement
        if (_achievementService != null)
        {
            var newUnlocks = _achievementService.EvaluateNewUnlocks(_state, _content);
            foreach (var unlock in newUnlocks)
            {
                result.AddLine(unlock, OutputLineType.Success);
            }
        }
        
        // Autosave
        if (_saveService != null && _state.TurnNumber % _config.AutosaveFrequency == 0)
        {
            try
            {
                _saveService.Save(_state, "autosave.json");
            }
            catch { /* Silenzio gli errori di autosave */ }
        }
    }
    
    private GameState InitializeGameState(GameContent content, GameConfig config)
    {
        var state = new GameState
        {
            CurrentRoomId = content.StartingRoomId,
            TurnNumber = 0,
            Suspicion = 0,
            CurrentLevel = 1
        };
        
        // Inizializza il trust dei NPC
        foreach (var npc in content.Npcs)
        {
            state.NpcTrust[npc.Key] = 0;
            state.NpcStates[npc.Key] = new NpcRuntimeState
            {
                NpcId = npc.Key,
                CurrentRoomId = npc.Value.RoomId,
                Trust = 0
            };
        }
        
        // Inizializza gli item
        foreach (var item in content.Items)
        {
            var room = content.Rooms.Values.FirstOrDefault(r => r.ItemIds.Contains(item.Key));
            state.ItemStates[item.Key] = new ItemRuntimeState
            {
                ItemId = item.Key,
                Location = room != null ? $"room_{room.Id}" : "unknown"
            };
        }
        
        // Inizializza le stanze sbloccate
        foreach (var room in content.Rooms)
        {
            state.UnlockedRooms[room.Key] = string.IsNullOrEmpty(room.Value.AccessRequiredFlag);
        }
        
        return state;
    }
}
