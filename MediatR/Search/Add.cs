using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using VAPI.Entities;
using VAPI.Interfaces;

namespace VAPI.MediatR.Search
{
    public class Add
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string Keyword { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _context;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == _userAccessor.GetUserId());

                var keyword = await _context.SearchOperations
                    .FirstOrDefaultAsync(so => so.AppUserId == user.Id && so.Keyword.ToLower() == request.Keyword.ToLower());

                if (keyword != null)
                {
                    keyword.Date = DateTime.UtcNow;
                }
                else 
                {
                    var searchOperation = new SearchOperation
                    {
                        AppUser = user,
                        AppUserId = user.Id,
                        Keyword = request.Keyword
                    };

                    await _context.SearchOperations.AddAsync(searchOperation);
                }

                var result = await _context.SaveChangesAsync() > 0;

                if (result) return Result<Unit>.Success(Unit.Value);

                return Result<Unit>.Failure("Operation Failed.");
            }
        }
    }
}