using COVT_Web.Models;
using COVT_Web.Models.DB;
using COVT_Web.Models.ViewModel;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NPOI.XWPF.UserModel;
using System.Diagnostics;
using System.Reflection;
using System.Web;
using Word = Microsoft.Office.Interop.Word;


namespace COVT_Web.Controllers
{
    public class AmbulatorkaController : Controller
    {
        ApplicationContext db;

        public AmbulatorkaController(ApplicationContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<IActionResult> Ambulatorka(string poisk)
        {
            ViewData["Poiskk"] = poisk;
            IQueryable<AmbulatorkaView> am = (from k in db.karta_patsienta
                                              join p in db.patsienti on k.id_patsienta equals p.id_patsienta
                                              select new AmbulatorkaView
                                              {
                                                  id_karti_patsienta = k.id_karti_patsienta,
                                                  fio = p.familiya,
                                                  data_priema = k.data_priema
                                              }).OrderByDescending(x => x.data_priema);

            if (!string.IsNullOrEmpty(poisk))
            {
                am = am.Where(x => x.fio.Contains(poisk));
            }
            var result = await am.ToListAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_PartialViewAmbulatorka", result); // возвращаем результат как частичное представление
            }
            else
            {
                return View(result); // в остальных случаях возвращаем полное представление
            }
        }

        public async Task<IActionResult> KartaAmView(int? id)
        {
            var kartaAm = await (from k in db.karta_patsienta
                                 join p in db.patsienti on k.id_patsienta equals p.id_patsienta
                                 join b in db.bolezni on k.id_bolezni equals b.id_bolezni
                                 join v in db.vrachi on k.id_vracha equals v.id_vracha
                                 where k.id_karti_patsienta == id
                                 select new KartaAmView
                                 {
                                     id_karti_patsienta = k.id_karti_patsienta,
                                     data_priema = k.data_priema,
                                     id_bolezni = b.id_bolezni,
                                     bolezn = b.nazvanie,
                                     id_kobineta = k.id_kobineta,
                                     id_patsienta = p.id_patsienta,
                                     patsient = p.familiya,
                                     adres = p.adres,
                                     dr=p.data_rozhdeniya,
                                     id_vracha = v.id_vracha,
                                     vrach = v.familiya,
                                     zhalobi = k.zhalobi,
                                     ist_zab = k.ist_zab,
                                     nast_stat = k.nast_stat,
                                     mest_stat = k.mest_stat,
                                     dop_met_obsl = k.dop_met_obsl,
                                     diagnoz = k.diagnoz,
                                     plan_obsl = k.plan_obsl,
                                     plan_lech = k.plan_lech,
                                     zakl = k.zakl
                                 }).FirstOrDefaultAsync();
            var files = db.files.Where(f => f.id_patsienta == kartaAm.id_patsienta).ToList();

            ViewBag.Files = files;

            return View(kartaAm);


        }

        [HttpPost]
        public async Task<IActionResult> KartaAmView(KartaAmView model, int id)
        {
            var ambulatorka = db.karta_patsienta.Find(id);
            ambulatorka.zhalobi = model.zhalobi;
            ambulatorka.ist_zab = model.ist_zab;
            ambulatorka.nast_stat = model.nast_stat;
            ambulatorka.mest_stat = model.mest_stat;
            ambulatorka.dop_met_obsl = model.dop_met_obsl;
            ambulatorka.diagnoz = model.diagnoz;
            ambulatorka.plan_obsl = model.plan_obsl;
            ambulatorka.plan_lech = model.plan_lech;
            ambulatorka.zakl = model.zakl;
            db.karta_patsienta.Update(ambulatorka);
            await db.SaveChangesAsync();
            return RedirectToAction("Ambulatorka");
        }

        public async Task<IActionResult> Create()
        {
            var vrachidata = await db.vrachi.ToListAsync();
            ViewBag.VrachiDb = new SelectList(vrachidata, "id_vracha", "familiya");
            var patsientidata = await db.patsienti.ToListAsync();
            ViewBag.PatsientiDb = new SelectList(patsientidata, "id_patsienta", "familiya");
            var boleznidata = await db.bolezni.ToListAsync();
            ViewBag.BolezniDb = new SelectList(boleznidata, "id_bolezni", "nazvanie");
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Create(AmbulatorkaDb karta)
        {
            db.karta_patsienta.Add(karta);
            await db.SaveChangesAsync();

            // Возвращение обновленного представления
            return RedirectToAction("Ambulatorka");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                AmbulatorkaDb? karta = await db.karta_patsienta.FirstOrDefaultAsync(p => p.id_karti_patsienta == id);
                if (karta != null)
                {
                    db.karta_patsienta.Remove(karta);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Ambulatorka");
                }
            }
            return NotFound();
        }

