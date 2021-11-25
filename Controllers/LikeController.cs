using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VAPI.Helpers;
using VAPI.MediatR.Likes;

namespace VAPI.Controllers
{
    [Authorize]
    public class LikeController : BaseApiController
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> List(int id, [FromQuery] PaginationParams param)
        {
            return HandlePagedResult(await Mediator.Send(new List.Query { PostId = id, Params = param }));
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> LikeToggle(int id)
        {
            return HandleResult(await Mediator.Send(new LikeToggle.Command { PostId = id}));
        }
    }
}