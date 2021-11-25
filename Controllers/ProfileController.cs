using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VAPI.Dto;
using VAPI.Dto.AccountDtos;
using VAPI.Helpers;
using VAPI.MediatR.Profiles;

namespace VAPI.Controllers
{
    //tested
    [Authorize]
    public class ProfileController : BaseApiController
    {
        [HttpPost("photo")]
        public async Task<IActionResult> SetProfilePhoto([FromForm] AddProfilePhotoDto addProfilePhotoDto)
        {
            return HandleResult( await Mediator.Send( new SetProfilePhoto.Command { AddProfilePhotoDto = addProfilePhotoDto }));
        }

        [HttpPost("bio")]
        public async Task<IActionResult> SetBio(SetUserBioDto setUserBioDto)
        {
            return HandleResult( await Mediator.Send( new SetBio.Command { SetUserBioDto = setUserBioDto }));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserProfile(string id)
        {
            return HandleResult( await Mediator.Send( new Get.Query { UserId = id }));
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserParams param)
        {
            return HandlePagedResult(await Mediator.Send( new List.Query { Params = param }));
        }

        [HttpGet("photos/{id}")]
        public async Task<IActionResult> GetPhotos([FromQuery] PaginationParams param, string id)
        {
            return HandlePagedResult(await Mediator.Send(new GetPhotos.Query { Params = param, UserId = id}));
        }

        [HttpGet("appUser/{id}")]
        public async Task<IActionResult> GetAppUser(string id)
        {
            return HandleResult(await Mediator.Send(new GetAppUser.Query { UserId = id}));
        }

        [HttpGet("posts/{id}")]
        public async Task<IActionResult> GetPosts([FromQuery] PaginationParams param, string id)
        {
            return HandlePagedResult(await Mediator.Send(new GetPosts.Query {Params = param, UserId = id}));
        }

    }
}