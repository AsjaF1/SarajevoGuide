using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SarajevoGuide.Data;
using SarajevoGuide.Models;

namespace SarajevoGuide.Controllers
{
    public class BookmarksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookmarksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookmarks
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Bookmark.Include(b => b.Event).Include(b => b.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Bookmarks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookmark = await _context.Bookmark
                .Include(b => b.Event)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookmark == null)
            {
                return NotFound();
            }

            return View(bookmark);
        }

       

        [HttpGet]
        public async Task<IActionResult> Exists(int userId, int eventId)
        {
            var exists = await _context.Bookmark
                .AnyAsync(b => b.UserId == userId && b.EventId == eventId);

            return Ok(new { exists });
        }



        // POST: Bookmarks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] Bookmark bookmark)
        {
    

            bookmark.UserId = 1;

            _context.Bookmark.Add(bookmark);
            await _context.SaveChangesAsync();
            return Ok();
        }


        // GET: Bookmarks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookmark = await _context.Bookmark.FindAsync(id);
            if (bookmark == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Event, "Id", "Id", bookmark.EventId);
            ViewData["UserId"] = new SelectList(_context.RegistrovaniKorisnik, "id", "id", bookmark.UserId);
            return View(bookmark);
        }

        // POST: Bookmarks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EventId,UserId")] Bookmark bookmark)
        {
            if (id != bookmark.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookmark);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookmarkExists(bookmark.Id))
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
            ViewData["EventId"] = new SelectList(_context.Event, "Id", "Id", bookmark.EventId);
            ViewData["UserId"] = new SelectList(_context.RegistrovaniKorisnik, "id", "id", bookmark.UserId);
            return View(bookmark);
        }

        // GET: Bookmarks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookmark = await _context.Bookmark
                .Include(b => b.Event)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookmark == null)
            {
                return NotFound();
            }

            return View(bookmark);
        }

        // POST: Bookmarks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookmark = await _context.Bookmark.FindAsync(id);
            if (bookmark != null)
            {
                _context.Bookmark.Remove(bookmark);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookmarkExists(int id)
        {
            return _context.Bookmark.Any(e => e.Id == id);
        }
    }
}
