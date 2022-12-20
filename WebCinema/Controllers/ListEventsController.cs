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
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebCinema.Controllers
{
    [Authorize()]
    public class ListEventsController : Controller
    {
        private readonly CinemaContext _context;
        private const int _pageSize = 20;
        private CacheProvider _cacheProvider;
        private string modelName = "ListEventsViewModel";

        public ListEventsController(CinemaContext context, CacheProvider cacheProvider)
        {
            _context = context;
            _cacheProvider = cacheProvider;
        }

        public List<T> AddDefault<T>(IEnumerable<T> values) where T : new()
        {
            var list = values.ToList();
            list.Insert(0, new T { });
            return list;
        }

        // GET: ListEvents
        public async Task<IActionResult> Index(SortState SortOrder, int? HallId, string? description, int? FilmId, DateTime? Date, string FieldName, string OldFieldName, bool? SearchForDate, bool first = false, decimal Price = 0, int page = 1)
        {
            HallId = CookieProccesor.GetSetValue<int>("EventHallId", HallId.HasValue ? HallId.Value.ToString() : "", first, Request, Response);
            FilmId = CookieProccesor.GetSetValue<int>("EventFilmdId", FilmId.HasValue ? FilmId.Value.ToString() : "", first, Request, Response);
            description = CookieProccesor.GetSetValue("EventDescription", description ?? "", first, Request, Response);
            Date = CookieProccesor.GetSetValue<DateTime>("EventDate", Date.HasValue ? Date.Value.ToString() : "", first, Request, Response);
            SearchForDate = CookieProccesor.GetSetValue<bool>("EventForDate", SearchForDate.HasValue ? SearchForDate.Value.ToString() : "", first, Request, Response);
            Price = CookieProccesor.GetSetValue<decimal>("EventPrice", Price.ToString(), first, Request, Response);

            var model = _cacheProvider.GetItem<ListEventsViewModel>(modelName);

            if (model != null && model.PageViewModel.PageNumber == page && model.HallId == HallId &&
                model.FilmId == FilmId && model.Description == description && model.Date.Date == Date.Value.Date &&
                model.SearchForDate == SearchForDate && Price == model.TicketPrice &&
                ViewModelComparsion.Compare(model.SortViewModel, SortOrder, FieldName))
            {
                return View(model);
            }

            IEnumerable<ListEvents> cinemaContext = Search(_context.ListEvents.Include(l => l.Film), description ?? "",
                                                            FilmId ?? 0, SearchForDate ?? false, Date.Value, Price, HallId ?? 0);

            var count = cinemaContext.Count();

            var sortViewModel = new SortViewModel(SortOrder, FieldName, OldFieldName);

            cinemaContext = Sort(cinemaContext, sortViewModel);

            cinemaContext = cinemaContext.Skip((page - 1) * _pageSize).Take(_pageSize);

            Date = SearchForDate.HasValue ? Date : DateTime.Now;

            var viewModel = new ListEventsViewModel()
            {
                ListEvents = cinemaContext.ToList(),
                Films = new SelectList(AddDefault(_context.Films), "Id", "Name", FilmId),
                Halls = new SelectList(AddDefault(_context.CinemaHalls), "Id", "HallNumber", HallId),
                PageViewModel = new PageViewModel(count, page, _pageSize),
                SortViewModel = sortViewModel,
                Description = description ?? "",
                Date =  Date.Value,
                FilmId = FilmId ?? 0,
                TicketPrice = Price,
                HallId = HallId ?? 0,
                SearchForDate = SearchForDate.Value,
            };

            _cacheProvider.SetItem(viewModel, modelName);

            return View(viewModel);
        }

        // GET: ListEvents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ListEvents == null)
            {
                return NotFound();
            }

            var listEvents = await _context.ListEvents
                .Include(l => l.Film)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (listEvents == null)
            {
                return NotFound();
            }

            var places = _context.Places.Where(p => p.ListEventId == listEvents.Id).OrderBy(p => p.PlaceNumber).ToList();

            var hall = _context.CinemaHalls.First(h => h.Id == listEvents.CinemaHallId);

            var staffs = (from cast in _context.StaffCasts.Where(c => c.ListEventId == listEvents.Id)
                         join staff in _context.Staffs
                         on cast.StaffId equals staff.Id
                         select staff).ToList();

            return View(new ListEventsEditViewModel() { ListEvents = listEvents, Places = places, CinemaHalls = hall, Staffs = staffs});
        }

        // GET: ListEvents/Create
        public IActionResult Create()
        {
            ViewData["FilmId"] = new SelectList(_context.Films, "Id", "Name");
            ViewData["CinemaHallId"] = new SelectList(_context.CinemaHalls, "Id", "HallNumber");
            return View();
        }

        // POST: ListEvents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,StartTime,EndTime,TicketPrice,FilmId,CinemaHallId")] ListEvents listEvents)
        {
            var dict = new ModelStateDictionary();

            foreach(var pair in ModelState.Where(p => p.Key != "Film"))
            {
                dict.Append(pair);
            }

            if (dict.IsValid)
            {
                _context.Add(listEvents);
                await _context.SaveChangesAsync();
                _cacheProvider.SetItem(null, modelName);
                return RedirectToAction(nameof(Index));
            }
            ViewData["FilmId"] = new SelectList(_context.Films, "Id", "Name", listEvents.FilmId);
            return View(listEvents);
        }

        // GET: ListEvents/Edit/5
        public async Task<IActionResult> Edit(int? id, int? SeatNumber, int[] StaffsId, bool AddNewStaff, int? DeleteStaffId)
        {
            if (id == null || _context.ListEvents == null)
            {
                return NotFound();
            }

            if (AddNewStaff)
            {
                var random = Random.Shared;
                var cast = new StaffCasts() { ListEventId = id.Value, StaffId = random.Next(1, _context.Staffs.ToList().OrderBy(s => s.Id).Last().Id) };
                _context.StaffCasts.Add(cast);
                _context.SaveChanges();
                return RedirectToAction("Edit", id);
            }

            var listEvents = await _context.ListEvents.FindAsync(id);
            if (listEvents == null)
            {
                return NotFound();
            }

            ViewData["FilmId"] = new SelectList(_context.Films, "Id", "Name", listEvents.FilmId);
            ViewData["CinemaHallsId"] = new SelectList(_context.CinemaHalls, "Id", "HallNumber", listEvents.CinemaHallId);

            var places = _context.Places.Where(p => p.ListEventId == listEvents.Id).OrderBy(p => p.PlaceNumber).ToList();

            var hall = _context.CinemaHalls.FirstOrDefault(h => h.Id == listEvents.CinemaHallId);

            var casts = _context.StaffCasts.Where(c => c.ListEventId == listEvents.Id).ToList();

            if (DeleteStaffId.HasValue)
            {
                var cast = casts.FirstOrDefault(c => c.StaffId == DeleteStaffId);

                if (cast != null)
                {
                    _context.StaffCasts.Remove(cast);
                }
            }

            for(var i = 0; i < StaffsId.Length; i++)
            {
                casts[i].StaffId = StaffsId[i];
            }

            _context.SaveChanges();

            var staffs = (from cast in _context.StaffCasts.Where(c => c.ListEventId == listEvents.Id)
                          join staff in _context.Staffs
                          on cast.StaffId equals staff.Id
                          select staff).ToList();

            ViewData["Places"] = places;
            ViewData["CinemaHalls"] = hall;
            ViewData["Staff"] = staffs;

            var staffsSelected = staffs.Select(s => new SelectList(_context.Staffs.Select(s => new { Id = s.Id, Name = $"{s.Surname} {s.Name} {s.MiddleName}"}), "Id", "Name", s.Id)).ToList();

            ViewData["StaffsSelect"] = staffsSelected;

            if (SeatNumber != null)
            {
                var place = places.FirstOrDefault(p => p.PlaceNumber == SeatNumber);
                if (place == null)
                {
                    place = new Places()
                    {
                        CinemaHallId = listEvents.CinemaHallId,
                        ListEventId = listEvents.Id,
                        PlaceNumber = SeatNumber.Value,
                        TakenSeat = true,
                    };
                    _context.Places.Add(place);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Edit), id);
                }
                else
                {
                    place.TakenSeat = !place.TakenSeat;
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Edit), id);
                }

            }

            return View(listEvents);
        }

        // POST: ListEvents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,StartTime,EndTime,TicketPrice,FilmId,CinemaHallId")] ListEvents listEvents)
        {
            if (id != listEvents.Id)
            {
                return NotFound();
            }

            ModelStateDictionary dict = new ModelStateDictionary();

            foreach(var pair in ModelState.Where(s => s.Key != "Film"))
            {
                dict.Append(pair);
            }
                                  
            if (dict.IsValid)
            {
                try
                {
                    _context.Update(listEvents);
                    await _context.SaveChangesAsync();
                    _cacheProvider.SetItem(null, modelName);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ListEventsExists(listEvents.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //rurn RedirectToAction(nameof(Index));
            }   
            ViewData["FilmId"] = new SelectList(_context.Films, "Id", "Name", listEvents.FilmId);
            ViewData["CinemaHallsId"] = new SelectList(_context.CinemaHalls, "Id", "HallNumber", listEvents.CinemaHallId);

            var staffs = (from cast in _context.StaffCasts.Where(c => c.ListEventId == listEvents.Id)
                          join staff in _context.Staffs
                          on cast.StaffId equals staff.Id
                          select staff).ToList();

            ViewData["Staff"] = staffs;

            var staffsSelected = staffs.Select(s => new SelectList(_context.Staffs.Select(s => new { Id = s.Id, Name = $"{s.Surname} {s.Name} {s.MiddleName}" }), "Id", "Name", s.Id)).ToList();

            ViewData["StaffsSelect"] = staffsSelected;

            var places = _context.Places.Where(p => p.ListEventId == listEvents.Id).OrderBy(p => p.PlaceNumber).ToList();

            var hall = _context.CinemaHalls.First(h => h.Id == listEvents.CinemaHallId);

            ViewData["Places"] = places;
            ViewData["CinemaHalls"] = hall;

            return View(listEvents);
        }

        // GET: ListEvents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ListEvents == null)
            {
                return NotFound();
            }

            var listEvents = await _context.ListEvents
                .Include(l => l.Film)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (listEvents == null)
            {
                return NotFound();
            }

            return View(listEvents);
        }

        // POST: ListEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ListEvents == null)
            {
                return Problem("Entity set 'CinemaContext.ListEvents'  is null.");
            }
            var listEvents = await _context.ListEvents.FindAsync(id);
            if (listEvents != null)
            {
                _context.ListEvents.Remove(listEvents);
            }
            
            await _context.SaveChangesAsync();
            _cacheProvider.SetItem(null, modelName);
            return RedirectToAction(nameof(Index));
        }

        private bool ListEventsExists(int id)
        {
          return _context.ListEvents.Any(e => e.Id == id);
        }

        private IEnumerable<ListEvents> Search(IQueryable<ListEvents> listEvents, string description, int filmId, bool searchForDate, DateTime dateTime, decimal price, int hallId)
        {
            return listEvents.Where(e =>
                (filmId != 0 ? e.FilmId == filmId : true) &&
                (hallId != 0 ? e.CinemaHallId == hallId : true) &&
                (searchForDate ? e.Date.Date == dateTime.Date : true) &&
                (e.Film.Description.Contains(description)) &&
                (e.TicketPrice >= price)
            );
        }

        private IEnumerable<ListEvents> Sort(IEnumerable<ListEvents> listEvents, SortViewModel sortViewModel)
        {
            Func<ListEvents, object> func = null;

            switch(sortViewModel.FieldName)
            {
                case "FilmName":
                    func = e => e.Film.Name;
                    break;
                case "Description":
                    func = e => e.Film.Description;
                    break;
                case "Price":
                    func = e => e.TicketPrice;
                    break;
                case "Date":
                    func = e => e.Date.Add(e.StartTime);
                    break;
                case "HallNumber":
                    func = e => e.CinemaHall.HallNumber;
                    break;
                default:
                    func = e => e.Id;
                    break;
            }

            switch(sortViewModel.CurrentState)
            {
                case SortState.Ascending:
                    listEvents = listEvents.OrderBy(func);
                    break;
                case SortState.Descending:
                    listEvents = listEvents.OrderByDescending(func);
                    break;
            }

            return listEvents;
        }
    }
}
