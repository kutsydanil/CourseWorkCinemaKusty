using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CinemaCore.Models;
using WebCinema;
using WebCinema.ViewModel;

namespace WebCinema.Controllers
{
    public class GenresController : Controller
    {
        private readonly CinemaContext _context;
        private const int _pageSize = 20;

        public GenresController(CinemaContext context)
        {
            _context = context;
        }

        // GET: Genres
        public async Task<IActionResult> Index(string? GenreName, SortState SortOrder, int page = 1)
        {
            var sortModel = new SortViewModel(SortOrder);

            var genresContext = SortSearch(_context.Genres, sortModel.CurrentState, GenreName);

            var count = genresContext.Count();

            genresContext = genresContext.Skip((page - 1) * _pageSize).Take(_pageSize);

            var model = new GenresViewModel()
            {
                Genres = genresContext,
                PageViewModel = new PageViewModel(count, page, _pageSize),
                SortViewModel = sortModel,
                GenreName = GenreName,
            };

            return View(model);
        }

        // GET: Genres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Genres == null)
            {
                return NotFound();
            }

            var genres = await _context.Genres
                .FirstOrDefaultAsync(m => m.Id == id);
            if (genres == null)
            {
                return NotFound();
            }

            return View(genres);
        }

        // GET: Genres/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Genres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Genres genres)
        {
            if (ModelState.IsValid)
            {
                _context.Add(genres);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(genres);
        }

        // GET: Genres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Genres == null)
            {
                return NotFound();
            }

            var genres = await _context.Genres.FindAsync(id);
            if (genres == null)
            {
                return NotFound();
            }
            return View(genres);
        }

        // POST: Genres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Genres genres)
        {
            if (id != genres.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(genres);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GenresExists(genres.Id))
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
            return View(genres);
        }

        // GET: Genres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Genres == null)
            {
                return NotFound();
            }

            var genres = await _context.Genres
                .FirstOrDefaultAsync(m => m.Id == id);
            if (genres == null)
            {
                return NotFound();
            }

            return View(genres);
        }

        // POST: Genres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Genres == null)
            {
                return Problem("Entity set 'CinemaContext.Genres'  is null.");
            }
            var genres = await _context.Genres.FindAsync(id);
            if (genres != null)
            {
                _context.Genres.Remove(genres);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GenresExists(int id)
        {
          return _context.Genres.Any(e => e.Id == id);
        }

        private IQueryable<Genres> SortSearch(IQueryable<Genres> genres, SortState sortState, string genreName)
        {
            switch (sortState)
            {
                case SortState.Ascending:
                    genres = genres.OrderBy(g => g.Name);
                    break;
                case SortState.Descending:
                    genres = genres.OrderByDescending(g => g.Name);
                    break;
            }
            genres = genres.Where(g => g.Name.Contains(genreName ?? ""));
            return genres;
        }
    }
}
