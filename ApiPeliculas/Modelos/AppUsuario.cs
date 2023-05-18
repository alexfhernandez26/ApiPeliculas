using Microsoft.AspNetCore.Identity;

namespace ApiPeliculas.Modelos
{
    public class AppUsuario : IdentityUser
    {
        /*Agregar campos personalizados, identity trae sus campos como userName,id,password pero si queremos agregar mas podemos hacerlo
         * y cuando se cree la migracion estos campos adicionales se van a agregar a esa tabla que se va a llamar aspnetUser*/

        public string Nombre { get; set; }
    }
}
