using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos.Dtos
{
    public class UsuarioLoginDto
    {

        [Required(ErrorMessage = "El nombre Usuario es requerido")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "El password es Requerido")]
        public string Password { get; set; }
     
    }
}
