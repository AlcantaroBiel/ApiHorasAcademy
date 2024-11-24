using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CadastroApi.Models
{
    public class Usuario
    {
        [Key]
        public int aluno_id { get; set; }

        public string nome { get; set; }

        public string email { get; set; }

        public int curso { get; set; }

        public int matricula { get; set; }

        public string senha { get; set; }

        public int admin { get; set; }
    }
}