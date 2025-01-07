using Aiges.Core.DTO;
using Aiges.Core.Models;
using Aiges.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiges.Core.Services
{
    public class ReplyService
    {
        private readonly IReplyRepo _replyRepo;
        private readonly UserService _userService;

        public ReplyService(IReplyRepo replyRepo, UserService userService)
        {
            _replyRepo = replyRepo;
            _userService = userService;
        }

        public List<Reply> GetRepliesByProjectId(int projectId)
        {
            List<ReplyDto> replyDtos = _replyRepo.GetRepliesByProjectIdDto(projectId);

            if (replyDtos == null || replyDtos.Count == 0)
            {
                return new List<Reply>(); 
            }

            List<Reply> replies = new List<Reply>();

            foreach (var replyDto in replyDtos)
            {
                Reply reply = new Reply
                {
                    Id = replyDto.Id,
                    UserId = replyDto.UserId,
                    ProjectId = replyDto.ProjectId,
                    EventId = replyDto.EventId,
                    Date = replyDto.Date,
                    Comment = replyDto.Comment
                };

                if (replyDto.UserId.HasValue)
                {
                    reply.User = _userService.GetUserById(replyDto.UserId.Value); 
                }

                replies.Add(reply);
            }

            return replies;
        }
        public int AddComment(Reply replyToAdd)
        {
            if (replyToAdd == null || string.IsNullOrWhiteSpace(replyToAdd.Comment))
            {
                throw new ArgumentException("Reply cannot be null or empty.");
            }

            ReplyDto replyDto = new ReplyDto(replyToAdd);
            int newReplyId = _replyRepo.AddCommentDto(replyDto);
            return newReplyId;
        }
    }
      
}
