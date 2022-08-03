using Application.Activities;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    public class ActivityController : BaseAPIController
    {
        [AllowAnonymous]

        [HttpGet]
        public async Task<ActionResult<List<Activity>>> GetActivities()
        {
            var result = await Mediator.Send(new List.Query());
            return HandleResult(result);
        }
        [Authorize(Policy = "IsActivityHost")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> GetActivity(Guid id)
        {
            var result = await Mediator.Send(new Detail.Query() { Id = id });
            return HandleResult(result);

        }
        [HttpPost]
        public async Task<IActionResult> AddActivity(Activity activity)
        {
            return HandleResult(await Mediator.Send(new Create.Command() { Activity = activity }));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateActivity(Guid id, Activity activity)
        {
            activity.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command() { Activity = activity }));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveActivity(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command() { Id = id }));
        }
        [Authorize(Policy = "IsActivityHost")]
        [HttpPost("{id}/attend")]
        public async Task<IActionResult> AttendActivity(Guid id)
        {
            return HandleResult(await Mediator.Send(new UpdateAttendance.Command() { Id = id }));
        }
    }
}
