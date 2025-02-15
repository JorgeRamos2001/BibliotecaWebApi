using BibliotecaWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BibliotecaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly BibliotecaDbContext _contexto;

        public CategoriasController(BibliotecaDbContext contexto)
        {
            _contexto = contexto;
        }

        [HttpPost]
        [Route("/AddCategory")]
        public IActionResult Add([FromBody] Categoria categoria)
        {
            try
            {
                _contexto.Categorias.Add(categoria);
                _contexto.SaveChanges();
                return Ok(categoria);
            }
            catch (Exception ex)
            { 
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("/GetAllCategories")]
        public IActionResult GetAll()
        {
            List<Categoria> categorias = (from e in _contexto.Categorias select e).ToList();

            return Ok(categorias);
        }

        [HttpGet]
        [Route("/GetVCategoryById")]
        public IActionResult GetById(int CategoriaId)
        {
            var categoria = (from c in _contexto.Categorias
                             join l in _contexto.Libros
                             on c.CategoriaId equals l.CategoriaId into CategoriaLibros
                             where c.CategoriaId == CategoriaId
                             select new
                             {
                                 c.CategoriaId,
                                 c.Nombre,
                                 Libros = CategoriaLibros.ToList()
                             }).FirstOrDefault();   

            if (categoria == null)
            {
                return NotFound();
            }

            return Ok(categoria);
        }

        [HttpGet]
        [Route("/GetCategoryByName")]
        public IActionResult GetByName(string Nombre)
        {
            var categoria = (from c in _contexto.Categorias
                             join l in _contexto.Libros
                             on c.CategoriaId equals l.CategoriaId into CategoriaLibros
                             where c.Nombre == Nombre
                             select new
                             {
                                 c.CategoriaId,
                                 c.Nombre,
                                 Libros = CategoriaLibros.ToList()
                             }).FirstOrDefault();

            if (categoria == null)
            {
                return NotFound();
            }

            return Ok(categoria);
        }

        [HttpPut]
        [Route("/ModifyCategoryById")]
        public IActionResult ModifyById(int CategoriaId, [FromBody]Categoria categoriaModificada)
        {
            Categoria? categoria = (from e in _contexto.Categorias where e.CategoriaId == CategoriaId select e).FirstOrDefault();

            if (categoria == null)
            {
                return NotFound();
            }

            categoria.Nombre = categoriaModificada.Nombre;

            _contexto.Categorias.Entry(categoria).State = EntityState.Modified;
            _contexto.SaveChanges();

            return Ok(categoria);
        }

        [HttpPut]
        [Route("/ModifyCategoryByName")]
        public IActionResult ModifyByName(string Nombre, [FromBody] Categoria categoriaModificada)
        {
            Categoria? categoria = (from e in _contexto.Categorias where e.Nombre == Nombre select e).FirstOrDefault();

            if (categoria == null)
            {
                return NotFound();
            }

            categoria.Nombre = categoriaModificada.Nombre;

            _contexto.Categorias.Entry(categoria).State = EntityState.Modified;
            _contexto.SaveChanges();

            return Ok(categoria);
        }

        [HttpDelete]
        [Route("/DeleteCategoryById")]
        public IActionResult DeleteById(int CategoriaId)
        {
            Categoria? categoria = (from e in _contexto.Categorias where e.CategoriaId == CategoriaId select e).FirstOrDefault();

            if (categoria == null)
            {
                return NotFound();
            }

            _contexto.Categorias.Attach(categoria);
            _contexto.Categorias.Remove(categoria);
            _contexto.SaveChanges();    

            return Ok(categoria);
        }

        [HttpDelete]
        [Route("/DeleteCategoryByName")]
        public IActionResult DeleteByName(string Nombre)
        {
            Categoria? categoria = (from e in _contexto.Categorias where e.Nombre == Nombre select e).FirstOrDefault();

            if (categoria == null)
            {
                return NotFound();
            }

            _contexto.Categorias.Attach(categoria);
            _contexto.Categorias.Remove(categoria);
            _contexto.SaveChanges();

            return Ok(categoria);
        }

    }
}
