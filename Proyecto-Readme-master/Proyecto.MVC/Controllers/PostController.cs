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
    public class PostController : Controller
    {
        private PostBusiness _business = new PostBusiness();

        public PostController()
        {
            _business = new PostBusiness();
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
            var posts = _business.GetAll();
            return View(posts.ToList());
        }

        // GET: comunidades/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            post post = _business.GetById(id.Value);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // GET: comunidades/Create
        public ActionResult Create()
        {
            ViewBag.id_creador = SessionHelper.UsuarioId;
            return View();
        }

        // POST: comunidades/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "titulo,contenido")] post Ppost)
        {
            if (ModelState.IsValid)
            {
                Ppost.id_usuario = UserId;
                Ppost.fecha_publicacion = DateTime.Now;
                Ppost.id_comunidad = 1; // Parametro temporal !!!

                _business.Crear(Ppost);
                return RedirectToAction("Index");
            }

            return View(Ppost);
        }

        // GET: comunidades/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            post post = _business.GetById(id.Value);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: comunidades/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_post,titulo,contenido,fecha_publicacion,id_usuario,id_comunidad")] post post)
        {
            if (ModelState.IsValid)
            {
                _business.Actualizar(post);
                return RedirectToAction("Index");
            }
            return View(post);
        }

        // GET: comunidades/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            post post = _business.GetById(id.Value);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: comunidades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            post post = _business.GetById(id);
            _business.Eliminar(id);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult ReaccionarPost(int idPost, string tipo)
        {
            try
            {
                int idUsuario = SessionHelper.UsuarioId ?? 1; // toma usuario de sesión
                ReaccionBusiness reaccion = new ReaccionBusiness();
                reaccion.GuardarReaccion(idUsuario, idPost, null, tipo);
                return Json(new { success = true, message = "Reacción guardada correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }
        [HttpPost]
        public ActionResult ReaccionarComentario(int idComentario, string tipo)
        {
            try
            {
                int idUsuario = SessionHelper.UsuarioId ?? 1; // usuario logueado
                ReaccionBusiness reaccion = new ReaccionBusiness();
                reaccion.GuardarReaccion(idUsuario, null, idComentario, tipo); // idPost=null
                return Json(new { success = true, message = "Reacción guardada correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
