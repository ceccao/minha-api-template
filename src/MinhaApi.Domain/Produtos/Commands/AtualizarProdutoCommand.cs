namespace MinhaApi.Domain.Produtos.Commands;

// Carrega o Id junto (diferente do Request, que recebe o Id separado via rota) -
// um Command representa a intencao COMPLETA de uma acao, entao fica autocontido.
public record AtualizarProdutoCommand(int Id, string Nome, decimal Preco);
