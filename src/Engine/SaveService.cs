namespace OltreIlTempo.Engine;

using OltreIlTempo.Domain;
using System.Text.Json;

/// <summary>
/// Implementazione del servizio di salvataggio e caricamento.
/// NICK: gestisce la persistenza dei dati di gioco.
/// </summary>
public class SaveService : ISaveService
{
    private const string SaveFormatVersion = "1.0";
    
    public void Save(GameState state, string filePath)
    {
        // TODO: Implementare il salvataggio
        // 1. Crea un wrapper con versione
        // 2. Serializza a JSON
        // 3. Salva su file temporaneo
        // 4. Rinomina atomicamente
        // 5. Crea backup del vecchio file
        
        var saveWrapper = new SaveData
        {
            Version = SaveFormatVersion,
            SavedAt = DateTime.UtcNow,
            State = state
        };
        
        var json = JsonSerializer.Serialize(saveWrapper, new JsonSerializerOptions { WriteIndented = true });
        
        // Salva il file
        File.WriteAllText(filePath, json);
    }
    
    public GameState Load(string filePath)
    {
        // TODO: Implementare il caricamento
        // 1. Leggi il file JSON
        // 2. Deserializza
        // 3. Controlla la versione
        // 4. Se versione non corrisponde, avvisa l'utente
        
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File di salvataggio non trovato: {filePath}");
        }
        
        var json = File.ReadAllText(filePath);
        var saveWrapper = JsonSerializer.Deserialize<SaveData>(json);
        
        if (saveWrapper?.Version != SaveFormatVersion)
        {
            throw new InvalidOperationException("Versione di salvataggio incompatibile.");
        }
        
        return saveWrapper.State ?? new GameState();
    }
    
    public bool IsValidSaveFile(string filePath)
    {
        if (!File.Exists(filePath))
            return false;
        
        try
        {
            var json = File.ReadAllText(filePath);
            var save = JsonSerializer.Deserialize<SaveData>(json);
            return save?.Version == SaveFormatVersion;
        }
        catch
        {
            return false;
        }
    }
    
    private class SaveData
    {
        public string Version { get; set; } = string.Empty;
        public DateTime SavedAt { get; set; }
        public GameState? State { get; set; }
    }
}
