using Application.Activities.Dtos;
using Application.Core;
using Application.Interfaces;
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

namespace Application.Activities.Queries
{
    public class Detail
    {
        public class Query : IRequest<Result<ActivityDto>>, ICacheableMediatrQuery
        {
            public Guid Id { get; set; }
            public bool BypassCache { get; set; }
            public string CacheKey => $"Activity-{Id}";
            public TimeSpan? SlidingExpiration { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<ActivityDto>>
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

            async Task<Result<ActivityDto>> IRequestHandler<Query, Result<ActivityDto>>.Handle(Query request, CancellationToken cancellationToken)
            {
                var activity = await _dataContext.Activities.ProjectTo<ActivityDto>(_mapper.ConfigurationProvider, new { currentUserName = _userAccessor.GetUserName() }).FirstOrDefaultAsync(x => x.Id == request.Id);
                return Result<ActivityDto>.Success(activity);
            }
        }
    }
}
