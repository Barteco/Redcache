using Bogus;

namespace Redfish.Tests.Fakes
{
    class PersonFaker
    {
        private Faker<Person> _faker;

        public PersonFaker()
        {
            _faker = new Faker<Person>()
                .RuleFor(o => o.Name, f => f.Name.FirstName())
                .RuleFor(o => o.DateOfBirth, f => f.Date.Past())
                .RuleFor(o => o.Address, f => f.Address.FullAddress());
        }

        public Person GenerateOne()
        {
            return _faker.Generate();
        }
    }
}
