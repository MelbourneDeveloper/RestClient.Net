using RestClient.Net;
using RestClientApiSamples;
using System;
using System.Threading.Tasks;


await Go().ConfigureAwait(false);
_ = Console.ReadLine();



static async Task Go()
{
    try
    {
        Console.WriteLine($"This sample is calling the local Api in ApiExamples. It must be running for this sample to work.");

        var person = new Person { FirstName = "Bob", Surname = "Smith" };
        using var client = new Client(new ProtobufSerializationAdapter(), baseUrl: new("http://localhost:42908/person"));

        Console.WriteLine($"Sending a POST with body of person {person.FirstName} {person.Surname} serialized to binary with Google Protobuffers");
        person = await client.PostAsync<Person, Person>(person).ConfigureAwait(false);

        Console.WriteLine($"Success! The response has a body of person {person?.FirstName} {person?.Surname} serialized from binary with Google Protobuffers");
    }
    catch (Exception ex)
    {
        Console.WriteLine("The sample failed. Is the ApiExamples web service running?\r\nTry: Right click on ApiExamples -> View -> View In Browser -> Run this sample again\r\n\r\n");
        Console.WriteLine(ex.ToString());
    }

    _ = Console.ReadLine();
}
