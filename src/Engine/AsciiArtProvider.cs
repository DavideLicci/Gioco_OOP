namespace OltreIlTempo.Engine;

/// <summary>
/// Fornisce ASCII/ANSI art per stanze, dialoghi, mappa, NPC e schermate di sistema.
/// </summary>
public static class AsciiArtProvider
{
    // ==================== COLORI ANSI ====================
    // Stile / base
    public const string RESET = "\u001b[0m";
    public const string BOLD = "\u001b[1m";
    public const string DIM = "\u001b[2m";
    public const string ITALIC = "\u001b[3m";

    // 16-color base
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
    public const string B_GREEN = "\u001b[92m";
    public const string B_RED = "\u001b[91m";
    public const string B_BLUE = "\u001b[94m";

    // === RAMPE GRADIENT 256-color ===
    // Fuoco (dal rosso profondo al giallo brillante)
    public const string FIRE_DEEP = "\u001b[38;5;52m";
    public const string FIRE_LOW = "\u001b[38;5;124m";
    public const string FIRE_MID = "\u001b[38;5;202m";
    public const string FIRE_HIGH = "\u001b[38;5;214m";
    public const string FIRE_TOP = "\u001b[38;5;220m";
    public const string EMBER = "\u001b[38;5;166m";

    // Oro / ottone (dal bronzo scuro all'oro brillante)
    public const string GOLD_DEEP = "\u001b[38;5;94m";
    public const string GOLD_LOW = "\u001b[38;5;136m";
    public const string GOLD_MID = "\u001b[38;5;178m";
    public const string GOLD_HIGH = "\u001b[38;5;221m";
    public const string GOLD_TOP = "\u001b[38;5;229m";

    // Legno (mahogany → quercia chiara)
    public const string WOOD_DEEP = "\u001b[38;5;58m";
    public const string WOOD_LOW = "\u001b[38;5;94m";
    public const string WOOD_MID = "\u001b[38;5;130m";
    public const string WOOD_HIGH = "\u001b[38;5;179m";

    // Pietra
    public const string STONE_DEEP = "\u001b[38;5;236m";
    public const string STONE_LOW = "\u001b[38;5;240m";
    public const string STONE_MID = "\u001b[38;5;246m";
    public const string STONE_HIGH = "\u001b[38;5;252m";

    // Ghiaccio / freddo
    public const string ICE_DEEP = "\u001b[38;5;24m";
    public const string ICE_LOW = "\u001b[38;5;39m";
    public const string ICE_MID = "\u001b[38;5;111m";
    public const string ICE_HIGH = "\u001b[38;5;195m";

    // Cremisi / sangue / velluto
    public const string CRIMSON_DEEP = "\u001b[38;5;52m";
    public const string CRIMSON_LOW = "\u001b[38;5;88m";
    public const string CRIMSON_MID = "\u001b[38;5;124m";
    public const string CRIMSON_HIGH = "\u001b[38;5;161m";
    public const string VELVET_DEEP = "\u001b[38;5;53m";
    public const string VELVET_MID = "\u001b[38;5;54m";

    // Ombra / notte
    public const string NIGHT = "\u001b[38;5;233m";
    public const string SHADOW_DEEP = "\u001b[38;5;235m";
    public const string SHADOW_MID = "\u001b[38;5;238m";
    public const string SHADOW_HIGH = "\u001b[38;5;243m";

    // Atmosferici
    public const string MIST = "\u001b[38;5;188m";
    public const string SMOKE = "\u001b[38;5;245m";
    public const string SPARK = "\u001b[38;5;226m";
    public const string GLOW = "\u001b[38;5;229m";
    public const string MOONLIGHT = "\u001b[38;5;195m";
    public const string STARLIGHT = "\u001b[38;5;231m";
    public const string LAVENDER = "\u001b[38;5;147m";
    public const string PARCHMENT = "\u001b[38;5;230m";
    public const string CREAM = "\u001b[38;5;223m";

    // Background per finestre illuminate, fuoco, notte
    public const string BG_NIGHT = "\u001b[48;5;232m";
    public const string BG_FIRE = "\u001b[48;5;52m";
    public const string BG_GLOW = "\u001b[48;5;58m";
    public const string BG_STONE = "\u001b[48;5;236m";

    /// <summary>Testo su sfondo colorato (finestre illuminate, fuochi, ecc.).</summary>
    private static string BG(string bg, string fg, string text) => $"{bg}{fg}{text}{RESET}";

    // === Alias compatibilità (pezzi che usano i nomi corti) ===
    public const string AMBER = FIRE_HIGH;
    public const string BRASS = GOLD_MID;
    public const string COPPER = EMBER;
    public const string EMERALD = "\u001b[38;5;42m";
    public const string LILAC = "\u001b[38;5;141m";
    public const string PAPER = PARCHMENT;
    public const string ROSE = "\u001b[38;5;211m";
    public const string SHADOW = SHADOW_MID;
    public const string STONE = STONE_MID;
    public const string WOOD = WOOD_MID;
    public const string IVY = "\u001b[38;5;28m";
    public const string BLOOD = CRIMSON_LOW;
    public const string GOLD = GOLD_HIGH;
    public const string MOON = MOONLIGHT;
    public const string DUST = "\u001b[38;5;180m";
    public const string FIRE = FIRE_MID;
    public const string ICE = ICE_HIGH;
    public const string VELVET = VELVET_MID;
    public const string SAGE = "\u001b[38;5;108m";
    public const string TEAL = "\u001b[38;5;37m";
    public const string PLUM = "\u001b[38;5;96m";

    private const int BoxWidth = 68;

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

