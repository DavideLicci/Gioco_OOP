# 👾 DADDA — Guida Completa

## Il Tuo Ruolo

Sei il **core** del gioco. Il tuo codice è il cuore che fa battere l'intero progetto. Tutte le altre persone dipendono da te per:

- **Esecuzione comandi**: Ricevi stringa → Esegui → Restituisci risultato
- **Coordinamento**: Chiami i servizi degli altri
- **Gestione turni**: Incrementi turno, progressione, achievement
- **Parsing**: Converti input utente in comandi strutturati
- **Valutazione condizioni**: Determini se un flag è vero, se un NPC ha abbastanza trust, ecc.

## File Tuoi

### 1. **CommandParser.cs** (260+ righe)

#### Cosa Fa
Converte stringhe di input (es. "raccogli coltello") in comandi strutturati.

#### Alias Supportati
```csharp
"n" → "north"
"s" → "south"
"e" → "east"
"o" → "west"
"nord" → "north"
"sud" → "south"
"est" → "east"
"ovest" → "west"
"raccogli" → "collect"
"prendi" → "take"
"parla" → "talk"
"esamina" → "examine"
"guarda" → "look"
"leggi" → "read"
"usa" → "use"
"apri" → "open"
"chiudi" → "close"
"inventario" → "inventory"
"inv" → "inventory"
"i" → "inventory"
"aiuto" → "help"
"?" → "help"
"esci" → "quit"
// ... e altri
```

#### Distanza Levenshtein
Se scrivi "ques" anziché "quest", il parser suggerisce il comando più simile!

```
Giocatore scrive: "ques"
Parser calcola: Levenshtein("ques", "quest") = 1 (molto simile)
Output: ⚠️ Comando non riconosciuto. Intendevi "quest"?
```

#### Uso
```csharp
var parser = new CommandParser();
var parsed = parser.Parse("raccogli coltello");
// parsed.Verb = "collect"
// parsed.Args = ["coltello"]
// parsed.IsValid = true
```

---

### 2. **ConditionEvaluator.cs** (350+ righe) — **Interpreter Pattern**

#### Cosa Fa
Legge condizioni dal JSON (come stringhe) e le interpreta per determinare vero/falso.

#### Tipi di Condizioni Supportate

```csharp
// Flag boolean
"flag_true:has_key_1"           // true se il flag esiste e vale true
"flag_false:is_suspicious"       // true se il flag è false o non esiste

// Trust NPC
"trust_at_least:npc_cook:50"    // true se cook ha almeno 50 di trust
"trust_at_most:npc_cook:80"     // true se cook ha al massimo 80 di trust
"npc_trust_range:npc_cook:30:70" // true se trust è tra 30 e 70

// Suspicion globale
"suspicion_at_least:25"          // true se suspicion >= 25
"suspicion_at_most:75"           // true se suspicion <= 75

// Indizi
"clue_found:clue_knife"          // true se l'indizio è stato trovato

// Inventario
"inventory_has:item_key_1"       // true se l'oggetto è nell'inventario

// Stanze
"current_room:room_kitchen"      // true se sei in questa stanza
"visited_room:room_library"      // true se hai visitato la stanza

// Achievement
"achievement_unlocked:ach_speedrunner" // true se il trofeo è sbloccato
```

#### Uso
```csharp
var evaluator = new ConditionEvaluator();
var state = gameState;

// Nel JSON:
// "completionConditions": [
//   { "type": "flag_true", "flagName": "solved_mystery" },
//   { "type": "trust_at_least", "npcId": "npc_cook", "minimumValue": 50 }
// ]

bool isSolved = evaluator.Evaluate("flag_true:solved_mystery", state);
bool isCooperative = evaluator.Evaluate("trust_at_least:npc_cook:50", state);
```

---

### 3. **GameEngine.cs** (850+ righe) — **Command Pattern**

#### Cosa Fa
Il cuore del gioco. Esegue comandi e coordina tutto.

#### Architettura

```
Input Utente
    ↓
CommandParser (tokenizza)
    ↓
Dispatcher dei Comandi (20+ comandi)
    ↓
Esecuzione (movimento, dialogo, inventario, ecc.)
    ↓
FinalizeTurn() (progressione, achievement, autosave)
    ↓
Output (CommandResult)
```

#### Comandi Principali

##### Movimento
```csharp
// "n", "south", "e", "ovest"
ExecuteMove(direction)
  - Controlla se l'uscita esiste
  - Controlla se la stanza è bloccata (requiresFlag)
  - Aggiorna CurrentRoomId
  - Marchia la stanza come visitata
  - Mostra descrizione della nuova stanza
```

##### Raccolta Oggetti
```csharp
// "raccogli coltello"
ExecuteCollect(args)
  - Trova l'oggetto nella stanza attuale
  - Chiama inventorySystem.Add()
  - Controlla limite peso
  - Rimuove dalla stanza
```

##### Dialoghi
```csharp
// "parla marco"
ExecuteTalk(args)
  - Trova l'NPC nella stanza
  - Chiama dialogueSystem.StartDialogue()
  - Filtra opzioni disponibili
  - Mostra menu numerico [1] [2] [3]...
```

