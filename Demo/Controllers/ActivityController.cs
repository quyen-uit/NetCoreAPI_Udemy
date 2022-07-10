using Application.Activities;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Persistence;

namespace Demo.Controllers
{
    public class ActivityController : BaseAPIController
    {

        [HttpGet]
        public async Task<ActionResult<List<Activity>>> GetActivities()
        {
            return await Mediator.Send(new List.Query());
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> GetActivity(Guid id)
        {
            return await Mediator.Send(new Detail.Query() { Id = id });
        }
        [HttpPost]
        public async Task<IActionResult> AddActivity(Activity activity)
        {
            return Ok(await Mediator.Send(new Create.Command() { Activity = activity }));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateActivity(Guid id,Activity activity)
        {
            activity.Id = id;
            return Ok(await Mediator.Send(new Edit.Command() { Activity = activity }));
        }
    }
}
