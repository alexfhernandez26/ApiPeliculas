using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.Repositorio.IRepositorio;

namespace ApiPeliculas.Repositorio
{
    public class CategoriaRepositorio : ICategoriaRepositorio
    {
        private readonly ApplicationDbContext _db;
        public CategoriaRepositorio(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool ActualizarCategoria(Categoria categoria)
        {
            categoria.FechaCreacion = DateTime.Now;
            _db.Categorias.Update(categoria);
            return Guardar();
        }

        public bool BorrarCategoria(Categoria categoria)
        {
            _db.Categorias.Remove(categoria);

            return Guardar();
        }

        public bool CrearCategoria(Categoria categoria)
        {
            categoria.FechaCreacion = DateTime.Now;
            _db.Categorias.Add(categoria);
            return Guardar();
        }

        public bool ExisteCategoria(string nombre)
        {
            bool valor = _db.Categorias.Any(c => c.Nombre.ToLower().Trim() == nombre.ToLower().Trim());

            return valor;
        }

        public bool ExisteCategoriaId(int categoriaId)
        {
           return  _db.Categorias.Any(c => c.Id == categoriaId);

            
        }

        public Categoria GetCategoria(int categoriaId)
        {
            return _db.Categorias.FirstOrDefault(c => c.Id == categoriaId);
        }

        public ICollection<Categoria> GetCategorias()
        {
           return _db.Categorias.OrderBy(c => c.Nombre).ToList();
        }

        public bool Guardar()
        {
            var valor = _db.SaveChanges();
            return _db.SaveChanges()  >= 0 ? true : false;
        }
    }
}
