using BibliotecaWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibrosController : ControllerBase
    {
        private readonly BibliotecaDbContext _contexto;

        public LibrosController(BibliotecaDbContext contexto)
        {
            _contexto = contexto;
        }

        [HttpPost]
        [Route("/AddBook")]
        public IActionResult AddBook([FromBody] Libro libro)
        {
            try
            {
                _contexto.Libros.Add(libro);
                _contexto.SaveChanges();
                return Ok(libro);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("/GetAllBooks")]
        public IActionResult GetAllBooks(int NumeroPagina)
        {
            var libros = (from l in _contexto.Libros
                          join a in _contexto.Autores
                          on l.AutorId equals a.AutorId
                          join c in _contexto.Categorias
                          on l.CategoriaId equals c.CategoriaId
                          select new
                          {
                              l.LibroId,
                              l.Titulo,
                              l.AnioPublicacion,
                              l.Resumen,
                              l.AutorId,
                              Autor = a.Nombre,
                              l.CategoriaId,
                              Categoria = c.Nombre
                          })
                          .Skip((NumeroPagina - 1) * 10)
                          .Take(10)
                          .ToList();

            return Ok(libros);
        }

        [HttpGet]
        [Route("/GetAllBooksByYear")]
        public IActionResult GetAllBooksByYear(int Anio)
        {
            var libros = (from l in _contexto.Libros
                          join a in _contexto.Autores
                          on l.AutorId equals a.AutorId
                          join c in _contexto.Categorias
                          on l.CategoriaId equals c.CategoriaId
                          where l.AnioPublicacion >= Anio
                          select new
                          {
                              l.LibroId,
                              l.Titulo,
                              l.AnioPublicacion,
                              l.Resumen,
                              l.AutorId,
                              Autor = a.Nombre,
                              l.CategoriaId,
                              Categoria = c.Nombre
                          }).ToList();

            return Ok(libros);
        }

        [HttpGet]
        [Route("/GetAllBooksByAuthor")]
        public IActionResult GetAllBooksByAuthor(int AutorId)
        {

            var librosPorAutor = (from a in _contexto.Autores
                                  where a.AutorId == AutorId
                                  select new
                                  {
                                      a.AutorId,
                                      a.Nombre,
                                      a.Nacionalidad,
                                      CatidadLibrosPublicados = (from l in _contexto.Libros where l.AutorId == a.AutorId select l).Count(),
                                      Libros = (from l in _contexto.Libros where l.AutorId == a.AutorId select l).ToList()
                                  }).FirstOrDefault();

            if (librosPorAutor == null)
            {
                return NotFound();
            }

            return Ok(librosPorAutor);
        }

        [HttpGet]
        [Route("/LatestBooks")]
        public IActionResult LatestBooks()
        {
            var libros = (from l in _contexto.Libros
                          join a in _contexto.Autores
                          on l.AutorId equals a.AutorId
                            orderby l.AnioPublicacion descending
                            select new
                            {
                                l.Titulo,
                                l.AnioPublicacion,
                                l.Resumen,
                                l.AutorId,
                                Autor = a.Nombre,
                                l.CategoriaId
                            }).ToList();
            return Ok(libros);
        }

        [HttpGet]
        [Route("/GetBooksByYear")]
        public IActionResult GetBooksByYear(int anio)
        {
            var libros = (from l in _contexto.Libros
                          join a in _contexto.Autores
                          on l.AutorId equals a.AutorId
                          where l.AnioPublicacion == anio
                          select new
                          {
                              l.Titulo,
                              l.AnioPublicacion,
                              l.Resumen,
                              l.AutorId,
                              Autor = a.Nombre,
                              l.CategoriaId
                          }).ToList();

            return Ok(libros);
        }

        [HttpGet]
        [Route("/GetBookById")]
        public IActionResult GetBookById(int LibroId)
        {
            var libro = (from l in _contexto.Libros
                          join a in _contexto.Autores
                          on l.AutorId equals a.AutorId
                          join c in _contexto.Categorias
                          on l.CategoriaId equals c.CategoriaId
                          where l.LibroId == LibroId
                          select new
                          {
                              l.LibroId,
                              l.Titulo,
                              l.AnioPublicacion,
                              l.Resumen,
                              l.AutorId,
                              Autor = a.Nombre,
                              l.CategoriaId,
                              Categoria = c.Nombre
                          }).FirstOrDefault();

            if (libro == null)
            {
                return NotFound();
            }
            return Ok(libro);
        }

        [HttpGet]
        [Route("/GetBookByTitle")]
        public IActionResult GetBookByTitle(string Titulo)
        {
            var libro = (from l in _contexto.Libros
                         join a in _contexto.Autores
                         on l.AutorId equals a.AutorId
                         join c in _contexto.Categorias
                         on l.CategoriaId equals c.CategoriaId
                         where l.Titulo == Titulo
                         select new
                         {
                             l.LibroId,
                             l.Titulo,
                             l.AnioPublicacion,
                             l.Resumen,
                             l.AutorId,
                             Autor = a.Nombre,
                             l.CategoriaId,
                             Categoria = c.Nombre
                         }).FirstOrDefault();

            if (libro == null)
            {
                return NotFound();
            }
            return Ok(libro);
        }

        [HttpPut]
        [Route("/ModifyBookById")]
        public IActionResult ModifyBookById(int LibroId, [FromBody]Libro libroModificado)
        {
            Libro? libro = (from e in _contexto.Libros where e.LibroId == LibroId select e).FirstOrDefault();

            if(libro == null)
            {
                return NotFound();
            }

            libro.Titulo = libroModificado.Titulo;
            libro.AnioPublicacion = libroModificado.AnioPublicacion;
            libro.Resumen = libroModificado.Resumen;
            libro.AutorId = libroModificado.AutorId;
            libro.CategoriaId = libroModificado.CategoriaId;

            _contexto.Libros.Entry(libro).State = EntityState.Modified;
            _contexto.SaveChanges();

            return Ok(libro);
        }

        [HttpPut]
        [Route("/ModifyBookByTitle")]
        public IActionResult ModifyBookByTitle(string Titulo, [FromBody] Libro libroModificado)
        {
            Libro? libro = (from e in _contexto.Libros where e.Titulo == Titulo select e).FirstOrDefault();

            if (libro == null)
            {
                return NotFound();
            }

            libro.Titulo = libroModificado.Titulo;
            libro.AnioPublicacion = libroModificado.AnioPublicacion;
            libro.Resumen = libroModificado.Resumen;
            libro.AutorId = libroModificado.AutorId;
            libro.CategoriaId = libroModificado.CategoriaId;

            _contexto.Libros.Entry(libro).State = EntityState.Modified;
            _contexto.SaveChanges();

            return Ok(libro);
        }

        [HttpDelete]
        [Route("/DeleteBookById")]
        public IActionResult DeleteBookById(int LibroId)
        {
            Libro? libro = (from e in _contexto.Libros where e.LibroId == LibroId select e).FirstOrDefault();

            if (libro == null)
            {
                return NotFound();
            }

            _contexto.Libros.Attach(libro);
            _contexto.Libros.Remove(libro);
            _contexto.SaveChanges();

            return Ok(libro);
        }
    }
}
