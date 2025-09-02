namespace Projeto1DW.Models
{
    public class Produto
    {
        public int ProdutoId { get; set; }
        public string? Nome { get; set; }   // pode ser NULO, pois é nullable
        public double Preco { get; set; }
    }
}
