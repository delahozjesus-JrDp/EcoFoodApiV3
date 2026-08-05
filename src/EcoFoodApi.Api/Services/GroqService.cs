using System.Text;
using System.Text.Json;
using EcoFoodApi.Api.DTOs;

namespace EcoFoodApi.Api.Services;

public class GroqService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GroqService> _logger;

    public GroqService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<GroqService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<RecetaResponseDTO> ObtenerRecetaAsync(List<string> productos)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("Groq");
            var model = _configuration["Groq:Model"] ?? "llama3-70b-8192";

            var prompt = $"Tengo estos productos disponibles en mi despensa: {string.Join(", ", productos)}. " +
                "Sugiere una receta que pueda preparar con la mayoría de ellos. Responde EXCLUSIVAMENTE con un JSON " +
                "con este formato exacto, sin texto adicional: " +
                "{ \"receta\": string, \"ingredientesFaltantes\": string[], \"productosConsumirPrimero\": string[] }";

            var requestBody = new
            {
                model,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                temperature = 0.5,
                max_tokens = 500,
                response_format = new { type = "json_object" }
            };

            /*var json = JsonSerializer.Serialize(requestBody);

_logger.LogInformation("Solicitud enviada a Groq:");
_logger.LogInformation(json);

var content = new StringContent(json, Encoding.UTF8, "application/json");

var response = await client.PostAsync("/openai/v1/chat/completions", content);

// Leer la respuesta ANTES de validar el código HTTP
var responseJson = await response.Content.ReadAsStringAsync();

_logger.LogInformation("Código de respuesta: {StatusCode}", response.StatusCode);
_logger.LogInformation("Respuesta de Groq:");
_logger.LogInformation(responseJson);

response.EnsureSuccessStatusCode();*/

           var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/openai/v1/chat/completions", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(responseJson);
            var mensajeContenido = document.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;

            var receta = JsonSerializer.Deserialize<RecetaResponseDTO>(mensajeContenido, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return receta ?? CrearRespuestaFallback();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "El servicio de IA (Groq) no respondió correctamente");
            return CrearRespuestaFallback();
        }
    }

    private static RecetaResponseDTO CrearRespuestaFallback() => new()
    {
        Receta = null,
        IngredientesFaltantes = new List<string>(),
        ProductosConsumirPrimero = new List<string>(),
        Mensaje = "Servicio de IA no disponible en este momento"
    };
}
