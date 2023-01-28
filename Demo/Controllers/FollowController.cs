using Application.Followers;
using Application.Profiles.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class FollowController:BaseAPIController
    {

        [HttpPost("{username}")]
        public async Task<ActionResult> Follow(string username)
        {
            return HandleResult(await Mediator.Send(new FollowToggle.Command { TargetUserName = username }));
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<Profile>> GetFollowings(string username, string predicate)
        {
            return HandleResult(await Mediator.Send(new List.Query { Predicate = predicate, UserName = username }));
        }
    }
}
