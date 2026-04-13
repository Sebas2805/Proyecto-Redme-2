using Proyecto.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Repositories
{
    /// <summary>
    /// Repository interface for Product entities.
    /// Defines the contract for Product-specific data access operations.
    /// </summary>
    public interface IRepositoryUsuario : IRepositoryBase<usuario>
    {
        usuario GetByEmail(string email);
        usuario GetByNombreUsuario(string nombreUsuario);
    }

    /// <summary>
    /// Repository implementation for Product entities.
    /// Provides data access operations for Product entities using Entity Framework.
    /// </summary>
    public class RepositoryUsuario: RepositoryBase<usuario>, IRepositoryUsuario
    {
        public usuario GetByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            email = email.Trim().ToLowerInvariant();

            return _set
                .AsNoTracking()
                .FirstOrDefault(u => u.email != null &&
                                      u.email.Trim().ToLower() == email);
        }

        public usuario GetByNombreUsuario(string nombreUsuario)
        {
            return _set.FirstOrDefault(u => u.nombre_usuario.ToLower() == nombreUsuario.ToLower());
        }
        /// <summary>
        /// Initializes a new instance of the RepositoryProduct class.
        /// </summary>
        public RepositoryUsuario() : base()
        {
        }

        
    }
}

