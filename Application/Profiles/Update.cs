using Application.Core;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
namespace Application.Profiles
{
    public class Update
    {
        public class Command: IRequest<Result<Unit>>
        {
             public ProfileDto Profile { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Profile).SetValidator(new ProfileDtoValidator());
            }
        }
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                string userName = _userAccessor.GetUserName();
                var user = await _context.Users.Where(u => u.UserName == userName).FirstOrDefaultAsync();
                if (user == null)
                    return Result<Unit>.Failure("user do not exist");

                user.Bio = request.Profile.Bio;
                user.DisplayName = request.Profile.Displayname;

                _context.Attach<AppUser>(user).State = EntityState.Modified; 
                var result = await _context.SaveChangesAsync() > 0;

                if (result)
                {
                    return Result<Unit>.Success(Unit.Value);
                }
                else
                {
                    return Result<Unit>.Failure("update profile error");
                }

            }
        }
    }
}
