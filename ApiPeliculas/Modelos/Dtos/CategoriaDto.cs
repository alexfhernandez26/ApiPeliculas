using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos.Dtos
{
    public class CategoriaDto
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
      
    }
}
