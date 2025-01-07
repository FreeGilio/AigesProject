using Aiges.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiges.Core.DTO
{
    public class ReplyDto
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int? ProjectId { get; set; }

        public int? EventId { get; set; }

        public DateTime Date { get; set; }

        public string Comment { get; set; }

        public User User { get; set; }

        public ReplyDto() { }

        public ReplyDto(Reply reply)
        {
            Id = reply.Id;
            UserId = reply.UserId;
            ProjectId = reply.ProjectId;
            EventId = reply.EventId;
            Date = reply.Date;
            Comment = reply.Comment;       
        }
    }
}