    private static string KITCHEN_ART
    {
        get
        {
            int VL(string s) =>
                System.Text.RegularExpressions.Regex.Replace(s, @"\x1B\[[0-9;]*[mGKHF]", "").Length;

            string bdr = RGB(140,92,38);
            string BR(string c) { int p=Math.Max(0,BoxWidth-VL(c)); return $"{bdr}  ║{RESET}{c}{new string(' ',p)}{bdr}║{RESET}"; }

            string TL = C(bdr,$"  ╔{new string('═',BoxWidth)}╗");
            string BL = C(bdr,$"  ╚{new string('═',BoxWidth)}╝");
            string ML = C(RGB(100,62,20),$"  ╠{new string('─',BoxWidth)}╣");
            string ER = $"  {bdr}║{RESET}{new string(' ',BoxWidth)}{bdr}║{RESET}";

            // Vapore
            string sm = RGB(90,98,118);
            string ms = RGB(140,155,180);

            // Spezieria
            string spW = RGB(132,88,38); // wood frame
            string spG = RGB(215,175,42); // gold label
            string spH = RGB(115,152,72); // herb green
            string spS = RGB(195,162,28); // spice amber
            string spP = RGB(190,140,200); // diary

            // Raggi finestra
            string rayH = RGB(255,242,180);
            string rayM = RGB(215,200,130);

            // Forno border
            string fnB = RGB(88,28,6);
            // Pixel-art fire 12 chars wide × 3 rows = 12×6 virtual pixels
            // Row 1: bright tips, white-yellow → orange gradient
            string fr1 =
                Px(255,255,165,255,238,52)+Px(255,255,165,255,238,52)+
                Px(255,228,38,255,198,8)+Px(255,228,38,255,198,8)+Px(255,228,38,255,198,8)+Px(255,228,38,255,198,8)+
                Px(255,188,0,255,155,0)+Px(255,188,0,255,155,0)+Px(255,188,0,255,155,0)+Px(255,188,0,255,155,0)+
                Px(255,255,165,255,238,52)+Px(255,255,165,255,238,52);
            // Row 2: orange → red-orange
            string fr2 =
                Px(255,195,8,255,142,0)+Px(255,195,8,255,142,0)+Px(255,195,8,255,142,0)+
                Px(255,112,0,238,68,0)+Px(255,112,0,238,68,0)+Px(255,112,0,238,68,0)+
                Px(228,48,0,202,18,0)+Px(228,48,0,202,18,0)+Px(228,48,0,202,18,0)+
                Px(192,8,0,165,0,0)+Px(192,8,0,165,0,0)+Px(192,8,0,165,0,0);
            // Row 3: deep red → coal embers
            string fr3 =
                Px(168,0,0,120,0,0)+Px(168,0,0,120,0,0)+Px(168,0,0,120,0,0)+Px(168,0,0,120,0,0)+
                Px(130,0,0,85,0,0)+Px(130,0,0,85,0,0)+Px(130,0,0,85,0,0)+Px(130,0,0,85,0,0)+
                Px(95,0,0,52,0,0)+Px(95,0,0,52,0,0)+Px(95,0,0,52,0,0)+Px(95,0,0,52,0,0);

            // Rame pots
            string cuB = RGB(188,108,32);
            string cuF = RGB(215,135,45);

            // Tavolone
            string tbW = RGB(165,110,45);
            string tbL = RGB(145,95,35);

            return Lines(
                TL,
                C(RGB(90,98,118),$"  ╠{new string('─',BoxWidth)}╣"),
                BR(C(sm,"  ")+C(ms,"◌")+C(sm," ")+C(ms,"◌◌")+C(sm," ")+C(ms,"◌")+C(sm,"  ")+C(ms,"◌")+C(sm," ")+C(ms,"◌◌")+C(sm,"  ")+C(ms,"◌")+C(sm," vapore")+C(ms," ◌◌")+C(sm," ")+C(ms,"◌")+C(sm,"  ")+C(ms,"◌")+C(sm," ")+C(ms,"◌◌")+C(sm," ")+C(ms,"◌")),
                BR(C(sm,"  ")+C(ms,"░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░")+C(sm," ")),
                ER,
                BR(C(spW," ╔═════════════════╗")+"  "+C(rayM," ╲  │  ╱  luce")+"  "+C(fnB,"╔════════════╗")+"  "+C(cuB," ╦  ╦  ╦")),
                BR(C(spW," ║")+C(spG," SPEZIE 1850   ")+C(spW,"║")+"   "+C(rayH," ╲ │ ╱")+"       "+C(fnB,"║")+C(cuF,"  ◐ ◐ ◐  ")+C(fnB,"║")+"   "+C(cuB," ◢█◣◢█◣◢█◣")),
                BR(C(spW," ╠═════════════════╣")+"  "+C(rayM,"──")+RCB(215,172,42,"✦")+C(rayM,"──")+"      "+C(fnB,"╠════════════╣")+"   "+C(cuB," ◥█◤◥█◤◥█◤")),
                BR(C(spW," ║")+RCB(215,175,42,"◇")+C(spH," erbe  ")+RCB(215,175,42,"◇")+C(spS," spezie")+C(spW," ║")+"  "+C(rayH,"  ╲ │ ╱")+"     "+C(fnB,"║")+" "+fr1+" "+C(fnB,"║")+"  "+C(cuB,"  │   │   │")),
                BR(C(spW," ║")+C(spG," 1850 ")+C(spP,"diario  ")+C(spW,"║")+"  "+C(rayM,"   ═══")+"      "+C(fnB,"║")+" "+fr2+" "+C(fnB,"║")+"  "+C(sm,"░░░░░░░░░░░░")),
                BR(C(spW," ║")+C(spP,"   ✒ nota    ")+C(spW," ║")+"  "+C(rayM,"   │")+"          "+C(fnB,"║")+" "+fr3+" "+C(fnB,"║")+"  "+C(RGB(105,102,115),"  credenza  ")),
                BR(C(spW," ╚═════════════════╝")+"  "+C(rayM,"   ═══")+"       "+C(fnB,"╚════════════╝")),
                ER,
                C(tbW,$"  ║ ┏{new string('━',BoxWidth-4)}┓ ║"),
                BR(" ┃ "+RCB(215,190,135,"▣ nota")+"   "+C(RGB(160,25,25),"▍▍")+RCB(190,30,30," coltello ")+C(RGB(160,25,25),"▍▍")+"   "+RCB(215,172,42,"◊◊ chiavi di rame")+"    ┃"),
                BR(" ┃ "+C(RGB(100,115,138),"anno 2024")+"  "+C(RGB(185,48,48),"lama affilata")+"        "+C(RGB(195,160,35),"⛓  tre serrature")+"      ┃"),
                BR(" ┃ "+C(tbL,"── tavolone di quercia massiccia ──")+" "+C(RGB(108,105,118),"≡≡ vassoio")+"       ┃"),
                C(tbW,$"  ║ ┗{new string('━',BoxWidth-4)}┛ ║"),
                ER,
                BR(C(RGB(48,44,60),"  ░░░░ ombre di lancette si allungano sul pavimento ░░░░░░░░░░░░")),
                BR(C(RGB(215,205,180),"  Chef Marco lavora senza voltarsi: il coltello batte ritmico.")),
                BL
            );
        }
    }

    private static string HALLWAY_ART => Lines(
        BoxTop(),
        Header(" CORRIDOIO DEI RITRATTI ", " il legno scricchiola a ogni passo"),
        BoxMid(),
        EmptyRow(),
        Row(
            (GOLD, "   ╔══════╗"), (DIM, "  "), (GOLD, "╔══════╗"), (DIM, "  "), (GOLD, "╔══════╗"), (DIM, "  "), (GOLD, "╔══════╗"), (DIM, "  "), (GOLD, "╔══════╗")),
        Row(
            (GOLD, "   ║"), (PAPER, " ◉◉ "), (GOLD, "║"), (DIM, "  "), (GOLD, "║"), (PAPER, " ▔▔ "), (GOLD, "║"), (DIM, "  "), (GOLD, "║"), (PAPER, " ◐◑ "), (GOLD, "║"), (DIM, "  "), (GOLD, "║"), (PAPER, " ✦✦ "), (GOLD, "║"), (DIM, "  "), (GOLD, "║"), (PAPER, " ◯◯ "), (GOLD, "║")),
        Row(
            (GOLD, "   ║"), (PAPER, " ╲╱ "), (GOLD, "║"), (DIM, "  "), (GOLD, "║"), (PAPER, " __ "), (GOLD, "║"), (DIM, "  "), (GOLD, "║"), (PAPER, " ─── "), (GOLD, "║"), (DIM, " "), (GOLD, "║"), (PAPER, " ╲╱ "), (GOLD, "║"), (DIM, "  "), (GOLD, "║"), (PAPER, " ◡  "), (GOLD, "║")),
        Row(
            (BRASS, "  ══╧══════╧════╧══════╧════╧══════╧════╧══════╧════╧══════╧══")),
        Row(
            (WOOD, "      ║"), (DIM, "         "), (WOOD, "║"), (DIM, "         "), (WOOD, "║"), (DIM, "         "), (WOOD, "║"), (DIM, "         "), (WOOD, "║")),
        Row(
            (AMBER, "      ✦"), (DIM, "         "), (AMBER, "✦"), (DIM, "         "), (AMBER, "✦"), (DIM, "         "), (AMBER, "✦"), (DIM, "         "), (AMBER, "✦"), (DIM, "  "), (GRAY, "lampade")),
        Row(
            (WOOD, "      ║"), (DIM, "         "), (WOOD, "║"), (DIM, "         "), (WOOD, "║"), (DIM, "         "), (WOOD, "║"), (DIM, "         "), (WOOD, "║"), (DIM, "  "), (GRAY, "a gas  ")),
        Row(
            (SHADOW, "  ─────╨──────────╨──────────╨──────────╨──────────╨───────────")),
        Row(
            (BLOOD, "  ▓▓▓▓▓▓▓▓▓▓ tappeto cremisi ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓")),
        EmptyRow(),
        Row(
            (GRAY, "    il corridoio tira verso biblioteca, camera e cucina")),
        BoxBot());

