using Application.Core;
using Application.Interfaces;
using Application.Profiles.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Profiles.Queries
{
    public class Detail
    {
        public class Query : IRequest<Result<Dtos.Profile>>
        {
            public string UserName { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<Dtos.Profile>>
        {
            private readonly DataContext _dataContext;
            private readonly IUserAccessor _userAccessor;
            private readonly IMapper _mapper;
            public Handler(DataContext dataContext, IUserAccessor userAccessor, IMapper mapper)
            {
                _dataContext = dataContext;
                _userAccessor = userAccessor;
                _mapper = mapper;
            }

            public async Task<Result<Dtos.Profile>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _dataContext.Users
                    .ProjectTo<Dtos.Profile>(_mapper.ConfigurationProvider, new { currentUserName = _userAccessor.GetUserName() })
                    .SingleOrDefaultAsync(x => x.UserName == request.UserName);

                if (user == null) return null;

                return Result<Dtos.Profile>.Success(user);

            }
        }
    }
}
