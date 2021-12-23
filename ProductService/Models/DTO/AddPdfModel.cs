using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Models.DTO
{
    /// <summary>
    /// Data model for the AddImage calls
    /// </summary>
    public class AddPdfModel
    {
        /// <summary>
        /// Contains primarykey of product or note
        /// </summary>
        public int LinkedPrimaryKey { get; set; }
        /// <summary>
        /// Contains info if LinkedPrimaryKey is linked to a product or note
        /// </summary>
        public LinkedTableType LinkedTableType { get; set; }
        /// <summary>
        /// Contains all images that need to be saved
        /// </summary>
        public string[] Pdfs { get; set; }

        public AddPdfModel(int linkedPrimaryKey, LinkedTableType linkedTableType, string[] pdfs)
        {
            LinkedPrimaryKey = linkedPrimaryKey;
            LinkedTableType = linkedTableType;
            Pdfs = pdfs;
        }
    }
}
