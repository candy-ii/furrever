namespace Backend.Models;

// One row in the "Pets" table. After adding a property here, generate a
// migration so the column gets added: dotnet ef migrations add <Name>
public class Pet
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Species { get; set; } = "";
}
