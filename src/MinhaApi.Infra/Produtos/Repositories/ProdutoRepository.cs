using MinhaApi.Domain.Produtos.Entities;
using MinhaApi.Domain.Produtos.Repositories;
using MinhaApi.Infra.Config;
using NHibernate;

namespace MinhaApi.Infra.Produtos.Repositories;

public class ProdutoRepository(ISession session) : RepositorioBase<Produto>(session), IProdutoRepository
{
}
