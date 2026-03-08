using Proyecto.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Repositories
{
    public interface IRepositoryComentario : IRepositoryBase<comentario>
    {
    }

    public class RepositoryComentario : RepositoryBase<comentario>, IRepositoryComentario
    {
        public RepositoryComentario() : base()
        {
        }
    }
}
