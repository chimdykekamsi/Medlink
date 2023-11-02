using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Medlink.Models;
using System;
using System.Linq;
using Medlink.Data;

namespace Medlink.Controllers
{
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context; // Replace YourDbContext with your actual DbContext

        public CommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [HttpPost]
        public IActionResult AddComment(string post_id, string comment)
        {
           
                CommentModel newComment = new CommentModel
                {
                    post_id = int.Parse(post_id), // Convert post_id to int
                    user_id = UserController.Loginid, // Convert post_id to int
                    comment = comment
                };

                _context.Comments.Add(newComment);
                _context.SaveChanges();

                var post = _context.Posts.FirstOrDefault(p => p.id == newComment.post_id);
                if (post != null)
                {
                    post.comment_count++;
                    _context.SaveChanges();
                }
                

            // Redirect back to the post details page
            return RedirectToAction("Details", "Post", new { id = newComment.post_id });
        }


        public IActionResult GetComments(int postId)
        {
            var comments = _context.Comments.Where(c => c.post_id == postId).OrderByDescending(c => c.created_at).ToList();

            List<string> userComments = new List<string>();

            foreach (var comment in comments)
            {
                UserModel user = _context.Users.FirstOrDefault(u => u.user_id == comment.user_id);

                if (user != null)
                {
                    userComments.Add(user.username);
                }
            }

            ViewBag.UserComments = userComments;

            return PartialView("_CommentListPartial", comments);
        }
    }
}
