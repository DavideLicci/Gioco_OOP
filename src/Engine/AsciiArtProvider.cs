namespace OltreIlTempo.Engine;

/// <summary>
/// Fornisce ASCII art per stanze, titoli e NPC.
/// DADDA: grafica testuale stile avventure classiche.
/// </summary>
public static class AsciiArtProvider
{
    // ==================== COLORI ANSI ====================
    public const string RESET = "\u001b[0m";
    public const string BOLD = "\u001b[1m";
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
        string upper = roomName.ToUpper();
        string border = new string('=', upper.Length + 6);
        return $"  +{border}+\n  |   {upper}   |\n  +{border}+";
    }

    public static string GetNpcArt(string npcId) => npcId switch
    {
        "npc_cook" => NPC_COOK_ART,
        "npc_librarian" => NPC_LIBRARIAN_ART,
        _ => ""
    };

    public static string GetVillaMap() => VILLA_MAP;
    public static string GetSplashScreen() => SPLASH_ART;
    public static string GetVillaExterior() => VILLA_EXTERIOR_ART;
    public static string GetTrophy() => TROPHY_ART;
    public static string GetInventoryHeader() => INVENTORY_ART;
    public static string GetDeductionArt() => DEDUCTION_ART;
    public static string GetLevelComplete() => LEVEL_COMPLETE_ART;
    public static string GetGameOver() => GAME_OVER_ART;
    public static string GetVictory() => VICTORY_ART;

    public static string GetItemArt(string type) => type.ToLower() switch
    {
        "key" => ITEM_KEY_ART,
        "document" => ITEM_DOC_ART,
        _ => ITEM_GENERIC_ART
    };

    public static string GetSeparator() =>
        "  ====================================================";

    public static string GetThinSeparator() =>
        "  ----------------------------------------------------";

    public static string GetCompass(Dictionary<string, string> exits)
    {
        bool n = exits.ContainsKey("n");
        bool s = exits.ContainsKey("s");
        bool e = exits.ContainsKey("e");
        bool o = exits.ContainsKey("o") || exits.ContainsKey("w");

        string nStr = n ? " N " : " . ";
        string sStr = s ? " S " : " . ";
        string eStr = e ? "E" : ".";
        string oStr = o ? "O" : ".";

        return $"        +---+\n" +
               $"        |{nStr}|\n" +
               $"      {oStr} | * | {eStr}\n" +
               $"        |{sStr}|\n" +
               $"        +---+";
    }

    // ==================== STANZE ====================
    private static string KITCHEN_ART =>
        $"{GRAY}aWB$$@@@@$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$@{RESET}\n" +
        $"{GRAY}bbbb#8@$@@@$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$@{RESET}\n" +
        $"{GRAY}bbbbbbbbW%@@@@$@@@$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$@{RESET}\n" +
        $"{WHITE}bbbbbbbbbbba&B@$@$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$@{RESET}\n" +
        $"{WHITE}bbkkkkbbbbkbdbd#%@$$$$@@@@$$$$$$$$$$$$$$$$$$$$$$$$$@@$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$@{RESET}\n" +
        $"{WHITE}kbu/)fu0W#fkkbkbbbbkbbk%@@@@$$$$$$$$$$$$$$@@@@WOQ[]1\\\\\\\\\\\\uvvvvvvvvv/\\jp$$$$$$$$$$$$$$$$$$$$$$$$$$$@{RESET}\n" +
        $"{BOLD}{B_CYAN}          [ MENSOLA ]                                                                              {RESET}\n" +
        $"{WHITE}YJz/1tu0W*pbbbbbbbbbbbbbbbM%@@@@@@@@@@@@@@&kqU{{l!l<+++++++++++++-1))))))_++++>q@@@@@@@@@@@@@@@@@@@@@{RESET}\n" +
        $"{WHITE}]rxnnLvYWZdL\\wBbkkbddbbbbbbkuJJUJUUUUUUUUUUutftf!l!I<+++++++++++]))))))?+++<inUJUJJJJUUUUUUUUUUUUUUUU{RESET}\n" +
        $"{WHITE}[}}~+}}/xxcYLL/YWQ_-ttdbbbbbbkuCUUUUUUUUUUUJJUxftt_llI>+++++++~+_)1))([+~~<i/UUUUUUUUUUUJJJUJUUJUUUU{RESET}\n" +
        $"{BOLD}{B_WHITE}    CHEF MARCO {RESET} {GRAY}               TAVOLO DI LEGNO MASSICCIO {RESET}\n" +
        $"{WHITE}xY-u/{{?_+((unULLZmkkkbbbbbbkuCUUUUUUUUUUUUJJUUztttt+lll<+++++++{{)))1[++<!1uJUUUUUUUUUUUC|nLQQwkkkkoo{RESET}\n" +
        $"{WHITE}xU-unuxnr1{{_-+(rnvCCJwdbbbbkuJUUUUUUUUUUUUJJUUUJrtff+lll!~~~~~?{{}}{{}}([~l!!!\\uJJJUJJUUUUJUUC/ncQQmkkh{RESET}\n" +
        $"{WHITE}xU-unnnnnnnnrt{{]++[}}uczLQQbkuJUUUUUUUUUUUUUUUUUUUYftfllll!_+++[))?<~!l![fnJJUUUUUUJuJUUUJYJL00ZZZZZ0{RESET}\n" +
        $"{WHITE}xU-unnnnnnnnnnnJ(n\\)-->1\\xYUvXUUUUUUUUUUUUUUUUUUUUtttlllll_++~]))?<~lll]tuUUUUUUUvtxxnnxnnnnnxnnnnnn{RESET}\n" +
        $"{WHITE}xU-unnnnnnnnnnxJ(nnnnnr))-__txuJUUUUUUUUUUUUUUUUUUfttlllll_+++]))?<<lll]tuUUUUYjfff/f))1111))1)))))))){RESET}\n" +
        $"{BOLD}{B_YELLOW}       / \\             __________________________________________ {RESET}\n" +
        $"{BOLD}{B_YELLOW}      | o o |         |                                          |{RESET}\n" +
        $"{BOLD}{B_YELLOW}       \\ _ /          |    [ COLTELLO ]          [ NOTA ]        |{RESET}\n" +
        $"{BOLD}{B_YELLOW}        | |           |       /|                  ____           |{RESET}\n" +
        $"{BOLD}{B_YELLOW}       /| |\\          |  ____/_|____             |    |          |{RESET}\n" +
        $"{BOLD}{B_YELLOW}      / | | \\         | |___________|            |____|          |{RESET}\n" +
        $"{BOLD}{B_YELLOW}     /  |_|  \\        |__________________________________________|{RESET}\n" +
        $"{WHITE}xU-unnnnnnnnnnxJ(nnnnnnnnnnnuYXUUUUUUUUUUUUUUUUUUUftt}}llll_++++{{)?~<lliftuzffttttttfnx/+/%O~_u8O~+~~Y{RESET}\n" +
        $"{WHITE}xU-unnnnnnnnnnxJ(nnnnnnnnnnnuYXUUUUUUUUUUUUUUUUUUUjtt}}llIi++++~1)-<<l!ittftttttttttfnx/>wJu+1B0W?<z/+{RESET}\n" +
        $"{CYAN} [ FORNO ] {RESET}                                                                                 \n" +
        $"{CYAN}  _______  {RESET}                                                                                 \n" +
        $"{CYAN} |  ___  | {RESET}                                                                                 \n" +
        $"{CYAN} | |   | | {RESET}                                                                                 \n" +
        $"{CYAN} | |___| | {RESET}                                                                                 \n" +
        $"{CYAN} |_______| {RESET}                                                                                 \n";

    private const string HALLWAY_ART = @"
    +============================================================+
    |                     .  .  .  .  .  .                        |
    |  +---------+  +---------+  +---------+  +---------+        |
    |  | ,-----. |  |  .---.  |  | /|   |\ |  |  .---.  |        |
    |  | | O O | |  | / ^ ^ \ |  |/ | _ | \|  | |   |  |        |
    |  | | \_/ | |  ||  \_/  ||  |  |/ \|  |  | | ~ |  |        |
    |  | '-----' |  | \_____/ |  |  |   |  |  | '---'  |        |
    |  +---------+  +---------+  +---------+  +---------+        |
    |   Ritratto     Ritratto     Ritratto     Ritratto          |
    |                                                             |
    |  =======================================================   |
    |     |              |              |              |          |
    |    _|_            _|_            _|_            _|_         |
    |   (   )          (   )          (   )          (   )        |
    |  Lampada        Lampada        Lampada        Lampada      |
    +============================================================+";

    private const string PANTRY_ART = @"
    +============================================================+
    |                       DISPENSA                              |
    |  +------------------------------------------------------+  |
    |  |  [Farina]  [Sale]  {Olio}  [Zucch.]  (Vino)          |  |
    |  +------------------------------------------------------+  |
    |  |  (Aceto)   {Miele} [Riso]  (Latte)   [Pasta]         |  |
    |  +------------------------------------------------------+  |
    |  |  [Caffe]   (Te)    (Acqua) {Spezie}  (Succo)          |  |
    |  +------------------------------------------------------+  |
    |  |  {Pane}    [Cons.] [Uova]  (Burro)   {Liev.}         |  |
    |  +------------------------------------------------------+  |
    |                                                             |
    |           .---.                                             |
    |           | K |  <-- Chiave trovata per terra               |
    |           '---'                                             |
    +============================================================+";

    private const string LIBRARY_ART = @"
    +============================================================+
    |  ||||||| ||||||| ||||||| |||||||      ___________          |
    |  ||||||| ||||||| ||||||| |||||||     /           \         |
    |  ||||||| ||||||| ||||||| |||||||    /     ___     \        |
    |  |LIBRI| |LIBRI| |LIBRI| |LIBRI|   |    /   \    |        |
    |  |_____| |_____| |_____| |_____|   |   | O O |   |        |
    |                                     |    \___/    |        |
    |  ||||||| ||||||| ||||||| |||||||     \___________/         |
    |  ||||||| ||||||| ||||||| |||||||       Mappamondo          |
    |  |_____| |_____| |_____| |_____|                           |
    |       _________________________________                    |
    |      |  Diario    Penna    Calamaio    |                   |
    |      |  aperto    d'oca    .---.       |                   |
    |      |    []       /       |ink|       |                   |
    |      |_________________________________|                   |
    +============================================================+";

    private const string BEDROOM_ART = @"
    +============================================================+
    |     ._____.                                                 |
    |     | /.\ |  Lampada            +------------------+        |
    |     |/   \|                     |   +============+ |        |
    |     '-----'              _____  |   | Specchio   | |        |
    |  +------------------+   |     | |   +============+ |        |
    |  |  ____/|  |\____  |   | ARM | |                  |        |
    |  | | ~~~~~~~~~~~~ | |   | ADI | |   +--+    +--+   |        |
    |  | | ~~ LETTO ~~~ | |   |  O  | |   |  |    |  |   |        |
    |  | | ~~~~~~~~~~~~ | |   |     | |   |  |    |  |   |        |
    |  | |______________| |   |_____| |   +--+    +--+   |        |
    |  +------------------+           +------------------+        |
    |        Baldacchino                 Armadio mogano            |
    |  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~   |
    +============================================================+";

    private const string UNKNOWN_ROOM_ART = @"
    +============================================================+
    |                                                             |
    |          . . .   S T A N Z A   . . .                        |
    |                                                             |
    |               ___________________                           |
    |              |                   |                          |
    |              |         ?         |                          |
    |              |      _______      |                          |
    |              |     |       |     |                          |
    |              |_____|_______|_____|                          |
    |                                                             |
    |          . . .  S C O N O S C I U T A  . . .                |
    |                                                             |
    +============================================================+";

    // ==================== NPC ====================

    private const string NPC_COOK_ART = @"
    +======================================+
    |           _________                  |
    |          |  CHEF   |                 |
    |          |_________|                 |
    |           /       \                  |
    |          |  (o) (o) |                |
    |          |    __    |                |
    |          |   /  \   |                |
    |           \_| -- |_/                 |
    |            _|    |_                  |
    |           / |    | \    Chef Marco   |
    |          /  |~~~~|  \   cuoco della  |
    |         /   |    |   \  villa        |
    |        /    |    |    \              |
    |       '_____|    |_____'             |
    +======================================+";

    private const string NPC_LIBRARIAN_ART = @"
    +======================================+
    |             _______                  |
    |            / ~~~~~ \                 |
    |           | ~~~~~~~ |                |
    |           |  -====-  |               |
    |           | (o)  (o) |               |
    |           |    <>    |               |
    |           |   ____   |               |
    |            \_|    |_/                |
    |             _|    |_   Sig.ra Lucia  |
    |            / |    | \  bibliotecaria |
    |           |  | [] |  | e custode     |
    |           |  |    |  | dei segreti   |
    |           |__|    |__|               |
    +======================================+";

    // ==================== SISTEMA ====================

    private const string TROPHY_ART = @"
    +======================================+
    |          _______________             |
    |         |               |            |
    |    _____|    TROFEI     |_____       |
    |   |     |   SBLOCCATI   |     |      |
    |   |     |_______________|     |      |
    |   |      \             /      |      |
    |    \      \           /      /       |
    |     \      \_________/      /        |
    |      \         | |         /         |
    |       '--------|_|--------'          |
    |            |_________|               |
    |            |_________|               |
    +======================================+";

    private const string INVENTORY_ART = @"
    +======================================+
    |        ____________________          |
    |       /                    \         |
    |      / ==================== \        |
    |     |  |                  |  |       |
    |     |  |    I N V E N     |  |       |
    |     |  |    T A R I O     |  |       |
    |     |  |                  |  |       |
    |     |  |==================|  |       |
    |     |  |                  |  |       |
    |      \ \__________________/ /        |
    |       \____________________/         |
    +======================================+";

    private const string DEDUCTION_ART = @"
    +======================================+
    |              ,---.                   |
    |             / /|\ \                  |
    |            / / | \ \                 |
    |           /  \ | /  \                |
    |          |    \|/    |               |
    |           \   /|\   /                |
    |            \_/ | \_/   DEDUZIONE     |
    |                |       Collega gli   |
    |            ____|____   indizi...     |
    |           |_________|                |
    +======================================+";

    private const string LEVEL_COMPLETE_ART = @"
    +==================================================+
    |                                                  |
    |     *  *  *   L I V E L L O   *  *  *           |
    |                                                  |
    |       ####  ####  #    # ####                    |
    |       #     #  #  ##  ## #  #                    |
    |       #     #  #  # ## # ####                    |
    |       ####  ####  #    # #                       |
    |                                                  |
    |     *  *  *  C O M P L E T A T O  *  *  *       |
    |                                                  |
    +==================================================+";

    private const string GAME_OVER_ART = @"
    +==================================================+
    |                                                  |
    |      ____   _   __  __ ___                       |
    |     / ___| / \ |  \/  | __|                      |
    |    | |  _ / _ \| |\/| | _|                       |
    |    | |_| / ___ | |  | | |___                     |
    |     \___/_/   \_|_|  |_|_____|                   |
    |                                                  |
    |       ___  _  _ ___ ___                          |
    |      / _ \| || | __| _ \                         |
    |     | (_) | || | _||   /                         |
    |      \___/ \__/|___|_|_\                         |
    |                                                  |
    |     Il sospetto ha raggiunto il massimo.         |
    |     La tua copertura e' saltata, agente.         |
    |                                                  |
    +==================================================+";

    private const string VICTORY_ART = @"
    +==================================================+
    |              *    *    *    *    *                |
    |         *                            *           |
    |     *    #   # # ##### #####  ####    *          |
    |          #   # #   #     #   #   #               |
    |     *     # #  #   #     #   #   #    *          |
    |            #   #   #     #    ####               |
    |     *                                 *          |
    |         *                            *           |
    |              *    *    *    *    *                |
    |                                                  |
    |     Hai risolto il mistero, CRONONAUTA!          |
    |                                                  |
    +==================================================+";

    // ==================== OGGETTI ====================

    private const string ITEM_KEY_ART = @"
    +============================+
    |        .-------.           |
    |       /  .----.  \         |
    |      |  /      \  |        |
    |      |  | (  ) |  |        |
    |      |  \      /  |        |
    |       \  '----'  /         |
    |        '---||---'          |
    |            ||              |
    |            ||              |
    |         ___||___           |
    |        |__[==]__|          |
    +============================+";

    private const string ITEM_DOC_ART = @"
    +============================+
    |     ___________________    |
    |    /                   /|  |
    |   /  ~~~~~~~~~~~~~~~~ / |  |
    |  |  ~~~~~~~~~~~~~~~~ |  |  |
    |  |  ~~~~  ~~~~~~  ~~ |  |  |
    |  |  ~~~~~~~~~~~~~~~~ |  |  |
    |  |  ~~~ ~~~~~~~~~ ~~ |  |  |
    |  |  ~~~~~~~~~~~~~~~~ | /   |
    |  |___________________|/    |
    +============================+";

    private const string ITEM_GENERIC_ART = @"
    +============================+
    |       ______________       |
    |      /              \      |
    |     |    OGGETTO     |     |
    |     |    ________    |     |
    |     |   |  ????  |   |     |
    |     |   |________|   |     |
    |      \______________/      |
    +============================+";

    // ==================== MAPPA ====================

    private const string VILLA_MAP = @"
    +===============================================+
    |              MAPPA DI VILLA REALE             |
    +===============================================+
    |                                               |
    |   +-----------+  +-----------+  +----------+  |
    |   |           |  |           |  |          |  |
    |   |  CAMERA   |--|  CORRIDOIO|--|BIBLIOTECA|  |
    |   | DA LETTO  |  |           |  |          |  |
    |   |           |  |           |  |          |  |
    |   +-----------+  +-----+-----+  +----------+  |
    |                        |                      |
    |                  +-----+-----+  +----------+  |
    |                  |           |  |          |  |
    |                  |  CUCINA   |--|DISPENSA  |  |
    |                  |           |  |          |  |
    |                  +-----------+  +----------+  |
    |                                               |
    +===============================================+";

    // ==================== ESTERNO E SPLASH ====================

    private const string VILLA_EXTERIOR_ART = @"
                          .         *          .        *
           *        .          .         .          .
                 _______________________________________________
                |      |                          |      |
                | |  | |       VILLA REALE        | |  | |
                | |__| |__________________________|_|__| |
               /        \     ____    ____     /        \
              /          \   |    |  |    |   /          \
             /    ____    \  | [] |  | [] |  /    ____    \
            |    |    |    | |    |  |    | |    |    |    |
            |    | [] |    | |____|  |____| |    | [] |    |
            |    |____|    |   ____[]____   |    |____|    |
            |              |  |          |  |              |
            |______________|__|__________|__|______________|
           /                                                \
          /==================================================/";

    private const string SPLASH_ART = @"

        +===================================================+
        |                                                   |
        |      ####  #    ##### ####  #####                 |
        |     #    # #      #   #   # #                     |
        |     #    # #      #   ####  ####                  |
        |     #    # #      #   #  #  #                     |
        |      ####  #####  #   #   # #####                 |
        |                                                   |
        |          ## #      ##### ##### #   # ####   ####  |
        |          ## #        #   #     ## ## #   # #    # |
        |          ## #        #   ####  # # # ####  #    # |
        |          ## #        #   #     #   # #     #    # |
        |          ## #####    #   ##### #   # #      ####  |
        |                                                   |
        |          Un mistero attraverso il tempo           |
        +===================================================+";
}

/// <summary>
/// Estensione per ripetere un carattere N volte.
/// </summary>
public static class CharExtensions
{
    public static string Repeat(this char c, int count)
    {
        return new string(c, count);
    }
}
