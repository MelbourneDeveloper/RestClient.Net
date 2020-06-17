using System;

namespace ApiExamples.Model.JsonModel
{
    public class Address
    {
        public Guid AddressKey { get; set; } = Guid.NewGuid();
        public string Street { get; set; } = "Test St";
        public string StreeNumber { get; set; }
        public string Suburb { get; set; }
    }
}
