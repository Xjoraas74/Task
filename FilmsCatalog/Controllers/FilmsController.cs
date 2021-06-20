using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FilmsCatalog.Data;
using FilmsCatalog.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using X.PagedList;

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
        public IActionResult Index(int page = 1)
        {
			int pageSize = 20;
            int pageNumber = page;
            return View(_context.Films.OrderBy(f => f.Name).ToPagedList(pageNumber, pageSize));
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
            (bool posterAdded, string fileName, string fileExt) = GetPosterInfo(model);
            if (posterAdded && !AllowedExtensions.Contains(fileExt)) {
                ModelState.AddModelError(nameof(model.PosterPhoto), "This file type is prohibited");
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
                    await SavePosterToFilmEntityAndDisk(film, model.PosterPhoto, fileName, fileExt);
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
            if (film.UserSenderId != _userManager.GetUserId(HttpContext.User)) {
                return Forbid();
			}
            ViewBag.PosterPath = film.PosterPath;
            return View(new FilmViewModel
            {
                Name = film.Name,
                Description = film.Description,
                ReleaseYear = film.ReleaseYear,
                Director = film.Director
            });
        }

        // POST: Films/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid? id, FilmViewModel model)
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
            if (film.UserSenderId != _userManager.GetUserId(HttpContext.User))
            {
                return Forbid();
            }
            (bool posterAdded, string fileName, string fileExt) = GetPosterInfo(model);
            if (posterAdded && !AllowedExtensions.Contains(fileExt))
            {
                ModelState.AddModelError(nameof(model.PosterPhoto), "This file type is prohibited");
            }
            if (ModelState.IsValid)
            {
                film.Name = model.Name;
                film.Description = model.Description;
                film.ReleaseYear = model.ReleaseYear;
                film.Director = model.Director;
                if (posterAdded)
                {
                    await SavePosterToFilmEntityAndDisk(film, model.PosterPhoto, fileName, fileExt);
                }
                _context.Update(film);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.PosterPath = film.PosterPath;
            return View(model);
        }

        private bool FilmExists(Guid id)
        {
            return _context.Films.Any(e => e.Id == id);
        }

        private (bool posterAdded, string, string) GetPosterInfo(FilmViewModel model) {
            var poster = model.PosterPhoto;
            if (poster != null)
            {
                string fileName = Path.GetFileName(ContentDispositionHeaderValue.Parse(poster.ContentDisposition).FileName.Trim('"'));
                string fileExt = Path.GetExtension(fileName);
                return (true, fileName, fileExt);
            }
            else {
                return (false, null, null);
			}
        }

        private async Task SavePosterToFilmEntityAndDisk(Film film, IFormFile poster, string fileName, string fileExt) {
            film.PosterName = fileName;
            var posterPath = Path.Combine(_hostingEnvironment.WebRootPath, "attachments", film.Id.ToString("N") + fileExt);
            film.PosterPath = $"/attachments/{film.Id:N}{fileExt}";
            if (System.IO.File.Exists(posterPath))
            {
                System.IO.File.Delete(posterPath);
            }
            using (var fileStream = new FileStream(posterPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read))
            {
                await poster.CopyToAsync(fileStream);
            }
        }
    }
}
