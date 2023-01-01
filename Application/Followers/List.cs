using Application.Core;
using Application.Interfaces;
using Application.Profiles;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Followers
{
    public class List
    {
        public class Query: IRequest<Result<List<Profiles.Profile>>>
        {
            public string UserName { get; set; }
            public string Predicate { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<Profiles.Profile>>>
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
            public async Task<Result<List<Profiles.Profile>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var profiles = new List<Profiles.Profile>();

                switch (request.Predicate)
                {
                    case "followers":
                        profiles = await _dataContext.UserFollowings.Where(x => x.Target.UserName == request.UserName)
                                          .Select(u => u.Observer)
                                          .ProjectTo<Profiles.Profile>(_mapper.ConfigurationProvider, new { currentUserName = _userAccessor.GetUserName()})
                                          .ToListAsync();
                        break;
                    case "followings":
                        profiles = await _dataContext.UserFollowings.Where(x => x.Observer.UserName == request.UserName)
                                          .Select(u => u.Target)
                                          .ProjectTo<Profiles.Profile>(_mapper.ConfigurationProvider, new { currentUserName = _userAccessor.GetUserName() })
                                          .ToListAsync();
                        break;
                    default:
                        break;
                }

                return Result<List<Profiles.Profile>>.Success(profiles);
            }
        }
    }
}
