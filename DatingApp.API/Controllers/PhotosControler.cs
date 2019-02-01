using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Dto;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosControler : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;
        public PhotosControler(IDatingRepository repo, IMapper mapper,
        IOptions<CloudinarySettings> cloudinaryConfig)
        {
            this._cloudinaryConfig = cloudinaryConfig;
            this._mapper = mapper;
            this._repo = repo;

            Account acc = new Account(
              _cloudinaryConfig.Value.CloudName,
              _cloudinaryConfig.Value.ApiKey,
              _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id){
            var photoFromRepo = await _repo.GetPhoto(id);
            var photo =  _mapper.Map<PhotoRetunDto>(photoFromRepo);

            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddUserPhoto(int userId, [FromForm]ImagePhotoDto photoDto)
        {
            // check if the Id is same with the current user loggedin (id from token0)
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userDataRepo =  await _repo.GetUser(userId);

            var file = photoDto.File;
            var uploadResult = new ImageUploadResult();
            if(file.Length > 0){
                using(var stream = file.OpenReadStream()){
                    var uploadParams = new ImageUploadParams(){
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).
                            Crop("fill").Gravity("face")                        
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            photoDto.Url = uploadResult.Uri.ToString();
            photoDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoDto);

            if (!userDataRepo.Photos.Any(u => u.IsMain))
                photo.IsMain = true;

            userDataRepo.Photos.Add(photo);
            
            if(await _repo.SaveAll()){
                var photoRetunDto = _mapper.Map<PhotoRetunDto>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoRetunDto);
            }

            return BadRequest("Couldn't add photo");
        }
        
        [HttpPost("{id}/setmain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            // check if the Id is same with the current user loggedin (id from token0)
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userDataRepo =  await _repo.GetUser(userId);

            if (!userDataRepo.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoDataRepo = await _repo.GetPhoto(id);

            if (photoDataRepo.IsMain)
                return BadRequest("This is already the main photo");

            var currentMainPhoto = await _repo.GetMainPhotoByUserId(userId);
            currentMainPhoto.IsMain = false;

            photoDataRepo.IsMain = true;

            if(await _repo.SaveAll()){
                return NoContent();
            }

            return BadRequest("Couldn't add photo");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            // check if the Id is same with the current user loggedin (id from token0)
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userDataRepo =  await _repo.GetUser(userId);

            if (!userDataRepo.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoDataRepo = await _repo.GetPhoto(id);

            if (photoDataRepo.IsMain)
                return BadRequest("You can't delete your main photo");

            if(photoDataRepo.PublicId != null){
                var delParams = new DeletionParams(photoDataRepo.PublicId);
                var delResult = _cloudinary.Destroy(delParams);

                if (delResult.Result == "ok") {
                    _repo.Delete(photoDataRepo);
                }
            }

            if(photoDataRepo.PublicId == null){
                _repo.Delete(photoDataRepo);
            }

            if(await _repo.SaveAll())
                return Ok();
            
            return BadRequest("Couldn't delete photo");
        }
    }
}