    private static string PANTRY_ART => Lines(
        BoxTop(),
        Header(" DISPENSA ", " scaffali stretti, vetro, polvere e una chiave antica"),
        BoxMid(),
        EmptyRow(),
        Row(
            (WOOD, "   ┌────────────────────────────────────────────────────────────┐")),
        Row(
            (WOOD, "   │ "), (PAPER, "[farina]"), (DIM, " "), (PAPER, "[sale]"), (DIM, " "), (PAPER, "[zucchero]"), (DIM, "  "), (SAGE, "{menta}"), (DIM, "  "), (BLOOD, "(vino)"), (WOOD, "      │")),
        Row(
            (WOOD, "   ├────────────────────────────────────────────────────────────┤")),
        Row(
            (WOOD, "   │ "), (AMBER, "{miele}"), (DIM, "  "), (PAPER, "[riso]"), (DIM, " "), (PAPER, "[pasta]"), (DIM, "   "), (BRASS, "◯ ◯ ◯"), (DIM, " "), (DUST, "spezie rare"), (WOOD, "  │")),
        Row(
            (WOOD, "   ├────────────────────────────────────────────────────────────┤")),
        Row(
            (WOOD, "   │ "), (ICE, "(acqua)"), (DIM, "  "), (WOOD, "[caffe]"), (DIM, " "), (SAGE, "[te]"), (DIM, "   "), (GRAY, "barattoli senza nome"), (WOOD, "    │")),
        Row(
            (WOOD, "   └────────────────────────────────────────────────────────────┘")),
        EmptyRow(),
        Row(
            (SHADOW, "       .-------------.       "), (GOLD, "   ✦✦✦✦"), (DIM, "  "), (GRAY, "bagliore metallico")),
        Row(
            (SHADOW, "      / pavim. freddo/|      "), (GOLD, "  ╔════╗"), (DIM, " "), (GRAY, "vicino allo stipite")),
        Row(
            (SHADOW, "     '-------------' |       "), (GOLD, "  ║ K ║"), (DIM, "  "), (PAPER, "una chiave antica")),
        Row(
            (SHADOW, "      |_____________|/       "), (GOLD, "  ╚════╝")),
        BoxBot());

    private static string LIBRARY_ART => Lines(
        BoxTop(),
        Header(" BIBLIOTECA ", " odore di cuoio, polvere e memoria non lineare"),
        BoxMid(),
        EmptyRow(),
        Row(
            (WOOD_LOW, " ╔═╤═╤═╤═╤═╤═╤═╤═╤═╗"), (DIM, "        "), (TEAL, "    ___    "), (DIM, "        "),
            (WOOD_LOW, "╔═╤═╤═╤═╤═╤═╤═╤═╤═╗")),
        Row(
            (WOOD_LOW, " ║"),
            (ROSE, "▐▐"), (GOLD_MID, "▐▐"), (ICE_LOW, "▐▐"), (SAGE, "▐▐"), (LAVENDER, "▐▐"), (CRIMSON_MID, "▐▐"), (PARCHMENT, "▐▐"), (GOLD_LOW, "▐▐"), (DUST, "▐▐"),
            (WOOD_LOW, "║"), (DIM, "       "), (TEAL, "  /  ◉  \\"), (DIM, "       "),
            (WOOD_LOW, "║"),
            (LILAC, "▐▐"), (ICE_MID, "▐▐"), (ROSE, "▐▐"), (SAGE, "▐▐"), (GOLD_MID, "▐▐"), (CRIMSON_LOW, "▐▐"), (LAVENDER, "▐▐"), (PARCHMENT, "▐▐"), (TEAL, "▐▐"),
            (WOOD_LOW, "║")),
        Row(
            (WOOD_LOW, " ╠═╪═╪═╪═╪═╪═╪═╪═╪═╣"), (DIM, "       "), (TEAL, "|  ─────  |"), (DIM, "       "),
            (WOOD_LOW, "╠═╪═╪═╪═╪═╪═╪═╪═╪═╣")),
        Row(
            (WOOD_LOW, " ║"), (ROSE, "1901"), (WOOD_LOW, "║"), (GOLD_LOW, "1850"), (WOOD_LOW, "║"), (LILAC, " ?? "), (WOOD_LOW, "║"), (PARCHMENT, "DIAR"),
            (WOOD_LOW, "║"), (DIM, "        "), (TEAL, " \\_____/ "), (DIM, "        "),
            (WOOD_LOW, "║"), (ICE_LOW, "MAP "), (WOOD_LOW, "║"), (ROSE, "ATTI"), (WOOD_LOW, "║"), (LAVENDER, "SIG "), (WOOD_LOW, "║")),
        Row(
            (WOOD_LOW, " ╚═╧═╧═╧═╧═╧═╧═╧═╧═╝"), (DIM, "       "), (GRAY, "mappamondo"), (DIM, "        "),
            (WOOD_LOW, "╚═╧═╧═╧═╧═╧═╧═╧═╧═╝")),
        EmptyRow(),
        Row((WOOD_MID, "       ╭──────────────────────────────────────────────────╮")),
        Row(
            (WOOD_MID, "      /  "),
            (PARCHMENT, "▣ diario"), (DIM, "   "), (GOLD_MID, "✒ penna"), (DIM, "   "),
            (CRIMSON_MID, "● calamaio"), (DIM, "   "),
            (FIRE_HIGH, "✦"), (DIM, " "), (FIRE_TOP, "│"), (WOOD_MID, "    /│")),
        Row(
            (WOOD_MID, "     /   "), (PARCHMENT, "└──┘"), (DIM, "       "), (GOLD_LOW, "│"), (DIM, "          "),
            (CRIMSON_LOW, "○"), (DIM, "        "), (FIRE_MID, "lume"), (WOOD_MID, "  / │")),
        Row((WOOD_MID, "    ╰──────────────────────────────────────────────────╯ /")),
        Row((SHADOW_MID, "                                                       │/")),
        EmptyRow(),
        Row((CREAM, "    la Signora Lucia segue ogni fruscio di pagina")),
        BoxBot());

