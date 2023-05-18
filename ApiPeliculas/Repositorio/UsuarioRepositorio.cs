using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ApiPeliculas.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        /*UserManager<TUser> y RoleManager<TRole> son clases proporcionadas por el framework para administrar usuarios y roles respectivamente. 
          se utilizan para realizar operaciones relacionadas con la autenticación, autorización y administración de usuarios y roles en una aplicación
        El UserManager<TUser> se utiliza para administrar usuarios, como crear nuevos usuarios, buscar usuarios, modificar información de usuarios, administrar contraseñas
        realizar operaciones de inicio de sesión y cierre de sesión, entre otras funcionalidades relacionadas con la autenticación y autorización de usuarios.
        
         El RoleManager<TRole> se utiliza para administrar roles, como crear nuevos roles, buscar roles, agregar o eliminar usuarios de un rol, realizar comprobaciones 
        de pertenencia a un rol,
        
         Ambas clases se utilizan en conjunto con el fin de administrar de manera eficiente y segura la autenticación y autorización de usuarios en una aplicación .NET
        utilizando .NET Identity. Al utilizar estas clases, puedes evitar tener que implementar manualmente la lógica para administrar usuarios y roles, ya que .NET Identity 
        proporciona una capa de abstracción que facilita estas tareas comunes.*/

        private readonly ApplicationDbContext _db;
        private string claveSecreta;
        private readonly UserManager<AppUsuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        public UsuarioRepositorio(ApplicationDbContext db, IConfiguration config,
                           UserManager<AppUsuario> userManager, RoleManager<IdentityRole> roleManager,
                           IMapper mapper)
        {
            _db = db;
            claveSecreta = config.GetValue<string>("ApiSettings:Secreta");
            _roleManager = roleManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper= mapper;
        }
        public AppUsuario GetUsuario(string usuarioId)
        {
            return _db.sppUsuario.FirstOrDefault(u => u.Id == usuarioId);
        }

        public ICollection<AppUsuario> GetUsuarios()
        {
           return _db.sppUsuario.OrderBy(u => u.UserName).ToList();
        }

        public bool IsUniqueUser(string nombreUsuario)
        {
            var usuariodb = _db.sppUsuario.FirstOrDefault(u => u.UserName == nombreUsuario);
            if (usuariodb ==null)
            {
                return true;
            }

            return false;
        }

        public async Task<UsuarioDatosDto> Registro(UsuarioRegistroDto usuarioRegistroDto)
        {
            

            AppUsuario usuario = new AppUsuario()
            {
                UserName = usuarioRegistroDto.NombreUsuario,
                Email = usuarioRegistroDto.NombreUsuario,
                NormalizedEmail = usuarioRegistroDto.NombreUsuario.ToUpper(),
                Nombre = usuarioRegistroDto.Nombre
            };
          

            var result =await _userManager.CreateAsync(usuario, usuarioRegistroDto.Password);


            if (result.Succeeded)
            {
                //Solo la primera vez y para crear los roles
                if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    await _roleManager.CreateAsync(new IdentityRole("Registrado"));
                }

                await _userManager.AddToRoleAsync(usuario, "Admin");
                var usuarioRetornado = _db.sppUsuario.FirstOrDefault(u => u.UserName == usuarioRegistroDto.NombreUsuario);

              
                return _mapper.Map<UsuarioDatosDto>(usuarioRetornado);
            }


            return new UsuarioDatosDto();
         
        }

        public async Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto)
        {
            //var passwordEncriptada = obtenermd5(usuarioLoginDto.Password);

            var usuario = _db.sppUsuario.FirstOrDefault
                            (u=> u.UserName.ToLower() == usuarioLoginDto.NombreUsuario.ToLower());


            bool isValida = await _userManager.CheckPasswordAsync(usuario, usuarioLoginDto.Password);
            //validamos si no existe usuario.
            if (usuario ==null || isValida ==false)
            {
                return new UsuarioLoginRespuestaDto()
                {
                    Token = "",
                    Usuario = null
                };
            }

            //si existe el usuario podemos procesar el login
            var roles = await _userManager.GetRolesAsync(usuario);


            var manejarToken =new  JwtSecurityTokenHandler();

            //creamos una variable que sera la llave secreta que valida los token
            var key = Encoding.ASCII.GetBytes(claveSecreta);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, usuario.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
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
                Usuario = _mapper.Map<UsuarioDatosDto>(usuario)
            };

            return usuarioLoginRespuestaDto;
        }



        //public static string obtenermd5(string valor)
        //{
         
        //      byte[] bytes = Encoding.UTF8.GetBytes(valor);
        //    using (MD5 md5 = MD5.Create())
        //    {

        //        byte[] hash = md5.ComputeHash(bytes);

                
        //        StringBuilder sb = new StringBuilder();
        //        for (int i = 0; i < hash.Length; i++)
        //        {
        //            sb.Append(hash[i].ToString("x2"));
        //        }
        //        return sb.ToString();
        //    }
        //}
    }
}
