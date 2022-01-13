using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDFService.DBContexts;
using PDFService.Models;
using PDFService.Models.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;

namespace PDFService.Controllers
{
    /// <summary>
    /// Image controller this controller is used for the calls between API and frontend for managing the images in the ACI Rental system
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class PDFController : ControllerBase
    {
        /// <summary>
        /// Database context for the image service, this is used to make calls to the image table
        /// </summary>
        private readonly PDFServiceDatabaseContext _dbContext;

        /// <summary>
        /// Constructor is used for receiving the database context at the creation of the image controller
        /// </summary>
        /// <param name="dbContext">Database context param used for calls to the image table</param>
        public PDFController(PDFServiceDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Gets all images bound to the product id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns>Returns a list of images</returns>
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetPdfByProductId(int productId)
        {
            var pdfs = await _dbContext.pdfs.Where(x => x.LinkedKey == productId && x.LinkedTableType == LinkedTableType.PRODUCT).ToListAsync();
            List<PdfBlobModel> pdfBlobModels = new List<PdfBlobModel>();
            foreach (var item in pdfs)
            {
                pdfBlobModels.Add(new PdfBlobModel() { Blob = item?.Blob });
            }

            return Ok(pdfBlobModels);
        }

        /// <summary>
        /// Adds image to the database
        /// </summary>
        /// <param name="addImageModel">The API call data object</param>
        /// <returns>Badrequest if fails, Created if success </returns>
        [HttpPost]
        public async Task<IActionResult> SavePdf(AddPdfModel addPdfModel)
        {
            if (addPdfModel == default)
            {
                return BadRequest("NO_DATA");
            }

            if (addPdfModel.Pdfs == default || !addPdfModel.Pdfs.Any())
            {
                return BadRequest("NO_PDF");
            }

            if (addPdfModel.LinkedPrimaryKey < 1)
            {
                return BadRequest("NO_LINKED_KEY");
            }

            List<Pdf> pdfs = new List<Pdf>();

            foreach (var pdf in addPdfModel.Pdfs)
            {
                //if (CheckPdf(new CheckPdfModel() { Pdf = pdf }).GetType() != typeof(OkResult))
                //{
                //    return BadRequest("File is not an Pdf");
                //}

                var newPdf = new Pdf()
                {
                    LinkedKey = addPdfModel.LinkedPrimaryKey,
                    LinkedTableType = addPdfModel.LinkedTableType,
                    Blob = Convert.FromBase64String(pdf[(pdf.IndexOf(",") + 1)..])
                };

                pdfs.Add(newPdf);
            }

            await _dbContext.pdfs.AddRangeAsync(pdfs);
            await _dbContext.SaveChangesAsync();

            return Created("/product", pdfs);
        }

        /// <summary>
        /// Checks if Base64 string is an image
        /// </summary>
        /// <param name="checkImageModel">Object containing the Base64 string</param>
        /// <returns>Badrequest if it's not an image. Ok if it is an image</returns>
        [HttpPost("checkPdfModel")]
        public IActionResult CheckPdf(CheckPdfModel checkPdfModel)
        {
            if (checkPdfModel == default || string.IsNullOrWhiteSpace(checkPdfModel.Pdf))
            {
                return BadRequest("Request did not contain data");
            }

            //if (!checkPdfModel.Pdf.Contains(","))
            //{
            //    return BadRequest("Base64string is not JS based Base64 (does not contain type)");
            //}

            var acceptedPdfTypes = new string[] { "pdf" };

            var fileType = checkPdfModel.Pdf.Split(",")[0];

            //if (!acceptedPdfTypes.Any(fileType.Contains))
            //{
            //    return BadRequest("Incorrect pdf type");
            //}

            try
            {
                //using var ms = new MemoryStream(Convert.FromBase64String(checkPdfModel.Pdf[(checkPdfModel.Pdf.IndexOf(",") + 1)..]));
                //var pdf = .FromStream(ms);
                //pdf.Dispose();
            }
            catch (Exception)
            {
                return BadRequest("Incorrect pdf type");
            }

            return Ok();
        }
    }
}