    private static string BEDROOM_ART => Lines(
        BoxTop(),
        Header(" CAMERA DA LETTO ", " seta pesante, specchi e silenzio trattenuto"),
        BoxMid(),
        Row(
            (MOONLIGHT, "  ╲"), (MIST, " ·"), (MOONLIGHT, " ╲"), (MIST, "  ·"), (MOONLIGHT, "  ╲"), (MIST, "  ·  luce lunare"), (DIM, "         "),
            (STONE_MID, "┌────────────────┐")),
        Row(
            (MOONLIGHT, "   ╲"), (MIST, "   ╲"), (MOONLIGHT, "     ╲"), (DIM, "                   "),
            (STONE_MID, "│"), (MOONLIGHT, "   SPECCHIO    "), (STONE_MID, "│")),
        Row(
            (WOOD_MID, "   ┌──"), (MOONLIGHT, "╲"), (WOOD_MID, "──────────────────────────────────┐"), (DIM, "  "),
            (STONE_MID, "│  "), (MOONLIGHT, ".─────────.  "), (STONE_MID, "│")),
        Row(
            (WOOD_MID, "   │  "), (VELVET_DEEP, "▓▓"), (VELVET_MID, "▓▓"), (PLUM, " baldacchino "), (VELVET_MID, "▓▓"), (VELVET_DEEP, "▓▓▓▓"), (WOOD_MID, "  │"), (DIM, "  "),
            (STONE_MID, "│  "), (MOONLIGHT, "│ ◐  ◐  ◐ │  "), (STONE_MID, "│")),
        Row(
            (WOOD_MID, "   │  ╔══════════════════════════════╗  │"), (DIM, "  "),
            (STONE_MID, "│  "), (MOONLIGHT, "│  ─────────  │  "), (STONE_MID, "│")),
        Row(
            (WOOD_MID, "   │  ║"), (CRIMSON_MID, "▓▓"), (CRIMSON_LOW, "▓▓"), (CRIMSON_MID, " coperte cremisi "), (CRIMSON_LOW, "▓▓"), (CRIMSON_MID, "▓▓"), (WOOD_MID, " ║  │"), (DIM, "  "),
            (STONE_MID, "│  "), (MOONLIGHT, "' ────────── '  "), (STONE_MID, "│")),
        Row(
            (WOOD_MID, "   │  ║"), (ROSE, "░░"), (CRIMSON_HIGH, " cuscini di seta "), (ROSE, "░░"), (WOOD_MID, "    ║  │"), (DIM, "  "),
            (STONE_MID, "└────────────────┘")),
        Row(
            (WOOD_MID, "   │  ╚══════════════════════════════╝  │"), (DIM, "    "), (WOOD_MID, "┌──────────┐")),
        Row(
            (WOOD_MID, "   └────────────────────────────────────┘"), (DIM, "    "), (WOOD_MID, "│ "), (STONE_MID, "ARMADIO"), (WOOD_MID, "  │")),
        Row(
            (CRIMSON_LOW, "   ▓▓"), (CRIMSON_MID, "▓▓▓"), (CRIMSON_LOW, " tappeto persiano "), (CRIMSON_MID, "▓▓▓"), (CRIMSON_LOW, "▓▓"), (DIM, "   "), (WOOD_MID, "│  "), (GOLD_HIGH, "◉"), (DIM, "   "), (GOLD_MID, "◉"), (WOOD_MID, "   │")),
        Row(
            (CRIMSON_DEEP, "   ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓"), (DIM, "      "), (WOOD_MID, "└──────────┘")),
        BoxBot());

    private static string UNKNOWN_ROOM_ART => Lines(
        BoxTop(),
        Header(" STANZA SCONOSCIUTA ", " la villa non dovrebbe avere questo spazio"),
        BoxMid(),
        EmptyRow(),
        Row(
            (SHADOW, "          .  .  "), (LILAC, "?"), (SHADOW, "  .  .  .  "), (LILAC, "?"), (SHADOW, "  .  .  .  "), (LILAC, "?"), (SHADOW, "  .  .")),
        Row(
            (SHADOW, "       .  "), (LILAC, "?"), (SHADOW, "  .  .  ░░░░░░░░░░░░░░  .  .  "), (LILAC, "?"), (SHADOW, "  .")),
        Row(
            (STONE, "                  ┌──────────────────────────┐")),
        Row(
            (STONE, "                  │"), (DIM, "                          "), (STONE, "│")),
        Row(
            (STONE, "                  │"), (DIM, "          "), (LILAC, "▓▓ ? ▓▓"), (DIM, "         "), (STONE, "│")),
        Row(
            (STONE, "                  │"), (DIM, "      "), (GRAY, "porta cieca"), (DIM, "          "), (STONE, "│")),
        Row(
            (STONE, "                  │"), (DIM, "                          "), (STONE, "│")),
        Row(
            (STONE, "                  └─────────────┬────────────┘")),
        Row(
            (SHADOW, "                                │")),
        EmptyRow(),
        Row(
            (GRAY, "         il tempo qui non sa che forma prendere")),
        BoxBot());

    // ==================== NPC ====================

    private static string NPC_COOK_ART => Lines(
        BoxTop(MAGENTA),
        Header(" CHEF MARCO ", " testa china, coltello fermo, niente sorrisi", MAGENTA),
        BoxMid(MAGENTA),
        Row(MAGENTA, (DIM, "")),
        Row(MAGENTA, (DIM, "              "), (PAPER, " .───────────────. "), (DIM, "                ")),
        Row(MAGENTA, (DIM, "             "), (PAPER, "/  "), (STONE, "◆ CAPPELLO ◆"), (DIM, "  "), (PAPER, "\\"), (DIM, "               ")),
        Row(MAGENTA, (DIM, "            "), (PAPER, "|"), (DUST, "_________________"), (PAPER, "|"), (DIM, "               ")),
        Row(MAGENTA, (DIM, "            "), (STONE, "("), (COPPER, "  ◉"), (DIM, "         "), (COPPER, "◉  "), (STONE, ")"), (DIM, "  "), (GRAY, "sguardo rapido"), (DIM, "    ")),
        Row(MAGENTA, (DIM, "             "), (STONE, "\\"), (DIM, "    "), (STONE, "╰─────╯"), (DIM, "    "), (STONE, "/"), (DIM, "  "), (GRAY, "mani ferme"), (DIM, "        ")),
        Row(MAGENTA, (DIM, "           __"), (STONE, "\\' ─────────── '/__"), (DIM, "                       ")),
        Row(MAGENTA, (DIM, "          "), (WOOD, "/│"), (DIM, "  "), (PAPER, "divisa bianca"), (DIM, "   "), (WOOD, "│\\"), (DIM, "  "), (AMBER, "profuma di brodo e rame"), (DIM, "  ")),
        Row(MAGENTA, (DIM, "         "), (WOOD, "/_│_______________│_\\"), (DIM, "                       ")),
        Row(MAGENTA, (DIM, "")),
        BoxBot(MAGENTA));

    private static string NPC_LIBRARIAN_ART => Lines(
        BoxTop(MAGENTA),
        Header(" SIGNORA LUCIA ", " occhi dietro lenti spesse, custode di date sbagliate", MAGENTA),
        BoxMid(MAGENTA),
        Row(MAGENTA, (DIM, "")),
        Row(MAGENTA, (DIM, "              "), (PAPER, " .─────────────────. "), (DIM, "              ")),
        Row(MAGENTA, (DIM, "             "), (PAPER, "/  "), (STONE, "_"), (DIM, "         "), (STONE, "_"), (DIM, "  "), (PAPER, "\\"), (DIM, "             ")),
        Row(MAGENTA, (DIM, "            "), (STONE, "| "), (ICE, "(◎)"), (DIM, "       "), (ICE, "(◎)"), (DIM, " "), (STONE, "|"), (DIM, "  "), (GRAY, "lenti spesse"), (DIM, "    ")),
        Row(MAGENTA, (DIM, "            "), (STONE, "|       "), (STONE, "╰────╯"), (DIM, "      "), (STONE, "|"), (DIM, "                     ")),
        Row(MAGENTA, (DIM, "             "), (STONE, "\\   "), (PAPER, ".────────."), (DIM, "   "), (STONE, "/"), (DIM, "                      ")),
        Row(MAGENTA, (DIM, "           ___"), (LILAC, "' .────. '"), (LILAC, "___"), (DIM, "    "), (GRAY, "custode di date sbagliate"), (DIM, "  ")),
        Row(MAGENTA, (DIM, "          /   "), (LILAC, "│ diario │"), (LILAC, "   \\"), (DIM, "                           ")),
        Row(MAGENTA, (DIM, "         /────"), (LILAC, "│_________│"), (LILAC, "────\\"), (DIM, "                          ")),
        Row(MAGENTA, (DIM, "")),
        BoxBot(MAGENTA));

    // ==================== SISTEMA ====================

