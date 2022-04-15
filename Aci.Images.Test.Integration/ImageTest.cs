using ACI.Images.Test.Integration.Fixtures;
using System.Net.Http;
using Xunit;

namespace Aci.Images.Test.Integration
{
    public class ImageTest : IClassFixture<ImageAppFactory>
    {
        private readonly HttpClient _apiClient;

        public ImageTest(ImageAppFactory factory)
        {
            _apiClient = factory.CreateClient();
        }

        [Fact]
        public void Test1()
        {

        }
    }
}