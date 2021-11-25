using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VAPI.Helpers;
using VAPI.MediatR.Search;

namespace VAPI.Controllers
{
    [Authorize]
    public class SearchController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetRecentSearches([FromQuery] PaginationParams param)
        {
            return HandlePagedResult(await Mediator.Send(new GetSearchKeywords.Query { Params = param }));
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromQuery] string keyword) 
        {
            return HandleResult(await Mediator.Send (new Add.Command { Keyword = keyword}));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return HandleResult(await Mediator.Send(new Remove.Command { Id = id }));
        }

        [HttpDelete]
        public async Task<IActionResult> ClearAll()
        {
            return HandleResult(await Mediator.Send(new Clear.Command()));
        }
    }
}