using Proyecto.Data;
using System;
using System.Data.Entity.Validation;
using System.Linq;

namespace Proyecto.Core.Business
{
    public class ReaccionBusiness
    {
        public void GuardarReaccion(int idUsuario, int? idPost, int? idComentario, string tipo)
        {
            using (var db = new ReadmeDBEntities())
            {
                try
                {
                    // Validar que exista el usuario
                    var usuario = db.usuarios.Find(idUsuario);
                    if (usuario == null)
                        throw new Exception("El usuario no existe.");

                    // Validar que exista el post si viene
                    if (idPost.HasValue)
                    {
                        var post = db.posts.Find(idPost.Value);
                        if (post == null)
                            throw new Exception("El post no existe.");
                    }

                    // Validar que exista el comentario si viene
                    if (idComentario.HasValue)
                    {
                        var comentario = db.comentarios.Find(idComentario.Value);
                        if (comentario == null)
                            throw new Exception("El comentario no existe.");
                    }

                    // Verificar si ya votó
                    var votoExistente = db.votoes
                        .FirstOrDefault(v =>
                            v.id_usuario == idUsuario &&
                            ((v.id_post == idPost) || (v.id_post == null && idPost == null)) &&
                            ((v.id_comentario == idComentario) || (v.id_comentario == null && idComentario == null))
                        );

                    if (votoExistente == null)
                    {
                        voto v = new voto
                        {
                            id_usuario = idUsuario,
                            id_post = idPost,
                            id_comentario = idComentario,
                            tipo = tipo,
                            fecha_voto = DateTime.Now
                        };
                        db.votoes.Add(v);
                    }
                    else
                    {
                        // Si ya existe, solo cambia el voto
                        votoExistente.tipo = tipo;
                        votoExistente.fecha_voto = DateTime.Now;
                    }

                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine($"Entidad: {eve.Entry.Entity.GetType().Name}, Propiedad: {ve.PropertyName}, Error: {ve.ErrorMessage}");
                        }
                    }
                    throw;
                }
            }
        }

        public string GuardarReaccionConCambio(int idUsuario, int? idPost, int? idComentario, string tipo)
        {
            using (ReadmeDBEntities db = new ReadmeDBEntities())
            {
                try
                {
                    var votoExistente = db.votoes
                        .FirstOrDefault(v =>
                            v.id_usuario == idUsuario &&
                            ((v.id_post == idPost) || (v.id_post == null && idPost == null)) &&
                            ((v.id_comentario == idComentario) || (v.id_comentario == null && idComentario == null))
                        );

                    if (votoExistente == null)
                    {
                        // Nuevo voto
                        db.votoes.Add(new voto
                        {
                            id_usuario = idUsuario,
                            id_post = idPost,
                            id_comentario = idComentario,
                            tipo = tipo,
                            fecha_voto = DateTime.Now
                        });
                        db.SaveChanges();
                        return "nuevo";
                    }
                    else
                    {
                        string cambio = votoExistente.tipo == tipo ? tipo : votoExistente.tipo;
                        // Cambiar tipo si es diferente
                        votoExistente.tipo = tipo;
                        votoExistente.fecha_voto = DateTime.Now;
                        db.SaveChanges();
                        return cambio; // devuelve el tipo anterior
                    }
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    // Aquí te dirá exactamente qué campo está causando problemas
                    string errores = "";
                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            errores += $"Entidad: {eve.Entry.Entity.GetType().Name}, Propiedad: {ve.PropertyName}, Error: {ve.ErrorMessage}\n";
                        }
                    }
                    throw new Exception("Errores de validación:\n" + errores);
                }
            }
        }
    }
}