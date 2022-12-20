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
using Microsoft.Data.SqlClient;
using WebCinema.CookieProccesorNamespace;

namespace WebCinema.Controllers
{
    [Authorize()]
    public class FilmProductionsController : Controller
    {
        private readonly CinemaContext _context;
        private const int _pageSize = 20;
        private CacheProvider _cashProvider;
        private string modelName = "FilmProductionViewModel";

        public FilmProductionsController(CinemaContext context, CacheProvider cashProvider)
        {
            _context = context;
            _cashProvider = cashProvider;
        }

        // GET: FilmProductions
        public async Task<IActionResult> Index(string ProductionName, string FieldName, string OldFieldName, SortState SortOrder, int page = 1, bool first = false)
        {
            ProductionName = CookieProccesor.GetSetValue("ProductionName", ProductionName, first, Request, Response);

            var model = _cashProvider.GetItem<FilmProductionViewModel>(modelName);

            if (model != null && ViewModelComparsion.Compare(model.PageViewModel, page) &&
                model.ProductionName == ProductionName &&
                ViewModelComparsion.Compare(model.SortViewModel, SortOrder, FieldName))
            {
                return View(model);
            }

            var sortModel = new SortViewModel(SortOrder, FieldName, OldFieldName);

            var productionsContext = SortSearch(_context.FilmProductions, sortModel.CurrentState, ProductionName);

            var count = productionsContext.Count();

            productionsContext = productionsContext.Skip((page - 1) * _pageSize).Take(_pageSize);

            model = new FilmProductionViewModel()
            {
                FilmProductions = productionsContext.ToList(),
                PageViewModel = new PageViewModel(count, page, _pageSize),
                SortViewModel = sortModel,
                ProductionName = ProductionName,
            };

            _cashProvider.SetItem(model, modelName);

            return View(model);
        }

        // GET: FilmProductions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FilmProductions == null)
            {
                return NotFound();
            }

            var filmProductions = await _context.FilmProductions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (filmProductions == null)
            {
                return NotFound();
            }

            return View(filmProductions);
        }

        // GET: FilmProductions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FilmProductions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] FilmProductions filmProductions)
        {
            if (ModelState.IsValid)
            {
                _context.Add(filmProductions);
                await _context.SaveChangesAsync();
                _cashProvider.SetItem(null, modelName);
                return RedirectToAction(nameof(Index));
            }
            return View(filmProductions);
        }

        // GET: FilmProductions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.FilmProductions == null)
            {
                return NotFound();
            }

            var filmProductions = await _context.FilmProductions.FindAsync(id);
            if (filmProductions == null)
            {
                return NotFound();
            }
            return View(filmProductions);
        }

        // POST: FilmProductions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] FilmProductions filmProductions)
        {
            if (id != filmProductions.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(filmProductions);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmProductionsExists(filmProductions.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                _cashProvider.SetItem(null, modelName);
                return RedirectToAction(nameof(Index));
            }
            return View(filmProductions);
        }

        // GET: FilmProductions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.FilmProductions == null)
            {
                return NotFound();
            }

            var filmProductions = await _context.FilmProductions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (filmProductions == null)
            {
                return NotFound();
            }

            return View(filmProductions);
        }

        // POST: FilmProductions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.FilmProductions == null)
            {
                return Problem("Entity set 'CinemaContext.FilmProductions'  is null.");
            }
            var filmProductions = await _context.FilmProductions.FindAsync(id);
            if (filmProductions != null)
            {
                _context.FilmProductions.Remove(filmProductions);
            }
            
            await _context.SaveChangesAsync();
            _cashProvider.SetItem(null, modelName);
            return RedirectToAction(nameof(Index));
        }

        private bool FilmProductionsExists(int id)
        {
          return _context.FilmProductions.Any(e => e.Id == id);
        }
        private IQueryable<FilmProductions> SortSearch(IQueryable<FilmProductions> genres, SortState sortState, string productionName)
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
            genres = genres.Where(g => g.Name.Contains(productionName ?? ""));
            return genres;
        }
    }
}
