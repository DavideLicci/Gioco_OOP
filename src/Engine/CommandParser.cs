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

    private static readonly HashSet<string> MovementCommandWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "vai",
        "va",
        "andare",
        "cammina",
        "muoviti",
        "spostati"
    };

    private static readonly HashSet<string> FillerWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "a",
        "ad",
        "al",
        "allo",
        "alla",
        "ai",
        "agli",
        "alle",
        "con",
        "il",
        "lo",
        "la",
        "i",
        "gli",
        "le",
        "un",
        "uno",
        "una",
        "verso",
        "in",
        "nel",
        "nello",
        "nella",
        "nei",
        "negli",
        "nelle"
    };
    
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
        { "mappa", "map" },
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
        "inventory", "status", "board", "deduce", "achievements", "quest", "map", "save", "load", "help", "quit", "exit"
    };
    
    public ParsedCommand Parse(string input)
    {
        var parts = input.Trim()
            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(CleanToken)
            .Where(part => !string.IsNullOrWhiteSpace(part))
            .ToArray();
        
        if (parts.Length == 0)
        {
            return new ParsedCommand { IsValid = false };
        }
        
        // Normalizza e converti alias
        string rawVerb = parts[0].ToLowerInvariant();
        List<string> args;
        string verb;

        if (ShouldParseAsNaturalMovement(rawVerb))
        {
            args = CleanArguments(parts.Skip(1));

            if (args.Count == 0)
            {
                return new ParsedCommand
                {
                    IsValid = false,
                    Verb = rawVerb,
                    Args = new(),
                    Suggestion = "nord"
                };
            }

            verb = NormalizeCommand(args[0].ToLowerInvariant());
            args = args.Skip(1).ToList();
        }
        else
        {
            verb = NormalizeCommand(rawVerb);
            args = CleanArguments(parts.Skip(1));
        }
        
        // Se il comando non è riconosciuto, suggerisci il più simile
        if (!KnownVerbs.Contains(verb))
        {
            var suggestion = FindClosestCommand(verb);
            return new ParsedCommand
            {
                IsValid = false,
                Verb = verb,
                Args = args,
                Suggestion = string.IsNullOrEmpty(suggestion) ? null : suggestion
            };
        }
        
        return new ParsedCommand
        {
            IsValid = true,
            Verb = verb,
            Args = args
        };
    }
    
    /// <summary>
    /// Normalizza un comando tramite alias.
    /// </summary>
    private string NormalizeCommand(string command)
    {
        if (_dynamicAliases.TryGetValue(command, out var normalized))
        {
            return CommandAliases.TryGetValue(normalized, out var normalizedAlias)
                ? normalizedAlias
                : normalized;
        }

        return CommandAliases.TryGetValue(command, out var hardcoded) ? hardcoded : command;
    }

    private bool ShouldParseAsNaturalMovement(string rawVerb)
    {
        return MovementCommandWords.Contains(rawVerb) &&
               !_dynamicAliases.ContainsKey(rawVerb) &&
               !CommandAliases.ContainsKey(rawVerb) &&
               !KnownVerbs.Contains(rawVerb);
    }
    
    /// <summary>
    /// Imposta gli alias dinamici dal contenuto del gioco.
    /// DADDA: questo permette di configurare alias via JSON.
    /// </summary>
    public void SetDynamicAliases(Dictionary<string, string> aliases)
    {
        _dynamicAliases = aliases?
            .Where(alias => !string.IsNullOrWhiteSpace(alias.Key) && !string.IsNullOrWhiteSpace(alias.Value))
            .ToDictionary(
                alias => alias.Key.Trim().ToLowerInvariant(),
                alias => alias.Value.Trim().ToLowerInvariant(),
                StringComparer.OrdinalIgnoreCase)
            ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
    
    /// <summary>
    /// Trova il comando più simile usando la distanza di Levenshtein.
    /// </summary>
    private string FindClosestCommand(string input)
    {
        int minDistance = int.MaxValue;
        string closest = string.Empty;
        
        var candidates = KnownVerbs
            .Concat(CommandAliases.Keys)
            .Concat(_dynamicAliases.Keys)
            .Distinct(StringComparer.OrdinalIgnoreCase);
        
        foreach (var verb in candidates)
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

    private static string CleanToken(string token)
    {
        var cleaned = token.Trim().Trim('.', ',', ';', ':', '!', '"');

        if (cleaned.Length > 1)
        {
            cleaned = cleaned.Trim('?');
        }

        if (cleaned.StartsWith("l'", StringComparison.OrdinalIgnoreCase))
        {
            cleaned = cleaned[2..];
        }

        return cleaned;
    }

    private static List<string> CleanArguments(IEnumerable<string> args)
    {
        var cleaned = new List<string>();
        bool skipFillers = true;

        foreach (var arg in args)
        {
            var normalized = arg.ToLowerInvariant();

            if (normalized == "su")
            {
                cleaned.Add(normalized);
                skipFillers = true;
                continue;
            }

            if (skipFillers && FillerWords.Contains(normalized))
            {
                continue;
            }

            cleaned.Add(arg);
            skipFillers = false;
        }

        return cleaned;
    }
    
    /// <summary>
    /// Calcola la distanza di Levenshtein tra due stringhe.
    /// </summary>
    private int LevenshteinDistance(string s1, string s2)
    {
        if (string.IsNullOrEmpty(s1)) return s2?.Length ?? 0;
        if (string.IsNullOrEmpty(s2)) return s1.Length;

        int n = s1.Length;
        int m = s2.Length;
        int[,] d = new int[n + 1, m + 1];

        for (int i = 0; i <= n; d[i, 0] = i++) ;
        for (int j = 0; j <= m; d[0, j] = j++) ;

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = (s2[j - 1] == s1[i - 1]) ? 0 : 1;
                d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
            }
        }
        return d[n, m];
    }
}