    private static string TROPHY_ART => Lines(
        BoxTop(BRASS),
        Header(" TROFEI SBLOCCATI ", " ogni traguardo è un'epoca superata", BRASS, GOLD, GRAY),
        BoxMid(BRASS),
        Row(BRASS, (DIM, "")),
        Row(BRASS, (DIM, "                     "), (GOLD, "   _____________"), (DIM, "                ")),
        Row(BRASS, (DIM, "                "), (GOLD, "_____/"), (BRASS, "             "), (GOLD, "\\_____"), (DIM, "             ")),
        Row(BRASS, (DIM, "               "), (GOLD, "/    \\"), (DIM, "   "), (AMBER, "OLTRE"), (DIM, "    "), (GOLD, "/    \\"), (DIM, "              ")),
        Row(BRASS, (DIM, "               "), (GOLD, "\\____/"), (DIM, " "), (AMBER, "IL TEMPO"), (DIM, " "), (GOLD, "\\____/"), (DIM, "              ")),
        Row(BRASS, (DIM, "                    "), (GOLD, "\\___________/"), (DIM, "                   ")),
        Row(BRASS, (DIM, "                         "), (GOLD, "│││"), (DIM, "                        ")),
        Row(BRASS, (DIM, "                      "), (GOLD, "___│││___"), (DIM, "                     ")),
        Row(BRASS, (DIM, "                     "), (GOLD, "|_________|"), (DIM, "                    ")),
        Row(BRASS, (DIM, "")),
        BoxBot(BRASS));

    private static string INVENTORY_ART => Lines(
        BoxTop(EMERALD),
        Header(" INVENTARIO ", " tasca temporale — tutto ciò che hai raccolto", EMERALD, B_GREEN, GRAY),
        BoxMid(EMERALD),
        Row(EMERALD, (DIM, "")),
        Row(EMERALD, (DIM, "            "), (EMERALD, "  ________________________"), (DIM, "               ")),
        Row(EMERALD, (DIM, "           "), (EMERALD, " / "), (SAGE, "tasca temporale"), (EMERALD, "       \\"), (DIM, "              ")),
        Row(EMERALD, (DIM, "          "), (EMERALD, " /__________________________\\"), (DIM, "              ")),
        Row(EMERALD, (DIM, "          "), (EMERALD, " | "), (GOLD, "□"), (DIM, " "), (GOLD, "□"), (DIM, " "), (GOLD, "□"), (DIM, " "), (GOLD, "□"), (DIM, " "), (GOLD, "□"), (DIM, " "), (GOLD, "□"), (EMERALD, "              |"), (DIM, "              ")),
        Row(EMERALD, (DIM, "          "), (EMERALD, " | "), (GRAY, "prove, chiavi, memorie"), (EMERALD, "   |"), (DIM, "              ")),
        Row(EMERALD, (DIM, "          "), (EMERALD, " |__________________________|"), (DIM, "              ")),
        Row(EMERALD, (DIM, "")),
        BoxBot(EMERALD));

    private static string DEDUCTION_ART => Lines(
        BoxTop(LILAC),
        Header(" DEDUZIONE ", " connetti gli indizi, verifica le prove, rischia", LILAC, B_MAGENTA, GRAY),
        BoxMid(LILAC),
        Row(LILAC, (DIM, "")),
        Row(LILAC, (DIM, "          "), (LILAC, " .────────────────────────."), (DIM, "              ")),
        Row(LILAC, (DIM, "         "), (LILAC, "/  "), (ROSE, "indizio"), (DIM, "  "), (LILAC, "──╮"), (DIM, "          "), (LILAC, "/│"), (DIM, "              ")),
        Row(LILAC, (DIM, "        "), (LILAC, "/  "), (DIM, "       "), (LILAC, "╭──────┼──"), (PAPER, "prova"), (LILAC, "/"), (DIM, " │"), (DIM, "              ")),
        Row(LILAC, (DIM, "       "), (LILAC, "/  "), (AMBER, "diario"), (LILAC, "╯       ╰──"), (ICE, "tempo"), (LILAC, "/"), (DIM, "  │"), (DIM, "              ")),
        Row(LILAC, (DIM, "      "), (LILAC, " '────────────────────────'"), (DIM, "   │"), (DIM, "              ")),
        Row(LILAC, (DIM, "")),
        Row(LILAC, (DIM, "           "), (B_MAGENTA, "collega"), (DIM, "  "), (LILAC, "·"), (DIM, "  "), (STONE, "verifica"), (DIM, "  "), (LILAC, "·"), (DIM, "  "), (ROSE, "rischia"), (DIM, "             ")),
        Row(LILAC, (DIM, "")),
        BoxBot(LILAC));

    private static string LEVEL_COMPLETE_ART => Lines(
        BoxTop(B_GREEN),
        Header(" LIVELLO COMPLETATO ", " nuovi fili entrano nel quadro", B_GREEN, GOLD, GRAY),
        BoxMid(B_GREEN),
        Row(B_GREEN, (DIM, "")),
        Row(B_GREEN, (DIM, "     "), (B_GREEN, "███╗   ███╗"), (DIM, "  "), (GOLD, "MISSIONE AVANZATA"), (DIM, "           ")),
        Row(B_GREEN, (DIM, "     "), (B_GREEN, "╚██╗ ██╔╝ "), (DIM, "  "), (SAGE, "nuovi fili entrano nel quadro"), (DIM, "   ")),
        Row(B_GREEN, (DIM, "      "), (B_GREEN, "╚███╔╝  "), (DIM, "   "), (SAGE, "il tempo concede un altro passo"), (DIM, "  ")),
        Row(B_GREEN, (DIM, "      "), (B_GREEN, "╚══╝    "), (DIM, "                               ")),
        Row(B_GREEN, (DIM, "")),
        Row(B_GREEN, (DIM, "   "), (GOLD, "✦ ✦ ✦ ✦ ✦ ✦ ✦ ✦ ✦ ✦ ✦ ✦"), (DIM, "  "), (GRAY, "una nuova epoca si apre"), (DIM, "  ")),
        Row(B_GREEN, (DIM, "")),
        BoxBot(B_GREEN));

    private static string GAME_OVER_ART => Lines(
        BoxTop(B_RED),
        Header(" GAME OVER ", " il sospetto ha bruciato la tua copertura", B_RED, RED, GRAY),
        BoxMid(B_RED),
        Row(B_RED, (DIM, "")),
        Row(B_RED, (DIM, "     "), (RED, "██████╗  █████╗ ███╗   ███╗███████╗"), (DIM, "          ")),
        Row(B_RED, (DIM, "    "), (RED, "██╔════╝ ██╔══██╗████╗ ████║██╔════╝"), (DIM, "         ")),
        Row(B_RED, (DIM, "    "), (RED, "██║  ███╗███████║██╔████╔██║█████╗  "), (DIM, "         ")),
        Row(B_RED, (DIM, "    "), (RED, "██║   ██║██╔══██║██║╚██╔╝██║██╔══╝  "), (DIM, "         ")),
        Row(B_RED, (DIM, "    "), (RED, "╚██████╔╝██║  ██║██║ ╚═╝ ██║███████╗"), (DIM, "         ")),
        Row(B_RED, (DIM, "     "), (BLOOD, "╚═════╝ ╚═╝  ╚═╝╚═╝     ╚═╝╚══════╝"), (DIM, "         ")),
        Row(B_RED, (DIM, "")),
        Row(B_RED, (DIM, "     "), (GRAY, "Il sospetto ha bruciato la tua copertura."), (DIM, "       ")),
        Row(B_RED, (DIM, "")),
        BoxBot(B_RED));

    private static string VICTORY_ART => Lines(
        BoxTop(B_GREEN),
        Header(" VITTORIA ", " crononauta — il mistero è risolto", B_GREEN, GOLD, GRAY),
        BoxMid(B_GREEN),
        Row(B_GREEN, (DIM, "")),
        Row(B_GREEN, (DIM, "   "), (GOLD, "✦  ✦  ✦  ✦  ✦  ✦  ✦  ✦  ✦  ✦  ✦  ✦  ✦  ✦  ✦  ✦  ✦  ✦")),
        Row(B_GREEN, (DIM, "")),
        Row(B_GREEN, (DIM, "          "), (B_GREEN, "CRONONAUTA"), (DIM, "  "), (GOLD, "MISTERO RISOLTO"), (DIM, "               ")),
        Row(B_GREEN, (DIM, "")),
        Row(B_GREEN, (DIM, "   "), (GOLD, "✦  ✦  ✦  ✦  ✦  ✦  ✦  ✦  ✦  ✦  ✦  ✦  ✦  ✦  ✦  ✦  ✦  ✦")),
        Row(B_GREEN, (DIM, "")),
        Row(B_GREEN, (DIM, "   "), (SAGE, "La villa tace. Il tempo torna a respirare."), (DIM, "        ")),
        Row(B_GREEN, (DIM, "")),
        BoxBot(B_GREEN));

