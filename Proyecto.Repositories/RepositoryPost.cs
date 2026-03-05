using Proyecto.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Repositories
{
    public interface IRepositoryPost : IRepositoryBase<post>
    {
    }

    public class RepositoryPost : RepositoryBase<post>, IRepositoryPost
    {
        public RepositoryPost() : base()
        {
        }
    }
}
