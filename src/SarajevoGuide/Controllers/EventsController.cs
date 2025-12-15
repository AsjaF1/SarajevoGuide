using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SarajevoGuide.Data;
using SarajevoGuide.Models;
using SarajevoGuide.Enums;

namespace SarajevoGuide.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Event.Select(e => new
            {
                e.Id,
                name = e.Name,
                description = e.Description,
                lat = e.Lat,
                lng = e.Lng,
                cost=e.Price,
                StartDate=e.StartDate,
                EndDate=e.EndDate,
                kategorija = e.Kategorija.ToString()
            }).ToListAsync();

            return Json(events);
        }

        [HttpGet]
        public async Task<IActionResult> GetByIds(string ids)
        {
            var idList = ids.Split(',').Select(int.Parse).ToList();

            var events = await _context.Event
                .Where(e => idList.Contains(e.Id))
                .Select(e => new
                {
                    e.Id,
                    name = e.Name,
                    description = e.Description,
                    kategorija = e.Kategorija.ToString()
                }).ToListAsync();

            return Json(events);
        }

        // GET: Events
        public async Task<IActionResult> Index(string category)
        {
            var events = _context.Event.AsQueryable();

            if (!string.IsNullOrEmpty(category) && Enum.TryParse(category, out Kategorija parsedCategory))
            {
                events = events.Where(e => e.Kategorija == parsedCategory);
            }

            return View(await events.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            PopulateKategorijaDropdown();
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Kategorija,Description,StartDate,EndDate,Lat,Lng,Price")] Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();

                // Check if the request is from the modal (AJAX)
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Event created successfully!" });
                }

                return RedirectToAction(nameof(Index));
            }

            // If validation failed and it's an AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                    .ToArray();
                return Json(new { success = false, errors = errors });
            }

            PopulateKategorijaDropdown(@event.Kategorija);
            return View(@event);
        }

        // GET: Events/Edit/5


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            PopulateKategorijaDropdown(@event.Kategorija);
            return View(@event);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Kategorija,Description,StartDate,EndDate,Lat,Lng,Price")] Event @event)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();

                    // Check if the request is from AJAX (modal)
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "Event updated successfully!" });
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
                    {
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            return Json(new { success = false, message = "Event not found." });
                        }
                        return NotFound();
                    }
                    else
                    {
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            return Json(new { success = false, message = "A concurrency error occurred. Please try again." });
                        }
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // If validation failed and it's an AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                    .ToArray();
                return Json(new { success = false, errors = errors });
            }

            PopulateKategorijaDropdown(@event.Kategorija);
            return View(@event);
        }

        // NEW: AJAX Edit method for modal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditModal([FromBody] EditEventRequest request)
        {
            if (request.Id <= 0)
            {
                return Json(new { success = false, message = "Invalid event ID." });
            }

            var existingEvent = await _context.Event.FindAsync(request.Id);
            if (existingEvent == null)
            {
                return Json(new { success = false, message = "Event not found." });
            }

            // Update properties
            existingEvent.Name = request.Name;
            existingEvent.Description = request.Description;
            existingEvent.StartDate = request.StartDate;
            existingEvent.EndDate = request.EndDate;
            // Fix for CS0266 and CS8629 errors
            existingEvent.Price = request.Price.HasValue ? (double)request.Price.Value : 0.0;
            existingEvent.Lat = request.Lat.HasValue ? request.Lat.Value : 0.0;
            existingEvent.Lng = request.Lng.HasValue ? request.Lng.Value : 0.0;

            // Parse and update category
            if (Enum.TryParse(request.Kategorija, out Kategorija parsedKategorija))
            {
                existingEvent.Kategorija = parsedKategorija;
            }

            // Validate the model
            TryValidateModel(existingEvent);
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                    .ToArray();
                return Json(new { success = false, errors = errors });
            }

            try
            {
                _context.Update(existingEvent);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Event updated successfully!" });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(existingEvent.Id))
                {
                    return Json(new { success = false, message = "Event not found." });
                }
                else
                {
                    return Json(new { success = false, message = "A concurrency error occurred. Please try again." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while updating the event." });
            }
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Event.FindAsync(id);
            if (@event != null)
            {
                _context.Event.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.Id == id);
        }

        private void PopulateKategorijaDropdown(object selectedKategorija = null)
        {
            var kategorijaList = from Kategorija k in Enum.GetValues(typeof(Kategorija))
                                 select new { Value = (int)k, Text = k.ToString() };
            ViewBag.Kategorija = new SelectList(kategorijaList, "Value", "Text", selectedKategorija);
        }
    }

    // Request model for AJAX edit
    public class EditEventRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Kategorija { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Price { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }
}