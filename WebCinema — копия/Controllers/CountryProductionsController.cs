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
    public class CountryProductionsController : Controller
    {
        private readonly CinemaContext _context;
        private const int _pageSize = 20;

        public CountryProductionsController(CinemaContext context)
        {
            _context = context;
        }

        // GET: CountryProductions
        public async Task<IActionResult> Index(string CountryName, string OldFieldName, string FieldName, SortState SortOrder,  int page = 1)
        {
            var sortModel = new SortViewModel(SortOrder, FieldName, OldFieldName);

            var countryContext = SortSearch(_context.CountryProductions, sortModel.CurrentState, CountryName);

            var count = countryContext.Count();

            countryContext = countryContext.Skip((page - 1) * _pageSize).Take(_pageSize);

            var model = new CountryProductionsViewModel()
            {
                CountryProductions = countryContext,
                PageViewModel = new PageViewModel(count, page, _pageSize),
                SortViewModel = sortModel,
                CountryName = CountryName,
            };

            return View(model);
        }

        // GET: CountryProductions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CountryProductions == null)
            {
                return NotFound();
            }

            var countryProductions = await _context.CountryProductions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (countryProductions == null)
            {
                return NotFound();
            }

            return View(countryProductions);
        }

        // GET: CountryProductions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CountryProductions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] CountryProductions countryProductions)
        {
            if (ModelState.IsValid)
            {
                _context.Add(countryProductions);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(countryProductions);
        }

        // GET: CountryProductions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CountryProductions == null)
            {
                return NotFound();
            }

            var countryProductions = await _context.CountryProductions.FindAsync(id);
            if (countryProductions == null)
            {
                return NotFound();
            }
            return View(countryProductions);
        }

        // POST: CountryProductions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] CountryProductions countryProductions)
        {
            if (id != countryProductions.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(countryProductions);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountryProductionsExists(countryProductions.Id))
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
            return View(countryProductions);
        }

        // GET: CountryProductions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CountryProductions == null)
            {
                return NotFound();
            }

            var countryProductions = await _context.CountryProductions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (countryProductions == null)
            {
                return NotFound();
            }

            return View(countryProductions);
        }

        // POST: CountryProductions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CountryProductions == null)
            {
                return Problem("Entity set 'CinemaContext.CountryProductions'  is null.");
            }
            var countryProductions = await _context.CountryProductions.FindAsync(id);
            if (countryProductions != null)
            {
                _context.CountryProductions.Remove(countryProductions);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CountryProductionsExists(int id)
        {
          return _context.CountryProductions.Any(e => e.Id == id);
        }

        private IQueryable<CountryProductions> SortSearch(IQueryable<CountryProductions> countryProdctions, SortState sortState, string genreName)
        {
            switch (sortState)
            {
                case SortState.Ascending:
                    countryProdctions = countryProdctions.OrderBy(g => g.Name);
                    break;
                case SortState.Descending:
                    countryProdctions = countryProdctions.OrderByDescending(g => g.Name);
                    break;
            }
            countryProdctions = countryProdctions.Where(g => g.Name.Contains(genreName ?? ""));
            return countryProdctions;
        }
    }
}
