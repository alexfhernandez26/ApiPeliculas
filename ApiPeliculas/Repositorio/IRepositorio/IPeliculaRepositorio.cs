using ApiPeliculas.Modelos;

namespace ApiPeliculas.Repositorio.IRepositorio
{
    public interface IPeliculaRepositorio
    {
        ICollection<Pelicula> GetPeliculas();

        Pelicula GetPelicula(int peliculaId);
        bool ExistePelicula(string nombre);
        bool ExistePeliculaId(int peliculaId);
        bool CrearPelicula(Pelicula pelicula);
        bool ActualizarPelicula(Pelicula pelicula);
        bool BorrarPelicula(Pelicula pelicula);

        //Buscar peliculas por categoria y por nombre
        ICollection<Pelicula> GetPeliculasPorCategoria(int catId);
        ICollection<Pelicula> ObtenerPeliculasPorNombre(string nombre);
        bool Guardar();
    }
}
