using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Photos
{
    public class Upload
    {
        public class Command: IRequest<Result<Photo>>
        {
            public IFormFile File { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Photo>>
        {
            private readonly DataContext _dataContext;
            private readonly IPhotoAccessor _photoAccessor;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext dataContext, IPhotoAccessor photoAccessor, IUserAccessor userAccessor)
            {
                _dataContext = dataContext;
                _photoAccessor = photoAccessor;
                _userAccessor = userAccessor;
            }

            public async Task<Result<Photo>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _dataContext.Users.Include(x=>x.Photos).SingleOrDefaultAsync(x => x.UserName == _userAccessor.GetUserName());

                if (user == null) return null;

                var photoResult = await _photoAccessor.UploadPhoto(request.File);

                var photo = new Photo
                {
                    Url = photoResult.Url,
                    Id = photoResult.PublicId
                };
                
                if ( !user.Photos.Any(x=> x.IsMain==true))
                {
                    photo.IsMain = true;
                }

                user.Photos.Add(photo);
                var result = await _dataContext.SaveChangesAsync();

                if (result > 0) return  Result<Photo>.Success(photo);

                return Result<Photo>.Failure("upload fail");
             }
        }
    }


}
