using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Activities.Commands
{
    public class UpdateAttendance
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid Id { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities
                    .Include(x => x.ActivityAttendees)
                    .ThenInclude(x => x.AppUser)
                    .SingleOrDefaultAsync(x => x.Id == request.Id);

                if (activity == null) return null;

                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUserName());

                if (user == null) return null;

                var hostUserName = activity.ActivityAttendees.FirstOrDefault(x => x.IsHost).AppUser.UserName;
                var attendance = activity.ActivityAttendees.FirstOrDefault(x => x.AppUser.UserName == user.UserName);

                if (attendance != null && hostUserName == user.UserName)
                {
                    activity.IsCancelled = !activity.IsCancelled;
                }

                if (hostUserName != user.UserName && attendance != null)
                {
                    activity.ActivityAttendees.Remove(attendance);
                }

                if (attendance == null)
                {
                    _context.ActivityAttendees
                      .Add(new Domain.ActivityAttendee
                      {
                          AppUser = user,
                          Activity = activity,
                          IsHost = true
                      });
                }
                var result = await _context.SaveChangesAsync();
                return result > 0 ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("update attendance fail");
            }
        }

    }
}
