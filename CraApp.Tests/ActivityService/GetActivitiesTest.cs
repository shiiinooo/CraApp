using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CraApp.Tests.ActivityService
{
    public class GetActivitiesTest
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private  APIResponse _response;

        public GetActivitiesTest()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [Fact]
        public async void Sould_Return_Activities()
        {

            var result = await _client.GetAsync("/api/activity");
            _response = result.Adapt<APIResponse>();

            Assert.NotNull(_response);
            Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
            Assert.NotNull(_response.Result);
        }
    }
}
