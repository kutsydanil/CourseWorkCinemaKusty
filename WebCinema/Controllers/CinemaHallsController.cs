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
using WebCinema.CookieProccesorNamespace;
using WebCinema.CacheService;

namespace WebCinema.Controllers
{
    [Authorize()]
    public class CinemaHallsController : Controller
    {
        private readonly CinemaContext _context;
        private const int _pageSize = 20;
        private CacheProvider _cacheProvider;
        private string modelName = "CinemaHallViewModel";

        public CinemaHallsController(CinemaContext context, CacheProvider cashProvider)
        {
            _context = context;
            _cacheProvider = cashProvider;
        }

        // GET: CinemaHalls
        public async Task<IActionResult> Index(int? Number, int? PlaceNumber, string FieldName, string OldFieldName, SortState SortOrder = SortState.No, int page = 1, bool first = false)
        {
            Number = CookieProccesor.GetSetValue<int>("HallNumber", Number.HasValue ? Number.Value.ToString() : "", first, Request, Response);
            PlaceNumber = CookieProccesor.GetSetValue<int>("PlaceNumber", PlaceNumber.HasValue ? PlaceNumber.Value.ToString() : "", first, Request, Response);

            var model = _cacheProvider.GetItem<CinemaHallViewModel>(modelName);

            if (model != null && ViewModelComparsion.Compare(model.PageViewModel, page) && model.HallNumber == Number &&
                model.MaxPlaceNumber == PlaceNumber &&
                ViewModelComparsion.Compare(model.SortViewModel, SortOrder, FieldName))
            {
                return View(model);
            }

            var result = SearchSort(_context.CinemaHalls, SortOrder, FieldName, OldFieldName, Number, PlaceNumber);

            var hallsContext = result.Item1.ToList();

            var sortView = result.Item2;

            var count = hallsContext.Count();

            hallsContext = hallsContext.Skip((page - 1) * _pageSize).Take(_pageSize).ToList();

            var viewModel = new CinemaHallViewModel()
            {
                CinemaHalls = hallsContext.ToList(),
                HallNumber = Number,
                MaxPlaceNumber = PlaceNumber,
                PageViewModel = new PageViewModel(count, page, _pageSize),
                SortViewModel = sortView
            };

            _cacheProvider.SetItem(viewModel, modelName);

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
        public async Task<IActionResult> Create([Bind("Id,HallNumber,Columns,Rows")] CinemaHalls cinemaHalls)
        {
            cinemaHalls.MaxPlaceNumber = cinemaHalls.Columns * cinemaHalls.Rows;

            if (ModelState.IsValid)
            {
                _context.Add(cinemaHalls);
                await _context.SaveChangesAsync();
                _cacheProvider.SetItem(null, modelName);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,HallNumber,Rows,Columns")] CinemaHalls cinemaHalls)
        {
            cinemaHalls.MaxPlaceNumber = cinemaHalls.Columns * cinemaHalls.Rows;

            if (id != cinemaHalls.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.ChangeTracker.Clear();
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
                _cacheProvider.SetItem(null, modelName);
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
            _cacheProvider.SetItem(null, modelName);
            return RedirectToAction(nameof(Index));
        }

        private bool CinemaHallsExists(int id)
        {
          return _context.CinemaHalls.Any(e => e.Id == id);
        }

        private (List<CinemaHalls>, SortViewModel) SearchSort(IQueryable<CinemaHalls> cinemaHalls, SortState sortOrder, string fieldName,
                                                   string oldFieldName, int? HallNumber, int? MaxPlaceNumber)
        {
            var hallsContext = Search(cinemaHalls, HallNumber ?? 0, MaxPlaceNumber ?? 0);

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
                    hallsContext = hallsContext.OrderBy(func);
                    break;
                case SortState.Descending:
                    hallsContext = hallsContext.OrderByDescending(func);
                    break;
            }

            return (hallsContext.ToList(), sortViewModel);
        }

        private IEnumerable<CinemaHalls> Search(IQueryable<CinemaHalls> cinemaHalls, int HallNumber, int MinPlaceNumber)
        {
            var result = cinemaHalls.Where(c =>
                (HallNumber > 0 ? c.HallNumber == HallNumber : true) &&
                (MinPlaceNumber > 0 ? c.MaxPlaceNumber >= MinPlaceNumber : true) 
                );
            return result;
        }
    }
}
