using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using VAPI.Dto;
using VAPI.Entities;
using VAPI.Interfaces;

namespace VAPI.MediatR.Posts
{
    public class Create
    {
        public class Command : IRequest<Result<Unit>>
        {
            public CreatePostDto CreatePostDto { get; set; }

        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
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

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == _userAccessor.GetUserId());

                var post = new Post();

                if (user == null) return Result<Unit>.Failure("Unauthorized");

                post.AppUser = user;

                if (request.CreatePostDto.File != null) {

                    var fileUploadResult = await _fileService.AddFile(request.CreatePostDto.File);

                    var file = new File
                    {
                        Url = fileUploadResult.SecureUrl.AbsoluteUri,
                        Id = fileUploadResult.PublicId,
                        AppUser = user
                    };

                    post.File = file;
                    
                }

                
                if (request.CreatePostDto.Description != null) {
                    post.Description = request.CreatePostDto.Description;
                }

                await _context.Posts.AddAsync(post);
                
                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to create post");

                return Result<Unit>.Success(Unit.Value);

            }
        }
    }
}