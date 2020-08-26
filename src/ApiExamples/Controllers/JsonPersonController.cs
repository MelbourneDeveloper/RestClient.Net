using ApiExamples.Model.JsonModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace ApiExamples.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JsonPersonController : ControllerBase
    {
        [HttpGet]
        public Person Get(string personKey)
        {
            var person = new Person
            {
                PersonKey = new Guid(personKey),
                FirstName = "Sam",
                BillingAddress = new Address
                {
                    StreeNumber = "100",
                    Street = "Somewhere",
                    Suburb = "Sometown"
                },
                Surname = "Smith"
            };

            return person;
        }

        [HttpGet]
        [Route("People")]
        public List<Person> Get()
        {
            var people = new List<Person>();

            for (var i = 0; i < 500; i++)
            {
                var person = new Person
                {
                    FirstName = $"Sam{i}",
                    BillingAddress = new Address
                    {
                        StreeNumber = "100",
                        Street = "Somewhere",
                        Suburb = "Sometown"
                    },
                    Surname = "Smith"
                };
                people.Add(person);
            }

            return people;
        }

        [HttpPost]
        [Route("people")]
        public List<Person> PostPeople([FromBody] List<Person> people) => people;

        [HttpPost]
        [Route("save")]
        public Person Post([FromBody] Person person) => person;

        [HttpPut]
        [Route("save")]
        public Person Put([FromBody] Person person) => person;

        [HttpPatch]
        [Route("save")]
        public Person Patch([FromBody] Person person) => person;

        [HttpDelete]
#pragma warning disable IDE0060 // Remove unused parameter
        public IActionResult Delete(string personKey) => Ok();
#pragma warning restore IDE0060 // Remove unused parameter
    }
}
