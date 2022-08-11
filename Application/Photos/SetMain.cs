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

namespace Application.Photos
{
    public class SetMain
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _dataContext;

            public Handler(IUserAccessor userAccessor, DataContext dataContext)
            {
                _userAccessor = userAccessor;
                _dataContext = dataContext;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _dataContext.Users.Include(x => x.Photos).SingleOrDefaultAsync(x => x.UserName == _userAccessor.GetUserName());
                if (user == null) return null;

                var mainPhoto = user.Photos.FirstOrDefault(Photos => Photos.IsMain);
                if (mainPhoto == null)
                    return null;

                mainPhoto.IsMain = false;

                var curPhoto = user.Photos.FirstOrDefault(x => x.Id == request.Id);

                if (curPhoto == null)
                    return null;

                curPhoto.IsMain = true;

                var result = await _dataContext.SaveChangesAsync();
                if (result > 0)
                    return Result<Unit>.Success(Unit.Value);
                return Result<Unit>.Failure("Set main fail");
            }
        }
    }
}
