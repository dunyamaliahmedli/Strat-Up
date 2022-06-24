using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DAL;
using WebApplication1.Models;

namespace WebApplication1.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class UserController : Controller
    {
        private AppDbContext _context { get; }
        private IWebHostEnvironment _env { get; set; }
        public UserController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            var page = _context.Users.ToList();
            return View(page);
        }
        public IActionResult Create() {
            return View();
        }
    

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user)
        {
            if (!ModelState.IsValid) return NotFound();
            if (user.Photo != null)
            {
                if (user.Photo.ContentType != "image/jpeg" && user.Photo.ContentType != "image/png" && user.Photo.ContentType != "image/webp")
                {
                    ModelState.AddModelError("", "Faylin tipi png ve ya jpeg olmalidir");
                    return View(user);
                }
                if (user.Photo.Length / 1024 > 3000)
                {
                    ModelState.AddModelError("", "Faylin olcusu max 3mb ola biler");
                    return View(user);
                }
                string filename = user.Photo.FileName;
                if (filename.Length > 64)
                {
                    filename.Substring(filename.Length - 64, 64);

                }
                string newFileName = Guid.NewGuid().ToString() + filename;
                string path = Path.Combine(_env.WebRootPath, "assets", "img","Firma" ,newFileName);
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    user.Photo.CopyTo(fs);
                }
                user.Image = newFileName;
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public IActionResult Delete(int Id) {

            var deleted = _context.Users.Find(Id);
            if (deleted != null)
            {
                _context.Users.Remove(deleted);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
          
        }

        public IActionResult Edit(int Id) {
            User users = _context.Users.FirstOrDefault(x => x.Id == Id);
            if (users == null) return NotFound();
            return View(users);
        }

        [HttpPost]
        public IActionResult Edit(User user ) {
            var old = _context.Users.FirstOrDefault(x => x.Id == user.Id);
            
            if (old == null) return NotFound();

            if (user.Photo != null)
            {
                if (user.Photo.ContentType != "image/jpeg" && user.Photo.ContentType != "image/png" && user.Photo.ContentType != "image/webp")
                {
                    ModelState.AddModelError("", "Faylin tipi png ve ya jpeg olmalidir");
                    return View(user);
                }
                if (user.Photo.Length / 1024 > 3000)
                {
                    ModelState.AddModelError("", "Faylin olcusu max 3mb ola biler");
                    return View(user);
                }
                string filename = user.Photo.FileName;
                if (filename.Length > 64)
                {
                    filename.Substring(filename.Length - 64, 64);

                }
                string newFileName = Guid.NewGuid().ToString() + filename;
                string path = Path.Combine(_env.WebRootPath, "assets", "img", "Firma", newFileName);
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    user.Photo.CopyTo(fs);
                }
                old.Image = newFileName;
                old.FullName = user.FullName;
                old.Job = user.Job;
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

           


            return RedirectToAction(nameof(Index));        
        }
    }
}
