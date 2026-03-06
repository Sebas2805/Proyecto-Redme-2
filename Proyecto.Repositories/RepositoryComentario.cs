using Proyecto.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Repositories
{
    public interface IRepositoryComentario : IRepositoryBase<comentario>
    {
        Task<List<comentario>> GetByPostId(int postId);
    }

    public class RepositoryComentario : RepositoryBase<comentario>, IRepositoryComentario
    {
        public RepositoryComentario() : base()
        {
        }

        public async Task<List<comentario>> GetByPostId(int postId)
        {
            return await _set
                .Where(c => c.id_post == postId)
                .ToListAsync();
        }
    }
}
