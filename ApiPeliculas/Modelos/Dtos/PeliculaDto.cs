﻿using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos.Dtos
{
    public class PeliculaDto
    {
        public int Id { get; set; }

        [Required (ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }
        public string RutaImagen { get; set; }
        [Required(ErrorMessage = "La descripcion es obligatoria")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "La duracion es obligatoria")]
        public int Duracion { get; set; }
        public enum TipoDeClasificacion { Siete, Trece, Dieciseis, Dieciocho }
        public TipoDeClasificacion Clasificacion { get; set; }
        public DateTime FechaCreacion { get; set; }

        public int categoriaId { get; set; }
    }
}
