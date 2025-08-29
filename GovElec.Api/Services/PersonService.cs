using GovElec.Api.Data;

namespace GovElec.Api.Services;

public interface IPersonService
{
    Task<Person> GetById(int id);
    Task<Person> Create(Person person);
    Task<Person> Update(Person person);
    void Delete(int id);
    Task<IEnumerable<Person>> GetAll();
}
public class PersonService : IPersonService
{
    static List<Person> _persons = new List<Person>()
    {
        new Person
            {
                Id=1,
                FirstName = "John",
                LastName = "Doe",
                BirthDate = new DateOnly(1990, 1, 1)
            },
            new Person
            {
                Id=2,
                FirstName = "Jane",
                LastName = "Smith",
                BirthDate = new DateOnly(1985, 5, 15)
            },
            new Person
            {
                Id=3,
                FirstName = "Alice",
                LastName = "Johnson",
                BirthDate = new DateOnly(2000, 12, 31)
            }
    };
    

    public async Task<Person> GetById(int id)
    {
        await Task.Yield(); // Simulate async operation
        var result = _persons.FirstOrDefault(p => p.Id == id);
        if (result == null)
        {
            return new Person();
        }
        return result;
    }

    public async Task<Person> Create(Person person)
    {
        await Task.Yield(); // Simulate async operation
        person.Id = _persons.Max(p => p.Id) + 1;

        _persons.Add(person);
        return person;
    }

    public async Task<Person> Update(Person person)
    {
        await Task.Yield(); // Simulate async operation
        var existingPerson = _persons.Find(p => p.Id == person.Id);
        if (existingPerson != null)
        {
            existingPerson.FirstName = person.FirstName;
            existingPerson.LastName = person.LastName;
            existingPerson.BirthDate = person.BirthDate;
            // Update other properties as needed
        }
        _persons.RemoveAll(p => p.Id == person.Id);
        _persons.Add(existingPerson!);
        return existingPerson!;
    }

    public void Delete(int id)
    {
        var person = _persons.FirstOrDefault(p => p.Id == id);
        if (person != null)
        {
            _persons.Remove(person);
        }
    }

    public async Task<IEnumerable<Person>> GetAll()
    {
        await Task.Yield(); // Simulate async operation
        return _persons;
    }
}
