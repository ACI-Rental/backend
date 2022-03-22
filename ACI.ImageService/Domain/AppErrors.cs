namespace ACI.ImageService.Domain
{
    public class AppErrors
    {
        public static readonly IError ProductNotFoundError = new(100, "Product not found");
    }
}