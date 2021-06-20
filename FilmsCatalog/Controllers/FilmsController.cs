using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FilmsCatalog.Data;
using FilmsCatalog.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;

namespace FilmsCatalog.Controllers
{
    public class FilmsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FilmsController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<User> _userManager;

        private static readonly HashSet<string> AllowedExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png" };

        public FilmsController(
            ApplicationDbContext context, 
            ILogger<FilmsController> logger,
            IWebHostEnvironment hostingEnvironment,
            UserManager<User> userManager
            )
        {
            _context = context;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }

        // GET: Films
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Films;//.Include(f => f.UserSender);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Films/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films
                .FirstOrDefaultAsync(m => m.Id == id);
            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // GET: Films/Create
        public IActionResult Create()
        {
            return View(new FilmViewModel());
        }

        // POST: Films/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FilmViewModel model)
        {
            bool posterAdded = model.PosterPhoto != null;
            string fileName = null;
            string fileExt = null;
            if (posterAdded) {
                fileName = Path.GetFileName(ContentDispositionHeaderValue.Parse(model.PosterPhoto.ContentDisposition).FileName.Trim('"'));
                fileExt = Path.GetExtension(fileName);
                if (!AllowedExtensions.Contains(fileExt))
                {
                    ModelState.AddModelError(nameof(model.PosterPhoto), "This file type is prohibited");
                }
            }
            if (ModelState.IsValid)
            {
                var film = new Film
                {
                    Name = model.Name,
                    Description = model.Description,
                    ReleaseYear = model.ReleaseYear,
                    Director = model.Director,
                    UserSenderId = _userManager.GetUserId(HttpContext.User)
                };

                if (posterAdded) {
                    film.PosterName = fileName;
                    var posterPath = Path.Combine(_hostingEnvironment.WebRootPath, "attachments", film.Id.ToString("N") + fileExt);
                    film.PosterPath = $"/attachments/{film.Id:N}{fileExt}";
                    using (var fileStream = new FileStream(posterPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read))
                    {
                        await model.PosterPhoto.CopyToAsync(fileStream);
                    }
                }

                _context.Add(film);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Films/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films.FindAsync(id);
            if (film == null)
            {
                return NotFound();
            }
            ViewData["UserSenderId"] = new SelectList(_context.Users, "Id", "Id", film.UserSenderId);
            return View(film);
        }

        // POST: Films/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Description,ReleaseYear,Director,UserSenderId,PosterName,PosterPath")] Film film)
        {
            if (id != film.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(film);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmExists(film.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserSenderId"] = new SelectList(_context.Users, "Id", "Id", film.UserSenderId);
            return View(film);
        }

        // GET: Films/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films
                .Include(f => f.UserSender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // POST: Films/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var film = await _context.Films.FindAsync(id);
            _context.Films.Remove(film);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilmExists(Guid id)
        {
            return _context.Films.Any(e => e.Id == id);
        }
    }
}
