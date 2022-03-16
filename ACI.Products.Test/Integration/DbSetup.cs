using System.Collections.Generic;
using System.Linq;
using ACI.Products.Data;
using ACI.Products.Models;
using Bogus;

namespace ACI.Products.Test.Integration;

public class DbSetup
{
    public static void InitializeForTests(ProductContext db)
    {
        db.Categories.AddRange(GetCategories());
        db.SaveChanges();

        var categories = db.Categories.ToList();

        foreach (var category in categories)
        {
            db.Products.AddRange(GetProducts(20, category.Id));
        }

        db.SaveChanges();
    }

    public static void Clean(ProductContext db)
    {
        db.Products.RemoveRange(db.Products.ToList());
        db.Categories.RemoveRange(db.Categories.ToList());
        db.SaveChanges();

        InitializeForTests(db);
    }

    public static List<ProductCategory> GetCategories()
    {
        return new List<ProductCategory>
        {
            new() { Name = "Laptops" },
            new() { Name = "Camera's" },
            new() { Name = "Statieven" },
            new() { Name = "Opslagmedia" },
            new() { Name = "Koptelefoons" },
            new() { Name = "VR-brillen" },
            new() { Name = "Lampen" },
            new() { Name = "Speakers" },
        };
    }

    public static List<Product> GetProducts(int amount = 200, int categoryId = 1)
    {
        return new Faker<Product>()
            .RuleFor(p => p.Name, f => $"{f.Commerce.ProductAdjective()} {f.Commerce.ProductName()}")
            .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
            .RuleFor(p => p.CategoryId, categoryId)
            .RuleFor(p => p.IsDeleted, false)
            .RuleFor(p => p.RequiresApproval, false)
            .Generate(amount);
    }
}