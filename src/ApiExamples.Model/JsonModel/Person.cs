using System;

namespace ApiExamples.Model.JsonModel
{
    public class Person
    {
        public Guid PersonKey { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public Address BillingAddress { get; set; } = new Address();
        public DateTime? DateOfBirth { get; set; } = new DateTime(2001, 1, 1);
        public DateTime DateAdded { get; set; } = new DateTime(2010, 1, 1);
        public int Id { get; set; } = 50;
        public decimal AccountBalance { get; set; } = (decimal)10000.78;
        public double Weight { get; set; } = 210.34;
    }
}
