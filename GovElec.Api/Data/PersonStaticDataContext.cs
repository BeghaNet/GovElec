namespace GovElec.Api.Data;

public static class PersonStaticDataContext
{
    public static List<Person> Persons { get; } = new List<Person>
    {
        new Person
        {
            FirstName = "John",
            LastName = "Doe",
            BirthDate = new DateOnly(1990, 1, 1)
        },
        new Person
        {
            FirstName = "Jane",
            LastName = "Smith",
            BirthDate = new DateOnly(1985, 5, 15)
        },
        new Person
        {
            FirstName = "Alice",
            LastName = "Johnson",
            BirthDate = new DateOnly(2000, 12, 31)
        }
    };
}
