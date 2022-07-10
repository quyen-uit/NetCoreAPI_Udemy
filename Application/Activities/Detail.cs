using Domain;
using MediatR;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Activities
{
    public class Detail
    {
        public class Query : IRequest<Activity> {
            public Guid Id { get; set; }
        }
        public class Handler : IRequestHandler<Query, Activity>
        {
            private DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }
             async Task<Activity> IRequestHandler<Query, Activity>.Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Activities.FindAsync(request.Id);
            }
        }
    }
}
