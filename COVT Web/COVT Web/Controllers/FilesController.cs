using COVT_Web.Models;
using COVT_Web.Models.DB;
using COVT_Web.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace COVT_Web.Controllers
{
    public class FilesController : Controller
    {
        ApplicationContext db;

        public FilesController(ApplicationContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Files(string poisk)
        {
            ViewData["Poiskk"] = poisk;
            IQueryable<FilesView> am = (from k in db.files
                                             join p in db.patsienti on k.id_patsienta equals p.id_patsienta
                                             select new FilesView
                                             {
                                                 id_file = k.id_file,
                                                 fio = p.familiya,
                                                 opisanie =k.opisanie,
                                                 date = k.date
                                             }).OrderByDescending(x => x.date);

            if (!string.IsNullOrEmpty(poisk))
            {
                am = am.Where(x => x.fio.Contains(poisk));
            }
            var result = await am.ToListAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_PartialViewFiles", result); 
            }
            else
            {
                return View(result); // в остальных случаях возвращаем полное представление
            }
        }

        public async Task<IActionResult> Create()
        {
            var patsientidata = await db.patsienti.ToListAsync();
            ViewBag.PatsientiDb = new SelectList(patsientidata, "id_patsienta", "familiya");
            return View();

        }

        [HttpPost]
        public async Task<ActionResult> Create(FilesDb model, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                byte[] fileData = null;

                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    fileData = memoryStream.ToArray();
                }

                model.name = file.FileName;
                model.Fl = fileData;

                db.files.Add(model);
                await db.SaveChangesAsync();
            }

            return RedirectToAction("Files");
        }

        public ActionResult OpenFile(int id)
        {
            var fileData = db.files.Where(f => f.id_file == id).Select(f => f.Fl).FirstOrDefault();
            var fileName = db.files.Where(f => f.id_file == id).Select(f => f.name).FirstOrDefault();

            string contentType = "application/octet-stream"; // Другой MIME тип, если нужно

            return File(fileData, contentType, fileName);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                FilesDb? file = await db.files.FirstOrDefaultAsync(p => p.id_file == id);
                if (file != null)
                {
                    db.files.Remove(file);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Files");
                }
            }
            return NotFound();
        }
    }
}