    // ==================== OGGETTI ====================

    private static string ITEM_KEY_ART => Lines(
        BoxTop(GOLD),
        Header(" CHIAVE ", " piccola, fredda, vecchia di secoli", GOLD, B_YELLOW, GRAY),
        BoxMid(GOLD),
        Row(GOLD, (DIM, "")),
        Row(GOLD, (DIM, "                     "), (GOLD, "  .──────."), (DIM, "                    ")),
        Row(GOLD, (DIM, "                    "), (GOLD, " /  "), (BRASS, ".──."), (GOLD, "  \\"), (DIM, "                   ")),
        Row(GOLD, (DIM, "                   "), (GOLD, "|  ("), (BRASS, "◉◉"), (GOLD, ")  |═══════════[__]"), (DIM, "   ")),
        Row(GOLD, (DIM, "                    "), (GOLD, " \\        /        "), (DIM, "              ")),
        Row(GOLD, (DIM, "                     "), (GOLD, "  '──────'"), (DIM, "          "), (GOLD, "__|__"), (DIM, "        ")),
        Row(GOLD, (DIM, "")),
        Row(GOLD, (DIM, "      "), (GRAY, "aprirà qualcosa che non dovrebbe essere aperto"), (DIM, "    ")),
        Row(GOLD, (DIM, "")),
        BoxBot(GOLD));

    private static string ITEM_DOC_ART => Lines(
        BoxTop(PAPER),
        Header(" DOCUMENTO ", " inchiostro sbiadito, data impossibile", PAPER, B_WHITE, GRAY),
        BoxMid(PAPER),
        Row(PAPER, (DIM, "")),
        Row(PAPER, (DIM, "              "), (PAPER, "  __________________"), (DIM, "                  ")),
        Row(PAPER, (DIM, "             "), (PAPER, " /"), (SHADOW, " ─────────────────── "), (PAPER, "/│"), (DIM, "               ")),
        Row(PAPER, (DIM, "            "), (PAPER, "/"), (SHADOW, " data: "), (BLOOD, "????"), (SHADOW, "           "), (PAPER, "/ │"), (DIM, "               ")),
        Row(PAPER, (DIM, "           "), (PAPER, "/"), (SHADOW, " firma tremante      "), (PAPER, "/  │"), (DIM, "               ")),
        Row(PAPER, (DIM, "          "), (PAPER, "/____________________/  │"), (DIM, "               ")),
        Row(PAPER, (DIM, "          "), (PAPER, "|____________________|  /"), (DIM, "               ")),
        Row(PAPER, (DIM, "")),
        Row(PAPER, (DIM, "      "), (GRAY, "la data non torna: impossibile essere qui"), (DIM, "       ")),
        Row(PAPER, (DIM, "")),
        BoxBot(PAPER));

    private static string ITEM_EVIDENCE_ART => Lines(
        BoxTop(ROSE),
        Header(" PROVA ", " reperti che il tempo non ha potuto nascondere", ROSE, B_RED, GRAY),
        BoxMid(ROSE),
        Row(ROSE, (DIM, "")),
        Row(ROSE, (DIM, "              "), (ROSE, "   .──────────────."), (DIM, "                  ")),
        Row(ROSE, (DIM, "             "), (ROSE, "  /   "), (BLOOD, "SIGILLO"), (ROSE, "   /│"), (DIM, "                  ")),
        Row(ROSE, (DIM, "            "), (ROSE, "  /________________/ │"), (DIM, "                  ")),
        Row(ROSE, (DIM, "            "), (ROSE, "  │  "), (BLOOD, "reperto #"), (ROSE, "   │ /"), (DIM, "                  ")),
        Row(ROSE, (DIM, "            "), (ROSE, "  '──────────────' /"), (DIM, "                   ")),
        Row(ROSE, (DIM, "")),
        Row(ROSE, (DIM, "      "), (GRAY, "ogni segno racconta ciò che non si doveva vedere"), (DIM, "   ")),
        Row(ROSE, (DIM, "")),
        BoxBot(ROSE));

    private static string ITEM_FUTURE_ART => Lines(
        BoxTop(ICE),
        Header(" OGGETTO FUTURO ", " eco temporale — non dovrebbe esistere qui", ICE, B_CYAN, GRAY),
        BoxMid(ICE),
        Row(ICE, (DIM, "")),
        Row(ICE, (DIM, "                  "), (ICE, "    .─────────."), (DIM, "                  ")),
        Row(ICE, (DIM, "                 "), (ICE, "___/ "), (B_CYAN, "20??"), (ICE, "    \\___"), (DIM, "              ")),
        Row(ICE, (DIM, "                "), (ICE, "/   \\"), (DIM, "  "), (B_CYAN, "████"), (DIM, "  "), (ICE, "/   \\"), (DIM, "              ")),
        Row(ICE, (DIM, "                "), (ICE, "\\___/──"), (B_CYAN, "░░░░"), (ICE, "──\\___/"), (DIM, "              ")),
        Row(ICE, (DIM, "                  "), (ICE, "    '─────────'"), (DIM, "                  ")),
        Row(ICE, (DIM, "")),
        Row(ICE, (DIM, "      "), (GRAY, "un oggetto che verrà inventato tra decenni"), (DIM, "        ")),
        Row(ICE, (DIM, "")),
        BoxBot(ICE));

    private static string ITEM_CONTAINER_ART => Lines(
        BoxTop(WOOD),
        Header(" CONTENITORE ", " legno e ferro, custodisce qualcosa di pesante", WOOD, AMBER, GRAY),
        BoxMid(WOOD),
        Row(WOOD, (DIM, "")),
        Row(WOOD, (DIM, "              "), (WOOD, "   .─────────────────."), (DIM, "                ")),
        Row(WOOD, (DIM, "             "), (WOOD, "  /___________________/│"), (DIM, "               ")),
        Row(WOOD, (DIM, "             "), (WOOD, "  │  "), (GOLD, "__"), (WOOD, "         "), (GOLD, "__"), (WOOD, "  │ │"), (DIM, "               ")),
        Row(WOOD, (DIM, "             "), (WOOD, "  │ "), (GOLD, "│  │"), (WOOD, "   "), (BRASS, "()"), (WOOD, "   "), (GOLD, "│  │"), (WOOD, " │ /"), (DIM, "               ")),
        Row(WOOD, (DIM, "             "), (WOOD, "  │___________________│/"), (DIM, "               ")),
        Row(WOOD, (DIM, "")),
        Row(WOOD, (DIM, "      "), (GRAY, "il coperchio è stato aperto di recente"), (DIM, "              ")),
        Row(WOOD, (DIM, "")),
        BoxBot(WOOD));

    private static string ITEM_GENERIC_ART => Lines(
        BoxTop(STONE),
        Header(" OGGETTO ", " non basta guardarlo — va toccato con la mente", STONE, GRAY, SHADOW),
        BoxMid(STONE),
        Row(STONE, (DIM, "")),
        Row(STONE, (DIM, "                  "), (STONE, "  __________"), (DIM, "                    ")),
        Row(STONE, (DIM, "                 "), (STONE, " /   "), (LILAC, "????"), (STONE, "   \\"), (DIM, "                   ")),
        Row(STONE, (DIM, "                 "), (STONE, "|  "), (GRAY, "non basta"), (STONE, "  |"), (DIM, "                   ")),
        Row(STONE, (DIM, "                 "), (STONE, "|  "), (GRAY, "guardarlo"), (STONE, "  |"), (DIM, "                   ")),
        Row(STONE, (DIM, "                  "), (STONE, " \\__________/"), (DIM, "                   ")),
        Row(STONE, (DIM, "")),
        Row(STONE, (DIM, "      "), (GRAY, "alcuni oggetti parlano solo a chi sa ascoltare"), (DIM, "   ")),
        Row(STONE, (DIM, "")),
        BoxBot(STONE));

