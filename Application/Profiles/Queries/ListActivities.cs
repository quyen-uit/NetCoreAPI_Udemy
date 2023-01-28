using Application.Core;
using Application.Enums;
using Application.Interfaces;
using Application.Profiles.Dtos;
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

namespace Application.Profiles.Queries
{
    public class ListActivities
    {
        public class Query : IRequest<Result<List<UserActivityDto>>>
        {
            public UserActivityPredicates Predicate { get; set; }
            public string UserName { get; set; }

        }

        public class Handler : IRequestHandler<Query, Result<List<UserActivityDto>>>
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

            public async Task<Result<List<UserActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _dataContext.ActivityAttendees.Include(a => a.Activity)
                    .Where(a => a.AppUser.UserName == request.UserName)
                    .ProjectTo<UserActivityDto>(_mapper.ConfigurationProvider).AsQueryable();


                if (request.Predicate == UserActivityPredicates.InPast)
                {
                    query = query.Where(x => x.Date < DateTime.UtcNow);
                }
                else if (request.Predicate == UserActivityPredicates.InFuture)
                {
                    query = query.Where(x => x.Date >= DateTime.UtcNow);
                }
                else
                {
                    query = query.Where(x => x.HostUserName == request.UserName);
                }
                var activities = await query.ToListAsync();
                return Result<List<UserActivityDto>>.Success(activities);
            }
        }

    }
}