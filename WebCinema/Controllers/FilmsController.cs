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
using MessagePack;
using System.Collections;
using System.Diagnostics;
using WebCinema.CookieProccesorNamespace;

namespace WebCinema.Controllers
{
    [Authorize()]
    public class FilmsController : Controller
    {
        private readonly CinemaContext _context;
        private const int _pageSize = 20;
        private CacheProvider _cacheProvider;
        private string modelName = "FilmsViewModel";

        public FilmsController(CinemaContext context, CacheProvider cashProvider)
        {
            _context = context;
            _cacheProvider = cashProvider;
        }

        public List<T> GetListWithDefault<T>(IEnumerable<T> entities) where T : new()
        {
            var list = entities.ToList();
            list.Insert(0, new T { });
            return list;
        }

        // GET: Films
        public async Task<IActionResult> Index(SortState SortOrder, string FieldName, string OldFieldName, string Name,
                                               string Description, int? AgeLimit, int? Duration, int? GenreId, int? CompanyId,
                                               int? CountryId, int page = 1, bool first = false)
        {
            Name = CookieProccesor.GetSetValue("FilmName", Name, first, Request, Response);
            Description = CookieProccesor.GetSetValue("FilmDescription", Description, first, Request, Response);
            AgeLimit = CookieProccesor.GetSetValue<int>("FilmAgeLimit", AgeLimit.HasValue ? AgeLimit.Value.ToString() : "", first, Request, Response);
            Duration = CookieProccesor.GetSetValue<int>("FilmDuration", Duration.HasValue ? Duration.Value.ToString() : "", first, Request, Response);
            GenreId = CookieProccesor.GetSetValue<int>("FilmGenreId", GenreId.HasValue ? GenreId.Value.ToString() : "", first, Request, Response);
            CompanyId = CookieProccesor.GetSetValue<int>("FilmCompanyId", CompanyId.HasValue ? CompanyId.Value.ToString() : "", first, Request, Response);
            CountryId = CookieProccesor.GetSetValue<int>("FilmCountryId", CountryId.HasValue ? CountryId.Value.ToString() : "", first, Request, Response);

            var model = _cacheProvider.GetItem<FilmsViewModel>(modelName);

            if (model != null && model.PageViewModel.PageNumber == page && model.Duration == Duration &&
                model.CountryProductionId == CountryId && model.AgeLimit == AgeLimit && model.GenreId == GenreId &&
                model.FilmProductionId == CompanyId && model.Description == Description && model.Name == Name &&
                ViewModelComparsion.Compare(model.SortViewModel, SortOrder, FieldName))
            {
                return View(model);
            }

            var filmsContext = Search(_context.Films.Include(f => f.CountryProduction).Include(f => f.FilmProduction).
                                      Include(f => f.Genre), Name ?? "", Description ?? "", AgeLimit ?? 0, Duration ?? 0,
                                      GenreId ?? 0, CompanyId ?? 0, CountryId ?? 0);

            var count = filmsContext.Count();
             
            var sortViewModel = new SortViewModel(SortOrder, FieldName, OldFieldName);

            filmsContext = Sort(filmsContext, sortViewModel);

            filmsContext =  filmsContext.Skip((page - 1) * _pageSize).Take(_pageSize);

            model = (new FilmsViewModel()
            {
                Films = filmsContext.ToList(),
                Genres = new SelectList(GetListWithDefault(_context.Genres), "Id", "Name", GenreId),
                FilmProductions = new SelectList(GetListWithDefault(_context.FilmProductions), "Id", "Name", CompanyId),
                CountryProductions = new SelectList(GetListWithDefault(_context.CountryProductions), "Id", "Name", CountryId),
                SortViewModel = sortViewModel,
                PageViewModel = new PageViewModel(count, page, _pageSize),
                Duration = Duration ?? 0,
                AgeLimit = AgeLimit ?? 0,
                Name = Name,
                Description = Description,
                GenreId = GenreId ?? 0,
                FilmProductionId = CompanyId ?? 0,
                CountryProductionId = CountryId ?? 0
            });

            _cacheProvider.SetItem(model, "FilmsViewModel");

            return View(model);
        }

        // GET: Films/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Films == null)
            {
                return NotFound();
            }

            var films = await _context.Films
                .Include(f => f.CountryProduction)
                .Include(f => f.FilmProduction)
                .Include(f => f.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (films == null)
            {
                return NotFound();
            }

            var actors = (from cast in _context.ActorCasts.Where(c => c.FilmId == films.Id)
                          join actor in _context.ActorCasts
                          on cast.FilmId equals films.Id
                          select actor).ToList();

            return View(films);
        }

