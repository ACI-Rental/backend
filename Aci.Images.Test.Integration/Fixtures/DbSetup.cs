using ACI.Images.Data;
using ACI.Images.Models;
using Bogus;
using System;
using System.Collections.Generic;

namespace ACI.Images.Test.Integration.Fixtures
{
    public class DbSetup
    {
        public const int Images = 5;
        
        public static void InitializeForTests(ImageContext db)
        {
            db.Images.AddRange(GetImages());
            db.SaveChanges();
        }

        public static List<ProductImageBlob> GetImages(int amount = Images)
        {
            return new Faker<ProductImageBlob>()
                .RuleFor(p => p.Id, f => f.Random.Guid())
                .RuleFor(p => p.BlobId, "testString")
                .RuleFor(p => p.ProductId, f => f.Random.Guid())
                .Generate(amount);
        }
    }
}
