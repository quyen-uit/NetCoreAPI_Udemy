using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Security
{
    public class IsHostRequirement : IAuthorizationRequirement
    {
    }
    public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement>
    {
        private DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IsHostRequirementHandler(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Task.CompletedTask;

            var activityId = Guid.Parse(_httpContextAccessor.HttpContext?.Request.RouteValues.FirstOrDefault(x => x.Key == "id").Value?.ToString());

            var attendee = _context.ActivityAttendees.AsNoTracking().SingleOrDefaultAsync(x => x.ActivityId == activityId && x.AppUserId == userId).Result;

            if (attendee == null) return Task.CompletedTask;

            if (attendee.IsHost)
                context.Succeed(requirement);

            return Task.CompletedTask;

        }
    }
}
