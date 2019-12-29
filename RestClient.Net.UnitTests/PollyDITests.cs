#if NETCOREAPP3_1

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using System;
using System.Collections;
using System.Collections.Generic;
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
            serviceCollection.AddHttpClient();
            var asdasd = serviceCollection.BuildServiceProvider();
            var adaadf = asdasd.GetService<IHttpClientFactory>();

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

    //public class asdasd : IServiceCollection
    //{
    //    public ServiceDescriptor this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    //    public int Count => throw new NotImplementedException();

    //    public bool IsReadOnly => throw new NotImplementedException();

    //    public void Add(ServiceDescriptor item)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Clear()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public bool Contains(ServiceDescriptor item)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IEnumerator<ServiceDescriptor> GetEnumerator()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public int IndexOf(ServiceDescriptor item)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Insert(int index, ServiceDescriptor item)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public bool Remove(ServiceDescriptor item)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void RemoveAt(int index)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
#endif