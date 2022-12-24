using Application.Profiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
namespace API.Controllers
{
    [Authorize]
    public class ProfileController : BaseAPIController
    {
        [HttpGet("{username}")]
        public async Task<IActionResult> GetProfile(string username)
        {
            return HandleResult(await Mediator.Send(new Detail.Query { UserName = username }));
        }
        [HttpPut]
        public async Task<IActionResult> UpdateProfile(ProfileDto profile)
        {
            return HandleResult(await Mediator.Send(new Update.Command {Profile = profile }));
        }

        [HttpGet("{username}/activities")]
        public async Task<IActionResult> GetActivities(string username, UserActivityPredicates predicate)
        {
            return HandleResult(await Mediator.Send(new ListActivities.Query { UserName = username, Predicate = predicate }));
        }
    }
}
