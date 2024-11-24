using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CadastroApi.Models
{
    public class Curso
    {
        [Key]
        public int id_curso { get; set; }
        public string nome_curso { get; set; }
        public int horas { get; set; } // Ou o tipo apropriado para carga horária
    }
}