using Medlink.Data;
using Medlink.Models;
using Microsoft.AspNetCore.Mvc;

namespace Medlink.Controllers
{
    public class LikeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public LikeController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public IActionResult Like(int postId)
        {
            // Retrieve the post from the database
            var post = _db.Posts.FirstOrDefault(p => p.id == postId);
            if(UserController.Loginid == null)
            {
                TempData["error"] = "This feature is restricted to authorized users";
                return Redirect("Home/Login");
            }

            if (post != null)
            {
                // Increment the like count
                post.like_count++;

                LikeModel obj = new LikeModel
                {
                    post_id = postId,
                    user_id = UserController.Loginid
                };
                _db.Likes.Add(obj);
                // Update the database
                _db.SaveChanges();



                // You can return a JSON response if using AJAX
                return Json(new { success = true, likeCount = post.like_count });
            }

            // Handle the case where the post is not found
            return Json(new { success = false, error = "Post not found" });
        }
        [HttpGet]
        public IActionResult HasUserLikedPost(string userId, int postId)
        {
            // Check if the user with the specified user_id has liked the post with the given post_id
            bool userLikedPost = _db.Likes.Any(l => l.user_id == UserController.Loginid && l.post_id == postId);

            return Json(new { userLikedPost });
        }
    }
}
