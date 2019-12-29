using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
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
            var defaultHttpClientFactory = new DefaultHttpClientFactory();
            defaultHttpClientFactory.AddClient("test", new HttpClient { BaseAddress = new Uri("http://www.test.com") });

            var asdasds = defaultHttpClientFactory.CreateClient("test");

            var container = new Container(c =>
            {
                c.Scan(s =>
                {
                    s.TheCallingAssembly();
                    s.WithDefaultConventions();
                });

                c.For<IHttpClientFactory>().Use<DefaultHttpClientFactory>(defaultHttpClientFactory);
            });
        }
    }
}
