using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiPeliculas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepositorio _usRepo;
        private readonly IMapper _mapper;
        protected RespuestaApi _respuestaApi;
        public UsuarioController(IUsuarioRepositorio usRepo, IMapper mapper)
        {
            _usRepo = usRepo;
            _mapper = mapper;
            this._respuestaApi = new();

        }
        //[AllowAnonymous]
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetUsuarios()
        {
            var listaCategoria = _usRepo.GetUsuarios();

            var listaUsuarioDto = new List<UsuarioDto>();

            foreach (var lista in listaCategoria)
            {
                listaUsuarioDto.Add(_mapper.Map<UsuarioDto>(lista));
            }

            return Ok(listaUsuarioDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{usuarioId:int}", Name = "Getusuario")]
        public IActionResult GetUsuario(string usuarioId)
        {
            var itemUsuarioId = _usRepo.GetUsuario(usuarioId);

            if (itemUsuarioId == null)
            {
                return NotFound($"No existe categoria con el id {usuarioId}");
            }

            var itemUsuarioIdDto = _mapper.Map<UsuarioDto>(itemUsuarioId);

            return Ok(itemUsuarioIdDto);
        }

        [AllowAnonymous]
        [HttpPost("Registro")]
        public async Task<IActionResult> Registro([FromBody] UsuarioRegistroDto usuarioRegistroDto)
        {
            bool validarNombreUsuarioUnico = _usRepo.IsUniqueUser(usuarioRegistroDto.NombreUsuario);

            if (!validarNombreUsuarioUnico)
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.isSucces = false;
                _respuestaApi.ErrorMessages.Add("El nombre de usuario ya existe");
                return BadRequest(_respuestaApi);
            }

            var usuario = _usRepo.Registro(usuarioRegistroDto);
            if (usuario == null)
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.isSucces = false;
                _respuestaApi.ErrorMessages.Add("Error al tratar de registrar usuario");
                return BadRequest(_respuestaApi);
            }

            _respuestaApi.StatusCode = HttpStatusCode.OK;
            _respuestaApi.isSucces = true;
            return Ok(_respuestaApi);


        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDto usuarioLoginDto)
        {
            var respuestaLogin = await _usRepo.Login(usuarioLoginDto);

        

            if (respuestaLogin.Usuario == null || string.IsNullOrEmpty(respuestaLogin.Token))
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.isSucces = false;
                _respuestaApi.ErrorMessages.Add("El nombre de usuario o password son incorrectos");
                return BadRequest(_respuestaApi);
            }

           

            _respuestaApi.StatusCode = HttpStatusCode.OK;
            _respuestaApi.isSucces = true;
            _respuestaApi.Result = respuestaLogin;
            return Ok(_respuestaApi);


        }
    }
}
