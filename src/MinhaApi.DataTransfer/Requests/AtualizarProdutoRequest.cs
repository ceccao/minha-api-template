namespace MinhaApi.DataTransfer.Requests;

// Classe separada de CriarProdutoRequest de propósito (README §6.3 - Mass Assignment),
// mesmo que hoje tenha os mesmos campos. Nenhuma das duas expõe Id, CriadoEm, Ativo ou Version.
public record AtualizarProdutoRequest(string Nome, decimal Preco);
