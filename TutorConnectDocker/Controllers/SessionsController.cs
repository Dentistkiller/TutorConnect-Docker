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
    public class SessionsController : Controller
    {
        private readonly TutorContext _context;

        public SessionsController(TutorContext context)
        {
            _context = context;
        }

        // GET: Sessions
        public async Task<IActionResult> Index()
        {
            var tutorContext = _context.Sessions.Include(s => s.Student).Include(s => s.Tutor);
            return View(await tutorContext.ToListAsync());
        }

        // GET: Sessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sessions = await _context.Sessions
                .Include(s => s.Student)
                .Include(s => s.Tutor)
                .FirstOrDefaultAsync(m => m.SessionId == id);
            if (sessions == null)
            {
                return NotFound();
            }

            return View(sessions);
        }

        // GET: Sessions/Create
        public IActionResult Create()
        {
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "FullName");
            ViewData["TutorId"] = new SelectList(_context.Tutors, "TutorId", "TutorId");
            return View();
        }

        // POST: Sessions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SessionId,StudentId,TutorId,SessionDate,DurationMinutes")] Sessions sessions)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sessions);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "StudentId", sessions.StudentId);
            ViewData["TutorId"] = new SelectList(_context.Tutors, "TutorId", "TutorId", sessions.TutorId);
            return View(sessions);
        }

        // GET: Sessions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sessions = await _context.Sessions.FindAsync(id);
            if (sessions == null)
            {
                return NotFound();
            }
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "StudentId", sessions.StudentId);
            ViewData["TutorId"] = new SelectList(_context.Tutors, "TutorId", "TutorId", sessions.TutorId);
            return View(sessions);
        }

        // POST: Sessions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SessionId,StudentId,TutorId,SessionDate,DurationMinutes")] Sessions sessions)
        {
            if (id != sessions.SessionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sessions);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SessionsExists(sessions.SessionId))
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
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "StudentId", sessions.StudentId);
            ViewData["TutorId"] = new SelectList(_context.Tutors, "TutorId", "TutorId", sessions.TutorId);
            return View(sessions);
        }

        // GET: Sessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sessions = await _context.Sessions
                .Include(s => s.Student)
                .Include(s => s.Tutor)
                .FirstOrDefaultAsync(m => m.SessionId == id);
            if (sessions == null)
            {
                return NotFound();
            }

            return View(sessions);
        }

        // POST: Sessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sessions = await _context.Sessions.FindAsync(id);
            if (sessions != null)
            {
                _context.Sessions.Remove(sessions);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SessionsExists(int id)
        {
            return _context.Sessions.Any(e => e.SessionId == id);
        }
    }
}
