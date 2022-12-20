global using Microsoft.AspNetCore.Authorization;
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

namespace WebCinema.Controllers
{
    [Authorize()]
    public class ActorsController : Controller
    {
        private readonly CinemaContext _context;
        private const int _pageSize = 20;
        private CacheProvider _cacheProvider;
        private string modelName = "ActorsViewModel";

        public ActorsController(CinemaContext context, CacheProvider cashProvider)
        {
            _context = context;
            _cacheProvider = cashProvider;
        }

        // GET: Actors
        public async Task<IActionResult> Index(string Name, string Surname, string MiddleName, string FieldName, string OldFieldName, SortState SortOrder = SortState.No,int page = 1, bool first = false)
        {
            Surname = CookieProccesor.GetSetValue("ActorSurname", Surname, first, Request, Response);
            Name = CookieProccesor.GetSetValue("ActorName", Name, first, Request, Response);
            MiddleName = CookieProccesor.GetSetValue("ActorMiddleName", MiddleName, first, Request, Response);

            ActorsViewModel model = _cacheProvider.GetItem<ActorsViewModel>(modelName);

            if (model != null && ViewModelComparsion.Compare(model.PageViewModel, page) && model.Name == Name
                && model.Surname == Surname && model.MiddleName == MiddleName &&
                ViewModelComparsion.Compare(model.SortViewModel, SortOrder, FieldName))
            {
                return View(model);
            }

            var sortViewModel = new SortViewModel(SortOrder, FieldName, OldFieldName);

            var actors = Search(_context.Actors, Name ?? "", Surname ?? "", MiddleName ?? "");

            var count = actors.Count();

            var actorsContext = Sort(actors, sortViewModel);

            actorsContext = actorsContext.Skip((page - 1) * _pageSize).Take(_pageSize);

            var viewModel = new ActorsViewModel()
            {
                Actors = actorsContext.ToList(),
                SortViewModel = sortViewModel,
                PageViewModel = new PageViewModel(count, page, _pageSize),
                Name = Name,
                Surname = Surname,
                MiddleName = MiddleName,
            };

            _cacheProvider.SetItem(viewModel, modelName);

            return View(viewModel);
        }

        // GET: Actors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Actors == null)
            {
                return NotFound();
            }

            var actors = await _context.Actors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actors == null)
            {
                return NotFound();
            }

            return View(actors);
        }

        // GET: Actors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Actors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Surname,MiddleName")] Actors actors)
        {
            if (ModelState.IsValid)
            {
                _context.Add(actors);
                await _context.SaveChangesAsync();
                _cacheProvider.SetItem(null, modelName);
                return RedirectToAction(nameof(Index));
            }
            return View(actors);
        }

        // GET: Actors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Actors == null)
            {
                return NotFound();
            }

            var actors = await _context.Actors.FindAsync(id);
            if (actors == null)
            {
                return NotFound();
            }
            return View(actors);
        }

        // POST: Actors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Surname,MiddleName")] Actors actors)
        {
            if (id != actors.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(actors);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorsExists(actors.Id))
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
            return View(actors);
        }

        // GET: Actors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Actors == null)
            {
                return NotFound();
            }

            var actors = await _context.Actors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actors == null)
            {
                return NotFound();
            }

            return View(actors);
        }

        // POST: Actors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Actors == null)
            {
                return Problem("Entity set 'CinemaContext.Actors'  is null.");
            }
            var actors = await _context.Actors.FindAsync(id);
            if (actors != null)
            {
                _context.Actors.Remove(actors);
            }
            
            await _context.SaveChangesAsync();
            _cacheProvider.SetItem(null, modelName);
            return RedirectToAction(nameof(Index));
        }

        private bool ActorsExists(int id)
        {
          return _context.Actors.Any(e => e.Id == id);
        }

        private IQueryable<Actors> Search(IQueryable<Actors> actors, string name, string surname, string middleName)
        {
            return actors.Where(a => a.Name.Contains(name) && a.Surname.Contains(surname) && a.MiddleName.Contains(middleName));
        }

        private IEnumerable<Actors> Sort(IEnumerable<Actors> actors, SortViewModel sortViewModel)
        {
            Func<Actors, object> func = null;

            switch(sortViewModel.FieldName)
            {
                case "Name":
                    func = a => a.Name;
                    break;
                case "Surname":
                    func = a => a.Surname;
                    break;
                case "MiddleName":
                    func = a => a.MiddleName;
                    break;
                default:
                    func = a => a.Id;
                    break;
            }

            switch (sortViewModel.CurrentState)
            {
                case SortState.Ascending:
                    actors = actors.OrderBy(func);
                    break;
                case SortState.Descending:
                    actors = actors.OrderByDescending(func);
                    break;
            }

            return actors;
        }
    }
}
