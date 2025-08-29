namespace GovElec.Shared;

public class Person
{
    public int Id{ get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }=new DateOnly(1900, 1, 1);
}
