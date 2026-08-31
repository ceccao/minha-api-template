using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using MinhaApi.Infra.Persistence.Mappings;
using NHibernate;
using NHibernate.Dialect;

namespace MinhaApi.Infra.Persistence;

public static class NHibernateConfiguration
{
    // ISessionFactory e cara de construir - por isso e Singleton (README §7.3/9.3),
    // registrada assim na FASE 10 (IoC). Este metodo so monta a configuracao.
    public static ISessionFactory CriarSessionFactory(string connectionString)
    {
        return Fluently.Configure()
            .Database(MySQLConfiguration.Standard
                .ConnectionString(connectionString)
                // Driver do pacote NHibernate.Driver.MySqlConnector (separado do NHibernate
                // core) - usamos ele porque nosso ADO.NET provider e o MySqlConnector, nao
                // o MySql.Data oficial da Oracle que o MySqlDataDriver padrao espera.
                .Driver<NHibernate.Driver.MySqlConnector.MySqlConnectorDriver>()
                .Dialect<MySQL8Dialect>())
            .Mappings(m => m.FluentMappings.AddFromAssemblyOf<ProdutoMap>())
            .ExposeConfiguration(cfg =>
            {
                // Timeout de comando (README §6.2/9.4) - evita que uma query travada
                // segure o pool de conexoes indefinidamente.
                cfg.SetProperty(NHibernate.Cfg.Environment.CommandTimeout, "30");
            })
            .BuildSessionFactory();
    }
}