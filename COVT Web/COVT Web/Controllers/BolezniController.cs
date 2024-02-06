using COVT_Web.Models;
using COVT_Web.Models.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace COVT_Web.Controllers
{
    public class BolezniController : Controller
    {
        ApplicationContext db;

        public BolezniController(ApplicationContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Bolezni(string poisk)
        {
            ViewData["Poiskk"] = poisk;
            var bolezni = from p in db.bolezni select p;
            if (!String.IsNullOrEmpty(poisk))
            {
                bolezni = bolezni.Where(p => p.nazvanie.Contains(poisk));
            }
            var result = await bolezni.ToListAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_PartialViewBolezni", result); // возвращаем результат как частичное представление
            }
            else
            {
                return View(result); // в остальных случаях возвращаем полное представление
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(BolezniDb bolezn)
        {
            db.bolezni.Add(bolezn);
            await db.SaveChangesAsync();
            return RedirectToAction("Bolezni");
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                BolezniDb? bolezn = await db.bolezni.FirstOrDefaultAsync(p => p.id_bolezni == id);
                if (bolezn != null)
                {
                    db.bolezni.Remove(bolezn);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Bolezni");
                }
            }
            return NotFound();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                BolezniDb? bolezn = await db.bolezni.FirstOrDefaultAsync(p => p.id_bolezni == id);
                if (bolezn != null) return View(bolezn);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BolezniDb bolezn, int id)
        {
            bolezn.id_bolezni= id;
            db.bolezni.Attach(bolezn);
            db.Entry(bolezn).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Bolezni");
        }
    }
}
