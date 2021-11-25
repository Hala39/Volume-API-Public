using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VAPI.Dto;
using VAPI.Helpers;
using VAPI.MediatR.Posts;

namespace VAPI.Controllers
{
    [Authorize]
    public class PostController : BaseApiController
    {
        //tested
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreatePostDto createPostDto)
        {
            return HandleResult(await Mediator.Send(new Create.Command{CreatePostDto = createPostDto}));
        }

        //tested
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PaginationParams param)
        {
            return HandlePagedResult(await Mediator.Send(new List.Query {Params = param}));
        }

        //tested
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            return HandleResult(await Mediator.Send(new Details.Query{PostId = id}));
        }

        //tested
        [Authorize(Policy = "CommandPolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command{PostId = id}));
        }

        //tested
        [Authorize(Policy = "CommandPolicy")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, string description)
        {
            return HandleResult(await Mediator.Send(new Update.Command{ PostId = id, Description = description }));
        }

        [HttpPost("save/{id}")]
        public async Task<IActionResult> SavePostToggle(int id)
        {
            return HandleResult(await Mediator.Send(new SavePostToggle.Command { PostId = id}));
        }

        [HttpGet("saved")]
        public async Task<IActionResult> GetSavedPosts([FromQuery] PaginationParams param)
        {
            return HandlePagedResult(await Mediator.Send(new GetSavedPosts.Query { Params = param}));
        }
    }
}