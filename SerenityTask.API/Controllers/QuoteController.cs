using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SerenityTask.API.Services;

namespace SerenityTask.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuoteController : ControllerBase
    {
        private readonly IQuoteService _quoteService;

        public QuoteController(IQuoteService quoteService)
        {
            _quoteService = quoteService;
        }

        [Authorize]
        [HttpGet("get-any")]
        public JsonResult GetAny()
        {
            var result = _quoteService.GetRandomQuote();
            return new JsonResult(result);
        }
    }
}
