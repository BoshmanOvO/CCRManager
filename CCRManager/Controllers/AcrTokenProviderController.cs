using CCRManager.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CCRManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcrTokenProviderController : ControllerBase
    {
        private readonly IAcrTokenProvider _acrAuthService;

        public AcrTokenProviderController(IAcrTokenProvider acrAuthService)
        {
            _acrAuthService = acrAuthService;
        }

        [HttpGet("get-token")]
        public async Task<IActionResult> GetAcrToken()
        {
            try
            {
                string token = await _acrAuthService.GetAcrAccessTokenAsync();
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
