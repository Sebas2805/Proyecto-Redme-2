using Proyecto.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Repositories
{
    public interface IRepositoryPost : IRepositoryBase<post>
    {
        Task<List<post>> GetByComunidadId(int comunidadId);
    }

    public class RepositoryPost : RepositoryBase<post>, IRepositoryPost
    {
        public RepositoryPost() : base()
        {
        }
        public async Task<List<post>> GetByComunidadId(int comunidadId)
        {
            return await _set
                .Where(p => p.id_comunidad == comunidadId)
                .ToListAsync();
        }

    }
}
