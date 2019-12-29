using RestClientApiSamples;
using RestClientDotNet;
using System;
using System.Threading.Tasks;

namespace RESTClient.NET.CoreSample
{
    internal class Program
    {
        #region Main Method
        private static void Main(string[] args)
        {
            Go();
            Console.ReadLine();
        }
        #endregion

        #region Methods
        private static async Task Go()
        {
            try
            {
                Console.WriteLine($"This sample is calling the local Api in ApiExamples. It must be running for this sample to work.");

                var person = new Person { FirstName = "Bob", Surname = "Smith" };
                var restClient = new RestClient(new ProtobufSerializationAdapter(), new Uri("http://localhost:42908/person"));

                Console.WriteLine($"Sending a POST with body of person {person.FirstName} {person.Surname} serialized to binary with Google Protobuffers");
                person = await restClient.PostAsync<Person, Person>(person);

                Console.WriteLine($"Success! The response has a body of person {person.FirstName} {person.Surname} serialized from binary with Google Protobuffers");
            }
            catch(Exception ex)
            {
                Console.WriteLine("The sample failed. Is the ApiExamples web service running?\r\nTry: Right click on ApiExamples -> View -> View In Browser -> Run this sample again\r\n\r\n");
                Console.WriteLine(ex.ToString());
            }

            Console.ReadLine();
        }

        #endregion
    }
}
