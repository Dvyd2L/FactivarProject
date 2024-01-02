using Microsoft.AspNetCore.Http;
using Ocelot.Middleware;
using Ocelot.Multiplexer;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;

namespace Services;

/// <summary>
/// Clase que agrega las respuestas de varios servicios en una sola respuesta.
/// Implementa la interfaz IDefinedAggregator.
/// </summary>
public class ServiceAggregator : IDefinedAggregator
{
    /// <summary>
    /// Construye una cadena JSON a partir de una lista de respuestas de servicio.
    /// </summary>
    /// <param name="serviceResponses">La lista de respuestas de servicio.</param>
    /// <returns>Una cadena JSON que representa las respuestas de servicio.</returns>
    private static string BuildJSON(List<string> serviceResponses)
    {
        StringBuilder jsonBuilder = new('{');

        for (int i = 0; i < serviceResponses.Count; i++)
        {
            _ = jsonBuilder.Append($"\"servicio{i + 1}\": {serviceResponses[i]}");

            // Añade una coma si no es el último elemento
            if (i < serviceResponses.Count - 1)
            {
                _ = jsonBuilder.Append(", ");
            }
        }

        _ = jsonBuilder.Append('}');

        return jsonBuilder.ToString();
    }

    /// <summary>
    /// Agrega las respuestas de varios servicios en una sola respuesta.
    /// </summary>
    /// <param name="responses">La lista de respuestas de HttpContext a agregar.</param>
    /// <returns>Una tarea que representa la operación de agregación asíncrona y devuelve una respuesta agregada.</returns>
    public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
    {
        List<string> serviceResponses = [];

        foreach (HttpContext response in responses)
        {
            if (response.Items.DownstreamResponse().StatusCode == HttpStatusCode.OK)
            {
                serviceResponses.Add(await response.Items.DownstreamResponse().Content.ReadAsStringAsync());
            }
        }

        string json = BuildJSON(serviceResponses);

        StringContent stringContent = new(json)
        {
            Headers = { ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Json) }
        };

        return new DownstreamResponse(stringContent, HttpStatusCode.OK, new List<Header>(), "OK");
    }
}