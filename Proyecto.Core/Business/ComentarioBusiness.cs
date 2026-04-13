using Proyecto.Data;
using Proyecto.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Core.Business
{
    public class ComentarioBusiness
    {
        private readonly IRepositoryComentario _cometarioRepository;
        private readonly ReaccionBusiness _reaccionBusiness;
        private const string Up = "UP";
        private const string Down = "DOWN";

        public ComentarioBusiness()
        {
            _cometarioRepository = new RepositoryComentario();
            _reaccionBusiness = new ReaccionBusiness();
        }
        public List<comentario> GetByUsuarioId(int userId)
        {
            using (var db = new ReadmeDBEntities())
            {
                return db.comentarios
                    .Where(c => c.id_usuario == userId)
                    .Include("post")
                    .ToList();
            }
        }


        // Obtener todos los comentarios
        public IEnumerable<comentario> GetAll()
        {
            return _cometarioRepository.GetAll();
        }

        // Obtener comentario por ID
        public comentario GetById(int id)
        {
            return _cometarioRepository.GetById(id);
        }

        // Crear comentario
        public void Crear(comentario comentario)
        {
            _cometarioRepository.Add(comentario);
        }

        // Actualizar comentario
        public void Actualizar(comentario comentario)
        {
            _cometarioRepository.Update(comentario);
        }

        // Eliminar comentario
        public void Eliminar(int id)
        {
            _cometarioRepository.Delete(id);
        }

        // Obtener los comentarios de un post y el postID para crear uno nuevo
        public async Task<List<comentario>> GetByPostId(int postId)
        {
            return await _cometarioRepository.GetByPostId(postId);
        }

        public void ReaccionarComentario(int idUsuario, int idComentario, string tipo)
        {
            var tipoDb = NormalizarTipoParaDb(tipo);
            if (string.IsNullOrEmpty(tipoDb))
                throw new ArgumentException("Tipo de reacción inválido.");

            _reaccionBusiness.GuardarReaccion(idUsuario, null, idComentario, tipoDb);
        }

        public Dictionary<int, int> GetLikeCountsByComentario(IEnumerable<int> comentarioIds)
        {
            var ids = comentarioIds?.Distinct().ToList() ?? new List<int>();
            if (!ids.Any()) return new Dictionary<int, int>();

            using (var db = new ReadmeDBEntities())
            {
                return db.votoes
                    .Where(v => v.id_comentario.HasValue && ids.Contains(v.id_comentario.Value) && v.tipo == Up)
                    .GroupBy(v => v.id_comentario.Value)
                    .Select(g => new { ComentarioId = g.Key, Count = g.Count() })
                    .ToDictionary(x => x.ComentarioId, x => x.Count);
            }
        }

        public Dictionary<int, int> GetDislikeCountsByComentario(IEnumerable<int> comentarioIds)
        {
            var ids = comentarioIds?.Distinct().ToList() ?? new List<int>();
            if (!ids.Any()) return new Dictionary<int, int>();

            using (var db = new ReadmeDBEntities())
            {
                return db.votoes
                    .Where(v => v.id_comentario.HasValue && ids.Contains(v.id_comentario.Value) && v.tipo == Down)
                    .GroupBy(v => v.id_comentario.Value)
                    .Select(g => new { ComentarioId = g.Key, Count = g.Count() })
                    .ToDictionary(x => x.ComentarioId, x => x.Count);
            }
        }

        public Dictionary<int, string> GetUserReactionByComentario(IEnumerable<int> comentarioIds, int idUsuario)
        {
            var ids = comentarioIds?.Distinct().ToList() ?? new List<int>();
            if (!ids.Any()) return new Dictionary<int, string>();

            using (var db = new ReadmeDBEntities())
            {
                return db.votoes
                    .Where(v => v.id_usuario == idUsuario && v.id_comentario.HasValue && ids.Contains(v.id_comentario.Value))
                    .GroupBy(v => v.id_comentario.Value)
                    .Select(g => g.OrderByDescending(x => x.fecha_voto).FirstOrDefault())
                    .Where(v => v != null)
                    .ToDictionary(v => v.id_comentario.Value, v => TipoDbToUi(v.tipo));
            }
        }

        public int GetLikeCountByComentario(int idComentario)
        {
            using (var db = new ReadmeDBEntities())
            {
                return db.votoes.Count(v => v.id_comentario == idComentario && v.tipo == Up);
            }
        }

        public int GetDislikeCountByComentario(int idComentario)
        {
            using (var db = new ReadmeDBEntities())
            {
                return db.votoes.Count(v => v.id_comentario == idComentario && v.tipo == Down);
            }
        }

        public string GetUserReactionByComentario(int idComentario, int idUsuario)
        {
            using (var db = new ReadmeDBEntities())
            {
                var voto = db.votoes
                    .Where(v => v.id_usuario == idUsuario && v.id_comentario == idComentario)
                    .OrderByDescending(v => v.fecha_voto)
                    .FirstOrDefault();

                return voto == null ? string.Empty : TipoDbToUi(voto.tipo);
            }
        }

        private static string NormalizarTipoParaDb(string tipoUi)
        {
            var t = (tipoUi ?? string.Empty).Trim().ToLowerInvariant();
            if (t == "like" || t == "up") return Up;
            if (t == "dislike" || t == "down") return Down;
            return string.Empty;
        }

        private static string TipoDbToUi(string tipoDb)
        {
            var t = (tipoDb ?? string.Empty).Trim().ToUpperInvariant();
            if (t == Up) return "like";
            if (t == Down) return "dislike";
            return string.Empty;
        }

    }
}
