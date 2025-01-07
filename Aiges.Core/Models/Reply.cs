using Aiges.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiges.Core.Models
{
    public class Reply
    {
        public int Id { get; set; }

        public int? UserId {  get; set; }

        public int? ProjectId { get; set; }

        public int? EventId { get; set; }

        public DateTime Date { get; set; }

        public string Comment { get; set; }

        public User User { get; set; } 

        public Reply() { }

        public Reply(ReplyDto replydto) 
        { 
            Id = replydto.Id;
            UserId = replydto.UserId;
            ProjectId = replydto.ProjectId;
            EventId = replydto.EventId;
            Date = replydto.Date;
            Comment = replydto.Comment;
        }

        public static List<Reply> ConvertToReplies(List<ReplyDto> replyDtos)
        {

            List<Reply> replies = new List<Reply>();

            try
            {
                foreach (ReplyDto replyDto in replyDtos)
                {
                    replies.Add(new Reply(replyDto));
                }
            }
            catch (Exception ex)
            {

            }


            return replies;
        }
    }
}
