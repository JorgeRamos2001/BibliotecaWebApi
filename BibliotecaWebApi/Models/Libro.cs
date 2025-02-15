namespace BibliotecaWebApi.Models
{
    public class Libro
    {
        public int LibroId { get; set; }    
        public string Titulo { get; set; }
        public int AnioPublicacion { get; set; }
        public string? Resumen { get; set; }
        public int AutorId { get; set; }
        public int CategoriaId { get; set; }
    }
}
