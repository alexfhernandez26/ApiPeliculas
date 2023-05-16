using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ApiPeliculas.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ApplicationDbContext _db;
        private string claveSecreta;
        public UsuarioRepositorio(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            claveSecreta = config.GetValue<string>("ApiSettings:Secreta");
        }
        public Usuario GetUsuario(int usuarioId)
        {
            return _db.Usuario.FirstOrDefault(u => u.Id == usuarioId);
        }

        public ICollection<Usuario> GetUsuarios()
        {
           return _db.Usuario.OrderBy(u => u.Nombre).ToList();
        }

        public bool IsUniqueUser(string nombreUsuario)
        {
            var usuariodb = _db.Usuario.FirstOrDefault(u => u.NombreUsuario == nombreUsuario);
            if (usuariodb ==null)
            {
                return true;
            }

            return false;
        }

        public async Task<Usuario> Registro(UsuarioRegistroDto usuarioRegistroDto)
        {
            var paswordEncriptado = obtenermd5(usuarioRegistroDto.Password);

            Usuario usuario = new Usuario()
            {
                Nombre = usuarioRegistroDto.Nombre,
                NombreUsuario = usuarioRegistroDto.NombreUsuario,
                Password = paswordEncriptado,
                Rol = usuarioRegistroDto.Rol
            };

            _db.Usuario.Add(usuario);
            await _db.SaveChangesAsync();

            usuario.Password = paswordEncriptado;
            return usuario;
        }

        public async Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto)
        {
            var passwordEncriptada = obtenermd5(usuarioLoginDto.Password);

            var usuario = _db.Usuario.FirstOrDefault
                            (
                                u=> u.NombreUsuario.ToLower() == usuarioLoginDto.NombreUsuario.ToLower()
                                && u.Password == passwordEncriptada
                            );

            //validamos si no existe usuario.
            if (usuario ==null)
            {
                return new UsuarioLoginRespuestaDto()
                {
                    Token = "",
                    Usuario = null
                };
            }

            //si existe el usuario podemos procesar el login
            //creacion del token,JwtSecurityTokenHandler(); esta libreria es para poder manejar el token
            var manejarToken =new  JwtSecurityTokenHandler();

            //creamos una variable que sera la llave secreta que valida los token
            var key = Encoding.ASCII.GetBytes(claveSecreta);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, usuario.NombreUsuario.ToString()),
                    new Claim(ClaimTypes.Role, usuario.Rol)
                }),
                 Expires = DateTime.UtcNow.AddDays(7),

                /*se utiliza para definir las credenciales de firma del token JWT. se está utilizando un algoritmo de firma HMACSHA256 para firmar el token
                 * JWT con una clave secreta, que se define en la variable key. Se crea un objeto SymmetricSecurityKey a partir de la clave, y se pasa 
                 * al constructor de SigningCredentials, junto con el algoritmo de firma a utilizar.*/
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                 SecurityAlgorithms.HmacSha256Signature)
            };

            var token = manejarToken.CreateToken(tokenDescriptor);

            UsuarioLoginRespuestaDto usuarioLoginRespuestaDto = new UsuarioLoginRespuestaDto()
            {
                Token = manejarToken.WriteToken(token),
                Usuario = usuario
            };

            return usuarioLoginRespuestaDto;
        }



        public static string obtenermd5(string valor)
        {
         
              byte[] bytes = Encoding.UTF8.GetBytes(valor);
            using (MD5 md5 = MD5.Create())
            {

                byte[] hash = md5.ComputeHash(bytes);

                
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
