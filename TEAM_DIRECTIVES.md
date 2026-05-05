# 📋 Direttive per il Team — Oltre il Tempo

> Questo documento descrive **cosa deve fare ogni membro del team** e **come integrare il proprio codice** con il GameEngine (DADDA).
> Leggete anche il `DADDA_GUIDE.md` e il `README.md` per il contesto completo.

---

## 🔗 Regola Fondamentale: Le Interfacce

Ogni membro lavora **solo sulla propria implementazione**. Il contratto è definito dalle interfacce in `src/Engine/`:

| Ruolo    | Interfaccia                        | File di implementazione                   |
|----------|------------------------------------|-------------------------------------------|
| Nick     | `ISaveService`                     | `src/Engine/SaveService.cs`               |
| Nick     | `IContentLoader`                   | `src/Content/ContentLoader.cs`            |
| Nick     | `IProgressionService`              | `src/Engine/ProgressionService.cs`        |
| Valkiria | `IDialogueSystem`                  | `src/Domain/DialogueSystem.cs`            |
| Alice    | `IInventorySystem`                 | `src/Domain/InventorySystem.cs`           |
| Sofia    | `IAchievementService`              | `src/Engine/AchievementService.cs`        |
| Sofia    | `IDeductionService`                | `src/Engine/DeductionService.cs`          |

**⚠️ NON modificare**: `GameEngine.cs`, `CommandParser.cs`, `ConditionEvaluator.cs`, `IGameEngine.cs` — sono di DADDA.

---

## 👤 NICK — Persistenza, Caricamento, Progressione

### File: `SaveService.cs`
Il salvataggio attuale funziona con `System.Text.Json`. Migliora:

1. **File temporaneo + rinomina atomica** — Scrivi su un `.tmp`, poi rinomina a `.json`. Previene corruzione se il gioco crasha durante il salvataggio.
2. **Backup automatico `.bak`** — Prima di sovrascrivere, copia il vecchio file in `.bak`.
3. **Validazione al caricamento** — Dopo `JsonSerializer.Deserialize`, verifica che `state.CurrentRoomId` esista nel contenuto e che `InventoryItemIds` contenga solo ID validi.

```csharp
// Esempio di salvataggio sicuro:
public void SaveGame(GameState state, string filePath)
{
    var json = JsonSerializer.Serialize(state, _options);
    var tempPath = filePath + ".tmp";
    var bakPath = filePath + ".bak";
    
    File.WriteAllText(tempPath, json);
    
    if (File.Exists(filePath))
        File.Copy(filePath, bakPath, overwrite: true);
    
    File.Move(tempPath, filePath, overwrite: true);
}
```

### File: `ContentLoader.cs`
Il caricamento JSON è basilare. Migliora:

1. **Validazione del contenuto** — Dopo il caricamento, verifica che ogni `RoomDefinition.StartingRoomId` esista nelle room, che gli `NpcIds` puntino a NPC reali, ecc.
2. **Path relativo/assoluto** — Gestisci sia `Content/game-content.json` che un path assoluto passato come argomento.

### File: `ProgressionService.cs`
Il metodo `CheckLevelComplete` funziona ma ha un **problema**: non gestisce `MinimumValue` nelle condizioni di tipo `trust_at_least`. Il `ConditionEvaluator` attuale supporta solo `flag_true` e `clue_found`. Nick deve:

1. **Estendere `ConditionEvaluator`** (in coordinazione con DADDA) per supportare condizioni con `MinimumValue`.
2. Oppure implementare il check direttamente nel `ProgressionService` per i tipi speciali.

---

## 👤 VALKIRIA — Sistema di Dialogo

### File: `DialogueSystem.cs`

Attualmente il `DialogueSystem` filtra le opzioni in base a `RequiredFlags`, `Repeatable`, e range di `Suspicion`. Migliora:

### 1. Strategy Pattern per il comportamento NPC

Implementa diverse strategie basate su `NpcDefinition.DialogueStrategy`:

```csharp
public interface IDialogueStrategy
{
    List<DialogueOptionDefinition> FilterOptions(
        List<DialogueOptionDefinition> options,
        GameState state,
        NpcDefinition npc);
}

public class FriendlyStrategy : IDialogueStrategy
{
    // Mostra più opzioni, trust si alza più rapidamente
    public List<DialogueOptionDefinition> FilterOptions(...) { ... }
}

public class SuspiciousStrategy : IDialogueStrategy
{
    // Nasconde opzioni se la suspicion è alta, richiede più trust per sbloccare
    public List<DialogueOptionDefinition> FilterOptions(...) { ... }
}
```

### 2. Aggiungere `forbidFlags`

Nel filtro delle opzioni, aggiungi un campo `ForbidFlags` a `DialogueOptionDefinition` — se uno di quei flag è `true`, l'opzione viene nascosta. Serve per far sparire opzioni dopo certi eventi.

> **Coordinamento**: aggiungi `ForbidFlags` a `ContentDefinitions.cs` → `DialogueOptionDefinition`. Prima comunica con DADDA.

### 3. `ChooseOption()` — effetti

