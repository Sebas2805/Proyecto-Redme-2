using System.Web.Mvc;

public class UsuariosController : Controller
{
    public ActionResult Details(int id)
    {

        return RedirectToAction("Index", "Perfil", new { id = id });
    }
}