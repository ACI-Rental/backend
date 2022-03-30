namespace ACI.ImageService.Domain
{
    public class AppErrors
    {
        public static readonly IError ImageNotFoundError = new(100, "Image not found");
        public static readonly IError ImageAlreadyDeletedError = new(101, "Image is already deleted");
    }
}