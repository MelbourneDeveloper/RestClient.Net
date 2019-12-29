using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace RestClientDotNet.UnitTests
{
    [TestClass]
    public class PollyDITests
    {
        [TestMethod]
        public void Test()
        {
            //var factory = new DefaultHttpClientFactory();

            var defaultHttpClientFactory = new DefaultHttpClientFactory();

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
