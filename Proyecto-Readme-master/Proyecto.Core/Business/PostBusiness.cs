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

        public PostBusiness()
        {
            _PostRepository = new RepositoryPost();
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
    }
}
