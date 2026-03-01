using Proyecto.Data;
using Proyecto.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Core.Business
{
    public class UsuarioBusiness
    {
        private readonly IRepositoryUsuario _usuarioRepository;

        public UsuarioBusiness()
        {
            _usuarioRepository = new RepositoryUsuario();
        }

        public usuario Login(string email, string password)
        {
            var usuario = _usuarioRepository.GetByEmail(email);

            if (usuario != null && usuario.password_hash == password)
            {
                return usuario;
            }

            return null;
        }

        public bool EmailExiste(string email)
        {
            return _usuarioRepository.GetByEmail(email) != null;
        }

        public bool NombreUsuarioExiste(string nombreUsuario)
        {
            return _usuarioRepository.GetByNombreUsuario(nombreUsuario) != null;
        }

        public usuario Registrar(string nombreUsuario, string email, string password)
        {
            var nuevoUsuario = new usuario
            {
                nombre_usuario = nombreUsuario,
                email = email,
                password_hash = password,
                fecha_registro = DateTime.Now
            };

            _usuarioRepository.Add(nuevoUsuario);

            return nuevoUsuario;
        }

    }
}
