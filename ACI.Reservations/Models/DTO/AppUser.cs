using System;

namespace ACI.Reservations.Models.DTO;

public class AppUser
{
    public AppUser(string name, string id, string email)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(id))
        {
            throw new ArgumentException("AppUser must have a non-empty name, email, and id");
        }

        Id = id;
        Name = name;
        Email = email;
    }

    public string Id { get; }
    public string Name { get; }
    public string Email { get; }
}