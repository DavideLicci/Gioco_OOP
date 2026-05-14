namespace OltreIlTempo.Engine;

/// <summary>
/// Fornisce ASCII/ANSI art per stanze, dialoghi, mappa, NPC e schermate di sistema.
/// </summary>
public static class AsciiArtProvider
{
    // ==================== COLORI ANSI ====================
    public const string RESET = "\u001b[0m";
    public const string BOLD = "\u001b[1m";
    public const string DIM = "\u001b[2m";
    public const string RED = "\u001b[31m";
    public const string GREEN = "\u001b[32m";
    public const string YELLOW = "\u001b[33m";
    public const string BLUE = "\u001b[34m";
    public const string MAGENTA = "\u001b[35m";
    public const string CYAN = "\u001b[36m";
    public const string WHITE = "\u001b[37m";
    public const string GRAY = "\u001b[90m";
    public const string B_WHITE = "\u001b[97m";
    public const string B_YELLOW = "\u001b[93m";
    public const string B_CYAN = "\u001b[96m";
    public const string B_MAGENTA = "\u001b[95m";
    public const string AMBER = "\u001b[38;5;214m";
    public const string BRASS = "\u001b[38;5;178m";
    public const string COPPER = "\u001b[38;5;166m";
    public const string EMERALD = "\u001b[38;5;42m";
    public const string LILAC = "\u001b[38;5;141m";
    public const string PAPER = "\u001b[38;5;230m";
    public const string ROSE = "\u001b[38;5;211m";
    public const string SHADOW = "\u001b[38;5;238m";
    public const string STONE = "\u001b[38;5;246m";
    public const string WOOD = "\u001b[38;5;130m";

    private const int BoxWidth = 66;

    // ==================== METODI PUBBLICI ====================

    public static string GetRoomArt(string roomId) => roomId switch
    {
        "room_kitchen" => KITCHEN_ART,
        "room_hallway" => HALLWAY_ART,
        "room_pantry" => PANTRY_ART,
        "room_library" => LIBRARY_ART,
        "room_bedroom" => BEDROOM_ART,
        _ => UNKNOWN_ROOM_ART
    };

    public static string GetRoomTitle(string roomName)
    {
        string title = $"LUOGO RILEVATO :: {roomName.ToUpperInvariant()}";
        return Lines(
            C(CYAN, $"  ╔{new string('═', BoxWidth)}╗"),
            Framed(title, B_WHITE),
            C(CYAN, $"  ╚{new string('═', BoxWidth)}╝"));
    }

    public static string GetNpcArt(string npcId) => npcId switch
    {
        "npc_cook" => NPC_COOK_ART,
        "npc_librarian" => NPC_LIBRARIAN_ART,
        _ => ""
    };

    public static string GetDialogueMenu(string npcName, IEnumerable<string> prompts)
    {
        var lines = new List<string>
        {
            C(MAGENTA, $"  ╔{new string('═', BoxWidth)}╗"),
            Framed($"  CANALE DIALOGO :: {npcName.ToUpperInvariant()}", B_MAGENTA),
            C(MAGENTA, $"  ╠{new string('═', BoxWidth)}╣"),
            Framed("  Frequenza stabile. Scegli con cura: ogni parola pesa.", GRAY),
            C(MAGENTA, $"  ╠{new string('═', BoxWidth)}╣")
        };

        int index = 1;
        foreach (var prompt in prompts)
        {
            var wrapped = Wrap(prompt, BoxWidth - 8).ToList();
            if (wrapped.Count == 0)
            {
                wrapped.Add("");
            }

            lines.Add(Framed($"  [{index}] {wrapped[0]}", PAPER));
            foreach (var extraLine in wrapped.Skip(1))
            {
                lines.Add(Framed($"      {extraLine}", PAPER));
            }

            index++;
        }

        lines.Add(C(MAGENTA, $"  ╠{new string('═', BoxWidth)}╣"));
        lines.Add(Framed("  Usa: scegli [numero]", B_CYAN));
        lines.Add(C(MAGENTA, $"  ╚{new string('═', BoxWidth)}╝"));

        return Lines(lines);
    }

    public static string GetDialogueExchange(string playerPrompt, string npcName, string npcResponse)
    {
        var lines = new List<string>
        {
            C(MAGENTA, $"  ╔{new string('═', BoxWidth)}╗"),
            Framed("  TRACCIA DIALOGO", B_MAGENTA),
            C(MAGENTA, $"  ╠{new string('═', BoxWidth)}╣")
        };

        lines.Add(Framed("  TU", B_CYAN));
        foreach (var line in Wrap($"\"{playerPrompt}\"", BoxWidth - 6))
        {
            lines.Add(Framed($"    {line}", WHITE));
        }

        lines.Add(C(MAGENTA, $"  ╠{new string('─', BoxWidth)}╣"));
        lines.Add(Framed($"  {npcName.ToUpperInvariant()}", B_YELLOW));
        foreach (var line in Wrap($"\"{npcResponse}\"", BoxWidth - 6))
        {
            lines.Add(Framed($"    {line}", PAPER));
        }

        lines.Add(C(MAGENTA, $"  ╚{new string('═', BoxWidth)}╝"));
        return Lines(lines);
    }

