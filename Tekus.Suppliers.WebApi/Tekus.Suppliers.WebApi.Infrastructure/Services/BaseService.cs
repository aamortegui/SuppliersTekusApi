using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tekus.Suppliers.WebApi.Domain.Entities;
using Tekus.Suppliers.WebApi.Domain.Interfaces;
using static Tekus.Suppliers.WebApi.Domain.Common.StaticDetails;

namespace Tekus.Suppliers.WebApi.Infrastructure.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BaseService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<Response?> GetAllCountriesAsync(Request requestApi)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("CountryAPI");
                HttpRequestMessage message = new();
                message.Headers.Add("Accept", "application/json");

                message.RequestUri = new Uri(requestApi.Url);
                if(requestApi.Data is not null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestApi.Data),
                        Encoding.UTF8, "application/json");
                }
                HttpResponseMessage? apiResponse = null;

                switch (requestApi.ApiType)
                {
                    case ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    case ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }
                apiResponse = await client.SendAsync(message);

                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new() { IsSuccess = false, Message = "Not Found" };
                    case HttpStatusCode.Forbidden:
                        return new() { IsSuccess = false, Message = "Access Denied" };
                    case HttpStatusCode.Unauthorized:
                        return new() { IsSuccess = false, Message = "Unauthorized" };
                    case HttpStatusCode.InternalServerError:
                        return new() { IsSuccess = false, Message = "Internal Server Error" };
                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var apiCountryDto = JsonConvert.DeserializeObject<List<Country>>(apiContent);
                        
                        return new Response()
                        {
                            IsSuccess = true,
                            Countries = apiCountryDto
                        };
                }
            }
            catch(Exception ex) 
            {
                var response = new Response()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
                return response;
            }
        }
    }
}