        // GET: Films/Create
        public IActionResult Create()
        {
            ViewData["CountryProductionId"] = new SelectList(_context.CountryProductions, "Id", "Name");
            ViewData["FilmProductionId"] = new SelectList(_context.FilmProductions, "Id", "Name");
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name");
            return View();
        }

        // POST: Films/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,GenreId,Duration,FilmProductionId,CountryProductionId,AgeLimit,Description")] Films films)
        {
            if (ModelState.IsValid)
            {
                _context.Add(films);
                await _context.SaveChangesAsync();
                _cacheProvider.SetItem(null, modelName);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CountryProductionId"] = new SelectList(_context.CountryProductions, "Id", "Name", films.CountryProductionId);
            ViewData["FilmProductionId"] = new SelectList(_context.FilmProductions, "Id", "Name", films.FilmProductionId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", films.GenreId);
            return View(films);
        }

        // GET: Films/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Films == null)
            {
                return NotFound();
            }

            var films = await _context.Films.FindAsync(id);
            if (films == null)
            {
                return NotFound();
            }
            ViewData["CountryProductionId"] = new SelectList(_context.CountryProductions, "Id", "Name", films.CountryProductionId);
            ViewData["FilmProductionId"] = new SelectList(_context.FilmProductions, "Id", "Name", films.FilmProductionId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", films.GenreId);
            return View(films);
        }

        // POST: Films/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,GenreId,Duration,FilmProductionId,CountryProductionId,AgeLimit,Description")] Films films)
        {
            if (id != films.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(films);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmsExists(films.Id))
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
            ViewData["CountryProductionId"] = new SelectList(_context.CountryProductions, "Id", "Name", films.CountryProductionId);
            ViewData["FilmProductionId"] = new SelectList(_context.FilmProductions, "Id", "Name", films.FilmProductionId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", films.GenreId);
            return View(films);
        }

        // GET: Films/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Films == null)
            {
                return NotFound();
            }

            var films = await _context.Films
                .Include(f => f.CountryProduction)
                .Include(f => f.FilmProduction)
                .Include(f => f.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (films == null)
            {
                return NotFound();
            }

            return View(films);
        }

        // POST: Films/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Films == null)
            {
                return Problem("Entity set 'CinemaContext.Films'  is null.");
            }
            var films = await _context.Films.FindAsync(id);
            if (films != null)
            {
                _context.Films.Remove(films);
            }
            
            await _context.SaveChangesAsync();
            _cacheProvider.SetItem(null, modelName);
            return RedirectToAction(nameof(Index));
        }

        private bool FilmsExists(int id)
        {
          return _context.Films.Any(e => e.Id == id);
        }

        private IEnumerable<Films> Search(IQueryable<Films> films, string name, string description, int minimalAge, 
                                         int minimalDuration, int genreId, int companyId, int countryId)
        {
            return films.Where(f =>
                f.Name.Contains(name) &&
                f.Description.Contains(description) &&
                f.AgeLimit >= minimalAge &&
                f.Duration >= minimalDuration &&
                (genreId != 0 ? f.GenreId == genreId : true) &&
                (companyId != 0 ? f.FilmProductionId == companyId : true) &&
                (countryId != 0 ? f.CountryProductionId == countryId : true)
            );
        }

        private IEnumerable<Films> Sort(IEnumerable<Films> films, SortViewModel sortViewModel)
        {
            Func<Films, object> func = null;

            switch(sortViewModel.FieldName)
            {
                case "AgeLimit":
                    func = f => f.AgeLimit;
                    break;
                case "Name":
                    func = f => f.Name;
                    break;
                case "Decription":
                    func = f => f.Description;
                    break;
                case "Duration":
                    func = f => f.Duration;
                    break;
                case "FilmProduction":
                    func = f => f.FilmProduction.Name;
                    break;
                case "CountryProduction":
                    func = f => f.CountryProduction.Name;
                    break;
                case "Genre":
                    func = f => f.Genre.Name;
                    break;
                default:
                    func = f => f.Id;
                    break;
            }

            switch (sortViewModel.CurrentState)
            {
                case SortState.Ascending:
                    films = films.OrderBy(func);
                    break;
                case SortState.Descending:
                    films = films.OrderByDescending(func);
                    break;
            }

            return films;
        }
    }
}
