using Aiges.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiges.Core.Interfaces
{
    public interface IImageRepo
    {
        public void AddImageDto(ImageDto imageToAdd);

        public List<ImageDto> GetImagesByProjectIdDto(int projectId);
    }
}
