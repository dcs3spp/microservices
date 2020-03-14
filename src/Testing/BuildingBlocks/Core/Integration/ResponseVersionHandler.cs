using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace dcs3spp.Testing.Utilities.Net.Http.Handlers
{
    public class ResponseVersionHandler : DelegatingHandler
    {
        /// <summary>
        /// Send a HTTP Request message and send the response matching the version of the request
        /// </summary>
        /// <returns>Response matching version of request message</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var response = await base.SendAsync(request, cancellationToken);
            response.Version = request.Version;

            return response;
        }
    }
}
