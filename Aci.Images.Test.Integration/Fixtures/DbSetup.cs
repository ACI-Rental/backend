using ACI.Images.Data;
using ACI.Images.Models;
using Bogus;
using System;
using System.Collections.Generic;

namespace ACI.Images.Test.Integration.Fixtures
{
    public class DbSetup
    {
        public const int Images = 1;
        
        public static void InitializeForTests(ImageContext db)
        {
            db.Images.AddRange(GetImages());
            db.SaveChanges();
        }

        public static List<ProductImageBlob> GetImages(int amount = Images)
        {
            var guid = new Guid("62FA647C-AD54-4BCC-A860-E5A2664B019D");

            return new Faker<ProductImageBlob>()
                .RuleFor(p => p.Id, f => f.Random.Guid())
                .RuleFor(p => p.BlobId, "testString")
                .RuleFor(p => p.ProductId, guid)    
                .Generate(amount);
        }
    }
}
