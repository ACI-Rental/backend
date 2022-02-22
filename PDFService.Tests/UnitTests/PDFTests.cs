using PDFService.Controllers;
using PDFService.DBContexts;
using PDFService.Models;
using PDFService.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PDFService.Tests.UnitTests
{
    public class PDFTests
    {
        private readonly PDFController _controller;
        private readonly PDFServiceDatabaseContext _context;

        public PDFTests()
        {
            var options = new DbContextOptionsBuilder<PDFServiceDatabaseContext>().UseInMemoryDatabase(databaseName: "InMemoryPDFDb").Options;

            _context = new PDFServiceDatabaseContext(options);
            _controller = new PDFController(_context);
            SeedImageInMemoryDatabaseWithData(_context);
        }

        [Fact]
        public async Task GetPdfByProductId_ReturnListOfPdfs()
        {
            var result = await _controller.GetPdfByProductId(1);

            var objectresult = Assert.IsType<OkObjectResult>(result);
            var images = Assert.IsAssignableFrom<IEnumerable<PdfBlobModel>>(objectresult.Value);
            Assert.Equal(3, images.Count());
        }

        private static void SeedImageInMemoryDatabaseWithData(PDFServiceDatabaseContext context)
        {
            var data = new List<Pdf>
                {
                    new Pdf { Blob = Encoding.ASCII.GetBytes("BBB"), LinkedKey = 1, LinkedTableType = LinkedTableType.PRODUCT},
                    new Pdf { Blob = Encoding.ASCII.GetBytes("ZZZ"), LinkedKey = 1, LinkedTableType = LinkedTableType.PRODUCT },
                    new Pdf { Blob = Encoding.ASCII.GetBytes("AAA"), LinkedKey = 1, LinkedTableType = LinkedTableType.PRODUCT }
                };
            context.pdfs.AddRange(data);
            context.SaveChanges();

        }
    }
}
