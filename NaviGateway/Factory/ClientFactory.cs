using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using NaviAuthenticationShared;

namespace NaviGateway.Factory
{
    public class ClientFactory
    {
        public readonly Authentication.AuthenticationClient AuthenticationClient;

        public ClientFactory(IConfiguration configuration)
        {
            AuthenticationClient =
                new Authentication.AuthenticationClient(
                    GrpcChannel.ForAddress(configuration.GetConnectionString("AuthenticationConnection")));
        }
    }
}