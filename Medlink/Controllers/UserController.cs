using Medlink.Data;
using Medlink.Models;
using Microsoft.AspNetCore.Mvc;

namespace Medlink.Controllers
{
    public class UserController : Controller
    {
        public static string loginid { get; set; }

        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            if(UserController.loginid == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                UserModel user = _db.Users.FirstOrDefault(u => u.user_id == UserController.loginid);
                return View(user);
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
                if (id == UserController.loginid)
                {
                    return RedirectToAction("Index");
                }
                return user != null ? View(user) : RedirectToAction("List");
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
            UserModel user = _db.Users.FirstOrDefault(u => u.email == email);
            if (user != null)
            {
                if (user.password == password)
                {
                    UserController.loginid = user.user_id;
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("password", "login failed the password is incorrect");
                }
            }
            else
            {
                ModelState.AddModelError("email", "login failed email matched no row in d database");                
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

    }
}
