using OltreIlTempo.Content;
using OltreIlTempo.Domain;
using OltreIlTempo.Engine;

// Carica il contenuto
var contentLoader = new ContentLoader();
var content = contentLoader.Load("Content/game-content.json");

// Crea i servizi core
var parser = new CommandParser();
var evaluator = new ConditionEvaluator();

// Crea il game engine
var gameEngine = new GameEngine(content, content.Config, parser, evaluator);

// Registra i servizi opzionali
var saveService = new SaveService();
var progressionService = new ProgressionService(new ConditionEvaluator());
var dialogueSystem = new DialogueSystem();
var inventorySystem = new InventorySystem();
var achievementService = new AchievementService(new ConditionEvaluator());
var deductionService = new DeductionService();

gameEngine.RegisterServices(
    saveService, contentLoader, progressionService,
    dialogueSystem, inventorySystem, achievementService, deductionService
);

// ==================== INTRODUZIONE NARRATIVA ====================
Console.Clear();

// ASCII Art del titolo
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine(AsciiArtProvider.GetSplashScreen());
Console.ResetColor();

Console.ForegroundColor = ConsoleColor.DarkGray;
Console.WriteLine("    Un'avventura investigativa interattiva.");
Console.WriteLine("    Versione 1.0 — Progetto OOP 2024/2025");
Console.ResetColor();
Console.WriteLine();

// Mostra l'esterno della villa
Console.ForegroundColor = ConsoleColor.DarkYellow;
Console.WriteLine(AsciiArtProvider.GetVillaExterior());
Console.ResetColor();
Console.WriteLine();

// Mostra l'introduzione narrativa con effetto "typewriter"
var introLines = gameEngine.GetIntroduction();

foreach (var line in introLines)
{
    if (line.StartsWith("━") || line.StartsWith("╔") || line.StartsWith("╚"))
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
    }
    else if (line.Contains("⏳") || line.Contains("📋"))
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
    }
    else if (line.Contains("CRONONAUTA") || line.Contains("Agenzia Temporale"))
    {
        Console.ForegroundColor = ConsoleColor.Green;
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.White;
    }
    
    // Effetto typewriter per le righe narrative
    if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("━") && !line.StartsWith("╔") && !line.StartsWith("╚"))
    {
        foreach (char c in line)
        {
            Console.Write(c);
            Thread.Sleep(8); // Leggero ritardo per effetto drammatico
        }
        Console.WriteLine();
    }
    else
    {
        Console.WriteLine(line);
    }
}
Console.ResetColor();

// Chiedi al giocatore di continuare
Console.ForegroundColor = ConsoleColor.DarkGray;
Console.Write("  Premi [INVIO] per continuare...");
Console.ResetColor();
Console.ReadLine();

// ==================== TUTORIAL ====================
Console.Clear();
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine();
Console.WriteLine("  Vuoi seguire un breve tutorial? (s/n)");
Console.ResetColor();
Console.Write("> ");
string? tutorialChoice = Console.ReadLine();

bool runTutorial = !string.IsNullOrEmpty(tutorialChoice) && 
    (tutorialChoice.Trim().ToLower() == "s" || 
     tutorialChoice.Trim().ToLower() == "si" ||
     tutorialChoice.Trim().ToLower() == "sì" ||
     tutorialChoice.Trim().ToLower() == "y" ||
     tutorialChoice.Trim().ToLower() == "yes");

