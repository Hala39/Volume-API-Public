using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VAPI.Helpers;
using VAPI.MediatR.Comments;

namespace VAPI.Controllers
{
    [Authorize]
    public class CommentController : BaseApiController
    {
        [HttpPost("{id}")]
        public async Task<IActionResult> AddComment(int id, string content)
        {
            return HandleResult(await Mediator.Send(new Create.Command { PostId = id, Content = content }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ListComments(int id, [FromQuery] PaginationParams param)
        {
            return HandlePagedResult(await Mediator.Send(new List.Query { PostId = id, Params = param }));
        }

        [Authorize(Policy = "DeleteCommentPolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command { CommentId = id }));
        }
    }
}