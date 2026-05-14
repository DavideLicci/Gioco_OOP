namespace OltreIlTempo.Engine;

using OltreIlTempo.Domain;

/// <summary>
/// Gestisce l'introduzione narrativa e il tutorial interattivo.
/// DADDA: mostra il background del mondo e insegna i comandi base.
/// </summary>
public partial class GameEngine
{
    // Flag per tracciare lo stato dell'intro/tutorial
    private bool _introShown = false;
    private bool _tutorialCompleted = false;
    private int _tutorialStep = 0;
    
    /// <summary>
    /// Restituisce le righe dell'introduzione narrativa del gioco.
    /// Racconta il background del mondo, chi è il giocatore, e la missione.
    /// </summary>
    public List<string> GetIntroduction()
    {
        _introShown = true;
        
        return new List<string>
        {
            "",
            "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━",
            "              ⏳  O L T R E   I L   T E M P O  ⏳",
            "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━",
            "",
            "  Anno 2024. Qualcosa non va.",
            "",
            "  Da settimane, eventi inspiegabili scuotono la tranquilla",
            "  cittadina di Valdimonte: oggetti che appaiono dal nulla,",
            "  messaggi scritti in lingue sconosciute, e abitanti che",
            "  giurano di aver visto \"ombre del futuro\" camminare tra",
            "  le strade.",
            "",
            "  Tu sei un investigatore dell'Agenzia Temporale — un'unità",
            "  segreta incaricata di indagare sulle anomalie nel tessuto",
            "  del tempo. Il tuo nome in codice: CRONONAUTA.",
            "",
            "  La tua missione ti ha portato fino a Villa Reale, un'antica",
            "  dimora ai margini del paese. Secondo i rapporti, è qui che",
            "  le anomalie temporali sono più forti. Qualcuno — o qualcosa",
            "  — sta manipolando il tempo dall'interno di questa casa.",
            "",
            "  Gli abitanti della villa sembrano nascondere qualcosa.",
            "  Ognuno ha i propri segreti, le proprie motivazioni.",
            "  Dovrai parlare con loro, guadagnare la loro fiducia,",
            "  raccogliere indizi e, soprattutto, non destare sospetti.",
            "",
            "  Perché se il livello di sospetto sale troppo...",
            "  potresti non uscire mai più da questo luogo.",
            "",
            "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━",
            "  📋 Obiettivo: Esplora la villa, raccogli indizi,",
            "     parla con gli abitanti e scopri la verità.",
            "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━",
            ""
        };
    }
    
    /// <summary>
    /// Verifica se il tutorial è attivo (non ancora completato).
    /// </summary>
    public bool IsTutorialActive => !_tutorialCompleted && _introShown;
    
    /// <summary>
    /// Restituisce il passo attuale del tutorial.
    /// </summary>
    public int TutorialStep => _tutorialStep;
    
    /// <summary>
    /// Inizia il tutorial interattivo. Restituisce il primo messaggio.
    /// </summary>
    public List<string> StartTutorial()
    {
        _tutorialStep = 0;
        _tutorialCompleted = false;
        
        return new List<string>
        {
            "",
            "╔══════════════════════════════════════════════════════════╗",
            "║              📖 TUTORIAL — Primi Passi                  ║",
            "╚══════════════════════════════════════════════════════════╝",
            "",
            "  Benvenuto, agente! Prima di iniziare l'indagine,",
            "  impariamo i comandi base.",
            "",
            "  Ti trovi nella cucina della villa. Cominciamo.",
            "",
            GetTutorialStepMessage(0)
        };
    }
    
