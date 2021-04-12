namespace ImageService.Models.DTO
{
    /// <summary>
    /// Data model used when checking whether the send content is an image
    /// </summary>
    public class CheckImageModel
    {
        /// <summary>
        /// Image formatted in Base64
        /// </summary>
        public string Base64Image { get; set; }
    }
}
