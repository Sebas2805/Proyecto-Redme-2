using Proyecto.Core.Business;
using Proyecto.Data;
using Proyecto.MVC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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

        private bool IsPostOwner(post post)
        {
            return post != null && UserId > 0 && post.id_usuario == UserId;
        }

        // GET: comunidades
        public async Task<ActionResult> Index(int comunidadId)
        {
            var posts = await _business.GetByComunidadId(comunidadId);
            var postIds = posts.Select(p => p.id_post).ToList();

            ViewBag.ComunidadId = comunidadId;
            ViewBag.LikeCounts = _business.GetLikeCountsByPost(postIds);
            ViewBag.DislikeCounts = _business.GetDislikeCountsByPost(postIds);
            ViewBag.UserReactions = UserId > 0
                ? _business.GetUserReactionByPost(postIds, UserId)
                : new Dictionary<int, string>();

            return View(posts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reaccionar(int idPost, string tipo)
        {
            if (UserId <= 0)
            {
                return Json(new { success = false, message = "Debes iniciar sesión para reaccionar." });
            }

            try
            {
                var tipoNormalizado = (tipo ?? string.Empty).Trim().ToLowerInvariant();
                _business.ReaccionarPost(UserId, idPost, tipoNormalizado);

                var likes = _business.GetLikeCountByPost(idPost);
                var dislikes = _business.GetDislikeCountByPost(idPost);
                var userReaction = _business.GetUserReactionByPost(idPost, UserId);

                return Json(new
                {
                    success = true,
                    likes,
                    dislikes,
                    userReaction
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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
        public ActionResult Create(int comunidadId)
        {
            if (UserId <= 0)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.id_creador = UserId;
            ViewBag.ComunidadId = comunidadId;

            return View();
        }

        // POST: comunidades/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "titulo,contenido")] post Ppost, int comunidadId)
        {
            if (UserId <= 0)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                Ppost.id_usuario = UserId;
                Ppost.fecha_publicacion = DateTime.Now;
                Ppost.id_comunidad = comunidadId;
                 
                _business.Crear(Ppost);

                return RedirectToAction("Index", new { comunidadId = comunidadId });
            }

            ViewBag.ComunidadId = comunidadId;
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
            if (!IsPostOwner(post))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            ViewBag.ComunidadId = post.id_comunidad;
            return View(post);
        }

        // POST: comunidades/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_post,titulo,contenido")] post posted)
        {
            var existing = posted != null ? _business.GetById(posted.id_post) : null;
            if (existing == null)
            {
                return HttpNotFound();
            }
            if (!IsPostOwner(existing))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (ModelState.IsValid)
            {
                existing.titulo = posted.titulo;
                existing.contenido = posted.contenido;
                _business.Actualizar(existing);
                return RedirectToAction("Index", new { comunidadId = existing.id_comunidad });
            }

            ViewBag.ComunidadId = existing.id_comunidad;
            return View(posted);
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
            if (!IsPostOwner(post))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            return View(post);
        }

        // POST: comunidades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            post post = _business.GetById(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            if (!IsPostOwner(post))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            int comunidadId = post.id_comunidad;
            _business.Eliminar(id);
            return RedirectToAction("Index", new { comunidadId = comunidadId });
        }
    }
}