if (runTutorial)
{
    Console.Clear();
    var tutorialStart = gameEngine.StartTutorial();
    
    foreach (var line in tutorialStart)
    {
        if (line.StartsWith("╔") || line.StartsWith("╚") || line.StartsWith("║"))
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
        }
        else if (line.Contains("📌") || line.Contains("👉"))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.White;
        }
        Console.WriteLine(line);
    }
    Console.ResetColor();
    
    // Loop del tutorial
    while (gameEngine.IsTutorialActive)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("\n  [Tutorial] ");
        Console.ResetColor();
        Console.Write("> ");
        string? tutorialInput = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(tutorialInput))
            continue;
        
        // Permetti di saltare il tutorial
        if (tutorialInput.Trim().ToLower() == "skip" || 
            tutorialInput.Trim().ToLower() == "salta")
        {
            var skipLines = gameEngine.SkipTutorial();
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (var line in skipLines)
            {
                Console.WriteLine(line);
            }
            Console.ResetColor();
            break;
        }
        
        var tutorialResult = gameEngine.ProcessTutorialInput(tutorialInput);
        
        foreach (var line in tutorialResult.Lines)
        {
            if (line.Contains("✅"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if (line.Contains("❌") || line.Contains("⚠️"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else if (line.Contains("📌") || line.Contains("👉"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else if (line.StartsWith("╔") || line.StartsWith("╚") || line.StartsWith("║"))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine(line);
        }
        Console.ResetColor();
        
        if (tutorialResult.IsComplete)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("\n  Premi [INVIO] per iniziare l'indagine...");
            Console.ResetColor();
            Console.ReadLine();
            break;
        }
    }
}
else
{
    var skipLines = gameEngine.SkipTutorial();
    Console.ForegroundColor = ConsoleColor.Yellow;
    foreach (var line in skipLines)
    {
        Console.WriteLine(line);
    }
    Console.ResetColor();
}

// ==================== FUNZIONE DI RENDERING COLORATO ====================

/// <summary>
/// Renderizza una riga di testo con colori appropriati per l'ASCII art.
/// </summary>
void RenderColoredLine(string text)
{
    // Rileva il tipo di contenuto e applica il colore
    if (string.IsNullOrWhiteSpace(text))
    {
        Console.WriteLine();
        return;
    }
    
    string trimmed = text.TrimStart();
    
    // Bordi e cornici
    if (trimmed.StartsWith("┌") || trimmed.StartsWith("└") || 
        trimmed.StartsWith("│") || trimmed.StartsWith("╔") || 
        trimmed.StartsWith("╚") || trimmed.StartsWith("╠") ||
        trimmed.StartsWith("║") || trimmed.StartsWith("═") ||
        trimmed.StartsWith("━") || trimmed.StartsWith("─"))
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
    }
    // Grandi titoli block-letter (favorendo il ciano per i titoli)
    if (trimmed.Contains("██████") || trimmed.Contains("██  ██"))
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
    }
    // Blocchi di arte generica (scaffali, mobili, etc.)
    else if (trimmed.Contains("██") || trimmed.Contains("░░") || 
             trimmed.Contains("▓▓") || trimmed.Contains("║║") ||
             trimmed.Contains("┃║┃"))
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
    }
    // Bussola
    else if (trimmed.StartsWith("┌───┐") || trimmed.StartsWith("└───┘") ||
             trimmed.Contains("☼"))
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
    }
    // Emoji di sistema
    else if (trimmed.StartsWith("📍"))
    {
        Console.ForegroundColor = ConsoleColor.White;
    }
    else if (trimmed.StartsWith("🔍"))
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
    }
    else if (trimmed.StartsWith("👤"))
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
    }
    else if (trimmed.StartsWith("📋") || trimmed.StartsWith("📌"))
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
    }
    // Contenuto arte (interno delle stanze)
    else if (trimmed.Contains("[##]") || trimmed.Contains("{  }") || 
             trimmed.Contains("(::)") || trimmed.Contains("||||||"))
    {
        Console.ForegroundColor = ConsoleColor.DarkGreen;
    }
    // NPC art
    else if (trimmed.Contains("() ()") || trimmed.Contains("o   o") ||
             trimmed.Contains("_____") && trimmed.Contains("/"))
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
    }
    // Separatori
    else if (trimmed.StartsWith("════") || trimmed.StartsWith("────"))
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.White;
    }
    
    Console.WriteLine(text);
    Console.ResetColor();
}

/// <summary>
/// Renderizza un blocco di testo multi-riga con colori.
/// </summary>
void RenderColoredBlock(string text)
{
    foreach (var line in text.Split('\n'))
    {
        RenderColoredLine(line.TrimEnd('\r'));
    }
}

// ==================== LOOP PRINCIPALE ====================
Console.Clear();

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
Console.WriteLine("   ⏳ OLTRE IL TEMPO — L'indagine ha inizio");
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
Console.ResetColor();
Console.WriteLine();

// Mostra la stanza iniziale con arte colorata
RenderColoredBlock(gameEngine.DescribeCurrentRoom());
Console.WriteLine();

while (true)
{
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write($"[Turno {gameEngine.GetGameState().TurnNumber}] ");
    Console.ResetColor();
    Console.Write("> ");
    string? input = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(input))
        continue;
    
    if (input.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("esci", StringComparison.OrdinalIgnoreCase))
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("  Grazie per aver giocato a Oltre il Tempo!");
        Console.WriteLine("  Alla prossima indagine, agente CRONONAUTA.");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");
        Console.ResetColor();
        break;
    }
    
    var result = gameEngine.Execute(input);
    
    foreach (var line in result.Lines)
    {
        // Per le righe di output standard, usa il tipo per i colori
        if (line.Type != OutputLineType.Normal)
        {
            Console.ForegroundColor = line.Type switch
            {
                OutputLineType.Success => ConsoleColor.Green,
                OutputLineType.Warning => ConsoleColor.Yellow,
                OutputLineType.Error => ConsoleColor.Red,
                OutputLineType.Subtle => ConsoleColor.DarkGray,
                _ => ConsoleColor.White
            };
            Console.WriteLine(line.Text);
            Console.ResetColor();
        }
        else
        {
            // Per le righe normali, usa il rendering colorato intelligente
            RenderColoredBlock(line.Text);
        }
    }
    Console.ResetColor();
    Console.WriteLine();
}
