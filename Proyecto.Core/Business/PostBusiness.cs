using Proyecto.Data;
using Proyecto.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Core.Business
{
    public class PostBusiness
    {
        private readonly IRepositoryPost _PostRepository;
        private readonly ReaccionBusiness _reaccionBusiness;
        private const string Up = "UP";
        private const string Down = "DOWN";

        public PostBusiness()
        {
            _PostRepository = new RepositoryPost();
            _reaccionBusiness = new ReaccionBusiness();
        }

        // Obtener todos los posts
        public IEnumerable<post> GetAll()
        {
            return _PostRepository.GetAll();
        }

        // Obtener post por ID
        public post GetById(int id)
        {
            return _PostRepository.GetById(id);
        }

        // Crear post
        public void Crear(post post)
        {
            _PostRepository.Add(post);
        }

        // Actualizar post
        public void Actualizar(post post)
        {
            _PostRepository.Update(post);
        }

        // Eliminar post
        public void Eliminar(int id)
        {
            _PostRepository.Delete(id);
        }



        public List<post> GetByUsuarioId(int userId)
        {
            using (var db = new ReadmeDBEntities())
            {
                return db.posts.Where(p => p.id_usuario == userId).ToList();
            }
        }   


        // obtener la comunidad donde se va a publicar el post 
        public async Task<List<post>> GetByComunidadId(int comunidadId)
        {
            return await _PostRepository.GetByComunidadId(comunidadId);
        }

        public void ReaccionarPost(int idUsuario, int idPost, string tipo)
        {
            var tipoDb = NormalizarTipoParaDb(tipo);
            if (string.IsNullOrEmpty(tipoDb))
                throw new ArgumentException("Tipo de reacción inválido.");

            _reaccionBusiness.GuardarReaccion(idUsuario, idPost, null, tipoDb);
        }

        public Dictionary<int, int> GetLikeCountsByPost(IEnumerable<int> postIds)
        {
            var ids = postIds?.Distinct().ToList() ?? new List<int>();
            if (!ids.Any()) return new Dictionary<int, int>();

            using (var db = new ReadmeDBEntities())
            {
                return db.votoes
                    .Where(v => v.id_post.HasValue && ids.Contains(v.id_post.Value) && v.tipo == Up)
                    .GroupBy(v => v.id_post.Value)
                    .Select(g => new { PostId = g.Key, Count = g.Count() })
                    .ToDictionary(x => x.PostId, x => x.Count);
            }
        }

        public Dictionary<int, int> GetDislikeCountsByPost(IEnumerable<int> postIds)
        {
            var ids = postIds?.Distinct().ToList() ?? new List<int>();
            if (!ids.Any()) return new Dictionary<int, int>();

            using (var db = new ReadmeDBEntities())
            {
                return db.votoes
                    .Where(v => v.id_post.HasValue && ids.Contains(v.id_post.Value) && v.tipo == Down)
                    .GroupBy(v => v.id_post.Value)
                    .Select(g => new { PostId = g.Key, Count = g.Count() })
                    .ToDictionary(x => x.PostId, x => x.Count);
            }
        }

        public Dictionary<int, string> GetUserReactionByPost(IEnumerable<int> postIds, int idUsuario)
        {
            var ids = postIds?.Distinct().ToList() ?? new List<int>();
            if (!ids.Any()) return new Dictionary<int, string>();

            using (var db = new ReadmeDBEntities())
            {
                return db.votoes
                    .Where(v => v.id_usuario == idUsuario && v.id_post.HasValue && ids.Contains(v.id_post.Value))
                    .GroupBy(v => v.id_post.Value)
                    .Select(g => g.OrderByDescending(x => x.fecha_voto).FirstOrDefault())
                    .Where(v => v != null)
                    .ToDictionary(v => v.id_post.Value, v => TipoDbToUi(v.tipo));
            }
        }

        public int GetLikeCountByPost(int idPost)
        {
            using (var db = new ReadmeDBEntities())
            {
                return db.votoes.Count(v => v.id_post == idPost && v.tipo == Up);
            }
        }

        public int GetDislikeCountByPost(int idPost)
        {
            using (var db = new ReadmeDBEntities())
            {
                return db.votoes.Count(v => v.id_post == idPost && v.tipo == Down);
            }
        }

        public string GetUserReactionByPost(int idPost, int idUsuario)
        {
            using (var db = new ReadmeDBEntities())
            {
                var voto = db.votoes
                    .Where(v => v.id_usuario == idUsuario && v.id_post == idPost)
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
