using Aiges.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiges.Core.DTO
{
    public class ImageDto
    {
        public int Id { get; set; }

        public int? ProjectId { get; set; }

        public int? EventId { get; set; }

        public string Link { get; set; }

        public ImageDto()
        {

        }

        public ImageDto(int id, int? projectId, int? eventId, string link)
        {
            Id = id;
            ProjectId = projectId;
            EventId = eventId;
            Link = link;
        }

        public ImageDto(Image image)
        {
            Id = image.Id;
            ProjectId = image.ProjectId;
            EventId = image.EventId;
            Link = image.Link;
        }
    }
}
