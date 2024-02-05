using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Dapper;
using COVT_Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using COVT_Web.Models.DB;

namespace COVT_Web.Controllers
{
    public class PatsientiController : Controller
    {
        private readonly IConfiguration _config;

        ApplicationContext db;

        public PatsientiController(ApplicationContext context)
        {
            db = context;
        }



        public async Task<IActionResult> Patsienti(string poisk)
        {
            ViewData["Poiskk"]=poisk;
            var patsienti = from p in db.patsienti select p;
            if (!String.IsNullOrEmpty(poisk))
            {
                patsienti = patsienti.Where(p => p.familiya.Contains(poisk));
            }
            var result = await patsienti.ToListAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_PartialViewPatsienti", result); // возвращаем результат как частичное представление
            }
            else
            {
                return View(result); // в остальных случаях возвращаем полное представление
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create ()
        {
            var vrachidata = await db.vrachi.ToListAsync();
            ViewBag.VrachiDb = new SelectList(vrachidata, "id_vracha", "familiya");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PatsientiDb patsient)
        {
            db.patsienti.Add(patsient);
            await db.SaveChangesAsync();
            return RedirectToAction("Patsienti");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                PatsientiDb? patsient = await db.patsienti.FirstOrDefaultAsync(p => p.id_patsienta== id);
                if (patsient!= null)
                {
                    db.patsienti.Remove(patsient);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Patsienti");
                }
            }
            return NotFound();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                PatsientiDb? patsient = await db.patsienti.FirstOrDefaultAsync(p => p.id_patsienta== id);
                if (patsient != null) return View(patsient);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PatsientiDb patsient, int id)
        {
            patsient.id_patsienta = id;
            db.patsienti.Attach(patsient);
            db.Entry(patsient).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Patsienti");
        }
    }
}
