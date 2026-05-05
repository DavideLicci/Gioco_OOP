using OltreIlTempo.Content;
using OltreIlTempo.Domain;
using OltreIlTempo.Engine;

// TODO: Implementare il loop principale del gioco

// Carica il contenuto
var contentLoader = new ContentLoader();
var content = contentLoader.Load("Content/game-content.json");

// Crea il game engine
var gameEngine = new GameEngine(content, content.Config);

// Registra i servizi
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

// Loop principale del gioco
Console.WriteLine("=== OLTRE IL TEMPO ===");
Console.WriteLine("Un'avventura investigativa interattiva.\n");

Console.WriteLine(gameEngine.DescribeCurrentRoom());
Console.WriteLine();

while (true)
{
    Console.Write("> ");
    string? input = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(input))
        continue;
    
    if (input.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Grazie per aver giocato!");
        break;
    }
    
    var result = gameEngine.Execute(input);
    
    foreach (var line in result.Lines)
    {
        Console.WriteLine(line.Text);
    }
}
