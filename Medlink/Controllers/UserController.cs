using Medlink.Data;
using Medlink.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Medlink.Controllers
{
    public class UserController : Controller
    {
        public static string? Loginid { get; set; }
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<UserController> _logger;

        public UserController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment, ILogger<UserController> logger)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (UserController.Loginid == null)
            {
                return RedirectToAction("Login");
            }
            else if (UserController.Loginid == "Admin123")
            {
                return RedirectToAction("List");
            }
            else
            {
                UserModel user = _db.Users.FirstOrDefault(u => u.user_id == UserController.Loginid);
                IEnumerable<PostModel> posts = _db.Posts.Where(p => p.user_id == UserController.Loginid).ToList();
                return View(Tuple.Create(user, posts));
            }
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(UserModel obj,string repassword, IFormFile profile)
        {
            Guid uniqueId = Guid.NewGuid();

            string uniqueIdString = uniqueId.ToString("N").Substring(0, 10);
            obj.user_id = uniqueIdString;
            if (obj.password != repassword)
            {
                // Add model error for password mismatch
                ModelState.AddModelError("repassword", "Password and confirmation password do not match");
                return View(obj);
            }

            if (profile != null && profile.Length > 0)
            {
                // Save the profile image to a folder or a storage service
                string imagePath = SaveProfileImage(profile);

                // Update the UserModel with the image path
                obj.profile = imagePath;
            }

            if (ModelState.IsValid)
            {
               
                _db.Users.Add(obj);
                _db.SaveChanges();
                TempData["success"] = "Registration Successfull Please login";

                return RedirectToAction("Login");
            }
            return View(obj);
        }


        [HttpGet("Doctor/{id}")]
        public IActionResult Doctor(string id)
        {
            // Retrieve the user with the specified id from the database
            if (id != null)
            {
                UserModel user = _db.Users.FirstOrDefault(u => u.user_id == id);
                IEnumerable<PostModel> posts = _db.Posts.Where(p => p.user_id == id).ToList();
                
                if (id == UserController.Loginid)
                {
                    return RedirectToAction("Index");
                }
                return user != null ? View(Tuple.Create(user, posts)) : RedirectToAction("List");
            }
            return RedirectToAction("List");
        }

        [HttpGet("List/{id?}")]
        public IActionResult List(string? id)
        {
            if (id!= null)
            {
                // Redirect to the Details action when an id is provided
                return RedirectToAction("Doctor", new { id = id });
            }

            // Retrieve and display all users
            IEnumerable<UserModel> userList = _db.Users;
            return View(userList);
        }


        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email,string password)
        {
            if(password == "Admin123")
            {
                TempData["success"] = "Welcome Admin";
                UserController.Loginid = "Admin123";
                return RedirectToAction("List");
            }
            else
            {
                UserModel user = _db.Users.FirstOrDefault(u => u.email == email);
                if (user != null)
                {
                    if (user.password == password)
                    {
                        UserController.Loginid = user.user_id;
                        TempData["success"] = "login Successful";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["warning"] = "login failed the password is incorrect";
                    }
                }
                else
                {
                    TempData["warning"] = "login failed email matched no row in d database";
                }
            }
            return View();
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

        public IActionResult Setting()
        {
            if (UserController.Loginid == null)
            {
                return RedirectToAction("Login");
            }else if (UserController.Loginid == "Admin123")
            {
                return RedirectToAction("List");
            }
            else
            {
                UserModel user = _db.Users.FirstOrDefault(u => u.user_id == UserController.Loginid);
                return View(user);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Setting(UserModel obj,string boolyValue, IFormFile? profile = null)
        {
            try
            {
                if (profile != null && profile.Length > 0)
                {
                    // Save the new profile image to a folder or storage service
                    string newImagePath = SaveProfileImage(profile);

                    // Update the UserModel with the new image path
                    obj.profile = newImagePath;
                }
                else
                {
                    // No new profile image provided, maintain the existing one
                    obj.profile = _db.Users.AsNoTracking().FirstOrDefault(u => u.user_id == obj.user_id)?.profile;
                }
               
                    obj.visibility = boolyValue;

                _db.Users.Update(obj);
                _db.SaveChanges();
                TempData["success"] = "Profile changes saved successfully";

                return View(obj);
            }
            catch (Exception ex)
            {
                LogException(ex);
                TempData["warning"] =  "An error occurred while processing your request.";
            }

            return View(obj);
        }



        private void LogException(Exception ex)
        {
            _logger.LogError($"Exception: {ex.Message}", ex);
        }

        public IActionResult Logout()
        {
            UserController.Loginid = null;
            return Redirect("../Home/");
        }

        public  string GetUserUsername(string userId)
        {
            // Assuming you have a method to retrieve a user by ID
            UserModel? user = _db.Users.FirstOrDefault(u => u.user_id == userId);

            // Check if the user exists
            if (user != null)
            {
                // Return the username
                return user.username;
            }

            // If the user doesn't exist, you can return a default value or handle it accordingly
            return "User Not Found";
        }
    }
}
