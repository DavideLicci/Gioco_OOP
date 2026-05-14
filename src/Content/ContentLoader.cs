namespace OltreIlTempo.Content;

using OltreIlTempo.Domain;
using OltreIlTempo.Engine;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false));
        options.Converters.Add(new IdDictionaryJsonConverter<RoomDefinition>());
        options.Converters.Add(new IdDictionaryJsonConverter<NpcDefinition>());
        options.Converters.Add(new IdDictionaryJsonConverter<ItemDefinition>());
        options.Converters.Add(new IdDictionaryJsonConverter<ClueDefinition>());
        options.Converters.Add(new IdDictionaryJsonConverter<DeductionDefinition>());
        options.Converters.Add(new IdDictionaryJsonConverter<AchievementDefinition>());
        
        var content = JsonSerializer.Deserialize<GameContent>(json, options);
        
        if (content == null)
        {
            throw new InvalidOperationException("Impossibile deserializzare il contenuto del gioco.");
        }
        
        return content;
    }

    private sealed class IdDictionaryJsonConverter<T> : JsonConverter<Dictionary<string, T>>
        where T : class
    {
        public override Dictionary<string, T> Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            var result = new Dictionary<string, T>();

            if (document.RootElement.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in document.RootElement.EnumerateObject())
                {
                    var item = property.Value.Deserialize<T>(options)
                        ?? throw new JsonException($"Valore nullo per '{property.Name}'.");

                    EnsureId(item, property.Name);
                    result[property.Name] = item;
                }

                return result;
            }

            if (document.RootElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var element in document.RootElement.EnumerateArray())
                {
                    var item = element.Deserialize<T>(options)
                        ?? throw new JsonException("Valore nullo in una lista indicizzata per id.");

                    string id = GetId(item);
                    if (string.IsNullOrWhiteSpace(id))
                    {
                        throw new JsonException($"Elemento {typeof(T).Name} senza campo id.");
                    }

                    result[id] = item;
                }

                return result;
            }

            throw new JsonException($"Formato non valido per dizionario {typeof(T).Name}.");
        }

        public override void Write(
            Utf8JsonWriter writer,
            Dictionary<string, T> value,
            JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var item in value)
            {
                writer.WritePropertyName(item.Key);
                JsonSerializer.Serialize(writer, item.Value, options);
            }

            writer.WriteEndObject();
        }

        private static string GetId(T item)
        {
            var idProperty = typeof(T).GetProperty("Id");
            return idProperty?.GetValue(item) as string ?? "";
        }

        private static void EnsureId(T item, string fallbackId)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null || !idProperty.CanWrite || !string.IsNullOrWhiteSpace(GetId(item)))
            {
                return;
            }

            idProperty.SetValue(item, fallbackId);
        }
    }
}
