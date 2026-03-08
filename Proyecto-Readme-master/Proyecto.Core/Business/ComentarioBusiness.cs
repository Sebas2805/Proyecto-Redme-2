using Proyecto.Data;
using Proyecto.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Core.Business
{
    public class ComentarioBusiness
    {
        private readonly IRepositoryComentario _cometarioRepository;

        public ComentarioBusiness()
        {
            _cometarioRepository = new RepositoryComentario();
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
    }
}
