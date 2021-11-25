using System.Collections.Generic;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace VAPI.Interfaces
{
    public interface IFileService
    {
         Task<ImageUploadResult> AddFile(IFormFile file);
         Task<DeletionResult> DeleteFile(string FileId);
    }
}