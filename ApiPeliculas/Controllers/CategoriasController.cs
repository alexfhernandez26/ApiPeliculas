using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaRepositorio _ctRepo;
        private readonly IMapper _mapper;

        public CategoriasController(ICategoriaRepositorio ctRepo, IMapper mapper)
        {
                _ctRepo = ctRepo;
                _mapper = mapper;
        }

        [HttpGet]
        public  IActionResult GetCategorias()
        {
            var listaCategoria = _ctRepo.GetCategorias();

            var listaCategoriaDto = new List<CategoriaDto>();

            foreach (var lista in listaCategoria)
            {
                listaCategoriaDto.Add(_mapper.Map<CategoriaDto>(lista));
            }

            return Ok(listaCategoriaDto);
        }

        [HttpGet("{categoriaId:int}", Name = "GetCategoria")]
        public IActionResult GetCategoria(int categoriaId)
        {
            var itemCategoriaId = _ctRepo.GetCategoria(categoriaId);

            if (itemCategoriaId ==null)
            {
                return NotFound($"No existe categoria con el id {categoriaId}");
            }

            var itemCategoriaIdDto = _mapper.Map<CategoriaDto>(itemCategoriaId);

            return Ok(itemCategoriaId);
        }

        [HttpPost]
        public IActionResult CrearCategoria([FromBody] CrearCategoriaDto crearCategoriaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (crearCategoriaDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_ctRepo.ExisteCategoria(crearCategoriaDto.Nombre))
            {
                ModelState.AddModelError("","La categoria ya existe, no se puede crear otra con el mismo nombre");
                return StatusCode(400, ModelState);
            }

            var categoria = _mapper.Map<Categoria>(crearCategoriaDto);

            if (!_ctRepo.CrearCategoria(categoria))
            {
                ModelState.AddModelError("", $"Error al tratar de crear la categoria: {categoria}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetCategoria",new {categoriaId = categoria.Id},categoria);
          
        }

        [HttpPatch("{categoriaId:int}")]
        public IActionResult ActualizarPatchCategoria(int categoriaId, [FromBody] CategoriaDto categoriaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (categoriaDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_ctRepo.ExisteCategoria(categoriaDto.Nombre))
            {
                ModelState.AddModelError("", "La categoria ya existe, no se puede crear otra con el mismo nombre");
                return StatusCode(400, ModelState);
            }

            var categoria = _mapper.Map<Categoria>(categoriaDto);

            if (!_ctRepo.ActualizarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Error al tratar de crear la categoria: {categoria}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetCategoria", new { categoriaId = categoria.Id }, categoria);

        }
        [HttpDelete("{categoriaId:int}")]
        public IActionResult BorrarCategoria(int categoriaId)
        {
            
           

            if (!_ctRepo.ExisteCategoriaId(categoriaId))
            {
                ModelState.AddModelError("", "La categoria no existe, no se puede eliminar una categoria que no existe");
                return StatusCode(400, ModelState);
            }

            var categoria = _ctRepo.GetCategoria(categoriaId);

            if (!_ctRepo.BorrarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Error al tratar de borrar la categoria: {categoria}");
                return StatusCode(500, ModelState);
            }
            return NoContent();

        }

    }
}
