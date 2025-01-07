using Aiges.Core.DTO;
using Aiges.Core.Interfaces;
using Aiges.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiges.Core.Services
{
    public class ImageService
    {
        private readonly IImageRepo _imageRepo;

        public ImageService(IImageRepo imageRepo)
        {
            _imageRepo = imageRepo;
        }
        public void AddImage(Image image)
        {
            ImageDto imageDto = new ImageDto(image);
            _imageRepo.AddImageDto(imageDto);
        }

        public List<Image> GetImagesByProjectId(int projectId)
        {
            List<ImageDto> images = _imageRepo.GetImagesByProjectIdDto(projectId);

            if (images == null)
            {
                return new List<Image>();
            }

            return images.Select(img => new Image(img)).ToList(); 
        }
    }
}
