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
using Microsoft.CodeAnalysis.Operations;

namespace WebCinema.Controllers
{
    public class StaffsController : Controller
    {
        private readonly CinemaContext _context;
        private const int _pageSize = 20;

        public StaffsController(CinemaContext context)
        {
            _context = context;
        }

        // GET: Staffs
        public async Task<IActionResult> Index(SortState SortOrder, string FieldName, string OldFieldName, string Post, string Name, string Surname, string MiddleName, int WorkExperience = 0, int page = 1)
        {
            var staffs = Search(_context.Staffs, Name ?? "", Surname ?? "", MiddleName ?? "", WorkExperience, Post ?? "");

            var count = staffs.Count();

            var sortViewModel = new SortViewModel(SortOrder, FieldName, OldFieldName);

            var staffContext = Sort(staffs, sortViewModel).Skip((page - 1) * _pageSize).Take(_pageSize);

            var viewModel = new StaffsViewModel()
            {
                PageViewModel = new PageViewModel(count, page, _pageSize),
                SortViewModel = sortViewModel,
                Name = Name, 
                Surname = Surname,
                MiddleName = MiddleName, 
                Post = Post, 
                WorkExperience = WorkExperience,
                Staffs = staffContext
            };

            return View(viewModel);
        }

        // GET: Staffs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Staffs == null)
            {
                return NotFound();
            }

            var staffs = await _context.Staffs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (staffs == null)
            {
                return NotFound();
            }

            return View(staffs);
        }

        // GET: Staffs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Staffs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Surname,MiddleName,Post,WorkExperience")] Staffs staffs)
        {
            if (ModelState.IsValid)
            {
                _context.Add(staffs);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(staffs);
        }

        // GET: Staffs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Staffs == null)
            {
                return NotFound();
            }

            var staffs = await _context.Staffs.FindAsync(id);
            if (staffs == null)
            {
                return NotFound();
            }
            return View(staffs);
        }

        // POST: Staffs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Surname,MiddleName,Post,WorkExperience")] Staffs staffs)
        {
            if (id != staffs.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(staffs);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaffsExists(staffs.Id))
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
            return View(staffs);
        }

        // GET: Staffs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Staffs == null)
            {
                return NotFound();
            }

            var staffs = await _context.Staffs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (staffs == null)
            {
                return NotFound();
            }

            return View(staffs);
        }

        // POST: Staffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Staffs == null)
            {
                return Problem("Entity set 'CinemaContext.Staffs'  is null.");
            }
            var staffs = await _context.Staffs.FindAsync(id);
            if (staffs != null)
            {
                _context.Staffs.Remove(staffs);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StaffsExists(int id)
        {
          return _context.Staffs.Any(e => e.Id == id);
        }

        private IEnumerable<Staffs> Search(IQueryable<Staffs> staffs, string name, string surname, string middleName, int workExp, string post)
        {
            return staffs.Where(s => s.Name.Contains(name) && s.MiddleName.Contains(middleName) &&
                                s.Surname.Contains(surname) && s.Post.Contains(post) && s.WorkExperience >= workExp);
        }

        private IEnumerable<Staffs> Sort(IEnumerable<Staffs> staffs, SortViewModel sortViewModel)
        {
            Func<Staffs, object> func = null;

            switch(sortViewModel.FieldName)
            {
                case "Name":
                    func = s => s.Name;
                    break;
                case "Surname":
                    func = s => s.Surname;
                    break;
                case "MiddleName":
                    func = s => s.MiddleName;
                    break;
                case "WorkExperience":
                    func = s => s.WorkExperience;
                    break;
                case "Post":
                    func = s => s.Post;
                    break;
                default:
                    func = s => s.Id;
                    break;
            }

            switch (sortViewModel.CurrentState)
            {
                case SortState.Ascending:
                    staffs = staffs.OrderBy(func);
                    break;
                case SortState.Descending:
                    staffs = staffs.OrderByDescending(func);
                    break;
            }

            return staffs;
        }
    }
}
