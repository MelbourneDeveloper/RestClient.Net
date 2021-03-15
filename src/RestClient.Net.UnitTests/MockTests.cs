
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestClient.Net.Abstractions;
using RestClientApiSamples;
using System;
using System.Threading.Tasks;

namespace RestClient.Net.UnitTests
{
    [TestClass]
    public class MockTests
    {
        [TestMethod]
        public async Task TestPersonService()
        {
            //Create a Person object to be sent as the Request
            var requestPerson = new Person { FirstName = "TestGuy", PersonKey = "" };

            //Create a Person object to be received from the Response
            var responsePerson = new Person { FirstName = "TestGuy", PersonKey = "123" };

            //Create mock objects
            var loggerMock = new Mock<ILogger>();
            var clientFactoryMock = new Mock<CreateClient>();
            var clientMock = new Mock<IClient>();
            var responseMock = new Mock<HttpResponseMessageResponse<Person>>(new object[] { responsePerson });
            var serializationAdapterMock = new Mock<ISerializationAdapter>();

            //Set the factory up to return the mock client
            _ = clientFactoryMock.Setup(f => f.Invoke("Person", null)).Returns(clientMock.Object);

            //Set the client up to return the response mock
            _ = clientMock.Setup(c => c.SendAsync<Person, Person>(It.IsAny<Request<Person>>())).Returns(Task.FromResult<Response<Person>>(responseMock.Object));

            _ = clientMock.Setup(c => c.BaseUri).Returns(new Uri("http://www.asdasd.com"));

            _ = serializationAdapterMock.Setup(c => c.Deserialize<Person>(It.IsAny<byte[]>(), It.IsAny<IHeadersCollection>())).Returns(responsePerson);

            _ = responseMock.Setup(r => r.Body).Returns(responsePerson);

            //Create the service and call SavePerson
            var personService = new PersonService(clientFactoryMock.Object);
            var returnPersonResponse = await personService.SavePerson(requestPerson).ConfigureAwait(false);

            Assert.AreEqual("123", returnPersonResponse?.Body?.PersonKey);
        }
    }
}
