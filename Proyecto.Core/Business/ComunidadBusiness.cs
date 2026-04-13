using Proyecto.Data;
using Proyecto.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Core.Business
{
    public class ComunidadBusiness
    {
        private readonly IRepositoryComunidad _comunidadRepository;

        public ComunidadBusiness()
        {
            _comunidadRepository = new RepositoryComunidad();
        }




        public List<comunidad> GetByCreadorId(int userId)
        {
            using (var db = new ReadmeDBEntities())
            {
                return db.comunidads.Where(c => c.id_creador == userId).ToList();
            }
        }


        // Obtener todas las comunidades
        public IEnumerable<comunidad> GetAll()
        {
            return _comunidadRepository.GetAll();
        }

        // Obtener comunidad por ID
        public comunidad GetById(int id)
        {
            return _comunidadRepository.GetById(id);
        }

        // Crear comunidad
        public void Crear(comunidad comunidad)
        {
            _comunidadRepository.Add(comunidad);
        }

        // Actualizar comunidad
        public void Actualizar(comunidad comunidad)
        {
            _comunidadRepository.Update(comunidad);
        }

        // Eliminar comunidad
        public void Eliminar(int id)
        {
            _comunidadRepository.Delete(id);
        }
    }
}
