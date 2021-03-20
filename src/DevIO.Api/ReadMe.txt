03:35

add-migration Identity -Context ApplicationDbContext
update-database -Context ApplicationDbContext


 
public class FornecedoreViewModel
{
    public Guid Id { get; set; }

    public string Nome { get; set; }

    public string Documento { get; set; }

    public int TipoFornecedor { get; set; }

    public EnderecoViewModel Endereco { get; set; }

    public bool Ativo { get; set; }

    public IEnumerable<ProdutoViewModel> Produtos { get; set; }

}

public class EnderecoViewModel
{
      
    public Guid Id { get; set; }

       
    public string Logradouro { get; set; }

        
    public string Numero { get; set; }

    public string Complemento { get; set; }

    public string Bairro { get; set; }

      
    public string Cep { get; set; }

        
    public string Cidade { get; set; }

        
    public string Estado { get; set; }

    public Guid FornecedorId { get; set; }
}

public class ProdutoViewModel
{
     
    public Guid Id { get; set; }

    public Guid FornecedorId { get; set; }

        
    public string Nome { get; set; }
       
    public string Descricao { get; set; }

    public string ImagemUpload { get; set; }

    public string Imagem { get; set; }

      
    public decimal Valor { get; set; }

      
    public DateTime DataCadastro { get; set; }

    public bool Ativo { get; set; }

       
    public string NomeFornecedor { get; set; }
}