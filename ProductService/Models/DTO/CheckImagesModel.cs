namespace ProductService.Models.DTO
{
    /// <summary>
    /// Data model used when checking whether the send content are images
    /// </summary>
    public class CheckImagesModel
    {
        /// <summary>
        /// Collection of images formattes as an Base64 string
        /// </summary>
        public string[] Base64Images { get; set; }
    }
}
