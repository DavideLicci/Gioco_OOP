namespace OltreIlTempo.Content;

using OltreIlTempo.Engine;
using System.Text.Json;

/// <summary>
/// Implementazione del caricatore di contenuti.
/// NICK: carica il file game-content.json e lo deserializza.
/// </summary>
public class ContentLoader : IContentLoader
{
    public GameContent Load(string filePath)
    {
        // TODO: Implementare il caricamento effettivo
        // 1. Leggi il file JSON
        // 2. Deserializza in GameContent
        // 3. Valida il contenuto
        // 4. Gestisci errori
        
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File di contenuto non trovato: {filePath}");
        }
        
        var json = File.ReadAllText(filePath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        var content = JsonSerializer.Deserialize<GameContent>(json, options);
        
        if (content == null)
        {
            throw new InvalidOperationException("Impossibile deserializzare il contenuto del gioco.");
        }
        
        return content;
    }
}
