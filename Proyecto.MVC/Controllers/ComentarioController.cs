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
    public class ComentarioController : Controller
    {
        private ComentarioBusiness _business = new ComentarioBusiness();
        private PostBusiness _postBusiness = new PostBusiness();

        public ComentarioController()
        {
            _business = new ComentarioBusiness();
            _postBusiness = new PostBusiness();
        }

        // Id del usuario en sesion

        private int UserId
        {
            get
            {
                return SessionHelper.UsuarioId ?? 0;
            }
        }

        private bool IsComentarioOwner(comentario comentario)
        {
            return comentario != null && UserId > 0 && comentario.id_usuario == UserId;
        }

        // GET: hilo del post (post arriba, comentarios abajo)
        public async Task<ActionResult> Index(int postId)
        {
            var post = _postBusiness.GetById(postId);
            if (post == null)
            {
                return HttpNotFound();
            }

            var comentarios = await _business.GetByPostId(postId);
            var comentarioIds = comentarios.Select(c => c.id_comentario).ToList();

            ViewBag.PostId = postId;
            ViewBag.ComunidadId = post.id_comunidad;
            ViewBag.ThreadPost = post;
            ViewBag.PostLikeCount = _postBusiness.GetLikeCountByPost(postId);
            ViewBag.PostDislikeCount = _postBusiness.GetDislikeCountByPost(postId);
            ViewBag.PostUserReaction = UserId > 0
                ? (_postBusiness.GetUserReactionByPost(postId, UserId) ?? string.Empty).ToLowerInvariant()
                : string.Empty;

            ViewBag.LikeCounts = _business.GetLikeCountsByComentario(comentarioIds);
            ViewBag.DislikeCounts = _business.GetDislikeCountsByComentario(comentarioIds);
            ViewBag.UserReactions = UserId > 0
                ? _business.GetUserReactionByComentario(comentarioIds, UserId)
                : new Dictionary<int, string>();

            return View(comentarios);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reaccionar(int idComentario, string tipo)
        {
            if (UserId <= 0)
            {
                return Json(new { success = false, message = "Debes iniciar sesión para reaccionar." });
            }

            try
            {
                var tipoNormalizado = (tipo ?? string.Empty).Trim().ToLowerInvariant();
                _business.ReaccionarComentario(UserId, idComentario, tipoNormalizado);

                var likes = _business.GetLikeCountByComentario(idComentario);
                var dislikes = _business.GetDislikeCountByComentario(idComentario);
                var userReaction = _business.GetUserReactionByComentario(idComentario, UserId);

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
            comentario comentario = _business.GetById(id.Value);
            if (comentario == null)
            {
                return HttpNotFound();
            }
            return View(comentario);
        }

        // GET: comunidades/Create
        public ActionResult Create(int postId)
        {
            if (UserId <= 0)
            {
                return RedirectToAction("Login", "Account");
            }

            if (_postBusiness.GetById(postId) == null)
            {
                return HttpNotFound();
            }

            ViewBag.PostId = postId;
            ViewBag.UsuarioId = UserId;

            return View();
        }

        // POST: comunidades/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "contenido")] comentario Pcomentario, int postId)
        {
            if (UserId <= 0)
            {
                return RedirectToAction("Login", "Account");
            }

            if (_postBusiness.GetById(postId) == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                Pcomentario.id_usuario = UserId;
                Pcomentario.fecha_comentario = DateTime.Now;
                Pcomentario.id_post = postId;

                _business.Crear(Pcomentario);

                return RedirectToAction("Index", new { postId = postId });
            }

            ViewBag.PostId = postId;
            return View(Pcomentario);
        }

        // GET: comunidades/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            comentario comentario = _business.GetById(id.Value);
            if (comentario == null)
            {
                return HttpNotFound();
            }
            if (!IsComentarioOwner(comentario))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            ViewBag.PostId = comentario.id_post;
            return View(comentario);
        }

        // POST: comunidades/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_comentario,contenido")] comentario posted)
        {
            var existing = posted != null ? _business.GetById(posted.id_comentario) : null;
            if (existing == null)
            {
                return HttpNotFound();
            }
            if (!IsComentarioOwner(existing))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (ModelState.IsValid)
            {
                existing.contenido = posted.contenido;
                _business.Actualizar(existing);
                return RedirectToAction("Index", new { postId = existing.id_post });
            }

            ViewBag.PostId = existing.id_post;
            return View(posted);
        }

        // GET: comunidades/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            comentario comentario = _business.GetById(id.Value);
            if (comentario == null)
            {
                return HttpNotFound();
            }
            if (!IsComentarioOwner(comentario))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            return View(comentario);
        }

        // POST: comunidades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            comentario comentario = _business.GetById(id);
            if (comentario == null)
            {
                return HttpNotFound();
            }
            if (!IsComentarioOwner(comentario))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            int postId = comentario.id_post;
            _business.Eliminar(id);
            return RedirectToAction("Index", new { postId = postId });
        }
    }
}
