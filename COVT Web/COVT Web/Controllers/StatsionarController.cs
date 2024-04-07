using COVT_Web.Models;
using COVT_Web.Models.DB;
using COVT_Web.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NPOI.XWPF.UserModel;
using System.Drawing;
using System.Web;

namespace COVT_Web.Controllers
{
    public class StatsionarController : Controller
    {
        ApplicationContext db;

        public StatsionarController(ApplicationContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<IActionResult> Statsionar (string poisk)
        {
            ViewData["Poiskk"] = poisk;
            IQueryable<StatsionarView> am = (from k in db.karta_lecheniya
                                              join p in db.patsienti on k.id_patsienta equals p.id_patsienta
                                              select new StatsionarView
                                              {
                                                  id_karti = k.id_karti,
                                                  fio = p.familiya,
                                                  data_postupleniya = k.data_postupleniya
                                              }).OrderByDescending(x => x.data_postupleniya);

            if (!string.IsNullOrEmpty(poisk))
            {
                am = am.Where(x => x.fio.Contains(poisk));
            }
            var result = await am.ToListAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_PartialViewStatsionar", result); // возвращаем результат как частичное представление
            }
            else
            {
                return View(result); // в остальных случаях возвращаем полное представление
            }
        }

        public async Task<IActionResult> KartaStView(int? id)
        {

            var kartaSt = await (from k in db.karta_lecheniya
                                 join p in db.patsienti on k.id_patsienta equals p.id_patsienta
                                 join b in db.bolezni on k.id_bolezni equals b.id_bolezni
                                 join v in db.vrachi on k.id_vracha equals v.id_vracha
                                 where k.id_karti == id
                                 select new KartaStView
                                 {
                                     id_karti = k.id_karti,
                                     data_postupleniya = k.data_postupleniya,
                                     id_bolezni = b.id_bolezni,
                                     bolezn = b.nazvanie,
                                     id_patsienta = p.id_patsienta,
                                     patsient = p.familiya,
                                     adres = p.adres,
                                     dr = p.data_rozhdeniya,
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
                                     nablyudenie = k.nablyudenie,
                                     khod_operat = k.khod_operat,
                                     zakl = k.zakl,
                                     vip_epikr = k.vip_epikr,
                                     data_vipiski = k.data_vipiski,
                                     predoper = k.predoper,
                                     osman = k.osman,
                                     osmspets = k.osmspets,
                                     ist_zhizni = k.ist_zhizni
                                 }).FirstOrDefaultAsync();

            var files = db.files.Where(f => f.id_patsienta == kartaSt.id_patsienta).ToList();

            ViewBag.Files = files;
            return View(kartaSt);
        }

        [HttpPost]
        public async Task<IActionResult> KartaStView(KartaStView model, int id)
        {
            var statsionar = db.karta_lecheniya.Find(id);
            statsionar.zhalobi = model.zhalobi;
            statsionar.ist_zab = model.ist_zab;
            statsionar.nast_stat = model.nast_stat;
            statsionar.mest_stat = model.mest_stat;
            statsionar.dop_met_obsl = model.dop_met_obsl;
            statsionar.diagnoz = model.diagnoz;
            statsionar.plan_obsl = model.plan_obsl;
            statsionar.plan_lech = model.plan_lech;
            statsionar.nablyudenie = model.nablyudenie;
            statsionar.khod_operat = model.khod_operat;
            statsionar.zakl = model.zakl;
            statsionar.vip_epikr = model.vip_epikr;
            statsionar.data_vipiski = model.data_vipiski;
            statsionar.predoper = model.predoper;
            statsionar.osman = model.osman;
            statsionar.osmspets = model.osmspets;
            statsionar.ist_zhizni = model.ist_zhizni;
            statsionar.data_vipiski = model.data_vipiski;
            db.karta_lecheniya.Update(statsionar);
            await db.SaveChangesAsync();
            return RedirectToAction("KaratStView");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                StatsionarDb? karta = await db.karta_lecheniya.FirstOrDefaultAsync(p => p.id_karti == id);
                if (karta != null)
                {
                    db.karta_lecheniya.Remove(karta);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Statsionar");
                }
            }
            return NotFound();
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
        public async Task<IActionResult> Create(StatsionarDb karta)
        {
            db.karta_lecheniya.Add(karta);
            await db.SaveChangesAsync();

            // Возвращение обновленного представления
            return RedirectToAction("Statsionar");
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
        public ActionResult GenerateDocument(KartaStView model)
        {
            /// Путь к шаблонному файлу и путь к сгенерированному файлу
            string templateFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", "выписка2.docx");

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
                    paragraph.ReplaceText("<ist_zhizni>", model.ist_zhizni);
                    paragraph.ReplaceText("<nast_stat>", model.nast_stat);
                    paragraph.ReplaceText("<mest_stat>", model.mest_stat);
                    paragraph.ReplaceText("<dmo>", model.dop_met_obsl);
                    paragraph.ReplaceText("<plan_obsl>", model.plan_obsl);
                    paragraph.ReplaceText("<plan_lech>", model.plan_lech);
                    paragraph.ReplaceText("<nabl>", model.nablyudenie);
                    paragraph.ReplaceText("<osan>", model.osman);
                    paragraph.ReplaceText("<ossp>", model.osmspets);
                    paragraph.ReplaceText("<peredoper>", model.predoper);
                    paragraph.ReplaceText("<khodop>", model.khod_operat);
                    paragraph.ReplaceText("<vipisn>", model.vip_epikr);
                    paragraph.ReplaceText("<zakl>", model.zakl);
                    paragraph.ReplaceText("<diagnoz>", model.diagnoz);
                    paragraph.ReplaceText("<adres>", model.adres);
                    paragraph.ReplaceText("<dr>", model.dr.ToString());
                    paragraph.ReplaceText("<patsient>", model.patsient);
                    paragraph.ReplaceText("<data_postupleniya>", model.data_postupleniya.ToString());
                    paragraph.ReplaceText("<data_vipiski>", model.data_vipiski.ToString());
                }
                docFile.Write(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                var fileName = HttpUtility.UrlEncode($"Выписка_{model.patsient}.docx");
                return File(memoryStream.ToArray(), "application/octet-stream", fileName);
            }
        }
    }
}
