using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeliculasController : ControllerBase
    {
        private readonly IPeliculaRepositorio _pelRepo;
        private readonly IMapper _mapper;

        public PeliculasController(IPeliculaRepositorio pelRepo, IMapper mapper)
        {
            _pelRepo = pelRepo;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetPeliculas()
        {
            var listaPeliculas = _pelRepo.GetPeliculas();

            var listaPeliculasDto = new List<PeliculaDto>();

            foreach (var lista in listaPeliculas)
            {
                listaPeliculasDto.Add(_mapper.Map<PeliculaDto>(lista));
                
            }
            listaPeliculasDto.OrderBy(x => x.Id);
            return Ok(listaPeliculasDto);
        }

        [AllowAnonymous]
        [HttpGet("{peliculaId:int}", Name = "Getpelicula")]
        public IActionResult GetPelicula(int peliculaId)
        {
            var itemPeliculaId = _pelRepo.GetPelicula(peliculaId);

            if (itemPeliculaId == null)
            {
                return NotFound($"No existe pelicula con el id {peliculaId}");
            }

            var itemPeliculaIdDto = _mapper.Map<PeliculaDto>(itemPeliculaId);

            return Ok(itemPeliculaIdDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult CrearPelicula([FromBody] PeliculaDto peliculaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (peliculaDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_pelRepo.ExistePelicula(peliculaDto.Nombre))
            {
                ModelState.AddModelError("", "La Pelicula ya existe, no se puede crear otra con el mismo nombre");
                return StatusCode(400, ModelState);
            }

            var pelicula = _mapper.Map<Pelicula>(peliculaDto);

            if ( !_pelRepo.CrearPelicula(pelicula))
            {
               
                ModelState.AddModelError("", $"Error al tratar de crear la pelicula: {pelicula}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("Getpelicula", new { peliculaId = pelicula.Id }, pelicula);

        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{peliculaId:int}", Name="ActualizarPeliculasPatch")]
        public IActionResult ActualizarPatchCategoria(int peliculaId, [FromBody] PeliculaDto peliculaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (peliculaDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_pelRepo.ExistePelicula(peliculaDto.Nombre))
            {
                ModelState.AddModelError("", "La categoria ya existe, no se puede crear otra con el mismo nombre");
                return StatusCode(400, ModelState);
            }

            var pelicula = _mapper.Map<Pelicula>(peliculaDto);

            if (!_pelRepo.ActualizarPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Error al tratar de crear la pelicila: {pelicula}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetPelicula", new { peliculaId = pelicula.Id }, pelicula);

        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{peliculaId:int}", Name ="BorrarPelicula")]
        public IActionResult BorrarPelicula(int peliculaId)
        {



            if (!_pelRepo.ExistePeliculaId(peliculaId))
            {
                ModelState.AddModelError("", "La categoria no existe, no se puede eliminar una categoria que no existe");
                return StatusCode(400, ModelState);
            }

            var pelicula = _pelRepo.GetPelicula(peliculaId);

            if (!_pelRepo.BorrarPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Error al tratar de borrar la categoria: {pelicula}");
                return StatusCode(500, ModelState);
            }
            return NoContent();

        }

        [AllowAnonymous]
        [HttpGet("GetPeliculasEnCategoria{categoriaId:int}")]
        public IActionResult GetPeliculasEnCategoria(int categoriaId)
        {
            var ListaPeliculas = _pelRepo.GetPeliculasPorCategoria(categoriaId);

            if (ListaPeliculas == null)
            {
                return NotFound($"No existe pelicula con el id {categoriaId}");
            }
            if (ListaPeliculas.Count()== 0)
            {
                return NotFound($"No existe pelicula con el id {categoriaId}");
            }

            var itemPeliculas = new List<PeliculaDto>();
            foreach (var item in ListaPeliculas)
            {
                itemPeliculas.Add(_mapper.Map<PeliculaDto>(item));
            }

            

            return Ok(itemPeliculas);
        }

        [AllowAnonymous]
        [HttpGet("GetPeliculasEnCategoriaPorNombre{nombre}")]
        public IActionResult GetPeliculasEnCategoriaPorNombre(string nombre)
        {
            var ListaPeliculas = _pelRepo.ObtenerPeliculasPorNombre(nombre);

            if (ListaPeliculas == null)
            {
                return NotFound($"No existe pelicula con el nombre {nombre}");
            }
            if (ListaPeliculas.Count() == 0)
            {
                return NotFound($"No existe pelicula con el nombre {nombre}");
            }

            var itemPeliculas = new List<PeliculaDto>();
            foreach (var item in ListaPeliculas)
            {
                itemPeliculas.Add(_mapper.Map<PeliculaDto>(item));
            }



            return Ok(itemPeliculas);
        }
    }
}
