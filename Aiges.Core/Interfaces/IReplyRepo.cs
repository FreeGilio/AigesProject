using Aiges.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiges.Core.Interfaces
{
    public interface IReplyRepo
    {
        public List<ReplyDto> GetRepliesByProjectIdDto(int projectId);

        public int AddCommentDto(ReplyDto replyToAdd);
    }
}
