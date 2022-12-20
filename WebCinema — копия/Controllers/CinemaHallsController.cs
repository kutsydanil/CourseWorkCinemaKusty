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
    public class CinemaHallsController : Controller
    {
        private readonly CinemaContext _context;
        private const int _pageSize = 20;

        public CinemaHallsController(CinemaContext context)
        {
            _context = context;
        }

        // GET: CinemaHalls
        public async Task<IActionResult> Index(int? Number, int? PlaceNumber, string FieldName, string OldFieldName, SortState SortOrder, int page = 1)
        {
            var result = SearchSort(_context.CinemaHalls, SortOrder, FieldName, OldFieldName, Number, PlaceNumber);

            var hallsContext = result.Item1;

            var sortView = result.Item2;

            var count = hallsContext.Count();

            hallsContext = hallsContext.Skip((page - 1) * _pageSize).Take(_pageSize);

            var viewModel = new CinemaHallViewModel()
            {
                CinemaHalls = hallsContext,
                HallNumber = Number,
                MaxPlaceNumber = PlaceNumber,
                PageViewModel = new PageViewModel(count, page, _pageSize),
                SortViewModel = sortView
            };

            return View(viewModel);
        }

        // GET: CinemaHalls/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CinemaHalls == null)
            {
                return NotFound();
            }

            var cinemaHalls = await _context.CinemaHalls
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cinemaHalls == null)
            {
                return NotFound();
            }

            return View(cinemaHalls);
        }

        // GET: CinemaHalls/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CinemaHalls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,HallNumber,MaxPlaceNumber")] CinemaHalls cinemaHalls)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cinemaHalls);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cinemaHalls);
        }

        // GET: CinemaHalls/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CinemaHalls == null)
            {
                return NotFound();
            }

            var cinemaHalls = await _context.CinemaHalls.FindAsync(id);
            if (cinemaHalls == null)
            {
                return NotFound();
            }
            return View(cinemaHalls);
        }

        // POST: CinemaHalls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HallNumber,MaxPlaceNumber")] CinemaHalls cinemaHalls)
        {
            if (id != cinemaHalls.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cinemaHalls);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CinemaHallsExists(cinemaHalls.Id))
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
            return View(cinemaHalls);
        }

        // GET: CinemaHalls/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CinemaHalls == null)
            {
                return NotFound();
            }

            var cinemaHalls = await _context.CinemaHalls
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cinemaHalls == null)
            {
                return NotFound();
            }

            return View(cinemaHalls);
        }

        // POST: CinemaHalls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CinemaHalls == null)
            {
                return Problem("Entity set 'CinemaContext.CinemaHalls'  is null.");
            }
            var cinemaHalls = await _context.CinemaHalls.FindAsync(id);
            if (cinemaHalls != null)
            {
                _context.CinemaHalls.Remove(cinemaHalls);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CinemaHallsExists(int id)
        {
          return _context.CinemaHalls.Any(e => e.Id == id);
        }

        private (IEnumerable<CinemaHalls>, SortViewModel) SearchSort(IQueryable<CinemaHalls> cinemaHalls, SortState sortOrder, string fieldName,
                                                   string oldFieldName, int? HallNumber, int? MaxPlaceNumber)
        {
            var hallsContext = Search(cinemaHalls, HallNumber, MaxPlaceNumber);

            var sortViewModel = new SortViewModel(sortOrder, fieldName, oldFieldName);

            Func<CinemaHalls, object> func = null;

            switch(fieldName)
            {
                case "HallNumber":
                    func = c => c.HallNumber;
                    break;
                case "MaxPlaceNumber":
                    func = c => c.MaxPlaceNumber;
                    break;
                default:
                    func = c => c.Id;
                    break;
            }

            switch (sortViewModel.CurrentState)
            {
                case SortState.Ascending:
                    hallsContext = cinemaHalls.OrderBy(func);
                    break;
                case SortState.Descending:
                    hallsContext = cinemaHalls.OrderByDescending(func);
                    break;
            }

            return (hallsContext, sortViewModel);
        }

        private IEnumerable<CinemaHalls> Search(IQueryable<CinemaHalls> cinemaHalls, int? HallNumber, int? MinPlaceNumber)
        {
            var result = cinemaHalls.Where(c =>
                c.HallNumber.ToString().Contains(HallNumber.HasValue ? HallNumber.Value.ToString() : "") &&
                (MinPlaceNumber.HasValue ? c.MaxPlaceNumber >= MinPlaceNumber : true) 
                );
            return result;
        }
    }
}
