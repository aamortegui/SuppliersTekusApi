using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tekus.Suppliers.WebApi.Domain.Common;
using Tekus.Suppliers.WebApi.Domain.Entities;
using Tekus.Suppliers.WebApi.Domain.Interfaces;
using static Tekus.Suppliers.WebApi.Domain.Common.StaticDetails;

namespace Tekus.Suppliers.WebApi.Infrastructure.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ServiceSuppliersDBContext _context;

        public BaseService(IHttpClientFactory httpClientFactory, ServiceSuppliersDBContext context)
        {
            _httpClientFactory = httpClientFactory;
            _context = context;
        }
        public async Task<ResponseCountry?> GetCountriesAsync(Request requestApi)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("CountryAPI");
                HttpRequestMessage message = new();
                message.Headers.Add("Accept", "application/json");

                message.RequestUri = new Uri(requestApi.Url);
                if (requestApi.Data is not null)
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



                        return new ResponseCountry()
                        {
                            IsSuccess = true,
                            Countries = apiCountryDto
                        };
                }
            }
            catch (Exception ex)
            {
                var response = new ResponseCountry()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
                return response;
            }
        }
        public async Task<bool> SyncCountriesToDatabaseAsync()
        {
            var response = await GetCountriesAsync(new Request()
            {
                ApiType = ApiType.GET,
                Url = StaticDetails.CountryAPIBase + "/all"
            });

            if (!response.IsSuccess || response.Countries == null)
                return false;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try { 
            foreach (var country in response.Countries)
            {
                var existingCountry = await _context.Countries
                    .FirstOrDefaultAsync(c => c.CommonName.ToUpper() == country.Name.Common.ToUpper());

                if (existingCountry is null)
                {
                    _context.Countries.Add(new Persistence.Entities.Country()
                    {
                        CommonName = country.Name.Common,
                        OfficialName = country.Name.Official
                    });
                }

                else if (existingCountry.CommonName != country.Name.Common ||
                    existingCountry.OfficialName != country.Name.Official)
                {
                    existingCountry.CommonName = country.Name.Common;
                    existingCountry.OfficialName = country.Name.Official;
                    _context.Countries.Update(existingCountry);
                }

            }
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
