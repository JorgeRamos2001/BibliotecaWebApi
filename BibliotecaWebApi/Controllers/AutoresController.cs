using BibliotecaWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BibliotecaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoresController : ControllerBase
    {
        private readonly BibliotecaDbContext _contexto;

        public AutoresController(BibliotecaDbContext contexto)
        {
            _contexto = contexto;
        }

        [HttpPost]
        [Route("/AddAuthor")]
        public IActionResult Add([FromBody]Autor autor)
        {
            try
            {
                _contexto.Autores.Add(autor);
                _contexto.SaveChanges();
                return Ok(autor);
            }catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("/GetAllAuthors")]
        public IActionResult GetAll()
        {
            List<Autor> autores = (from e in _contexto.Autores select e).ToList();

            return Ok(autores);
        }

        [HttpGet]
        [Route("/GetAuthorWithMostBooks")]
        public IActionResult GetAuthorWithMostBooks()
        {
            var autoresConMasLibros = (from a in _contexto.Autores
                                       join l in _contexto.Libros
                                       on a.AutorId equals l.AutorId into librosAgrupados
                                       select new
                                       {
                                           a.AutorId,
                                           a.Nombre,
                                           a.Nacionalidad,
                                           CantidadLibrosPublicados = librosAgrupados.Count()
                                       })
                                       .OrderByDescending(a => a.CantidadLibrosPublicados)
                                       .ToList();

            return Ok(autoresConMasLibros);
        }

        [HttpGet]
        [Route("/GetAuthorById")]
        public IActionResult GetById(int AutorId)
        {
            var autor = (from a in _contexto.Autores 
                         join l in _contexto.Libros 
                         on a.AutorId equals l.AutorId into LibrosAutor
                         where a.AutorId == AutorId
                         select new
                         {
                             a.AutorId,
                             a.Nombre,
                             a.Nacionalidad,
                             Libros = LibrosAutor.ToList()
                         }).FirstOrDefault();

            if (autor == null)
            {
                return NotFound();
            }

            return Ok(autor);
        }

        [HttpGet]
        [Route("/GetAuthorByName")]
        public IActionResult GetByName(string Nombre)
        {
            var autor = (from a in _contexto.Autores
                         join l in _contexto.Libros
                         on a.AutorId equals l.AutorId into LibrosAutor
                         where a.Nombre == Nombre
                         select new
                         {
                             a.AutorId,
                             a.Nombre,
                             a.Nacionalidad,
                             Libros = LibrosAutor.ToList()
                         }).FirstOrDefault();

            if (autor == null)
            {
                return NotFound();
            }

            return Ok(autor);
        }

        [HttpPut]
        [Route("/ModifyAuthorById")]
        public IActionResult ModifyById(int AutorId, [FromBody]Autor autorModificado)
        {
            Autor? autor = (from e in _contexto.Autores where e.AutorId == AutorId select e).FirstOrDefault();

            if(autor == null)
            {
                return NotFound();
            }

            autor.Nombre = autorModificado.Nombre;
            autor.Nacionalidad = autorModificado.Nacionalidad;

            _contexto.Autores.Entry(autor).State = EntityState.Modified;
            _contexto.SaveChanges();

            return Ok(autor);
        }


        [HttpPut]
        [Route("/ModifyAuthorByName")]
        public IActionResult ModifyByName(string Nombre, [FromBody] Autor autorModificado)
        {
            Autor? autor = (from e in _contexto.Autores where e.Nombre == Nombre select e).FirstOrDefault();

            if (autor == null)
            {
                return NotFound();
            }

            autor.Nombre = autorModificado.Nombre;
            autor.Nacionalidad = autorModificado.Nacionalidad;

            _contexto.Autores.Entry(autor).State = EntityState.Modified;
            _contexto.SaveChanges();

            return Ok(autor);
        }

        [HttpDelete]
        [Route("/DeleteAuthorById")]
        public IActionResult DeleteById(int AutorId)
        {
            Autor? autor = (from e in _contexto.Autores where e.AutorId == AutorId select e).FirstOrDefault();

            if (autor == null)
            {
                return NotFound();
            }

            _contexto.Autores.Attach(autor);
            _contexto.Autores.Remove(autor);
            _contexto.SaveChanges();

            return Ok(autor);
        }

        [HttpDelete]
        [Route("/DeleteAuthorByName")]
        public IActionResult DeleteByName(string Nombre)
        {
            Autor? autor = (from e in _contexto.Autores where e.Nombre == Nombre select e).FirstOrDefault();

            if (autor == null)
            {
                return NotFound();
            }

            _contexto.Autores.Attach(autor);
            _contexto.Autores.Remove(autor);
            _contexto.SaveChanges();

            return Ok(autor);
        }
    }
}
