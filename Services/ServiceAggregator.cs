using Microsoft.AspNetCore.Http;
using Ocelot.Middleware;
using Ocelot.Multiplexer;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;

namespace Services;

public class ServiceAggregator : IDefinedAggregator
{
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