using Microsoft.EntityFrameworkCore;
using CadastroApi.Models;

namespace CadastroApi.Data
{
    public class CadastroContext : DbContext
    {
        public CadastroContext(DbContextOptions<CadastroContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Palestra> Palestras { get; set;}
        public DbSet<Inscricao> Inscricao { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().ToTable("usuarios"); // Especifica o nome da tabela se necessário
            modelBuilder.Entity<Curso>().ToTable("cursos");
            modelBuilder.Entity<Palestra>().ToTable("palestras");
            modelBuilder.Entity<Inscricao>().ToTable("inscricoes");

        }
    }
}