using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VAPI.MediatR.Following;

namespace VAPI.Controllers
{
    //tested
    [Authorize]
    public class FollowController : BaseApiController
    {
        [HttpPost("{id}")]
        public async Task<IActionResult> FollowToggle(string id)
        {
            return HandleResult(await Mediator.Send( new FollowToggle.Command { TargetId = id }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFollowings(string id, string predicate)
        {
            return HandleResult(await Mediator.Send( new List.Query { UserId = id, Predicate = predicate }));
        }
    }
}