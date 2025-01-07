using Aiges.Core.Models;
using Aiges.Core.Services;
using Aiges.MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace Aiges.MVC.Controllers
{
    public class CommentController : Controller
    {
        private readonly ProjectService projectService;
        private readonly UserService userService;
        private readonly ReplyService replyService;
        public CommentController(ProjectService projectService, UserService userService, ReplyService replyService)
        {
            this.projectService = projectService;
            this.userService = userService;
            this.replyService = replyService;
        }

        [HttpPost]
        public IActionResult AddComment(AddCommentViewModel newComment)
        {
            int? loggedInUserId = HttpContext.Session.GetInt32("uId");

            if (!loggedInUserId.HasValue)
            {
                return Unauthorized();
            }

            if (newComment.ProjectId == null && newComment.EventId == null)
            {
                ModelState.AddModelError(string.Empty, "You must specify a ProjectId or EventId.");
                return RedirectToAction("Index", "Project"); 
            }

            if (ModelState.IsValid)
            {
                if (newComment.ProjectId != null)
                {
                    replyService.AddComment(new Reply
                    {
                        UserId = loggedInUserId.Value,
                        ProjectId = newComment.ProjectId.Value, 
                        EventId = null, 
                        Date = DateTime.UtcNow,
                        Comment = newComment.Comment
                    });

                    return RedirectToAction("ProjectDetails", "Project", new { id = newComment.ProjectId });
                }
                else if (newComment.EventId != null)
                {
                    replyService.AddComment(new Reply
                    {
                        UserId = loggedInUserId.Value,
                        ProjectId = null, 
                        EventId = newComment.EventId.Value, 
                        Date = DateTime.UtcNow,
                        Comment = newComment.Comment
                    });

                    return RedirectToAction("EventDetails", "Event", new { id = newComment.EventId });
                }
            }


            return RedirectToAction("Index", "Project");

        }
    }
}
