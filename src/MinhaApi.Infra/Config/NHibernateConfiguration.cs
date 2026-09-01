using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using MinhaApi.Infra.Produtos.Mappings;
using NHibernate;
using NHibernate.Dialect;

namespace MinhaApi.Infra.Config;

public static class NHibernateConfiguration
{
    // ISessionFactory e cara de construir - por isso e Singleton (README §7.3/9.4),
    // registrada assim na FASE 10 (IoC). Este metodo so monta a configuracao.
    public static ISessionFactory CriarSessionFactory(string connectionString)
    {
        return Fluently.Configure()
            .Database(MySQLConfiguration.Standard
                .ConnectionString(connectionString)
                .Driver<NHibernate.Driver.MySqlConnector.MySqlConnectorDriver>()
                .Dialect<MySQL8Dialect>())
            // AddFromAssemblyOf<ProdutoMap> so serve de "ancora" pra achar o assembly -
            // escaneia TODOS os ClassMap<> do assembly inteiro, nao so o de Produto.
            .Mappings(m => m.FluentMappings.AddFromAssemblyOf<ProdutoMap>())
            .ExposeConfiguration(cfg =>
            {
                cfg.SetProperty(NHibernate.Cfg.Environment.CommandTimeout, "30");
            })
            .BuildSessionFactory();
    }
}
