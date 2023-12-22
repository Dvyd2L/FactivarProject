using System.Net;

namespace Handlers;

public class ApiKeyHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!ValidateKey(request))
        {
            HttpResponseMessage response = new(HttpStatusCode.BadRequest);
            return response;
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private static bool ValidateKey(HttpRequestMessage request)
    {
        if (request.Headers.TryGetValues("ApiKey", out IEnumerable<string>? headerList))
        {
            string header = headerList.First();

            //do something with the header value
            if (!string.IsNullOrEmpty(header) && header == "123456")
                return true;
        }

        return false;
    }
}
