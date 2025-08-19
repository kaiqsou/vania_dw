using Microsoft.EntityFrameworkCore;
using SistemaAcademico.Models;

namespace SistemaAcademico.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Usuario> Usuarios {  get; set; } // criando tabela de Usuários
        public DbSet<Aluno> Alunos {  get; set; } // criando tabela de Alunos
        public DbSet<Curso> Cursos { get; set; } // criando tabela de Cursos
        public DbSet<Disciplina> Disciplinas { get; set; } // criando tabela de Disciplinas
        public DbSet<Matricula> Matriculas { get; set; } // criando tabela de Matrículas
    }

}
