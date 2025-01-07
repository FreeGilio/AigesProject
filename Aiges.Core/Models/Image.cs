using Aiges.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiges.Core.Models
{
    public class Image
    {
        public int Id { get; set; }

        public int? ProjectId { get; set; }

        public int? EventId { get; set; }

        public string Link { get; set; }

        public Image() 
        { 
        }
        public Image(int id, int? projectId, int? eventId, string link)
        {
            Id = id;
            ProjectId = projectId;
            EventId = eventId;
            Link = link;
        }
        public Image(ImageDto imageDto) 
        {
            Id = imageDto.Id;
            ProjectId = imageDto.ProjectId;
            EventId = imageDto.EventId;
            Link = imageDto.Link;
        }

        public static List<Image> ConvertToImages(List<ImageDto> imageDtos)
        {

            List<Image> images = new List<Image>();

            try
            {
                foreach (ImageDto imageDto in imageDtos)
                {
                    images.Add(new Image(imageDto));
                }
            }
            catch (Exception ex)
            {

            }


            return images ;
        }

    }
}
