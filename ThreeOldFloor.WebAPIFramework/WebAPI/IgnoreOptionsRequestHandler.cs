using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ThreeOldFloor.WebAPIFramework.WebAPI
{
    public class IgnoreOptionsRequestHandler:DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // 不处理任何Options的请求，直接返回200
            if (request.Method == HttpMethod.Options)
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);

                var tsc = new TaskCompletionSource<HttpResponseMessage>();
                tsc.SetResult(response);   
                return tsc.Task;
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
