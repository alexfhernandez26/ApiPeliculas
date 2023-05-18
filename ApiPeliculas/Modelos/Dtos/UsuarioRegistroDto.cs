using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos.Dtos
{
    public class UsuarioRegistroDto
    {
       [Required (ErrorMessage ="El nombre es Requerido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El nombre Usuario es requerido")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "El password es Requerido")]
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
