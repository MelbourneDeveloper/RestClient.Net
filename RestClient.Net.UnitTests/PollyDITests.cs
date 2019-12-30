#if NETCOREAPP3_1

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;

namespace RestClientDotNet.UnitTests
{
    [TestClass]
    public class PollyDITests
    {
        [TestMethod]
        public void Test()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddHttpClient("test", (a)=> { a.BaseAddress = new Uri("http://www.test.com"); });
            var asdasd = serviceCollection.BuildServiceProvider();
            var adaadf = asdasd.GetService<IHttpClientFactory>();

            var asdasds = adaadf.CreateClient("test");

        }
    }  
}
#endif