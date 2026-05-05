# OLTRE IL TEMPO — Guida di Sviluppo

## Panoramica

Un'avventura investigativa interattiva in C# (.NET 8) dove il giocatore deve risolvere un mistero attraverso l'esplorazione, i dialoghi e la deduzione logica.

## Architettura

Il progetto è organizzato in quattro layer principali:

```
Src/
├── Domain/          # Modelli e State del gioco
├── Engine/          # Logica principale
└── Content/         # JSON Loader

Content/             # File JSON di configurazione
OltreIlTempo.Tests/  # Test automatici con xUnit
```

## Ruoli e Responsabilità

### 👾 DADDA — GameEngine & Parser
- **File**: `Src/Engine/GameEngine.cs`, `CommandParser.cs`, `ConditionEvaluator.cs`
- **Responsabilità**: Core del gioco, coordinamento di tutti i servizi
- **Cosa fare**:
  - ✓ `GameEngine.Execute()` — riceve comando, lo processa, restituisce risultato
  - ✓ Movimento (n/s/e/o)
  - ✓ `DescribeCurrentRoom()` — descrizione stanza attuale
  - ✓ `FinalizeTurn()` — incrementa turno, triggera progressione
  - ✓ `CommandParser.Parse()` — splitta input in verb + args
  - ✓ `ConditionEvaluator.Evaluate()` — valuta condizioni flag
  - Suggerimento typo con distanza Levenshtein
- **Design Pattern**: Command, Interpreter

### 🔧 NICK — Save, Content & Progressione
- **File**: `Src/Engine/SaveService.cs`, `Src/Content/ContentLoader.cs`, `ProgressionService.cs`
- **Responsabilità**: Persistenza dati e caricamento contenuti
- **Cosa fare**:
  - ✓ `ContentLoader.Load()` — carica game-content.json
  - ✓ `SaveService.Save/Load()` — serializza/deserializza GameState
  - ✓ Versioning file di salvataggio
  - ✓ `ProgressionService.CheckLevelComplete()` — verifica completamento livello
  - Test: trust anti-farm, progressione lv1→lv2, save/load
- **Design Pattern**: Memento, Repository
- **Coordina con**: Dadda (SaveService.Save() ad ogni turno)

### 💬 VALKIRIA — Dialoghi & NPC
- **File**: `Src/Domain/DialogueSystem.cs`, `ContentDefinitions.cs` (parte NPC)
- **Responsabilità**: Tutto ciò che riguarda NPC e dialoghi
- **Cosa fare**:
  - ✓ `NpcDefinition` — classe che rappresenta un NPC dal JSON
  - ✓ `DialogueSystem.StartDialogue()` — filtra opzioni disponibili
  - ✓ `ChooseOption()` — applica scelta (trust, suspicion, flag, clue)
  - ✓ Anti-farm trust — ogni opzione non ripetibile conta una sola volta
  - ✓ Filtro opzioni (flag, suspicion range)
- **Design Pattern**: Strategy (comportamenti NPC diversi)
- **Coordina con**: Dadda (GameEngine chiama DialogueSystem)

### 🎒 ALICE — Inventario & Oggetti
- **File**: `Src/Domain/InventorySystem.cs`, `ItemDefinitions.cs`
- **Responsabilità**: Oggetti e sistema di inventario
- **Cosa fare**:
  - ✓ `ItemDefinition` — classe per JSON
  - ✓ `InventorySystem.Add/Remove()` — gestione inventario
  - ✓ Controllo peso massimo
  - ✓ `List()` — con filtri (documenti, chiavi, prove)
  - ✓ Container logic — oggetti che ne contengono altri
  - ✓ Oggetti futuri — visione temporale quando esaminati
- **Design Pattern**: Iterator, Decorator
- **Coordina con**: Dadda (comandi 'raccogli', 'usa', 'leggi')

### 🏆 SOFIA — Achievement, Deduzioni & Modelli
- **File**: `Src/Engine/AchievementService.cs`, `DeductionService.cs`, `Src/Domain/RuntimeModels.cs`
- **Responsabilità**: Trofei, deduzioni indizi, modelli dati condivisi
- **Cosa fare**:
  - ✓ `GameState` — stato principale (da usare da tutti!)
  - ✓ `CommandResult` — output dei comandi
  - ✓ `AchievementService.EvaluateNewUnlocks()` — unlocking automatico
  - ✓ `DeductionService.DescribeBoard()` / `Run()` — sistema indizi
  - ✓ Confidenza indizi — somma confidence per sbloccare deduzioni
- **Design Pattern**: Observer (monitora GameState)
- **CRITICO**: `RuntimeModels.cs` è usato da tutti!

## Convenzioni C#

```csharp
// Classi e metodi: PascalCase
public class GameEngine { }
public void Execute() { }

// Variabili private: _camelCase
private GameState _state;

// Variabili locali: camelCase
var currentRoom = _state.CurrentRoomId;

// Interfacce: prefisso I
public interface IGameEngine { }
```

## Flusso di Collaborazione

1. **Interfacce prima del codice**
   - Ogni persona definisce le interfacce pubbliche (es. Valkiria → `IDialogueSystem`)
   - Condividi con il gruppo prima di implementare
   - Dadda le usa in GameEngine senza aspettare l'implementazione

2. **Branch Git**
   ```bash
   git checkout -b feature/dadda
   git checkout -b feature/nick
   git checkout -b feature/valkiria
   git checkout -b feature/alice
   git checkout -b feature/sofia
   ```

3. **Pull Request e Review**
   - Quando finisci una feature, apri una PR
   - Chiedi review a Dadda o Nick
   - Merge solo dopo review

## Checkpoint Settimanali

- **Fine settimana 1**: Classi base create e compilabili (anche vuote)
- **Fine settimana 2**: Implementazione principale funzionante
- **Fine settimana 3**: Integrazione, test, debug, relazione PDF

## Come Compilare e Testare

```bash
# Compilare il progetto principale
dotnet build OltreIlTempo.csproj

# Eseguire il gioco
dotnet run --project OltreIlTempo.csproj

# Eseguire i test
dotnet test OltreIlTempo.Tests/OltreIlTempo.Tests.csproj

# Eseguire un test specifico
dotnet test --filter "TestName"
```

## Struttura del JSON

Il file `Content/game-content.json` contiene:
- **Rooms**: stanze, uscite, NPC, item
- **NPCs**: dialoghi con opzioni e effetti
- **Items**: oggetti raccoglibili, documenti, chiavi, contenitori
- **Clues**: indizi con qualità e confidenza
- **Achievements**: trofei con condizioni
- **Levels**: livelli con condizioni di completamento

Tutti devono leggere e capire questa struttura!

## Domande Frequenti

- **Dove metto il mio codice?** Vedi la tabella nella sezione Ruoli
- **Come chiamo i servizi degli altri?** Tramite le interfacce: `IDialogueSystem`, `IInventorySystem`, ecc.
- **Cosa succede se la versione del save non corrisponde?** Nick gestisce questo — avvisa l'utente e offre una nuova partita
- **Come funziona il trust anti-farm?** Nick traccia le opzioni già usate in `NpcRuntimeState.UsedDialogueOptions`

---

**Buona fortuna! 🎮**