    // ==================== ESTERNO E SPLASH ====================

    private static string VILLA_EXTERIOR_ART
    {
        get
        {
            // Cielo notturno RGB 24-bit
            string sk  = RGB(8,10,25);
            string s1  = BOLD+RGB(255,255,248);
            string s2  = RGB(200,215,255);
            string s3  = RGB(140,160,220);
            // Luna: 3 char × 2 righe con Px() (half-block pixel art)
            string mR1 = Px(10,14,42,130,155,210)+Px(130,155,210,230,238,255)+Px(10,14,42,130,155,210);
            string mR2 = Px(230,238,255,10,14,42)+Px(255,255,255,215,228,255)+Px(230,238,255,10,14,42);
            // Pietra gradient façade
            string stH = RGB(100,97,110); // high / roof
            string stM = RGB(120,117,130); // mid / walls
            string stL = RGB(82,79,90);  // low
            string stD = RGB(55,52,62);  // shadow base
            // Legno
            string wd1 = RGB(130,85,35);
            // Finestre illuminate (BG caldo + FG ambra)
            string wBG = BGRGB(85,32,4);
            string wFG = RGB(255,168,42);
            string win4 = $"{wBG}{wFG} ▓▓ {RESET}";
            string win2 = $"{wBG}{wFG}▓▓{RESET}";
            // Portale
            string dBG = BGRGB(18,10,4);
            string dFG = RGB(195,142,62);
            string dTop = $"{dBG}{dFG}╔══════════╗{RESET}";
            string dMid = $"{dBG}{dFG}║ INGRESSO ║{RESET}";
            // Mist / ground
            string mi1 = RGB(65,72,90);
            string mi2 = RGB(90,100,118);
            string shM = RGB(38,33,48);
            string shD = RGB(22,18,30);
            string gnd = RGB(30,26,38);
            // Insegna VILLA REALE
            string ins = $"{BOLD}{RGB(215,172,42)}        VILLA REALE        {RESET}";

            return Lines(
                RC(8,10,25,"     ")+C(s1,"✦")+RC(8,10,25,"       ")+C(s1,"✦")+RC(8,10,25,"    ")+mR1+RC(8,10,25,"   autunno 1901   ")+C(s1,"✦")+RC(8,10,25,"     ")+C(s2,"✧")+RC(8,10,25,"      ")+C(s1,"✦"),
                RC(8,10,25,"  ")+C(s2,"✧")+RC(8,10,25,"              ")+mR2+RC(8,10,25,"  luna piena sul vialetto   ")+C(s1,"✦")+RC(8,10,25,"             ")+C(s2,"✧"),
                C(mi1,"   ░")+C(mi2,"░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░")+C(mi1,"░   "),
                C(stH,"               ________________________________________________"),
                C(stH,"              /_______________________________________________/│"),
                C(stM,"             /_______________________________________________/ │"),
                C(stM,"            │     │ ")+win4+C(stM," │                  │ ")+win4+C(stM," │     │     │ │"),
                C(stM,"            │ │  │ │")+ins+C(stM,"│ │  │ │     │ │"),
                C(stM,"            │ │__│ │")+C(stL,"__________________________")+C(stM,"│_│__│ │     │ │"),
                C(stM,"           /│        \\")+C(stL,"     ┌──┐    ┌──┐")+C(stM,"    /        \\    │ │"),
                C(stM,"          / │         \\")+C(stL,"   │")+win2+C(stL,"│  │")+win2+C(stL,"│")+C(stM,"  /          \\   │ /"),
                C(stM,"         /__|   ")+C(wd1,"[ ]")+C(stM,"   \\")+C(stL,"   └──┘    └──┘")+C(stM," /    ")+C(wd1,"[ ]")+C(stM,"     \\__│/"),
                C(wd1,"        │   │         │  ")+dTop+C(wd1,"  │           │"),
                C(wd1,"        │___│_________│__")+dMid+C(wd1,"__│___________│"),
                C(shM,"            /_______________________________________________\\"),
                C(shD,"           /_________________________________________________\\"),
                C(gnd,"      ")+C(shD,"░░░")+C(gnd,"░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░")+C(shD,"░░░")
            );
        }
    }

    private static string SPLASH_ART
    {
        get
        {
            // Border
            string bdr   = RGB(0,155,210);
            string bdrDk = RGB(0,110,160);
            // Sky / stars RGB 24-bit
            string sky = RGB(8,12,30);
            string st1 = BOLD+RGB(255,255,248);
            string st2 = RGB(200,215,255);
            string st3 = RGB(135,158,215);
            // Gradient OLTRE (brillante→bronzo, per riga)
            string o1 = BOLD+RGB(255,240,100);
            string o2 = BOLD+RGB(255,220,65);
            string o3 = BOLD+RGB(248,195,40);
            string o4 = RGB(232,170,18);
            string o5 = RGB(210,140,5);
            string o6 = RGB(175,110,0);
            // Gradient TEMPO (oro medio→bronzo scuro)
            string t1 = RGB(218,158,10);
            string t2 = RGB(208,148,5);
            string t3 = RGB(192,132,0);
            string t4 = RGB(176,116,0);
            string t5 = RGB(154,95,0);
            string t6 = RGB(134,78,0);
            // Accenti
            string dv  = RGB(80,62,20);
            string dvH = BOLD+RGB(255,200,0);
            string sub = RGB(190,210,240);
            string loc = RGB(210,165,40);
            string flr = RGB(180,35,35);

            // Misura la larghezza visibile strippando i codici ANSI
            int VL(string s) =>
                System.Text.RegularExpressions.Regex.Replace(s, @"\x1B\[[0-9;]*[mGKHF]", "").Length;

            // Riga con bordo: il padding è calcolato automaticamente
            string BR(string content)
            {
                int pad = Math.Max(0, BoxWidth - VL(content));
                return $"{bdr}  ║{RESET}{content}{new string(' ', pad)}{bdr}║{RESET}";
            }

            string TL = C(bdr, $"  ╔{new string('═',BoxWidth)}╗");
            string BL = C(bdr, $"  ╚{new string('═',BoxWidth)}╝");
            string ML = C(bdrDk, $"  ╠{new string('─',BoxWidth)}╣");
            string ER = $"  {bdr}║{RESET}{new string(' ',BoxWidth)}{bdr}║{RESET}";

            return Lines(
                TL,
                BR(C(sky,"  ")+C(st1,"✦")+C(sky,"    ")+C(st2,"✧")+C(sky,"     ")+C(st3,"·")+C(sky,"     ")+C(st1,"✦")+C(sky,"     ")+C(st3,"·")+C(sky,"     ")+C(st2,"✧")+C(sky,"     ")+C(st1,"✦")+C(sky,"    ")+C(st3,"·")+C(sky,"  ")),
                BR(C(sky,"     ")+C(st3,"·")+C(sky,"   ")+C(st2,"✧")+C(sky,"   ")+C(st1,"✦")+C(sky,"    ")+C(st3,"·")+C(sky,"       ")+C(st2,"✧")+C(sky,"   ")+C(st1,"✦")+C(sky,"    ")+C(st3,"·")+C(sky,"   ")+C(st2,"✧")+C(sky,"  ")),
                ER,
                BR(C(o1,"     ██████╗ ██╗  ████████╗██████╗ ███████╗")),
                BR(C(o2,"    ██╔═══██╗██║  ╚══██╔══╝██╔══██╗██╔════╝")),
                BR(C(o3,"    ██║   ██║██║     ██║   ██████╔╝█████╗  ")),
                BR(C(o4,"    ██║   ██║██║     ██║   ██╔══██╗██╔══╝  ")),
                BR(C(o5,"    ╚██████╔╝███████╗██║   ██║  ██║███████╗")),
                BR(C(o6,"     ╚═════╝ ╚══════╝╚═╝   ╚═╝  ╚═╝╚══════╝")),
                BR(C(dv,"    ─── ")+C(dvH,"✦")+C(dv," ──────────────────── ")+C(dvH,"⧗")+C(dv," ──────────────────── ")+C(dvH,"✦")+C(dv," ─── ")),
                BR(C(t1,"      ████████╗███████╗███╗   ███╗██████╗  ██████╗")),
                BR(C(t2,"      ╚══██╔══╝██╔════╝████╗ ████║██╔══██╗██╔═══██╗")),
                BR(C(t3,"         ██║   █████╗  ██╔████╔██║██████╔╝██║   ██║")),
                BR(C(t4,"         ██║   ██╔══╝  ██║╚██╔╝██║██╔═══╝ ██║   ██║")),
                BR(C(t5,"         ██║   ███████╗██║ ╚═╝ ██║██║     ╚██████╔╝")),
                BR(C(t6,"         ╚═╝   ╚══════╝╚═╝     ╚═╝╚═╝      ╚═════╝ ")),
                ML,
                ER,
                BR(C(sub,"       Un'avventura investigativa attraverso epoche spezzate")),
                BR("               "+C(loc,"Villa Reale")+"  "+C(flr,"❦")+"  "+C(loc,"Autunno 1901")),
                ER,
                BR(C(sky,"  ")+C(st2,"✧")+C(sky,"   ")+C(st1,"✦")+C(sky,"    ")+C(st3,"·")+C(sky,"     ")+C(st2,"✧")+C(sky,"     ")+C(st3,"·")+C(sky,"     ")+C(st1,"✦")+C(sky,"     ")+C(st2,"✧")+C(sky,"    ")+C(st3,"·")+C(sky,"  ")),
                BL
            );
        }
    }

