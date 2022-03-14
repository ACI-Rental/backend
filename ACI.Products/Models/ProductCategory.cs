using System.ComponentModel.DataAnnotations.Schema;

namespace ACI.Products.Models;

public class ProductCategory
{
    public int Id { get; set; }

    public string Name { get; set; }

    public virtual List<Product> Products { get; set; }
}