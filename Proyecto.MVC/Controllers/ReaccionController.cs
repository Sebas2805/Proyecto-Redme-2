using Proyecto.Core.Business;
using Proyecto.MVC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proyecto.MVC.Controllers
{
    public class ReaccionController : Controller
    {
        // GET: Reaccion
        public ActionResult Index()
        {
            return View();
        }

        // GET: Reaccion/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Reaccion/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Reaccion/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Reaccion/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Reaccion/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Reaccion/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Reaccion/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult ReaccionarComentario(int idComentario, string tipo)
        {
            try
            {
                int idUsuario = SessionHelper.UsuarioId ?? 1; // usuario logueado
                ReaccionBusiness reaccion = new ReaccionBusiness();

                // Guarda la reacción y devuelve info sobre el cambio
                string cambio = reaccion.GuardarReaccionConCambio(idUsuario, null, idComentario, tipo);

                return Json(new { success = true, message = "Reacción guardada correctamente.", cambio = cambio });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
