namespace ACI.Products.Domain;

public static class AppErrors
{
    // Product Errors

    public static readonly IError ProductNotFoundError = new(100, "Product not found");

    public static readonly IError ProductAlreadyDeletedError = new(101, "Product is already deleted");

    public static readonly IError ProductInvalidCategoryError = new(102, "Category id is invalid");

    // Category Errors

    public static readonly IError CategoryNameAlreadyExistsError = new(200, "Category name already in use");
}