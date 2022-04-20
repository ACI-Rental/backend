namespace ACI.Products.Models.DTO;

public class AppUser
{
    public AppUser(string name, string id)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(id))
        {
            throw new ArgumentException("AppUser must have a non-empty name and id");
        }

        Name = name;
        Id = id;
    }

    public string Name { get; }
    public string Id { get; }
}