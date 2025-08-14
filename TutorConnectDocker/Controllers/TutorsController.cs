using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TutorConnectDocker.Models;

namespace TutorConnectDocker.Controllers
{
    public class TutorsController : Controller
    {
        private readonly TutorContext _context;

        public TutorsController(TutorContext context)
        {
            _context = context;
        }

        // GET: Tutors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tutors.ToListAsync());
        }

        // GET: Tutors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tutors = await _context.Tutors
                .FirstOrDefaultAsync(m => m.TutorId == id);
            if (tutors == null)
            {
                return NotFound();
            }

            return View(tutors);
        }

        // GET: Tutors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tutors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TutorId,FullName,Email,Subject")] Tutors tutors)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tutors);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tutors);
        }

        // GET: Tutors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tutors = await _context.Tutors.FindAsync(id);
            if (tutors == null)
            {
                return NotFound();
            }
            return View(tutors);
        }

        // POST: Tutors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TutorId,FullName,Email,Subject")] Tutors tutors)
        {
            if (id != tutors.TutorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tutors);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TutorsExists(tutors.TutorId))
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
            return View(tutors);
        }

        // GET: Tutors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tutors = await _context.Tutors
                .FirstOrDefaultAsync(m => m.TutorId == id);
            if (tutors == null)
            {
                return NotFound();
            }

            return View(tutors);
        }

        // POST: Tutors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tutors = await _context.Tutors.FindAsync(id);
            if (tutors != null)
            {
                _context.Tutors.Remove(tutors);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TutorsExists(int id)
        {
            return _context.Tutors.Any(e => e.TutorId == id);
        }
    }
}
