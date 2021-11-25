using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using VAPI.Dto;
using VAPI.Dto.AccountDtos;
using VAPI.Entities;
using VAPI.Interfaces;

namespace VAPI.MediatR.Profiles
{
    public class SetProfilePhoto
    {
        public class Command : IRequest<Result<MiniFile>>
        {
            public AddProfilePhotoDto AddProfilePhotoDto { get; set; }
            
        }

        public class Handler : IRequestHandler<Command, Result<MiniFile>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            private readonly IFileService _fileService;
            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor, IFileService fileService)
            {
                _fileService = fileService;
                _userAccessor = userAccessor;
                _mapper = mapper;
                _context = context;
            }

            public async Task<Result<MiniFile>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == _userAccessor.GetUserId());

                var file = request.AddProfilePhotoDto.File;
                var url = request.AddProfilePhotoDto.Url;

                if (file != null && url == null)
                {
                    var fileUploadResult = _fileService.AddFile(file).Result;

                    var newFile = new File
                    {
                        Url = fileUploadResult.SecureUrl.AbsoluteUri,
                        Id = fileUploadResult.PublicId,
                        AppUser = user
                    };

                    await _context.Files.AddAsync(newFile);
                    user.ProfilePhotoUrl = newFile.Url;
                }

                else if (file == null && url != null) 
                {
                    user.ProfilePhotoUrl = url;
                }

                var endUrl = new MiniFile
                {
                    Url = user.ProfilePhotoUrl
                };

                var success = await _context.SaveChangesAsync() > 0;

                if (success) return Result<MiniFile>.Success(endUrl);

                return Result<MiniFile>.Failure("Operation failed.");

            }

        }
    }
}