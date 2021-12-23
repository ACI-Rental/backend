using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDFService.Models.DTO
{
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
        public string[] Pdf { get; set; }

        public AddPdfModel(int linkedPrimaryKey, LinkedTableType linkedTableType, string[] pdf)
        {
            LinkedPrimaryKey = linkedPrimaryKey;
            LinkedTableType = linkedTableType;
            Pdf = pdf;
        }
    }
}
