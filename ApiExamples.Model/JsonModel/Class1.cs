using System;

namespace ApiExamples.Model.JsonModel
{
    public class Person
    {
        public Guid PersonKey { get; set; } = new Guid();
        public string firstName { get; set; }
        public string Surname { get; set; }
        public Address BillingAddress { get; set; } = new Address();
    }

    public class Address
    {
        public Guid AddressKey { get; set; } = Guid.NewGuid();
        public string Street { get; set; } = "Test St";
    }
}
