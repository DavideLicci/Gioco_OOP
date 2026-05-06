# 📋 Linee Guida per gli Altri Componenti del Team

> Questo documento fornisce istruzioni specifiche su cosa fare per completare le vostre parti del gioco.
> Leggete prima `DADDA_GUIDE.md` e `TEAM_DIRECTIVES.md` per il contesto completo.

---

## 🎯 Cosa Ho Completato (DADDA)

Ho aggiornato il sistema per supportare **alias dinamici configurabili da JSON**:

### 1. Interfaccia Aggiornata (`IGameEngine.cs`)
```csharp
public interface ICommandParser
{
    ParsedCommand Parse(string input);
    void SetDynamicAliases(Dictionary<string, string> aliases);  // NUOVO
}
```

### 2. CommandParser Aggiornato (`CommandParser.cs`)
- Aggiunto `_dynamicAliases` field per alias dal JSON
- Aggiunto metodo `SetDynamicAliases()` per caricare alias da ContentLoader
- Gli alias dinamici hanno **priorità** sugli alias hardcoded

### Cosa Dovete Fare Voi

---

## 🔧 NICK — SaveService, ContentLoader, ProgressionService

### 1. Aggiornare `ContentLoader.cs`
Devi caricare gli alias dal JSON:

```csharp
// In ContentLoader.cs
public GameContent Load(string filePath)
{
    var json = File.ReadAllText(filePath);
    var content = JsonSerializer.Deserialize<GameContent>(json);
    
    // 🔑 AGGIUNGERE: Carica gli alias dal JSON
    if (content.CommandAliases != null)
    {
        // Passa gli alias al parser
        // Qui serve un metodo nel GameEngine per impostare gli alias
    }
    
    return content;
}
```

### 2. Aggiornare `GameContent.cs` e `ContentDefinitions.cs`
Devi aggiungere la struttura per gli alias:

```csharp
// In GameContent.cs
public Dictionary<string, string>? CommandAliases { get; set; }
```

### 3. Aggiornare `game-content.json`
Aggiungi la sezione alias:

```json
{
  "commandAliases": {
    "vai": "north",
    "cammini": "walk",
    "cerca": "examine"
  }
}
```

---

## 💬 VALKIRIA — DialogueSystem

### Cosa Fare
Il tuo `DialogueSystem` è già ben strutturato. Assicurati che:

1. **`StartDialogue()`** filtri correttamente le opzioni in base a:
   - `RequiredFlags` - mostra solo se i flag sono true
   - `Repeatable` - nasconde opzioni già usate
   - `minSuspicion` / `maxSuspicion` - filtra per suspicion

2. **`ChooseOption()`** applichi gli effetti:
   - Aggiorna trust NPC
   - Imposta i granted flags
   - Aggiunge clue se presente

### TODO Future (se hai tempo)
- Aggiungi `ForbidFlags` - nasconde opzioni se certi flag sono attivi
- Implementa Strategy Pattern per comportamenti NPC diversi

---

## 🎒 ALICE — InventorySystem

### Cosa Fare
Il tuo sistema è già funzionante. Assicurati che:

1. **`Add()`** controlli il peso massimo
2. **`List()`** mostri gli item in modo ordinato
3. **`GetTotalWeight()`** calcoli correttamente il peso

### Coordination con DADDA
Quando DADDA chiama `ExecuteUse()`, lui gestisce:
- Chiavi → sblocco stanze
- Oggetti con LinkedClueId → scoperta indizio
- Oggetti Future → visione temporale

**Tu puoi gestire** (opzionale):
- Combinazioni item su item (es. "usa chiave su cassetto")
- Contenitori (già implementato in ExecuteUse/ExecuteOpen)

---

## 🏆 SOFIA — AchievementService, DeductionService

### 1. AchievementService
Assicurati che **`EvaluateNewUnlocks()`**:
- Controlli tutte le condizioni dell'achievement
- Non sblocchi due volte lo stesso achievement

### 2. DeductionService
Assicurati che **`Run()`**:
- Calcoli correttamente la somma di `Confidence` degli indizi
- Sblocchi solo quando `MinEvidenceScore` è raggiunto

### TODO Future (se hai tempo)
- Implementa Observer Pattern per monitorare i cambiamenti di stato
- Aggiungi condizioni achievement multiple

---

## 📝checkliste di Testing

### NICK
- [ ] SaveService.Save() crea file + .bak
- [ ] SaveService.Load() ripristina stato
- [ ] ContentLoader caricastanze, NPC, item
- [ ] ProgressionService.CheckLevelComplete() funziona

### VALKIRIA
- [ ] StartDialogue filtra per flag
- [ ] StartDialogue filtra per suspicion
- [ ] ChooseOption aggiorna trust
- [ ] ChooseOption imposta flags

### ALICE
- [ ] Add controlla peso massimo
- [ ] Remove rimuove item
- [ ] List mostra inventario
- [ ] GetTotalWeight calcola peso

### SOFIA
- [ ] EvaluateNewUnlocks sblocca trofei
- [ ] Describe mostra trofei
- [ ] DeductionService.Run() calcola score
- [ ] DescribeBoard mostra indizi

---

## 🚀 Come Testare

```bash
# Build
dotnet build

# Test
dotnet test

# Run
dotnet run
```

---

## 📞 Domande?

Se avete dubbi su come integrARVI con DADDA:
1. Leggete `DADDA_GUIDE.md` per capire come chiamo i vostri servizi
2. Guardate le interfacce in `src/Engine/I*.cs`
3. Chiedete sul gruppo WhatsApp

**Buon lavoro! 🎮**
