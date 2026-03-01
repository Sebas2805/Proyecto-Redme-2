using Proyecto.Core;
using Proyecto.Core.Business;
using Proyecto.ForoReadme.Models;
using System.Web.Mvc;
using System.Web.Security;

namespace Proyecto.ForoReadme.Controllers
{
    public class AccountController : Controller
    {
        private UsuarioBusiness _business = new UsuarioBusiness();

        public AccountController()
        {
            _business = new UsuarioBusiness();
        }

        // GET
        public ActionResult Login()
        {
            if (Session["UsuarioId"] != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = _business.Login(model.Email, model.Password);

                if (usuario != null)
                {
                    Session["UsuarioId"] = usuario.id_usuario;
                    Session["NombreUsuario"] = usuario.nombre_usuario;
                    Session["Email"] = usuario.email;

                    FormsAuthentication.SetAuthCookie(usuario.email, model.RememberMe);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Email o contraseña incorrectos.");
            }

            return View(model);
        }

        // GET
        public ActionResult Register()
        {
            if (Session["UsuarioId"] != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST
        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_business.EmailExiste(model.Email))
                {
                    ModelState.AddModelError("Email", "Este email ya está registrado.");
                    return View(model);
                }

                if (_business.NombreUsuarioExiste(model.NombreUsuario))
                {
                    ModelState.AddModelError("NombreUsuario", "Este nombre de usuario ya está en uso.");
                    return View(model);
                }

                var nuevoUsuario = _business.Registrar(
                    model.NombreUsuario,
                    model.Email,
                    model.Password
                );

                Session["UsuarioId"] = nuevoUsuario.id_usuario;
                Session["NombreUsuario"] = nuevoUsuario.nombre_usuario;
                Session["Email"] = nuevoUsuario.email;

                FormsAuthentication.SetAuthCookie(nuevoUsuario.email, false);

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }
    }
}