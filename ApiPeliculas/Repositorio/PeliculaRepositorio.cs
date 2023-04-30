using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Repositorio
{
    public class PeliculaRepositorio : IPeliculaRepositorio
    {
        private readonly ApplicationDbContext _db;
        public PeliculaRepositorio(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool ActualizarPelicula(Pelicula pelicula)
        {
            pelicula.FechaCreacion = DateTime.Now;
            _db.Peliculas.Update(pelicula);
            return Guardar();
        }

        public bool BorrarPelicula(Pelicula pelicula)
        {
            _db.Peliculas.Remove(pelicula);

            return Guardar();
        }

        public bool CrearPelicula(Pelicula pelicula)
        {
            pelicula.FechaCreacion = DateTime.Now;
            _db.Peliculas.Add(pelicula);
            
            return Guardar();
        }

        public bool ExistePelicula(string nombre)
        {
            bool valor = _db.Peliculas.Any(c => c.Nombre.ToLower().Trim() == nombre.ToLower().Trim());

            return valor;
        }

        public bool ExistePeliculaId(int peliculaId)
        {
           return  _db.Peliculas.Any(c => c.Id == peliculaId);

            
        }

        public Pelicula GetPelicula(int peliculaId)
        {
            return _db.Peliculas.FirstOrDefault(c => c.Id == peliculaId);
        }

        public ICollection<Pelicula> GetPeliculas()
        {
           return _db.Peliculas.OrderBy(c => c.Nombre).ToList();
        }

        public ICollection<Pelicula> GetPeliculasPorCategoria(int catId)
        {
            return _db.Peliculas.Include(x => x.Categoria).Where(c => c.categoriaId == catId).ToList();
        }


        public ICollection<Pelicula> ObtenerPeliculasPorNombre(string nombre)
        {
            IQueryable<Pelicula> query = _db.Peliculas;

            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(x => x.Nombre.Contains(nombre) || x.Descripcion.Contains(nombre));
            }
            return query.ToList();
        }

        public bool Guardar()
        {   
            return _db.SaveChanges() >= 0 ? true : false;
        }
    }
}