##### Inventario
```csharp
// "inventario"
ExecuteInventory()
  - Chiama inventorySystem.List()
  - Mostra peso totale
  - Mostra peso massimo
```

##### Sistema
```csharp
// "bacheca" → DeductionService.DescribeBoard()
// "deduci" → DeductionService.Run()
// "trofei" → AchievementService.Describe()
// "stato" → Mostra turno, livello, suspicion, indizi
// "salva [file]" → SaveService.Save()
// "carica [file]" → SaveService.Load()
```

#### Gestione Turni

Dopo ogni comando valido:
```csharp
FinalizeTurn()
  1. _state.TurnNumber++
  2. Controlla progressione livello (ProgressionService.CheckLevelComplete)
  3. Se livello completato → Messaggio e avanzamento
  4. Valuta nuovi achievement (AchievementService.EvaluateNewUnlocks)
  5. Se ogni N turni → Autosave
```

#### Dispatcher (Command Pattern)

```csharp
_commandDispatcher = new Dictionary<string, Func<List<string>, CommandResult>>
{
    { "north", args => ExecuteMove("n") },
    { "south", args => ExecuteMove("s") },
    { "collect", ExecuteCollect },
    { "talk", ExecuteTalk },
    { "inventory", args => ExecuteInventory(null) },
    // ... 20+ comandi
};

// In Execute():
if (_commandDispatcher.TryGetValue(parsed.Verb, out var handler))
    result = handler(parsed.Args);
```

---

## Pattern Implementati

### 1. **Command Pattern**
Ogni comando è una funzione nel dispatcher. Facile da estendere!

```csharp
// Aggiungere un nuovo comando:
{ "nuovocomando", ExecuteNuovoComando }

private CommandResult ExecuteNuovoComando(List<string> args) { ... }
```

### 2. **Interpreter Pattern**
Le condizioni sono stringhe che vengono "interpretate" dal ConditionEvaluator.

```csharp
// Nel JSON:
"condition": "flag_true:has_chiave"

// In C#:
evaluator.Evaluate("flag_true:has_chiave", state) // true/false
```

---

## Come Coordinare Con Gli Altri

### Con **NICK** (SaveService, ContentLoader, ProgressionService)
```csharp
// Tu chiami i suoi servizi:
_progressionService.CheckLevelComplete(_state, _content);
_saveService.Save(_state, "savegame.json");
_contentLoader.Load("game-content.json");
```

### Con **VALKIRIA** (DialogueSystem)
```csharp
// Tu chiami il suo servizio:
var options = _dialogueSystem.StartDialogue(npc, _state);
// Valkiria restituisce le opzioni disponibili
// Tu le mostri numericamente
```

### Con **ALICE** (InventorySystem)
```csharp
// Tu chiami il suo servizio:
_inventorySystem.Add(itemId, _state, _content);
_inventorySystem.Remove(itemId, _state);
_inventorySystem.List(_state, _content);
```

### Con **SOFIA** (AchievementService, DeductionService)
```csharp
// Tu chiami i suoi servizi nel FinalizeTurn:
var unlocks = _achievementService.EvaluateNewUnlocks(_state, _content);
var board = _deductionService.DescribeBoard(_state, _content);
```

---

## TODO / Completamenti Futuri

Questi sono placeholder — completa quando gli altri hanno finito!

- [ ] ExecuteUse() — Combinazioni oggetti
- [ ] ExecuteOpen()/ExecuteClose() — Contenitori
- [ ] ExecuteRead() con pagine — Lettura pagina per pagina
- [ ] Suggerimento typo nei comandi sconosciuti
- [ ] Sistema di alias dinamici (configurabili nel JSON?)

---

## Testing

Nel file `GameEngineTests.cs` ci sono test xUnit:

```bash
# Eseguire i test
dotnet test OltreIlTempo.Tests/OltreIlTempo.Tests.csproj

# Test specifici
dotnet test --filter "GameEngine_InitialState_IsValid"
dotnet test --filter "CommandParser_ParsesSimpleCommand"
```

Aggiungi più test per la tua logica!

---

## Checkpoint

### ✅ Fine Settimana 1
- [x] Classi base create
- [x] Interfacce definite
- [x] CommandParser funzionante
- [x] ConditionEvaluator funzionante
- [x] GameEngine framework pronto

### ✅ Fine Settimana 2
- [x] Tutti i comandi di movimento implementati
- [x] Tutti i comandi di azione implementati
- [x] Coordinamento con gli altri servizi
- [x] Gestione turni e progressione

### 🔄 Fine Settimana 3
- [ ] Integration testing
- [ ] Bug fixing
- [ ] Relazione PDF con UML

---

## Contatti Importanti

Se hai domande su:
- **Condizioni dal JSON?** → Chiedi a NICK
- **Struttura DialogueSystem?** → Chiedi a VALKIRIA
- **Come richiedere servizi?** → Chiedi agli altri nelle loro interfacce pubbliche
- **Problemi generali?** → Coordina con gli altri via WhatsApp

---

Buon lavoro! 🚀 Il tuo ruolo è il più critico — sei il collante che tiene tutto insieme.
