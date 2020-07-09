using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Ouikum.Web.Facebook
{
    public class FacebookBackChannelHandler : HttpClientHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!request.RequestUri.AbsolutePath.Contains("/oath"))
            { 
                request.RequestUri = new Uri(request.RequestUri.AbsoluteUri.Replace("?access_token","&access_token"));
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}