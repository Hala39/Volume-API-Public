using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VAPI.Dto.NotificationsDto;
using VAPI.Helpers;
using VAPI.MediatR.Notifications;

namespace VAPI.Controllers
{
    [Authorize]
    public class NotificationController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PaginationParams param)
        {
            return HandlePagedResult(await Mediator.Send(new List.Query { Params = param}));
        }

        [HttpPost]
        public async Task<IActionResult> Add(CreateNotificationDto createNotificationDto)
        {
            return HandleResult(await Mediator.Send(new Create.Command { CreateNotificationDto = createNotificationDto }));
        }

        [HttpPost("read")]
        public async Task<IActionResult> Read()
        {
            return HandleResult(await Mediator.Send(new Read.Command()));
        }

        [HttpPost("read/one")]
        public async Task<IActionResult> ReadOne([FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new ReadOne.Command { Id = id}));
        }

        [HttpDelete]
        public async Task<IActionResult> ClearAll(string predicate)
        {
            return HandleResult(await Mediator.Send(new ClearAll.Command { Predicate = predicate }));
        }

        [HttpGet("activities")]
        public async Task<IActionResult> GetActivities([FromQuery] PaginationParams param)
        {
            return HandlePagedResult(await Mediator.Send(new ListActivities.Query { Params = param }));
        }
    }
}