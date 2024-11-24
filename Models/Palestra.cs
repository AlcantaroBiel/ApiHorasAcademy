using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CadastroApi.Models
{
    public class Palestra
    {
        [Key]
        public int palestra_id { get; set; }

        public string titulo { get; set; }

        public DateTime data_palestra { get; set; }

        public int carga_horaria { get; set; }

        public string descricao { get; set; }

        public string local { get; set; }  // Novo campo

        public TimeSpan hora { get; set; } // Novo campo

        public string imagem { get; set; } // Novo campo (URL ou caminho da imagem)

        public string categoria { get; set; } // Novo campo
    }
}
