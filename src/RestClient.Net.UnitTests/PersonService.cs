using RestClientApiSamples;
using System;
using System.Threading.Tasks;

namespace RestClient.Net.UnitTests
{
    public class PersonService
    {
        private readonly IClient _client;

        public PersonService(CreateClient clientFactory) => _client = clientFactory != null ? clientFactory("Person") : throw new ArgumentNullException(nameof(clientFactory));

        public async Task<Response<Person>> SavePerson(Person person) => await _client.PostAsync<Person, Person>(person);
    }
}
