using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Tekus.Suppliers.WebApi.Application.Services.Interfaces;

namespace Tekus.Suppliers.WebApi.Controllers
{
    /// <summary>
    /// Controller for generating reports
    /// </summary>
    [Route("api/report")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "isadmin")]
    public class ReportController : ControllerBase
    {
        private readonly IIndicatorService _indicatorService;
        private readonly IOutputCacheStore _outputCacheStore;
        private const string cacheTag = "summary";
        public ReportController(IIndicatorService indicatorService, IOutputCacheStore outputCacheStore)
        {
            _indicatorService = indicatorService;
            _outputCacheStore = outputCacheStore;
        }
        /// <summary>
        /// Generate a report of the country summary
        /// </summary>
        /// <returns></returns>
        [HttpGet("generate-report")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<IActionResult> GenerateReport()
        {
            var summary = await _indicatorService.GetCountrySummaryAsync();
            return Ok(summary);
        }
    }
}
