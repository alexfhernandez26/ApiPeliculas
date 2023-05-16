using System.Net;

namespace ApiPeliculas.Modelos
{
    //aqui capturaremos los errores del servidor
    public class RespuestaApi
    {
        public RespuestaApi()
        {
            ErrorMessages = new List<string>();
        }

        public HttpStatusCode StatusCode { get; set; }
        public bool isSucces { get; set; }
        public List<string> ErrorMessages { get; set; }
        public object Result { get; set; }
    }
}
