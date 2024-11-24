using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CadastroApi.Models
{
    public class Inscricao
    {
        [Key]
        public int id_inscricao { get; set; }

        [ForeignKey("Usuario")]
        public int aluno_id { get; set; }
        public Usuario Usuario { get; set; } // Relacionamento com a tabela de usuários

        [ForeignKey("Palestra")]
        public int palestra_id { get; set; }
        public Palestra Palestra { get; set; } // Relacionamento com a tabela de palestras

        public DateTime data_inscricao { get; set; } = DateTime.Now;
    }
}
