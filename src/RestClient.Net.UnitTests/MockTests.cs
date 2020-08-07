#if NET45
using RestClient.Net.Abstractions.Logging;
#else
using Microsoft.Extensions.Logging;
#endif

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestClient.Net.Abstractions;
using RestClientApiSamples;
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
            var responseMock = new Mock<Response<Person>>();

            //Set the factory up to return the mock client
            _ = clientFactoryMock.Setup(f => f.Invoke("Person")).Returns(clientMock.Object);

            //Set the client up to return the response mock
            _ = clientMock.Setup(c => c.SendAsync<Person, Person>(It.IsAny<Request<Person>>())).Returns(Task.FromResult(responseMock.Object));

            //Set the response up to return responsePerson
            _ = responseMock.Setup(r => r.Body).Returns(responsePerson);

            //Create the service and call SavePerson
            var personService = new PersonService(clientFactoryMock.Object);
            var returnPerson = await personService.SavePerson(requestPerson);

            Assert.AreEqual("123", returnPerson.PersonKey);
        }
    }

    public class PersonService
    {
        private readonly IClient _client;

        public PersonService(CreateClient clientFactory)
        {
            _client = clientFactory("Person");
        }

        public async Task<Person> SavePerson(Person person)
        {
            return await _client.PostAsync<Person, Person>(person);
        }
    }
}
