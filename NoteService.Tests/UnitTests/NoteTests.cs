using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteService.Controllers;
using NoteService.DBContexts;
using NoteService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NoteService.Tests.UnitTests
{
    public class NoteTests
    {
        [Fact]
        public async Task GetNotes_WhenCalled_ReturnListOfNotes()
        {
            var options = new DbContextOptionsBuilder<NoteServiceDatabaseContext>().UseInMemoryDatabase(databaseName: "InMemoryNoteDb").Options;

            var context = new NoteServiceDatabaseContext(options);
            SeedNoteInMemoryDatabaseWithData(context);
            var controller = new NoteController(context);
            var result = await controller.GetNotes();

            var objectresult = Assert.IsType<OkObjectResult>(result.Result);
            var notes = Assert.IsAssignableFrom<IEnumerable<Note>>(objectresult.Value);

            Assert.Equal(3, notes.Count());
            Assert.Equal("BBB", notes.ElementAt(0).Content);
            Assert.Equal("YYY", notes.ElementAt(1).Content);
            Assert.Equal("ZZZ", notes.ElementAt(2).Content);
        }

        private static void SeedNoteInMemoryDatabaseWithData(NoteServiceDatabaseContext context)
        {
            var data = new List<Note>
                {
                    new Note { Content = "BBB" },
                    new Note { Content = "YYY" },
                    new Note { Content = "ZZZ" }
                };
            context.Notes.AddRange(data);
            context.SaveChanges();

        }
    }
}
