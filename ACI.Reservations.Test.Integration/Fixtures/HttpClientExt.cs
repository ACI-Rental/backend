using ACI.Reservations.Models.DTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ACI.Reservations.Test.Integration.Fixtures
{
    public static class HttpClientExt
    {
        public static async Task<HttpResponseMessage> GetAllReservations(this HttpClient client)
        {
            return await client.GetAsync("Reservations");
        }

        public static async Task<HttpResponseMessage> GetReservationsByStartDate(this HttpClient client, DateTime dateTime)
        {
            return await client.GetAsync($"Reservations/bystartdate/{dateTime.ToString("yyyy-MM-dd")}");
        }

        public static async Task<HttpResponseMessage> GetReservationsByEndDate(this HttpClient client, DateTime dateTime)
        {
            return await client.GetAsync($"Reservations/byenddate/{dateTime.ToString("yyyy-MM-dd")}");
        }
        public static async Task<HttpResponseMessage> GetReservationsByProductId(this HttpClient client, Guid productId)
        {
            return await client.GetAsync($"Reservations/{productId}");
        }

        public static async Task<HttpResponseMessage> ReserveProduct(this HttpClient client, ProductReservationDTO productReservationDTO)
        {
            return await client.PostAsJsonAsync("Reservations/reserveproduct", productReservationDTO);
        }

        public static async Task<HttpResponseMessage> ExecuteReservationAction(this HttpClient client, ReservationActionDTO reservationActionDTO)
        {
            return await client.PostAsJsonAsync("Reservations/action", reservationActionDTO);
        }
    }
}
