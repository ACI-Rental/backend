using System.Collections.Generic;
using ACI.Products.Models;

namespace ACI.Products.Test.Unit;

public class TestData
{
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
}