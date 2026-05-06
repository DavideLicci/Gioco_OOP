namespace OltreIlTempo.Engine;

using OltreIlTempo.Domain;

/// <summary>
/// Motore di parsing dei comandi per il gioco.
/// </summary>
/// <remarks>
/// Questa classe implementa il Command Pattern per il parsing degli input dell'utente.
/// Supporta:
/// - Alias multipli per ogni comando (es. "n", "nord" → "north")
/// - Suggerimenti automatici per typo usando distanza di Levenshtein
/// - Alias dinamici configurabili da JSON
/// 
/// Utilizzo tipico:
/// <code>
/// var parser = new CommandParser();
/// parser.SetDynamicAliases(aliases); // Opzionale
/// var parsed = parser.Parse("raccogli coltello");
/// </code>
/// </remarks>
public class CommandParser : ICommandParser
{
    // Alias dinamici dal JSON (ha priorità sugli alias hardcoded)
    private Dictionary<string, string> _dynamicAliases = new();
    
    // Alias dei comandi (es. "n" -> "north")
    private static readonly Dictionary<string, string> CommandAliases = new()
    {
        // Movimento
        { "n", "north" },
        { "s", "south" },
        { "e", "east" },
        { "o", "west" },
        { "nord", "north" },
        { "sud", "south" },
        { "est", "east" },
        { "ovest", "west" },
        { "avanti", "forward" },
        
        // Azioni comuni
        { "prendi", "take" },
        { "raccogli", "collect" },
        { "drop", "drop" },
        { "lascia", "drop" },
        { "parla", "talk" },
        { "chiedi", "ask" },
        { "esamina", "examine" },
        { "guarda", "look" },
        { "leggi", "read" },
        { "usa", "use" },
        { "apri", "open" },
        { "chiudi", "close" },
        
        // Selezione dialogo
        { "scegli", "choose" },
        { "rispondi", "choose" },
        
        // Inventario e status
        { "i", "inventory" },
        { "inv", "inventory" },
        { "inventario", "inventory" },
        { "stato", "status" },
        
        // Sistema di gioco
        { "bacheca", "board" },
        { "indizi", "board" },
        { "deduci", "deduce" },
        { "trofei", "achievements" },
        { "missione", "quest" },
        { "livello", "quest" },
        { "salva", "save" },
        { "carica", "load" },
        { "aiuto", "help" },
        { "?", "help" },
{ "esci", "quit" },
        { "quit", "quit" },
        { "exit", "exit" }
    };
    
    // Comandi riconosciuti dal sistema
    private static readonly HashSet<string> KnownVerbs = new()
    {
        // Movimento
        "north", "south", "east", "west", "forward",
        
        // Azioni
        "take", "collect", "drop", "talk", "ask", "examine", "look", "read", "use", "open", "close",
        "choose",
        
        // Sistema
        "inventory", "status", "board", "deduce", "achievements", "quest", "save", "load", "help", "quit", "exit"
    };
    
    public ParsedCommand Parse(string input)
    {
        var parts = input.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        
        if (parts.Length == 0)
        {
            return new ParsedCommand { IsValid = false };
        }
        
        // Normalizza e converti alias
        string verb = parts[0].ToLower();
        verb = NormalizeCommand(verb);
        
        // Se il comando non è riconosciuto, suggerisci il più simile
        if (!KnownVerbs.Contains(verb))
        {
            var suggestion = FindClosestCommand(verb);
            return new ParsedCommand
            {
                IsValid = false,
                Verb = verb,
                Args = parts.Length > 1 ? parts[1..].ToList() : new(),
                Suggestion = string.IsNullOrEmpty(suggestion) ? null : suggestion
            };
        }
        
        return new ParsedCommand
        {
            IsValid = true,
            Verb = verb,
            Args = parts.Length > 1 ? parts[1..].ToList() : new()
        };
    }
    
    /// <summary>
    /// Normalizza un comando tramite alias.
    /// </summary>
    private string NormalizeCommand(string command)
    {
        // Prima controlla gli alias dinamici (hanno priorità)
        if (_dynamicAliases.TryGetValue(command, out var normalized))
        {
            return normalized;
        }
        
        // Poi controlla gli alias hardcoded
        if (CommandAliases.TryGetValue(command, out var hardcoded))
        {
            return hardcoded;
        }
        
        return command;
    }
    
    /// <summary>
    /// Imposta gli alias dinamici dal contenuto del gioco.
    /// DADDA: questo permette di configurare alias via JSON.
    /// </summary>
    public void SetDynamicAliases(Dictionary<string, string> aliases)
    {
        _dynamicAliases = aliases ?? new Dictionary<string, string>();
    }
    
    /// <summary>
    /// Trova il comando più simile usando la distanza di Levenshtein.
    /// </summary>
    private string FindClosestCommand(string input)
    {
        int minDistance = int.MaxValue;
        string closest = string.Empty;
        
        foreach (var verb in KnownVerbs)
        {
            int distance = LevenshteinDistance(input, verb);
            
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = verb;
            }
        }
        
        // Se la distanza è troppo grande (più di 2 caratteri), non suggerire
        if (minDistance > 2)
        {
            return string.Empty;
        }
        
        return closest;
    }
    
    /// <summary>
    /// Calcola la distanza di Levenshtein tra due stringhe.
    /// </summary>
    private int LevenshteinDistance(string s1, string s2)
    {
        int len1 = s1.Length;
        int len2 = s2.Length;
        int[,] d = new int[len1 + 1, len2 + 1];
        
        for (int i = 0; i <= len1; i++)
            d[i, 0] = i;
        
        for (int j = 0; j <= len2; j++)
            d[0, j] = j;
        
        for (int i = 1; i <= len1; i++)
        {
            for (int j = 1; j <= len2; j++)
            {
                int cost = (s1[i - 1] == s2[j - 1]) ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost
                );
            }
        }
        
        return d[len1, len2];
    }
}
