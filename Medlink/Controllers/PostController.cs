using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Medlink.Data;
using Medlink.Models;
using Microsoft.AspNetCore.Hosting;

namespace Medlink.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PostController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Post
        public async Task<IActionResult> Index()
        {
              return _context.Posts != null ? 
                          View(await _context.Posts.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Posts'  is null.");
        }

        // PostController.cs
        public async Task<IActionResult> Details(int? id)
        {
            if (UserController.Loginid == null)
            {
                return Redirect("../../User/Login");
            }

            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var postModel = await _context.Posts
                .FirstOrDefaultAsync(m => m.id == id);

            UserModel user = _context.Users.FirstOrDefault(u => u.user_id == postModel.user_id);

            if (postModel == null)
            {
                return NotFound();
            }

            ViewData["Username"] = user.username;

            // Retrieve comments for the post
            var comments = _context.Comments.Where(c => c.post_id == postModel.id).ToList();

            var viewModel = new PostDetailsViewModel
            {
                Post = postModel,
                Comments = comments
            };

            return View(viewModel);
        }


        // GET: Post/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostModel postModel, IFormFile image)
        {
                postModel.user_id = UserController.Loginid;
            if (ModelState.IsValid)
            {
                string imagePath = SaveProfileImage(image);

                // Update the PostModel with the image path
                postModel.image = imagePath;

                _context.Add(postModel);
                await _context.SaveChangesAsync();
                TempData["success"] = "You started a discussion";
                return Redirect($"../Post/Details/{postModel.id}");
            }
            return View(postModel);
        }


        // GET: Post/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Posts == null || UserController.Loginid == null)
            {
                TempData["error"] = "Bad Request";
                return Redirect("../../Home/Index");
            }

            var postModel = await _context.Posts.FindAsync(id);
            if (postModel == null)
            {
                return NotFound();
            }
            return View(postModel);
        }

        // POST: Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PostModel postModel, IFormFile image = null)
        {
            if (id != postModel.id || postModel.user_id != UserController.Loginid)
            {
                TempData["error"] = "Bad request";
                return Redirect("../../User/");
            }

            //if (ModelState.IsValid)
            //{
                try
                {
                  if(image != null && image.Length > 0)
                    {
                        string imagePath = SaveProfileImage(image);

                        // Update the PostModel with the image path
                        postModel.image = imagePath;
                    }
                    else
                    {
                        postModel.image = _context.Posts.AsNoTracking().FirstOrDefault(u => u.id == postModel.id)?.image;
                    }

                    _context.Update(postModel);
                    await _context.SaveChangesAsync();

                    TempData["success"] = "post has been updated";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostModelExists(postModel.id))
                    {
                        TempData["error"] = "An error Occured reload";
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if(UserController.Loginid == "admin")
                {
                    return Redirect("../../Admin/");
                }
                else
                {
                    return Redirect("../../User/");
                }
            //}
            return View(postModel);
        }

        // GET: Post/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var postModel = await _context.Posts
                .FirstOrDefaultAsync(m => m.id == id);
            UserModel user = _context.Users.FirstOrDefault(u => u.user_id == postModel.user_id);
            if (postModel == null)
            {
                return NotFound();
            }
            ViewData["Username"] = user.username;
            return View(postModel);
        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Posts'  is null.");
            }
            var postModel = await _context.Posts.FindAsync(id);
            if (postModel != null)
            {
                _context.Posts.Remove(postModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostModelExists(int id)
        {
          return (_context.Posts?.Any(e => e.id == id)).GetValueOrDefault();
        }

        private string SaveProfileImage(IFormFile profileImage)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + profileImage.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                profileImage.CopyTo(stream);
            }

            return uniqueFileName;
        }
    }
}