Il metodo `ChooseOption` deve:
- Aggiornare `NpcRuntimeState.TrustLevel` e `SuspicionLevel`
- Impostare i `GrantsFlags` nello stato
- Aggiungere il `GrantsClueId` ai `FoundClues`
- Marcare l'opzione come usata (se `Repeatable == false`)

Il GameEngine già chiama `ChooseOption` nel comando `scegli`.

---

## 👤 ALICE — Sistema Inventario

### File: `InventorySystem.cs`

Il sistema inventario gestisce raccolta, rilascio, e peso. Migliora:

### 1. `Use()` — Combinazione oggetti

Implementa la logica per combinare due item:
```csharp
public CommandResult Use(string itemId, string? targetId, GameState state, GameContent content)
{
    // 1. Trova l'item nell'inventario
    // 2. Se targetId è specificato, cerca il target (stanza o inventario)
    // 3. Controlla il Type dell'item:
    //    - Key → sblocca flag
    //    - Evidence con LinkedClueId → scopri indizio
    //    - Future → mostra visione
    // 4. Se combinazione valida, applica effetti
}
```

> **Nota**: Il GameEngine ha già una implementazione base di `ExecuteUse()`. Se vuoi delegare la logica al `InventorySystem`, coordina con DADDA.

### 2. Contenitori `Open/Close`

Il GameEngine gestisce `ExecuteOpen/Close`. L'`InventorySystem` deve supportare:
- `ContainerOpen` nello stato degli item (`ItemState.ContainerOpen`)
- Metodo `GetContainerContents(string containerId)` per elencare gli item dentro
- Metodo `TransferFromContainer(string containerId, string itemId, GameState state)` per spostare un item dal contenitore all'inventario

### 3. Iterator Pattern per l'inventario

Implementa `IEnumerable<ItemDefinition>` per permettere iterazione ordinata:
```csharp
public class InventoryIterator : IEnumerable<ItemDefinition>
{
    // Ordina per tipo, poi per nome
    // Filtra item "nascosti" se necessario
}
```

### 4. Peso e limiti

- `MaxWeight` è nel `GameConfig` (default 10.0)
- Controlla il peso prima di aggiungere un item
- Mostra il peso corrente / massimo nel comando `inventario`

---

## 👤 SOFIA — Achievement, Deduzioni, Modelli Runtime

### File: `AchievementService.cs`

Attualmente il check è basilare. Migliora:

### 1. Observer Pattern per GameState

Implementa un sistema di notifiche per quando lo stato cambia:
```csharp
public interface IGameStateObserver
{
    void OnFlagChanged(string flagName, bool value);
    void OnClueFound(string clueId);
    void OnRoomVisited(string roomId);
}
```

L'`AchievementService` implementa `IGameStateObserver` e controlla i trofei ad ogni evento, senza dover iterare tutti gli achievement ad ogni turno.

### 2. Condizioni multiple

Supporta diversi tipi di condizione in `AchievementCondition`:
- `clue_count_at_least` — almeno N indizi trovati
- `achievement_at_turn` — trofeo sbloccato entro il turno N
- `flag_true` — un certo flag è attivo
- `all_rooms_visited` — tutte le stanze visitate

### File: `DeductionService.cs`

### 3. Evidenza pesata

Il punteggio minimo (`MinEvidenceScore`) dovrebbe essere calcolato sommando la `Confidence` di ciascun `ClueDefinition` trovato:
```csharp
public bool CanDeduce(DeductionDefinition deduction, GameState state, GameContent content)
{
    var foundClues = deduction.RequiredClueIds
        .Where(id => state.FoundClues.ContainsKey(id))
        .ToList();
    
    if (foundClues.Count < deduction.RequiredClueIds.Count) return false;
    
    int totalConfidence = foundClues
        .Where(id => content.Clues.ContainsKey(id))
        .Sum(id => content.Clues[id].Confidence);
    
    return totalConfidence >= deduction.MinEvidenceScore;
}
```

### RuntimeModels — ⚠️ Attenzione

`RuntimeModels.cs` contiene `GameState`, `CommandResult`, `NpcRuntimeState`, `ParsedCommand`, `ItemState`. Se devi aggiungere campi:
- **OK** aggiungere proprietà nuove con valori di default
- **NON fare** cambiare il tipo o il nome di proprietà esistenti — romperebbe il GameEngine
- Prima comunica con DADDA

---

## 📝 Flusso di Lavoro Consigliato

1. **Leggi** la tua interfaccia in `src/Engine/I*Services.cs` o `src/Domain/I*.cs`
2. **Implementa** i metodi nel tuo file di classe
3. **Testa** con unit test in `OltreIlTempo.Tests/`
4. **Comunica** con DADDA prima di modificare `RuntimeModels.cs`, `ContentDefinitions.cs`, o `GameContent.cs`
5. **PR** con nome branch `feat/[tuo-ruolo]/[descrizione]` (es. `feat/nick/safe-save`)

## 🧪 Come Testare

```bash
# Build
dotnet build

# Test
dotnet test

# Run
dotnet run
```

Ogni membro dovrebbe creare un file test `OltreIlTempo.Tests/[Ruolo]Tests.cs` con test unitari per la propria implementazione.
