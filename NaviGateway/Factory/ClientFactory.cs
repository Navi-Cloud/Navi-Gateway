using Grpc.Net.Client;
using Io.Github.NaviCloud.Shared.Authentication;
using Microsoft.Extensions.Configuration;

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