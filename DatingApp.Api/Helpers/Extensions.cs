using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DatingApp.Api.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message) {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public static void AddPagination(this HttpResponse response, 
            int currentPage, int itemsPerPage, int totalItems, int totalPages) {

            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages); 
            // Since Angular uses camelCase, we need to pass our Header in camel case
            var camelCaseFormatter = new JsonSerializerSettings();
            camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            response.Headers.Add("Pagination", 
                JsonConvert.SerializeObject(paginationHeader, camelCaseFormatter));
            // We're also going to need to expose the headers
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
        //We use 'this' because we are creating an extension.
        public static int CalculateAge(this DateTime theDateTime) {
            var age = DateTime.Today.Year - theDateTime.Year;

            if (theDateTime.AddYears(age) > DateTime.Today)
                age--;

            return age;
        }
    }
}