    public static string GetVillaMap() => GetVillaMap(null);

    public static string GetVillaMap(string? currentRoomId)
    {
        string camera = MapMark(currentRoomId, "room_bedroom");
        string corridor = MapMark(currentRoomId, "room_hallway");
        string library = MapMark(currentRoomId, "room_library");
        string kitchen = MapMark(currentRoomId, "room_kitchen");
        string pantry = MapMark(currentRoomId, "room_pantry");

        return Lines(
            C(CYAN, "  ╔════════════════════════════════════════════════════════════════════╗"),
            C(CYAN, "  ║") + C(B_WHITE, "                 MAPPA TATTICA DI VILLA REALE                 ") + C(CYAN, "║"),
            C(CYAN, "  ╠════════════════════════════════════════════════════════════════════╣"),
            C(CYAN, "  ║                                                                    ║"),
            C(CYAN, "  ║      ┌──────────────┐      ┌──────────────┐      ┌──────────────┐  ║"),
            C(CYAN, "  ║      │") + C(PAPER, " CAMERA       ") + camera + C(CYAN, "│◄────►│") + C(PAPER, " CORRIDOIO    ") + corridor + C(CYAN, "│◄────►│") + C(PAPER, " BIBLIOTECA   ") + library + C(CYAN, "│  ║"),
            C(CYAN, "  ║      │") + C(GRAY, " letto e ombre") + C(CYAN, " │      │") + C(GRAY, " quadri vivi  ") + C(CYAN, "│      │") + C(GRAY, " libri antichi") + C(CYAN, "│  ║"),
            C(CYAN, "  ║      └──────────────┘      └──────┬───────┘      └──────────────┘  ║"),
            C(CYAN, "  ║                                    │                               ║"),
            C(CYAN, "  ║                             ┌──────▼───────┐      ┌──────────────┐  ║"),
            C(CYAN, "  ║                             │") + C(PAPER, " CUCINA       ") + kitchen + C(CYAN, "│◄────►│") + C(PAPER, " DISPENSA     ") + pantry + C(CYAN, "│  ║"),
            C(CYAN, "  ║                             │") + C(GRAY, " fuoco e rame ") + C(CYAN, "│      │") + C(GRAY, " scaffali     ") + C(CYAN, "│  ║"),
            C(CYAN, "  ║                             └──────────────┘      └──────────────┘  ║"),
            C(CYAN, "  ║                                                                    ║"),
            C(CYAN, "  ║") + C(B_YELLOW, "   @ posizione attuale") + C(GRAY, "     ◄────► passaggio esplorabile                 ") + C(CYAN, "║"),
            C(CYAN, "  ╚════════════════════════════════════════════════════════════════════╝"));
    }

    public static string GetSplashScreen() => SPLASH_ART;
    public static string GetVillaExterior() => VILLA_EXTERIOR_ART;
    public static string GetTrophy() => TROPHY_ART;
    public static string GetInventoryHeader() => INVENTORY_ART;
    public static string GetDeductionArt() => DEDUCTION_ART;
    public static string GetLevelComplete() => LEVEL_COMPLETE_ART;
    public static string GetGameOver() => GAME_OVER_ART;
    public static string GetVictory() => VICTORY_ART;

    public static string GetItemArt(string type) => type.ToLowerInvariant() switch
    {
        "key" => ITEM_KEY_ART,
        "document" => ITEM_DOC_ART,
        "evidence" => ITEM_EVIDENCE_ART,
        "future" => ITEM_FUTURE_ART,
        "container" => ITEM_CONTAINER_ART,
        _ => ITEM_GENERIC_ART
    };

    public static string GetSeparator() =>
        C(CYAN, "  ════════════════════════════════════════════════════════════");

    public static string GetThinSeparator() =>
        C(GRAY, "  ────────────────────────────────────────────────────────────");

