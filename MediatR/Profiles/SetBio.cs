using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using VAPI.Dto;
using VAPI.Entities;
using VAPI.Interfaces;

namespace VAPI.MediatR.Profiles
{
    public class SetBio
    {
        public class Command : IRequest<Result<Unit>>
        {
            public SetUserBioDto SetUserBioDto { get; set; }
            
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == _userAccessor.GetUserId());

                user.Gender = request.SetUserBioDto.Gender;
                user.DisplayName = request.SetUserBioDto.DisplayName;
                user.Title = request.SetUserBioDto.Title;
                user.Dob = request.SetUserBioDto.Dob;
                user.PhoneNumber = request.SetUserBioDto.PhoneNumber;
                user.Hometown = request.SetUserBioDto.Hometown;

                var result = await _context.SaveChangesAsync() > 0;
                if (result) return Result<Unit>.Success(Unit.Value);

                return Result<Unit>.Failure("Operation failed.");
            }
        }

    }
}