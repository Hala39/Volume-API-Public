using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VAPI.Dto.MessageDtos;
using VAPI.Helpers;
using VAPI.MediatR.Messages;

namespace VAPI.Controllers
{
    [Authorize]
    public class MessageController : BaseApiController
    {
        //tested
        // [HttpPost]
        // public async Task<IActionResult> Create([FromForm] CreateMessageDto createMessageDto)
        // {
        //     return HandleResult(await Mediator.Send(new Create.Command {CreateMessageDto = createMessageDto}));
        // }

        //tested
        [HttpGet("{id}")]
        public async Task<IActionResult> Thread(string id, [FromQuery] PaginationParams param)
        {
            return HandlePagedResult(await Mediator.Send(new Thread.Query { ContactId = id, Params = param }));
        }

        //tested
        [Authorize(Policy = "DeleteMessagePolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }

        [HttpGet("contact")]
        public async Task<IActionResult> Get([FromQuery] UserParams param)
        {
            return HandlePagedResult(await Mediator.Send(new Contact.Query { Params = param }));
        }

        [HttpPost("read/{guid}")]
        public async Task<IActionResult> ReadOne(Guid guid)
        {
            return HandleResult(await Mediator.Send(new ReadOne.Command { Guid = guid }));
        }

        [HttpDelete("clear/{id}")]
        public async Task<IActionResult> ClearChat(string id)
        {
            return HandleResult(await Mediator.Send(new ClearAll.Command { ContactId = id }));
        }
    }
}