    /// <summary>
    /// Elabora l'input del giocatore durante il tutorial.
    /// Restituisce il feedback e il prossimo passo, oppure null se il tutorial è finito.
    /// </summary>
    public TutorialResult ProcessTutorialInput(string input)
    {
        if (_tutorialCompleted)
        {
            return new TutorialResult { IsComplete = true };
        }
        
        var result = new TutorialResult();
        string normalizedInput = input.Trim().ToLower();
        
        switch (_tutorialStep)
        {
            case 0: // Guarda intorno
                if (normalizedInput == "guarda" || normalizedInput == "look")
                {
                    result.Lines.Add("");
                    result.Lines.Add("  ✅ Perfetto! Ecco cosa vedi intorno a te:");
                    result.Lines.Add("");
                    result.Lines.Add(DescribeCurrentRoom());
                    result.Lines.Add("");
                    _tutorialStep++;
                    result.Lines.Add(GetTutorialStepMessage(_tutorialStep));
                }
                else
                {
                    result.Lines.Add("");
                    result.Lines.Add($"  ❌ Non è il comando giusto. Prova a digitare: guarda");
                }
                break;
                
            case 1: // Movimento
                if (normalizedInput == "nord" || normalizedInput == "n" || 
                    normalizedInput == "north")
                {
                    // Esegui il movimento
                    var moveResult = Execute(input);
                    result.Lines.Add("");
                    result.Lines.Add("  ✅ Ottimo! Ti sei spostato nel corridoio!");
                    result.Lines.Add("");
                    foreach (var line in moveResult.Lines)
                    {
                        result.Lines.Add(line.Text);
                    }
                    result.Lines.Add("");
                    _tutorialStep++;
                    result.Lines.Add(GetTutorialStepMessage(_tutorialStep));
                }
                else
                {
                    result.Lines.Add("");
                    result.Lines.Add($"  ❌ Prova a muoverti verso Nord. Digita: nord");
                }
                break;
                
            case 2: // Torna indietro
                if (normalizedInput == "sud" || normalizedInput == "s" || 
                    normalizedInput == "south")
                {
                    var moveResult = Execute(input);
                    result.Lines.Add("");
                    result.Lines.Add("  ✅ Bene! Sei tornato in cucina.");
                    result.Lines.Add("");
                    foreach (var line in moveResult.Lines)
                    {
                        result.Lines.Add(line.Text);
                    }
                    result.Lines.Add("");
                    _tutorialStep++;
                    result.Lines.Add(GetTutorialStepMessage(_tutorialStep));
                }
                else
                {
                    result.Lines.Add("");
                    result.Lines.Add($"  ❌ Prova a tornare verso Sud. Digita: sud");
                }
                break;
                
            case 3: // Esamina oggetto
                if (normalizedInput.StartsWith("esamina") || normalizedInput.StartsWith("examine"))
                {
                    var examineResult = Execute(input);
                    result.Lines.Add("");
                    if (examineResult.IsValid)
                    {
                        result.Lines.Add("  ✅ Ottimo investigatore! Esaminare gli oggetti");
                        result.Lines.Add("     è fondamentale per trovare indizi.");
                    }
                    else
                    {
                        result.Lines.Add("  ⚠️  L'oggetto non è stato trovato, ma hai capito il comando!");
                    }
                    result.Lines.Add("");
                    foreach (var line in examineResult.Lines)
                    {
                        result.Lines.Add(line.Text);
                    }
                    result.Lines.Add("");
                    _tutorialStep++;
                    result.Lines.Add(GetTutorialStepMessage(_tutorialStep));
                }
                else
                {
                    result.Lines.Add("");
                    result.Lines.Add($"  ❌ Prova ad esaminare qualcosa. Digita: esamina nota");
                }
                break;
                
            case 4: // Raccogli oggetto
                if (normalizedInput.StartsWith("raccogli") || normalizedInput.StartsWith("take") ||
                    normalizedInput.StartsWith("collect"))
                {
                    var collectResult = Execute(input);
                    result.Lines.Add("");
                    foreach (var line in collectResult.Lines)
                    {
                        result.Lines.Add(line.Text);
                    }
                    result.Lines.Add("");
                    _tutorialStep++;
                    result.Lines.Add(GetTutorialStepMessage(_tutorialStep));
                }
                else
                {
                    result.Lines.Add("");
                    result.Lines.Add($"  ❌ Prova a raccogliere un oggetto. Digita: raccogli nota");
                }
                break;
                
            case 5: // Inventario
                if (normalizedInput == "inventario" || normalizedInput == "inventory" ||
                    normalizedInput == "inv")
                {
                    var invResult = Execute(input);
                    result.Lines.Add("");
                    result.Lines.Add("  ✅ Ecco il tuo inventario:");
                    result.Lines.Add("");
                    foreach (var line in invResult.Lines)
                    {
                        result.Lines.Add(line.Text);
                    }
                    result.Lines.Add("");
                    _tutorialStep++;
                    result.Lines.Add(GetTutorialStepMessage(_tutorialStep));
                }
                else
                {
                    result.Lines.Add("");
                    result.Lines.Add($"  ❌ Controlla il tuo zaino! Digita: inventario");
                }
                break;
                
            case 6: // Aiuto
                if (normalizedInput == "aiuto" || normalizedInput == "help")
                {
                    var helpResult = Execute(input);
                    result.Lines.Add("");
                    result.Lines.Add("  ✅ Ecco tutti i comandi a tua disposizione:");
                    result.Lines.Add("");
                    foreach (var line in helpResult.Lines)
                    {
                        result.Lines.Add(line.Text);
                    }
                    result.Lines.Add("");
                    _tutorialStep++;
                    
                    // Tutorial completato!
                    _tutorialCompleted = true;
                    result.Lines.Add(GetTutorialCompletionMessage());
                    result.IsComplete = true;
                }
                else
                {
                    result.Lines.Add("");
                    result.Lines.Add($"  ❌ Ultimo passo! Digita: aiuto");
                }
                break;
        }
        
        return result;
    }
    
