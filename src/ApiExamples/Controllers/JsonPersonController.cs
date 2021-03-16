using ApiExamples.Model.JsonModel;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CA1724 
#pragma warning disable CA1716
#pragma warning disable CA1707
#pragma warning disable CA1056
#pragma warning disable CA1056
#pragma warning disable CA2227
#pragma warning disable CA1002
#pragma warning disable IDE0060 
#pragma warning disable CA1801 
#pragma warning disable IDE0060 // Remove unused parameter

namespace ApiExamples.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JsonPersonController : ControllerBase
    {
        [HttpGet]
#pragma warning disable IDE0060 // Remove unused parameter
        public Person Get(string personKey)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            var person = new Person
            {
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

        [HttpPatch]
        [Route("save2")]
        public Person Patch() => new Person { FirstName = "J" };

        [HttpPost]
        [Route("save2")]
        public Person PostSave2() => new Person { FirstName = "J" };
        
        [HttpPut]
        [Route("save2")]
        public Person PutSave2() => new Person { FirstName = "J" };
        
        [HttpDelete]
#pragma warning disable IDE0060 // Remove unused parameter
        public IActionResult Delete(string personKey) => Ok();
#pragma warning restore IDE0060 // Remove unused parameter
    }
}
