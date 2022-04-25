﻿using ACI.Images.Models.DTO;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ACI.Images.Test.Integration.Fixtures
{
    public static class HttpClientExt
    {
        public static async Task<HttpResponseMessage> GetImageByProductId(this HttpClient client, Guid productId)
        {
            return await client.GetAsync($"image/{productId}");
        }

        public static async Task<HttpResponseMessage> DeleteImageByProductId(this HttpClient client, Guid productId)
        {
            return await client.DeleteAsync($"image/{productId}");
        }
    }
}