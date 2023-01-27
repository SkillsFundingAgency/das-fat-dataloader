using System.Net;
using WireMock.Logging;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace SFA.DAS.Functions.Importer.MockAPIs;

public class MockApiBuilder
{
    private readonly WireMockServer _server;

    public MockApiBuilder(int port)
    {
        _server = WireMockServer.Start(new WireMockServerSettings
        {
            Port = port,
            UseSSL = true,
            StartAdminInterface = true,
            Logger = new WireMockConsoleLogger(),
        });
    }

    public static MockApiBuilder Create(int port)
    {
        return new MockApiBuilder(port);
    }

    public MockApi Build()
    {
        return new MockApi(_server);
    }

    public MockApiBuilder StartEndPoints()
    {
        _server.Given(Request.Create()
                        .WithPath("/ops/dataload")
                        .UsingPost())
               .RespondWith(Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBodyAsJson($"called /ops/dataload on port {_server.Port}"));

        return this;
    }
}
