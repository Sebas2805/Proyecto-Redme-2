using Proyecto.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Repositories
{
    public interface IRepositoryComunidad : IRepositoryBase<comunidad>
    {
    }

    public class RepositoryComunidad : RepositoryBase<comunidad>, IRepositoryComunidad
    {
        public RepositoryComunidad() : base()
        {
        }
    }
}
