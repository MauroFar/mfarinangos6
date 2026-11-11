using System.Text.Json.Serialization;

namespace mfarinangos6.Models;

public class Estudiante
{
    [JsonPropertyName("codigo")]
    public int Codigo { get; set; }

    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("apellido")]
    public string Apellido { get; set; } = string.Empty;

    [JsonPropertyName("edad")]
    public int Edad { get; set; }
}
