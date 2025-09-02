using Microsoft.EntityFrameworkCore;
using Projeto1DW.Models;

namespace Projeto1DW.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Produto> Produtos {  get; set; } // criando tabela de Produtos
    }
}
