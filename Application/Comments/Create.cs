using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comments
{
    public class Create
    {
        public class Command:IRequest<Result<CommentDto>>
        {
            public string Body { get; set; }   
            public Guid ActivityId { get; set; }
        }
        public class CommandValidator: AbstractValidator<CommentDto>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Body).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, Result<CommentDto>>
        {
            private DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _context = context;
                _mapper = mapper;
                _userAccessor = userAccessor;
            }

            public async Task<Result<CommentDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities.FindAsync(request.ActivityId);
                if (activity == null) return null;

                var user = await _context.Users
                    .Include(u => u.Photos)
                    .SingleOrDefaultAsync(x => x.UserName == _userAccessor.GetUserName());

                var comment = new Comment()
                {
                    Body = request.Body,
                    Activity = activity,
                    Author = user
                };

                _context.Comments.Add(comment);
                var result = await _context.SaveChangesAsync() > 0;

                var commentDto = _mapper.Map<Comment, CommentDto>(comment);

                if (result)
                    return Result<CommentDto>.Success(commentDto);
                else
                    return Result<CommentDto>.Failure("Create fail.");

            }
        }
    }
}