        public JsonResult GetInfoBolezni(int id)
        {

            var bolezn = (from b in db.bolezni
                          where b.id_bolezni == id
                          select b).FirstOrDefault();
            return Json(new
            {
                zhalobi = bolezn.zhalobi,
                ist_zab = bolezn.ist_zab,
                nast_stat = bolezn.nast_stat,
                mest_stat = bolezn.mest_stat,
                dmo = bolezn.dop_met_obsl,
                diagnoz = bolezn.diagnoz,
                plan_obsl = bolezn.plan_obsl,
                plan_lech = bolezn.plan_lech,
                zakl = bolezn.zakl
            });
        }

        [HttpPost]
        public ActionResult GenerateDocument(KartaAmView model)
        {
            /// Путь к шаблонному файлу и путь к сгенерированному файлу
            string templateFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", "выписка.docx");

            // Открываем шаблонный файл docx через поток данных
            using (FileStream fileStream = new FileStream(templateFilePath, FileMode.Open))
            {
                var memoryStream = new MemoryStream();
                using var docFile = new XWPFDocument(fileStream);
                foreach (var paragraph in docFile.Paragraphs)
                {
                    var text = paragraph.Text;

                    paragraph.ReplaceText("<zhalobi>", model.zhalobi);
                    paragraph.ReplaceText("<ist_zab>", model.ist_zab);
                    paragraph.ReplaceText("<nast_stat>", model.nast_stat);
                    paragraph.ReplaceText("<mest_stat>", model.mest_stat);
                    paragraph.ReplaceText("<dmo>", model.dop_met_obsl);
                    paragraph.ReplaceText("<plan_obsl>", model.plan_obsl);
                    paragraph.ReplaceText("<plan_lech>", model.plan_lech);
                    paragraph.ReplaceText("<zakl>", model.zakl);
                    paragraph.ReplaceText("<diagnoz>", model.diagnoz);
                    paragraph.ReplaceText("<patsient>", model.patsient);
                    paragraph.ReplaceText("<data_postupleniya>", model.data_priema.ToString());
                    paragraph.ReplaceText("<adres>", model.adres);
                    paragraph.ReplaceText("<dr>", model.dr.ToString());
                }
                docFile.Write(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                var fileName = HttpUtility.UrlEncode($"Выписка_{model.patsient}.docx");
                return File(memoryStream.ToArray(), "application/octet-stream", fileName);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Otpravka(int id)
        {
            var model = db.karta_patsienta.Find(id);
            var kartast = new StatsionarDb
            {
                id_bolezni = model.id_bolezni,
                id_patsienta = model.id_patsienta,
                id_vracha = model.id_vracha,
                data_postupleniya = System.DateTime.Now,
                zhalobi = model.zhalobi,
                ist_zab = model.ist_zab,
                nast_stat = model.nast_stat,
                mest_stat = model.mest_stat,
                dop_met_obsl = model.dop_met_obsl,
                diagnoz = model.diagnoz,
                plan_lech = model.plan_lech,
                plan_obsl = model.plan_obsl,
                zakl = model.zakl
            };
            db.karta_lecheniya.Add(kartast);
            await db.SaveChangesAsync();
            return RedirectToAction("Statsionar", "Statsionar");
        }

        [HttpPost]
        public async Task<IActionResult> Dubl(int id)
        {
            var model = db.karta_patsienta.Find(id);
            var kartaam = new AmbulatorkaDb
            {
                id_bolezni = model.id_bolezni,
                id_patsienta = model.id_patsienta,
                id_vracha = model.id_vracha,
                data_priema = System.DateTime.Now,
                zhalobi = model.zhalobi,
                ist_zab = model.ist_zab,
                nast_stat = model.nast_stat,
                mest_stat = model.mest_stat,
                dop_met_obsl = model.dop_met_obsl,
                diagnoz = model.diagnoz,
                plan_lech = model.plan_lech,
                plan_obsl = model.plan_obsl,
                zakl = model.zakl
            };
            db.karta_patsienta.Add(kartaam);
            await db.SaveChangesAsync();
            return RedirectToAction("Ambulatorka");
        }
    }
}
