using System;

namespace ApiExamples.Model.JsonModel
{
    public class Person
    {
        public Guid PersonKey { get; set; } = new Guid();
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public Address BillingAddress { get; set; } = new Address();
    }
}