    /// <summary>
    /// Permette di saltare il tutorial.
    /// </summary>
    public List<string> SkipTutorial()
    {
        _tutorialCompleted = true;
        _introShown = true;
        
        return new List<string>
        {
            "",
            "  ⏩ Tutorial saltato. Buona fortuna, agente!",
            "  Digita 'aiuto' se hai bisogno di una lista dei comandi.",
            ""
        };
    }
    
    // ==================== Messaggi del Tutorial ====================
    
    private string GetTutorialStepMessage(int step)
    {
        return step switch
        {
            0 => "  📌 Passo 1/7 — OSSERVA\n" +
                 "  Per prima cosa, guardati intorno.\n" +
                 "  👉 Digita: guarda",
                 
            1 => "  📌 Passo 2/7 — MUOVITI\n" +
                 "  Ora proviamo a spostarci. Le direzioni sono:\n" +
                 "  nord (n), sud (s), est (e), ovest (o)\n" +
                 "  👉 Digita: nord",
                 
            2 => "  📌 Passo 3/7 — TORNA INDIETRO\n" +
                 "  Bene! Ora torna nella cucina.\n" +
                 "  👉 Digita: sud",
                 
            3 => "  📌 Passo 4/7 — ESAMINA\n" +
                 "  In cucina ci sono degli oggetti. Esaminali!\n" +
                 "  👉 Digita: esamina nota",
                 
            4 => "  📌 Passo 5/7 — RACCOGLI\n" +
                 "  Ora raccogli la nota per il tuo inventario.\n" +
                 "  👉 Digita: raccogli nota",
                 
            5 => "  📌 Passo 6/7 — INVENTARIO\n" +
                 "  Controlla cosa hai nello zaino.\n" +
                 "  👉 Digita: inventario",
                 
            6 => "  📌 Passo 7/7 — AIUTO\n" +
                 "  Infine, se ti perdi, puoi sempre consultare\n" +
                 "  la lista completa dei comandi.\n" +
                 "  👉 Digita: aiuto",
                 
            _ => ""
        };
    }
    
    private string GetTutorialCompletionMessage()
    {
        return "\n" +
               "╔══════════════════════════════════════════════════════════╗\n" +
               "║           ✅ TUTORIAL COMPLETATO!                       ║\n" +
               "╚══════════════════════════════════════════════════════════╝\n" +
               "\n" +
               "  Ottimo lavoro, agente CRONONAUTA! Ora conosci le basi:\n" +
               "\n" +
               "  🔹 guarda      — osserva l'ambiente\n" +
               "  🔹 nord/sud... — muoviti tra le stanze\n" +
               "  🔹 esamina     — esamina oggetti e persone\n" +
               "  🔹 raccogli    — prendi oggetti\n" +
               "  🔹 inventario  — controlla il tuo zaino\n" +
               "  🔹 parla       — parla con le persone\n" +
               "  🔹 aiuto       — lista completa comandi\n" +
               "\n" +
               "  ⚠️  Ricorda: ogni azione può influenzare la fiducia\n" +
               "  degli abitanti e il tuo livello di sospetto.\n" +
               "  Agisci con cautela!\n" +
               "\n" +
               "  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
               "  L'indagine ha inizio. Buona fortuna, agente.\n" +
               "  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n";
    }
}

/// <summary>
/// Risultato di un passo del tutorial.
/// </summary>
public class TutorialResult
{
    public List<string> Lines { get; set; } = new();
    public bool IsComplete { get; set; } = false;
}
