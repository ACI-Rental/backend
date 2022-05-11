using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ACI.Products.Models.DTO;

namespace ACI.Products.Test.Integration.Fixtures;

public static class HttpClientExt
{
    public static async Task<HttpResponseMessage> PostCreateCategory(this HttpClient client, CreateCategoryRequest request)
    {
        return await client.PostAsJsonAsync("category", request);
    }

    public static async Task<HttpResponseMessage> PostCreateProduct(this HttpClient client, CreateProductRequest request)
    {
        return await client.PostAsJsonAsync("products", request);
    }

    public static async Task<HttpResponseMessage> PostCreateNote(this HttpClient client, CreateNoteRequest request)
    {
        return await client.PostAsJsonAsync("notes", request);
    }

    public static async Task<HttpResponseMessage> GetAllProducts(this HttpClient client)
    {
        return await client.GetAsync("products");
    }

    public static async Task<HttpResponseMessage> GetAllNotes(this HttpClient client, Guid productId)
    {
        return await client.GetAsync($"notes/product/{productId}");
    }

    public static async Task<HttpResponseMessage> GetProductById(this HttpClient client, Guid productId)
    {
        return await client.GetAsync($"products/{productId}");
    }

    public static async Task<HttpResponseMessage> GetNoteById(this HttpClient client, Guid noteId)
    {
        return await client.GetAsync($"notes/{noteId}");
    }

    public static async Task<HttpResponseMessage> ArchiveProduct(this HttpClient client, ProductArchiveRequest request)
    {
        return await client.PutAsJsonAsync("products/archive", request);
    }

    public static async Task<HttpResponseMessage> EditProduct(this HttpClient client, ProductUpdateRequest request)
    {
        return await client.PutAsJsonAsync("products/edit", request);
    }
}