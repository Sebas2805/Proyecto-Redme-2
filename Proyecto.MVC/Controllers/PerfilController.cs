using Proyecto.Core.Business;
using System.Web;
using System.Web.Mvc;

namespace Proyecto.MVC.Controllers
{
    public class PerfilController : Controller
    {
        private readonly UsuarioBusiness _usuarioBusiness = new UsuarioBusiness();
        private readonly PostBusiness _postBusiness = new PostBusiness();
        private readonly ComunidadBusiness _comunidadBusiness = new ComunidadBusiness();
        private readonly ComentarioBusiness _comentarioBusiness = new ComentarioBusiness();

        public ActionResult Index(int? id)
        {
            int userId = id ?? (int)(Session["UsuarioId"] ?? 0);

            if (userId <= 0)
                return RedirectToAction("Login", "Account");

            var usuario = _usuarioBusiness.GetById(userId);

            if (usuario == null)
                return HttpNotFound();

            ViewBag.Posts = _postBusiness.GetByUsuarioId(userId);
            ViewBag.Comunidades = _comunidadBusiness.GetByCreadorId(userId);
            ViewBag.Comentarios = _comentarioBusiness.GetByUsuarioId(userId);

            return View(usuario);
        }

        public ActionResult Editar(int? id)
        {
            int userId = id ?? (int)(Session["UsuarioId"] ?? 0);

            if (userId <= 0)
                return RedirectToAction("Login", "Account");

            var usuario = _usuarioBusiness.GetById(userId);

            if (usuario == null)
                return HttpNotFound();

            return View(usuario);
        }

        [HttpPost]
        public ActionResult Editar(Proyecto.Data.usuario user, HttpPostedFileBase foto)
        {
            var usuarioDb = _usuarioBusiness.GetById(user.id_usuario);

            if (usuarioDb == null)
                return HttpNotFound();

            usuarioDb.nombre_usuario = user.nombre_usuario;

            if (foto != null && foto.ContentLength > 0)
            {
                string carpeta = Server.MapPath("~/Fotos/");

                if (!System.IO.Directory.Exists(carpeta))
                    System.IO.Directory.CreateDirectory(carpeta);

                string nombre = "user_" + user.id_usuario + System.IO.Path.GetExtension(foto.FileName);
                string ruta = System.IO.Path.Combine(carpeta, nombre);

                foto.SaveAs(ruta);
                usuarioDb.foto_perfil = "/Fotos/" + nombre; // ← una sola línea
            }

            try
            {
                _usuarioBusiness.Actualizar(usuarioDb);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        ModelState.AddModelError("", $"Campo: {ve.PropertyName} - Error: {ve.ErrorMessage}");
                    }
                }
                return View(usuarioDb);
            }

            Session["UsuarioNombre"] = usuarioDb.nombre_usuario;
            Session["FotoPerfil"] = usuarioDb.foto_perfil;
            return RedirectToAction("Index", new { id = user.id_usuario });

        }
    }
}