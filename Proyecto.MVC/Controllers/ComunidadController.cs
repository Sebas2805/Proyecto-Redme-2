using Proyecto.Core.Business;
using Proyecto.Data;
using Proyecto.MVC.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Proyecto.MVC.Controllers
{
    // Solo usuarios loggeados pueden acceder a crear comunidades (Por segunda avance mantiene todo el controller restringido)
    //[Authorize]
    public class ComunidadController : Controller
    {
        private ComunidadBusiness _business = new ComunidadBusiness();

        public ComunidadController()
        {
            _business = new ComunidadBusiness();
        }

        // Id del usuario en sesion

        private int UserId
        {
            get
            {
                return SessionHelper.UsuarioId ?? 0;
            }
        }

        // GET: comunidades
        public ActionResult Index()
        {
            var comunidads = _business.GetAll();
            return View(comunidads.ToList());
        }

        // GET: comunidades/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            comunidad comunidad = _business.GetById(id.Value);
            if (comunidad == null)
            {
                return HttpNotFound();
            }
            return View(comunidad);
        }

        // GET: comunidades/Create
        public ActionResult Create()
        {
            var model = new Proyecto.Data.comunidad
            {
                id_creador = SessionHelper.UsuarioId ?? 0
            };
            return View(model);
        }

        // POST: comunidades/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "nombre,descripcion")] comunidad Pcomunidad)
        {
            if (ModelState.IsValid)
            {
                Pcomunidad.id_creador = UserId;
                Pcomunidad.fecha_creacion = DateTime.Now;

                _business.Crear(Pcomunidad);
                return RedirectToAction("Index");
            }

            return View(Pcomunidad);
        }

        // GET: comunidades/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            comunidad comunidad = _business.GetById(id.Value);
            if (comunidad == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_creador = this.UserId;
            return View(comunidad);
        }

        // POST: comunidades/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_comunidad,nombre,descripcion,fecha_creacion,id_creador")] comunidad comunidad)
        {
            if (ModelState.IsValid)
            {
                _business.Actualizar(comunidad);
                return RedirectToAction("Index");
            }
            ViewBag.id_creador = this.UserId;
            return View(comunidad);
        }

        // GET: comunidades/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            comunidad comunidad = _business.GetById(id.Value);
            if (comunidad == null)
            {
                return HttpNotFound();
            }
            return View(comunidad);
        }

        // POST: comunidades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            comunidad comunidad = _business.GetById(id);
            _business.Eliminar(id);
            return RedirectToAction("Index");
        }
    }
}
