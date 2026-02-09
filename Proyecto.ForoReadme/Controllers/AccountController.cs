using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Proyecto.Data;
using Proyecto.ForoReadme.Models;

namespace Proyecto.ForoReadme.Controllers
{
    public class AccountController : Controller
    {
        private README_DBEntities db = new README_DBEntities();

        // GET: Account/Login
        public ActionResult Login()
        {
            // Si ya está logueado, redirigir al home
            if (Session["UsuarioId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Buscar usuario por email
                var usuario = db.usuario.FirstOrDefault(u => u.email.ToLower() == model.Email.ToLower());
                
                if (usuario != null && model.Password == usuario.password_hash)
                {
                    // Crear sesión
                    Session["UsuarioId"] = usuario.id_usuario;
                    Session["NombreUsuario"] = usuario.nombre_usuario;
                    Session["Email"] = usuario.email;
                    
                    // Crear cookie de autenticación
                    FormsAuthentication.SetAuthCookie(usuario.email, model.RememberMe);
                    
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Email o contraseña incorrectos.");
                }
            }

            return View(model);
        }

        // GET: Account/Register
        public ActionResult Register()
        {
            // Si ya está logueado, redirigir al home
            if (Session["UsuarioId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el email ya existe
                if (db.usuario.Any(u => u.email.ToLower() == model.Email.ToLower()))
                {
                    ModelState.AddModelError("Email", "Este email ya está registrado.");
                    return View(model);
                }

                // Verificar si el nombre de usuario ya existe
                if (db.usuario.Any(u => u.nombre_usuario.ToLower() == model.NombreUsuario.ToLower()))
                {
                    ModelState.AddModelError("NombreUsuario", "Este nombre de usuario ya está en uso.");
                    return View(model);
                }

                // Crear nuevo usuario
                var nuevoUsuario = new usuario
                {
                    nombre_usuario = model.NombreUsuario,
                    email = model.Email,
                    password_hash = model.Password, // Guardar contraseña directamente
                    fecha_registro = DateTime.Now
                };

                db.usuario.Add(nuevoUsuario);
                db.SaveChanges();

                // Iniciar sesión automáticamente
                Session["UsuarioId"] = nuevoUsuario.id_usuario;
                Session["NombreUsuario"] = nuevoUsuario.nombre_usuario;
                Session["Email"] = nuevoUsuario.email;
                
                FormsAuthentication.SetAuthCookie(nuevoUsuario.email, false);

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        // POST: Account/Logout
        [HttpPost]
        public ActionResult Logout()
        {
            // Limpiar sesión
            Session.Clear();
            Session.Abandon();
            
            // Limpiar cookie de autenticación
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
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
