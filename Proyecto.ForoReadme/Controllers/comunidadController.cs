using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Proyecto.Data;

namespace Proyecto.ForoReadme.Controllers
{
    public class comunidadController : Controller
    {
        private README_DBEntities db = new README_DBEntities();

        // GET: comunidad
        public async Task<ActionResult> Index()
        {
            var comunidad = db.comunidad.Include(c => c.usuario);
            return View(await comunidad.ToListAsync());
        }

        // GET: comunidad/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            comunidad comunidad = await db.comunidad.FindAsync(id);
            if (comunidad == null)
            {
                return HttpNotFound();
            }
            return View(comunidad);
        }

        // GET: comunidad/Create
        public ActionResult Create()
        {
            ViewBag.id_creador = new SelectList(db.usuario, "id_usuario", "nombre_usuario");
            return View();
        }

        // POST: comunidad/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id_comunidad,nombre,descripcion")] comunidad comunidad)
        {
            // Asignar valores en el servidor para evitar overposting y garantizar FK válido
            comunidad.fecha_creacion = DateTime.Now;
            comunidad.id_creador = 1;

            if (ModelState.IsValid)
            {
                db.comunidad.Add(comunidad);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.id_creador = new SelectList(db.usuario, "id_usuario", "nombre_usuario", comunidad.id_creador);
            return View(comunidad);
        }

        // GET: comunidad/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            comunidad comunidad = await db.comunidad.FindAsync(id);
            if (comunidad == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_creador = new SelectList(db.usuario, "id_usuario", "nombre_usuario", comunidad.id_creador);
            return View(comunidad);
        }

        // POST: comunidad/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id_comunidad,nombre,descripcion,fecha_creacion,id_creador")] comunidad comunidad)
        {
            if (ModelState.IsValid)
            {
                // Corregido: actualizar la entidad en lugar de añadir una nueva
                db.Entry(comunidad).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.id_creador = new SelectList(db.usuario, "id_usuario", "nombre_usuario", comunidad.id_creador);
            return View(comunidad);
        }

        // GET: comunidad/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            comunidad comunidad = await db.comunidad.FindAsync(id);
            if (comunidad == null)
            {
                return HttpNotFound();
            }
            return View(comunidad);
        }

        // POST: comunidad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            comunidad comunidad = await db.comunidad.FindAsync(id);
            db.comunidad.Remove(comunidad);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
} 