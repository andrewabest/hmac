using System;
using System.Threading.Tasks;
using Hmac.Tests.API;
using Microsoft.Owin.Testing;
using NUnit.Framework;
using Shouldly;

namespace Hmac.Tests
{
    public class WebApiTests
    {
        [Test]
        public async Task Get_AuthenticatesSuccessfully()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var testing = await server.HttpClient.GetWithHmacAsync(new Uri($"{server.HttpClient.BaseAddress}/api/get"), "MyApp", "123456");

                Action testInspection = () => testing.EnsureSuccessStatusCode();

                testInspection.ShouldNotThrow();
            }
        }

        [Test]
        public async Task Post_AuthenticatesSuccessfully()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var testing = await server.HttpClient.PostAsJsonWithHmacAsync(new Uri($"{server.HttpClient.BaseAddress}/api/post"), new TestPayload(), "MyApp", "123456");

                Action testInspection = () => testing.EnsureSuccessStatusCode();

                testInspection.ShouldNotThrow();
            }
        }
    }
}