    public static string GetCompass(Dictionary<string, string> exits)
    {
        bool n = exits.ContainsKey("n");
        bool s = exits.ContainsKey("s");
        bool e = exits.ContainsKey("e");
        bool o = exits.ContainsKey("o") || exits.ContainsKey("w");

        return Lines(
            C(CYAN, "        ╔═══════╗"),
            C(CYAN, "        ║   ") + CompassPoint(n, "N") + C(CYAN, "   ║"),
            C(CYAN, "  ╔═════╬═══════╬═════╗"),
            C(CYAN, "  ║  ") + CompassPoint(o, "O") + C(CYAN, "  ║   ") + C(B_YELLOW, "◆") + C(CYAN, "   ║  ") + CompassPoint(e, "E") + C(CYAN, "  ║"),
            C(CYAN, "  ╚═════╬═══════╬═════╝"),
            C(CYAN, "        ║   ") + CompassPoint(s, "S") + C(CYAN, "   ║"),
            C(CYAN, "        ╚═══════╝"));
    }

    // ==================== STANZE ====================

    private static string KITCHEN_ART => Lines(
        C(CYAN, "  ╔════════════════════════════════════════════════════════════════════╗"),
        C(CYAN, "  ║") + C(B_YELLOW, " CUCINA DI SERVIZIO") + C(GRAY, "  pentole, vapore e tracce fuori tempo             ") + C(CYAN, "║"),
        C(CYAN, "  ╠════════════════════════════════════════════════════════════════════╣"),
        C(CYAN, "  ║") + C(WOOD, "  ┌─────────────┐        ") + C(STONE, " .-----------------. ") + C(COPPER, "       _________     ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(WOOD, "  │ SPEZIE 1850 │        ") + C(STONE, "|  piatti d'arg. | ") + C(COPPER, "      /  ___  /|     ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(WOOD, "  ├─────────────┤        ") + C(STONE, "'-----------------' ") + C(COPPER, "     /__/__/ / |     ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(WOOD, "  │ sale  pepe  │   ") + C(SHADOW, " fumo") + C(AMBER, "  ~ ~ ~") + C(SHADOW, " dal fornello ") + C(COPPER, "   |  __  |  |     ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(WOOD, "  └──────┬──────┘       ") + C(AMBER, "╭────────────╮") + C(COPPER, "       | |__| |  |     ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(BRASS, "         │              ") + C(AMBER, "│   FORNO    │") + C(COPPER, "       |______| /      ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(BRASS, "    .────┴────.         ") + C(AMBER, "│  [====]    │") + C(STONE, "       credenza       ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(WOOD, "   /  TAVOLO   \\        ") + C(AMBER, "╰────────────╯") + C(GRAY, "                      ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(WOOD, "  /_____________\\   ") + C(PAPER, "nota piegata") + C(WHITE, "  []") + C(RED, "   coltello") + C(WHITE, "  /|") + C(CYAN, "              ║"),
        C(CYAN, "  ║") + C(WOOD, "  |  _  _  _   |") + C(SHADOW, "     ombra di lancette sul pavimento") + C(CYAN, "             ║"),
        C(CYAN, "  ║") + C(WOOD, "  |_/ || || \\__|") + C(GRAY, "    Chef Marco lavora senza voltarsi") + C(CYAN, "             ║"),
        C(CYAN, "  ╚════════════════════════════════════════════════════════════════════╝"));

    private static string HALLWAY_ART => Lines(
        C(CYAN, "  ╔════════════════════════════════════════════════════════════════════╗"),
        C(CYAN, "  ║") + C(B_YELLOW, " CORRIDOIO DEI RITRATTI") + C(GRAY, "  il legno scricchiola a ogni passo          ") + C(CYAN, "║"),
        C(CYAN, "  ╠════════════════════════════════════════════════════════════════════╣"),
        C(CYAN, "  ║") + C(STONE, "     ╭──────╮    ╭──────╮    ╭──────╮    ╭──────╮               ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(STONE, "     │  o o │    │  - - │    │  ^ ^ │    │  @ @ │               ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(STONE, "     │  \\_/ │    │  ___ │    │  --- │    │  ___ │               ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(BRASS, "   ══╧══════╧════╧══════╧════╧══════╧════╧══════╧══             ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(WOOD, "       ║          ║          ║          ║                       ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(AMBER, "      ( )        ( )        ( )        ( )") + C(GRAY, "    lampade a gas       ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(WOOD, "       ║          ║          ║          ║                       ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(SHADOW, "  ─────╨──────────╨──────────╨──────────╨─────────────────────  ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(GRAY, "          il corridoio tira verso biblioteca, camera e cucina    ") + C(CYAN, "║"),
        C(CYAN, "  ╚════════════════════════════════════════════════════════════════════╝"));

    private static string PANTRY_ART => Lines(
        C(CYAN, "  ╔════════════════════════════════════════════════════════════════════╗"),
        C(CYAN, "  ║") + C(B_YELLOW, " DISPENSA") + C(GRAY, "  scaffali stretti, vetro, polvere e una chiave antica       ") + C(CYAN, "║"),
        C(CYAN, "  ╠════════════════════════════════════════════════════════════════════╣"),
        C(CYAN, "  ║") + C(WOOD, "  ┌──────────────────────────────────────────────────────────────┐  ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(WOOD, "  │ ") + C(PAPER, "[farina] [sale] [zucchero]") + C(EMERALD, "  {menta}") + C(ROSE, "  (vino rosso)") + C(WOOD, "       │  ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(WOOD, "  ├──────────────────────────────────────────────────────────────┤  ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(WOOD, "  │ ") + C(AMBER, "{miele}") + C(PAPER, "  [riso] [pasta]") + C(BRASS, "  o o o spezie rare") + C(WOOD, "              │  ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(WOOD, "  ├──────────────────────────────────────────────────────────────┤  ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(WOOD, "  │ ") + C(STONE, "(acqua)") + C(PAPER, " [caffe] [te]") + C(GRAY, "   barattoli senza etichetta") + C(WOOD, "       │  ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(WOOD, "  └──────────────────────────────────────────────────────────────┘  ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(SHADOW, "          .-----------------.                                       ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(SHADOW, "         / pavimento freddo /|    ") + C(B_YELLOW, ".---.") + C(GRAY, "  un bagliore metallico        ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(SHADOW, "        '-----------------' |    ") + C(B_YELLOW, "| K |") + C(GRAY, "  vicino allo stipite           ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(SHADOW, "         |__________________|/    ") + C(B_YELLOW, "'---'") + C(GRAY, "                              ") + C(CYAN, "║"),
        C(CYAN, "  ╚════════════════════════════════════════════════════════════════════╝"));

    private static string LIBRARY_ART => Lines(
        C(CYAN, "  ╔════════════════════════════════════════════════════════════════════╗"),
        C(CYAN, "  ║") + C(B_YELLOW, " BIBLIOTECA") + C(GRAY, "  odore di cuoio, polvere e memoria non lineare            ") + C(CYAN, "║"),
        C(CYAN, "  ╠════════════════════════════════════════════════════════════════════╣"),
        C(CYAN, "  ║") + C(WOOD, "  ╔════╦════╦════╦════╗") + C(STONE, "       _________") + C(WOOD, "        ╔════╦════╦════╗  ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(WOOD, "  ║") + C(ROSE, "1901") + C(WOOD, "║") + C(PAPER, "1850") + C(WOOD, "║") + C(LILAC, "??") + C(WOOD, "  ║") + C(PAPER, "DIAR") + C(WOOD, "║") + C(STONE, "      /  ___  \\") + C(WOOD, "       ║") + C(PAPER, "MAP ") + C(WOOD, "║") + C(ROSE, "ATTI") + C(WOOD, "║") + C(LILAC, "SIG ") + C(WOOD, "║  ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(WOOD, "  ╠════╬════╬════╬════╣") + C(STONE, "     /  /   \\  \\") + C(WOOD, "      ╠════╬════╬════╣  ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(WOOD, "  ║") + C(PAPER, "LIB ") + C(WOOD, "║") + C(PAPER, "LIB ") + C(WOOD, "║") + C(PAPER, "LIB ") + C(WOOD, "║") + C(PAPER, "LIB ") + C(WOOD, "║") + C(STONE, "    |  | o |  |") + C(WOOD, "     ║") + C(PAPER, "LIB ") + C(WOOD, "║") + C(PAPER, "LIB ") + C(WOOD, "║") + C(PAPER, "LIB ") + C(WOOD, "║  ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(WOOD, "  ╚════╩════╩════╩════╝") + C(STONE, "     \\  \\___/  /") + C(WOOD, "      ╚════╩════╩════╝  ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(GRAY, "                            \\_______/      mappamondo             ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(WOOD, "            .────────────────────────────────────────.             ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(WOOD, "           /") + C(PAPER, "  diario aperto   penna d'oca   calamaio") + C(WOOD, "     /|            ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(WOOD, "          /") + C(PAPER, "    []              /            ink") + C(WOOD, "       / |            ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(WOOD, "         '────────────────────────────────────────'  |            ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(GRAY, "             la Signora Lucia segue ogni fruscio di pagina        ") + C(CYAN, "║"),
        C(CYAN, "  ╚════════════════════════════════════════════════════════════════════╝"));

    private static string BEDROOM_ART => Lines(
        C(CYAN, "  ╔════════════════════════════════════════════════════════════════════╗"),
        C(CYAN, "  ║") + C(B_YELLOW, " CAMERA DA LETTO") + C(GRAY, "  seta pesante, specchi e silenzio trattenuto            ") + C(CYAN, "║"),
        C(CYAN, "  ╠════════════════════════════════════════════════════════════════════╣"),
        C(CYAN, "  ║") + C(ROSE, "       .----. ") + C(GRAY, "lume") + C(SHADOW, "                         ") + C(STONE, "┌────────────────┐       ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(ROSE, "      / .--. \\") + C(SHADOW, "                             ") + C(STONE, "│    SPECCHIO    │       ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(ROSE, "      \\ '--' /") + C(SHADOW, "     baldacchino             ") + C(STONE, "│   .--------.   │       ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(WOOD, "  ┌────'----'─────────────────────┐") + C(STONE, "   │   |  ....  |   │       ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(WOOD, "  │  ╔════════════════════════╗   │") + C(STONE, "   │   '--------'   │       ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(WOOD, "  │  ║") + C(ROSE, "  coperte color cremisi  ") + C(WOOD, "║   │") + C(STONE, "   └────────────────┘       ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(WOOD, "  │  ╚════════════════════════╝   │") + C(WOOD, "      ┌───────┐              ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(WOOD, "  └───────────────────────────────┘") + C(WOOD, "     │ ARM.  │              ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(SHADOW, "       tappeto consumato da passi nervosi") + C(WOOD, "     │  o    │              ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(SHADOW, "  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~") + C(WOOD, "     └───────┘              ") + C(CYAN, "║"),
        C(CYAN, "  ╚════════════════════════════════════════════════════════════════════╝"));

    private static string UNKNOWN_ROOM_ART => Lines(
        C(CYAN, "  ╔════════════════════════════════════════════════════════════════════╗"),
        C(CYAN, "  ║") + C(B_YELLOW, " STANZA SCONOSCIUTA") + C(GRAY, "  la villa non dovrebbe avere questo spazio        ") + C(CYAN, "║"),
        C(CYAN, "  ╠════════════════════════════════════════════════════════════════════╣"),
        C(CYAN, "  ║") + C(SHADOW, "                    . . . . . . . . . . . .                     ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(STONE, "                 ┌────────────────────────┐                    ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(STONE, "                 │           ?            │                    ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(STONE, "                 │      porta cieca       │                    ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(STONE, "                 └───────────┬────────────┘                    ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(SHADOW, "                             │                                  ") + C(CYAN, "║"),
        C(CYAN, "  ║") + C(GRAY, "                il tempo qui non sa che forma prendere          ") + C(CYAN, "║"),
        C(CYAN, "  ╚════════════════════════════════════════════════════════════════════╝"));

    // ==================== NPC ====================

    private static string NPC_COOK_ART => Lines(
        C(MAGENTA, "  ╔══════════════════════════════════════════════════════╗"),
        C(MAGENTA, "  ║") + C(B_YELLOW, "                  CHEF MARCO                         ") + C(MAGENTA, "║"),
        C(MAGENTA, "  ╠══════════════════════════════════════════════════════╣"),
        C(MAGENTA, "  ║") + C(PAPER, "              .-''''''-.                              ") + C(MAGENTA, "║"),
        C(MAGENTA, "  ║") + C(PAPER, "             /  CHEF   \\                             ") + C(MAGENTA, "║"),
        C(MAGENTA, "  ║") + C(PAPER, "            |___________|                            ") + C(MAGENTA, "║"),
        C(MAGENTA, "  ║") + C(STONE, "             (  o   o  )     ") + C(GRAY, "mani ferme, sguardo rapido") + C(MAGENTA, " ║"),
        C(MAGENTA, "  ║") + C(STONE, "              \\   ^   /                              ") + C(MAGENTA, "║"),
        C(MAGENTA, "  ║") + C(STONE, "             __\\ '-' /__                             ") + C(MAGENTA, "║"),
        C(MAGENTA, "  ║") + C(WOOD, "            /|  '---'  |\\     ") + C(AMBER, "profuma di brodo e rame") + C(MAGENTA, " ║"),
        C(MAGENTA, "  ║") + C(WOOD, "           /_|_________|_\\                           ") + C(MAGENTA, "║"),
        C(MAGENTA, "  ╚══════════════════════════════════════════════════════╝"));

    private static string NPC_LIBRARIAN_ART => Lines(
        C(MAGENTA, "  ╔══════════════════════════════════════════════════════╗"),
        C(MAGENTA, "  ║") + C(B_YELLOW, "                SIGNORA LUCIA                       ") + C(MAGENTA, "║"),
        C(MAGENTA, "  ╠══════════════════════════════════════════════════════╣"),
        C(MAGENTA, "  ║") + C(PAPER, "               .-~~~~~~~~-.                          ") + C(MAGENTA, "║"),
        C(MAGENTA, "  ║") + C(PAPER, "              /  _    _   \\                         ") + C(MAGENTA, "║"),
        C(MAGENTA, "  ║") + C(STONE, "             |  (o)  (o)  |   ") + C(GRAY, "occhi dietro lenti spesse") + C(MAGENTA, "║"),
        C(MAGENTA, "  ║") + C(STONE, "             |     <>     |                         ") + C(MAGENTA, "║"),
        C(MAGENTA, "  ║") + C(STONE, "              \\  .____.  /                          ") + C(MAGENTA, "║"),
        C(MAGENTA, "  ║") + C(LILAC, "            ___'-.____.-'___    ") + C(GRAY, "custode di date sbagliate") + C(MAGENTA, "║"),
        C(MAGENTA, "  ║") + C(LILAC, "           /   |  diario |   \\                      ") + C(MAGENTA, "║"),
        C(MAGENTA, "  ║") + C(LILAC, "          /____|_________|____\\                     ") + C(MAGENTA, "║"),
        C(MAGENTA, "  ╚══════════════════════════════════════════════════════╝"));

    // ==================== SISTEMA ====================

    private static string TROPHY_ART => Lines(
        C(BRASS, "  ╔══════════════════════════════════════════════════════╗"),
        C(BRASS, "  ║                  TROFEI SBLOCCATI                   ║"),
        C(BRASS, "  ╠══════════════════════════════════════════════════════╣"),
        C(BRASS, "  ║             _____________                            ║"),
        C(BRASS, "  ║        ____/             \\____                       ║"),
        C(BRASS, "  ║       /    \\   OLTRE    /    \\                      ║"),
        C(BRASS, "  ║       \\____/ IL TEMPO   \\____/                      ║"),
        C(BRASS, "  ║            \\___________/                             ║"),
        C(BRASS, "  ║                 |||                                  ║"),
        C(BRASS, "  ║              ___|||___                               ║"),
        C(BRASS, "  ║             |_________|                              ║"),
        C(BRASS, "  ╚══════════════════════════════════════════════════════╝"));

    private static string INVENTORY_ART => Lines(
        C(EMERALD, "  ╔══════════════════════════════════════════════════════╗"),
        C(EMERALD, "  ║                    INVENTARIO                       ║"),
        C(EMERALD, "  ╠══════════════════════════════════════════════════════╣"),
        C(EMERALD, "  ║             ________________________                 ║"),
        C(EMERALD, "  ║            /  tasca temporale       \\                ║"),
        C(EMERALD, "  ║           /__________________________\\               ║"),
        C(EMERALD, "  ║           |  []  []  []  []  []      |               ║"),
        C(EMERALD, "  ║           |  prove, chiavi, memorie  |               ║"),
        C(EMERALD, "  ║           |__________________________|               ║"),
        C(EMERALD, "  ╚══════════════════════════════════════════════════════╝"));

    private static string DEDUCTION_ART => Lines(
        C(LILAC, "  ╔══════════════════════════════════════════════════════╗"),
        C(LILAC, "  ║                      DEDUZIONE                      ║"),
        C(LILAC, "  ╠══════════════════════════════════════════════════════╣"),
        C(LILAC, "  ║              .----------------------.                ║"),
        C(LILAC, "  ║             /   indizio  ──╮        /|               ║"),
        C(LILAC, "  ║            /       ╭───────┼── prova/ |              ║"),
        C(LILAC, "  ║           /  diario╯       ╰── tempo/  |             ║"),
        C(LILAC, "  ║          '----------------------'   |                 ║"),
        C(LILAC, "  ║              collega, verifica, rischia               ║"),
        C(LILAC, "  ╚══════════════════════════════════════════════════════╝"));

    private static string LEVEL_COMPLETE_ART => Lines(
        C(GREEN, "  ╔══════════════════════════════════════════════════════╗"),
        C(GREEN, "  ║              LIVELLO COMPLETATO                     ║"),
        C(GREEN, "  ╠══════════════════════════════════════════════════════╣"),
        C(GREEN, "  ║        ███╗   ███╗ISSIONE  AVANZATA                 ║"),
        C(GREEN, "  ║        ╚██╗ ██╔╝  nuovi fili entrano nel quadro      ║"),
        C(GREEN, "  ║          ╚███╔╝   il tempo concede un altro passo    ║"),
        C(GREEN, "  ╚══════════════════════════════════════════════════════╝"));

    private static string GAME_OVER_ART => Lines(
        C(RED, "  ╔══════════════════════════════════════════════════════╗"),
        C(RED, "  ║                      GAME OVER                       ║"),
        C(RED, "  ╠══════════════════════════════════════════════════════╣"),
        C(RED, "  ║       ██████╗  █████╗ ███╗   ███╗███████╗            ║"),
        C(RED, "  ║      ██╔════╝ ██╔══██╗████╗ ████║██╔════╝            ║"),
        C(RED, "  ║      ██║  ███╗███████║██╔████╔██║█████╗              ║"),
        C(RED, "  ║      ██║   ██║██╔══██║██║╚██╔╝██║██╔══╝              ║"),
        C(RED, "  ║      ╚██████╔╝██║  ██║██║ ╚═╝ ██║███████╗            ║"),
        C(RED, "  ║       Il sospetto ha bruciato la tua copertura.       ║"),
        C(RED, "  ╚══════════════════════════════════════════════════════╝"));

    private static string VICTORY_ART => Lines(
        C(GREEN, "  ╔══════════════════════════════════════════════════════╗"),
        C(GREEN, "  ║                       VITTORIA                       ║"),
        C(GREEN, "  ╠══════════════════════════════════════════════════════╣"),
        C(GREEN, "  ║          *      *      *      *      *               ║"),
        C(GREEN, "  ║             CRONONAUTA, MISTERO RISOLTO              ║"),
        C(GREEN, "  ║          *      *      *      *      *               ║"),
        C(GREEN, "  ║       La villa tace. Il tempo torna a respirare.     ║"),
        C(GREEN, "  ╚══════════════════════════════════════════════════════╝"));

    // ==================== OGGETTI ====================

    private static string ITEM_KEY_ART => Lines(
        C(B_YELLOW, "  ╔════════════════════════════════════╗"),
        C(B_YELLOW, "  ║              CHIAVE                ║"),
        C(B_YELLOW, "  ╠════════════════════════════════════╣"),
        C(B_YELLOW, "  ║          .------.                  ║"),
        C(B_YELLOW, "  ║         /  .--.  \\                 ║"),
        C(B_YELLOW, "  ║        |  (____)  |=====[__]       ║"),
        C(B_YELLOW, "  ║         \\        /        |        ║"),
        C(B_YELLOW, "  ║          '------'       __|__      ║"),
        C(B_YELLOW, "  ╚════════════════════════════════════╝"));

    private static string ITEM_DOC_ART => Lines(
        C(PAPER, "  ╔════════════════════════════════════╗"),
        C(PAPER, "  ║             DOCUMENTO              ║"),
        C(PAPER, "  ╠════════════════════════════════════╣"),
        C(PAPER, "  ║        __________________          ║"),
        C(PAPER, "  ║       /  ~~~~~~~~~~~~~  /|         ║"),
        C(PAPER, "  ║      /  data: ????     / |         ║"),
        C(PAPER, "  ║     /  firma tremante /  |         ║"),
        C(PAPER, "  ║    /_________________/  /          ║"),
        C(PAPER, "  ║    |_________________| /           ║"),
        C(PAPER, "  ╚════════════════════════════════════╝"));

    private static string ITEM_EVIDENCE_ART => Lines(
        C(ROSE, "  ╔════════════════════════════════════╗"),
        C(ROSE, "  ║               PROVA                ║"),
        C(ROSE, "  ╠════════════════════════════════════╣"),
        C(ROSE, "  ║            .------------.          ║"),
        C(ROSE, "  ║           /   SIGILLO   /|         ║"),
        C(ROSE, "  ║          /_____________/ |         ║"),
        C(ROSE, "  ║          |  reperto #  | /         ║"),
        C(ROSE, "  ║          '------------'            ║"),
        C(ROSE, "  ╚════════════════════════════════════╝"));

    private static string ITEM_FUTURE_ART => Lines(
        C(B_CYAN, "  ╔════════════════════════════════════╗"),
        C(B_CYAN, "  ║          OGGETTO FUTURO            ║"),
        C(B_CYAN, "  ╠════════════════════════════════════╣"),
        C(B_CYAN, "  ║              .-====-.              ║"),
        C(B_CYAN, "  ║          ___/  20??  \\___          ║"),
        C(B_CYAN, "  ║         /   \\  ||||  /   \\         ║"),
        C(B_CYAN, "  ║         \\___/--====--\\___/         ║"),
        C(B_CYAN, "  ║              eco temporale         ║"),
        C(B_CYAN, "  ╚════════════════════════════════════╝"));

    private static string ITEM_CONTAINER_ART => Lines(
        C(WOOD, "  ╔════════════════════════════════════╗"),
        C(WOOD, "  ║             CONTENITORE            ║"),
        C(WOOD, "  ╠════════════════════════════════════╣"),
        C(WOOD, "  ║          .----------------.        ║"),
        C(WOOD, "  ║         /________________/|        ║"),
        C(WOOD, "  ║         |  __        __  | |       ║"),
        C(WOOD, "  ║         | |  |  ()  |  | | /       ║"),
        C(WOOD, "  ║         |________________|/        ║"),
        C(WOOD, "  ╚════════════════════════════════════╝"));

    private static string ITEM_GENERIC_ART => Lines(
        C(STONE, "  ╔════════════════════════════════════╗"),
        C(STONE, "  ║              OGGETTO               ║"),
        C(STONE, "  ╠════════════════════════════════════╣"),
        C(STONE, "  ║             __________             ║"),
        C(STONE, "  ║            /   ????   \\            ║"),
        C(STONE, "  ║           |  non basta |           ║"),
        C(STONE, "  ║           |  guardarlo |           ║"),
        C(STONE, "  ║            \\__________/            ║"),
        C(STONE, "  ╚════════════════════════════════════╝"));

    // ==================== ESTERNO E SPLASH ====================

    private static string VILLA_EXTERIOR_ART => Lines(
        C(B_CYAN, "                      .        *          .             *"),
        C(B_CYAN, "            *                 .       .          ."),
        C(STONE, "                 ______________________________________________"),
        C(STONE, "                /_____________________________________________/|"),
        C(STONE, "               /_____________________________________________/ |"),
        C(STONE, "              |      |                          |      |     | |"),
        C(STONE, "              | |  | |") + C(B_YELLOW, "       VILLA REALE        ") + C(STONE, "| |  | |     | |"),
        C(STONE, "              | |__| |__________________________|_|__| |     | |"),
        C(STONE, "             /|        \\     ____    ____     /        \\    | |"),
        C(STONE, "            / |         \\   | [] |  | [] |   /          \\   | /"),
        C(STONE, "           /__|    []    \\  |____|  |____|  /    []      \\__|/"),
        C(WOOD, "          |   |          |    ____[]____    |             |"),
        C(WOOD, "          |___|__________|___|          |___|_____________|"),
        C(SHADOW, "             /________________________________________________\\"),
        C(SHADOW, "            /__________________________________________________\\"));

    private static string SPLASH_ART => Lines(
        C(B_CYAN, "  ╔════════════════════════════════════════════════════════════════════╗"),
        C(B_CYAN, "  ║                                                                    ║"),
        C(B_CYAN, "  ║") + C(B_YELLOW, "        ██████╗ ██╗  ████████╗██████╗ ███████╗                 ") + C(B_CYAN, "║"),
        C(B_CYAN, "  ║") + C(B_YELLOW, "       ██╔═══██╗██║  ╚══██╔══╝██╔══██╗██╔════╝                 ") + C(B_CYAN, "║"),
        C(B_CYAN, "  ║") + C(B_YELLOW, "       ██║   ██║██║     ██║   ██████╔╝█████╗                   ") + C(B_CYAN, "║"),
        C(B_CYAN, "  ║") + C(B_YELLOW, "       ██║   ██║██║     ██║   ██╔══██╗██╔══╝                   ") + C(B_CYAN, "║"),
        C(B_CYAN, "  ║") + C(B_YELLOW, "       ╚██████╔╝███████╗██║   ██║  ██║███████╗                 ") + C(B_CYAN, "║"),
        C(B_CYAN, "  ║") + C(B_WHITE, "                 IL TEMPO                                      ") + C(B_CYAN, "║"),
        C(B_CYAN, "  ║") + C(GRAY, "              Un mistero attraverso epoche spezzate             ") + C(B_CYAN, "║"),
        C(B_CYAN, "  ║                                                                    ║"),
        C(B_CYAN, "  ╚════════════════════════════════════════════════════════════════════╝"));

    // ==================== HELPER ====================

    private static string C(string color, string text) => $"{color}{text}{RESET}";

    private static string Lines(params string[] lines) => string.Join(Environment.NewLine, lines);

    private static string Lines(IEnumerable<string> lines) => string.Join(Environment.NewLine, lines);

    private static string Framed(string text, string color)
    {
        string fitted = Fit(text, BoxWidth);
        return C(CYAN, "  ║") + C(color, fitted.PadRight(BoxWidth)) + C(CYAN, "║");
    }

    private static string Fit(string text, int width)
    {
        if (text.Length <= width)
        {
            return text;
        }

        return width <= 3 ? text[..width] : $"{text[..(width - 3)]}...";
    }

    private static IEnumerable<string> Wrap(string text, int width)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            yield break;
        }

        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var current = "";

        foreach (var word in words)
        {
            if (word.Length > width)
            {
                if (current.Length > 0)
                {
                    yield return current;
                    current = "";
                }

                for (int i = 0; i < word.Length; i += width)
                {
                    yield return word.Substring(i, Math.Min(width, word.Length - i));
                }

                continue;
            }

            if (current.Length == 0)
            {
                current = word;
            }
            else if (current.Length + word.Length + 1 <= width)
            {
                current += " " + word;
            }
            else
            {
                yield return current;
                current = word;
            }
        }

        if (current.Length > 0)
        {
            yield return current;
        }
    }

    private static string CompassPoint(bool active, string label) =>
        active ? C(B_YELLOW + BOLD, label) : C(SHADOW, "·");

    private static string MapMark(string? currentRoomId, string roomId) =>
        currentRoomId == roomId ? C(B_YELLOW + BOLD, "@") : " ";
}
