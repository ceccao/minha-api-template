using Mapster;
using MinhaApi.Application.Produtos.DataTransfer.Requests;
using MinhaApi.Application.Produtos.DataTransfer.Responses;
using MinhaApi.Domain.Produtos.Commands;
using MinhaApi.Domain.Produtos.Entities;

namespace MinhaApi.Application.Produtos.Profiles;

// Registrada via TypeAdapterConfig.GlobalSettings.Scan(assembly) no IoC.
// Concentra aqui as configs de conversao Request->Command e Entidade->Response -
// isso antes ficava implicito (por convencao) dentro da Service; agora que a
// Service virou so um wrapper de transacao, essa config explicita mora no Profile.
public class ProdutoProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Produto, ProdutoResponse>();

        // CriarProdutoRequest -> InserirProdutoCommand: nomes de propriedade batem
        // (Nome, Preco), o Mapster resolve via o construtor publico do Command.
        config.NewConfig<CriarProdutoRequest, InserirProdutoCommand>();

        // AtualizarProdutoRequest -> EditarProdutoCommand NAO tem config aqui de
        // proposito: o Command precisa do Id, que vem da rota (nao do Request) -
        // esse caso e montado direto na Service (Application), nao dá pra ser um
        // mapeamento 1:1 automatico.
    }
}