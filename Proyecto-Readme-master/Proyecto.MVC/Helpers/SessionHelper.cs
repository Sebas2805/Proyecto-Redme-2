using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto.MVC.Helpers
{
    public static class SessionHelper
    {
        public static int? UsuarioId
        {
            get
            {
                if (HttpContext.Current.Session["UsuarioId"] != null)
                    return (int)HttpContext.Current.Session["UsuarioId"];

                return null;
            }
        }

        public static string NombreUsuario
        {
            get
            {
                return HttpContext.Current.Session["NombreUsuario"]?.ToString();
            }
        }

        public static string Email
        {
            get
            {
                return HttpContext.Current.Session["Email"]?.ToString();
            }
        }

        public static bool EstaLogueado
        {
            get
            {
                return UsuarioId != null;
            }
        }
    }
}