    // ==================== HELPER ====================

    private static string C(string color, string text) => $"{color}{text}{RESET}";

    private static string CB(string color, string text) => $"{BOLD}{color}{text}{RESET}";

    /// <summary>Testo bold + colore acceso (effetto bagliore).</summary>
    private static string Glow(string color, string text) => $"{BOLD}{color}{text}{RESET}";

    // ── TRUE-COLOR RGB (24-bit) ──────────────────────────────────────
    /// <summary>Foreground RGB 24-bit.</summary>
    private static string RGB(int r, int g, int b) => $"\u001b[38;2;{r};{g};{b}m";
    /// <summary>Background RGB 24-bit.</summary>
    private static string BGRGB(int r, int g, int b) => $"\u001b[48;2;{r};{g};{b}m";
    /// <summary>Testo con foreground RGB.</summary>
    private static string RC(int r, int g, int b, string text) => $"{RGB(r,g,b)}{text}{RESET}";
    /// <summary>Testo bold con foreground RGB.</summary>
    private static string RCB(int r, int g, int b, string text) => $"{BOLD}{RGB(r,g,b)}{text}{RESET}";
    /// <summary>Half-block ▀: top pixel in foreground, bottom pixel in background (raddoppia risoluzione verticale).</summary>
    private static string Px(int fR, int fG, int fB, int bR, int bG, int bB)
        => $"{BGRGB(bR,bG,bB)}{RGB(fR,fG,fB)}▀{RESET}";
    /// <summary>Riga di pixel identici (stesso colore sopra e sotto = blocco pieno).</summary>
    private static string PxFlat(int r, int g, int b, int count = 1)
        => $"{RGB(r,g,b)}{new string('█', count)}{RESET}";
    /// <summary>Sfumatura orizzontale RGB su testo arbitrario.</summary>
    private static string RGBGrad(string text, (int r,int g,int b)[] stops)
    {
        if (string.IsNullOrEmpty(text) || stops.Length == 0) return text;
        var sb = new System.Text.StringBuilder();
        int n = text.Length;
        for (int i = 0; i < n; i++)
        {
            double t = stops.Length == 1 ? 0 : (double)i / (n - 1);
            int seg = (int)(t * (stops.Length - 1));
            if (seg >= stops.Length - 1) { sb.Append(RGB(stops[^1].r, stops[^1].g, stops[^1].b)); }
            else
            {
                double local = t * (stops.Length - 1) - seg;
                int r = (int)(stops[seg].r + (stops[seg+1].r - stops[seg].r) * local);
                int g = (int)(stops[seg].g + (stops[seg+1].g - stops[seg].g) * local);
                int b2 = (int)(stops[seg].b + (stops[seg+1].b - stops[seg].b) * local);
                sb.Append(RGB(r, g, b2));
            }
            sb.Append(text[i]);
        }
        sb.Append(RESET);
        return sb.ToString();
    }

    /// <summary>Applica un gradient orizzontale al testo carattere-per-carattere.</summary>
    private static string Gradient(string text, params string[] colors)
    {
        if (string.IsNullOrEmpty(text) || colors.Length == 0)
            return C(STONE_HIGH, text);

        var sb = new System.Text.StringBuilder();
        int n = text.Length;
        for (int i = 0; i < n; i++)
        {
            int idx = colors.Length == 1 ? 0 : (int)((double)i / Math.Max(1, n - 1) * (colors.Length - 1));
            if (idx >= colors.Length) idx = colors.Length - 1;
            sb.Append(colors[idx]).Append(text[i]);
        }
        sb.Append(RESET);
        return sb.ToString();
    }

    /// <summary>Applica un gradient bold (per titoli grandi).</summary>
    private static string GradientBold(string text, params string[] colors)
        => BOLD + Gradient(text, colors).Substring(0);

    private static string Lines(params string[] lines) => string.Join(Environment.NewLine, lines);

    private static string Lines(IEnumerable<string> lines) => string.Join(Environment.NewLine, lines);

    private static string BoxTop(string color = CYAN) => C(color, "  ╔" + new string('═', BoxWidth) + "╗");

    private static string BoxMid(string color = CYAN) => C(color, "  ╠" + new string('═', BoxWidth) + "╣");

    private static string BoxBot(string color = CYAN) => C(color, "  ╚" + new string('═', BoxWidth) + "╝");

    private static string BoxThin(string color = CYAN) => C(color, "  ╠" + new string('─', BoxWidth) + "╣");

    private static string Header(string title, string subtitle, string borderColor = CYAN, string titleColor = B_YELLOW, string subtitleColor = GRAY)
    {
        int rawLength = title.Length + subtitle.Length;
        int pad = Math.Max(0, BoxWidth - rawLength);
        return C(borderColor, "  ║") + CB(titleColor, title) + C(subtitleColor, subtitle) + new string(' ', pad) + C(borderColor, "║");
    }

    private static string Row(params (string color, string text)[] segments)
        => Row(CYAN, segments);

    private static string Row(string borderColor, params (string color, string text)[] segments)
    {
        int rawLength = segments.Sum(s => s.text.Length);
        int pad = Math.Max(0, BoxWidth - rawLength);
        var content = string.Concat(segments.Select(s => C(s.color, s.text)));
        return C(borderColor, "  ║") + content + new string(' ', pad) + C(borderColor, "║");
    }

    private static string SoloRow(string color, string content)
    {
        int pad = Math.Max(0, BoxWidth - content.Length);
        return C(CYAN, "  ║") + C(color, content) + new string(' ', pad) + C(CYAN, "║");
    }

    private static string EmptyRow() => C(CYAN, "  ║") + new string(' ', BoxWidth) + C(CYAN, "║");

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
