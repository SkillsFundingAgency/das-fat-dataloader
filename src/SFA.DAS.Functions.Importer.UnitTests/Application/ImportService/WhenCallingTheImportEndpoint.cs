using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SFA.DAS.Functions.Importer.Application.Services;
using SFA.DAS.Functions.Importer.Domain.Configuration;
using SFA.DAS.Functions.Importer.Domain.Interfaces;

namespace SFA.DAS.Functions.Importer.UnitTests.Application.ImportService
{
    public class WhenCallingTheImportEndpoint
    {
        [Test, AutoData]
        public void Then_The_Dataload_Endpoint_Is_Called(
            string authToken,
            ImporterConfiguration config)
        {
            //Arrange
            var azureClientCredentialHelper = new Mock<IAzureClientCredentialHelper>();
            azureClientCredentialHelper
                .Setup(x => x.GetAccessTokenAsync(It.Is<string>(c=>c.StartsWith("tenant"))))
                .ReturnsAsync(authToken);
            var configuration = new Mock<IOptions<ImporterConfiguration>>();
            var dataUrl = "https://test.local/";
            config.DataLoaderBaseUrlsAndIdentifierUris = $"{dataUrl}|tenant";
            configuration.Setup(x => x.Value).Returns(config);
            var response = new HttpResponseMessage
            {
                Content = new StringContent(""),
                StatusCode = HttpStatusCode.Accepted
            };
            var httpMessageHandler = SetupMessageHandlerMock(response, $"{dataUrl}ops/dataload");
            var client = new HttpClient(httpMessageHandler.Object);
            var service = new ImportDataService(client, configuration.Object, azureClientCredentialHelper.Object, new ImporterEnvironment("TEST"));
            
            //Act
            service.Import();

            //Assert
            httpMessageHandler.Protected()
                .Verify<Task<HttpResponseMessage>>(
                    "SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(c =>
                        c.Method.Equals(HttpMethod.Post)
                        && c.RequestUri.AbsoluteUri.Equals($"{dataUrl}ops/dataload")
                        && c.Headers.Authorization.Scheme.Equals("Bearer")
                        && c.Headers.Authorization.Parameter.Equals(authToken)),
                    ItExpr.IsAny<CancellationToken>()
                );
        }

        [Test, AutoData]
        public void Then_The_Dataload_Is_Called_For_Multiple_Endpoints(
            string authToken,
            ImporterConfiguration config)
        {
            //Arrange
            var azureClientCredentialHelper = new Mock<IAzureClientCredentialHelper>();
            azureClientCredentialHelper
                .Setup(x => x.GetAccessTokenAsync(It.Is<string>(c=>c.StartsWith("tenant"))))
                .ReturnsAsync(authToken);
            var configuration = new Mock<IOptions<ImporterConfiguration>>();
            var urls = new List<string> { "https://local.url1/|tenant1", "https://local.url2/|tenant2", "https://local.url3/|tenant3", "https://local.url4/|tenant4" };
            config.DataLoaderBaseUrlsAndIdentifierUris = string.Join(",", urls);
            configuration.Setup(x => x.Value).Returns(config);
            var response = new HttpResponseMessage
            {
                Content = new StringContent(""),
                StatusCode = HttpStatusCode.Accepted
            };
            var httpMessageHandler = SetupMessageHandlerMock(response, $"/ops/dataload");
            var client = new HttpClient(httpMessageHandler.Object);
            var service = new ImportDataService(client, configuration.Object, azureClientCredentialHelper.Object, new ImporterEnvironment("TEST"));
            //Act
            service.Import();
            //Assert
            foreach (var url in urls)
            {
                var dataUrl = url.Split("|").First();
                httpMessageHandler.Protected()
                    .Verify<Task<HttpResponseMessage>>(
                        "SendAsync", Times.Once(),
                        ItExpr.Is<HttpRequestMessage>(c =>
                            c.Method.Equals(HttpMethod.Post)
                            && c.RequestUri.AbsoluteUri.Equals($"{dataUrl}ops/dataload")
                            && c.Headers.Authorization.Scheme.Equals("Bearer")
                            && c.Headers.FirstOrDefault(h=>h.Key.Equals("X-Version")).Value.FirstOrDefault() == "1.0"
                            && c.Headers.Authorization.Parameter.Equals(authToken)),
                        ItExpr.IsAny<CancellationToken>()
                    );
            }
        }

        [Test, AutoData]
        public void Then_The_Bearer_Token_Is_Not_Added_If_Local(
            ImporterConfiguration config)
        {
            //Arrange
            var configuration = new Mock<IOptions<ImporterConfiguration>>();
            config.DataLoaderBaseUrlsAndIdentifierUris = "https://test.local/";
            configuration.Setup(x => x.Value).Returns(config);
            var response = new HttpResponseMessage
            {
                Content = new StringContent(""),
                StatusCode = HttpStatusCode.Accepted
            };
            var httpMessageHandler = SetupMessageHandlerMock(response, $"{config.DataLoaderBaseUrlsAndIdentifierUris}ops/dataload");
            var client = new HttpClient(httpMessageHandler.Object);
            var service = new ImportDataService(client, configuration.Object, Mock.Of<IAzureClientCredentialHelper>(), new ImporterEnvironment("LOCAL"));
            
            //Act
            service.Import();

            //Assert
            httpMessageHandler.Protected()
                .Verify<Task<HttpResponseMessage>>(
                    "SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(c =>
                        c.Method.Equals(HttpMethod.Post)
                        && c.RequestUri.AbsoluteUri.Equals($"{config.DataLoaderBaseUrlsAndIdentifierUris}ops/dataload")
                        && c.Headers.Authorization == null),
                    ItExpr.IsAny<CancellationToken>()
                );
        }

        private Mock<HttpMessageHandler> SetupMessageHandlerMock(HttpResponseMessage response, string baseUrl)
        {
            var httpMessageHandler = new Mock<HttpMessageHandler>();
            httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(c =>
                        c.Method.Equals(HttpMethod.Get)
                        && c.RequestUri.AbsoluteUri.EndsWith(baseUrl)),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) => response);
            return httpMessageHandler;
        }
    }
}