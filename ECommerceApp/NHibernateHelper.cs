using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NHibernate;
using ECommerceApp.Mappers;

namespace ECommerceApp
{
    public static class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;

        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    var configuration = Fluently.Configure()
                        .Database(PostgreSQLConfiguration.PostgreSQL82
                            .ConnectionString(c => c
                                .Host("172.17.0.3") // use localhost since the PostgreSQL server is running in a Docker container on your local machine
                                .Port(5432)
                                .Username("root") // the default PostgreSQL username
                                .Password("root") // the password you set when starting the PostgreSQL Docker container
                                .Database("postgres")) // the default PostgreSQL database name
                            .ShowSql())
                        .Mappings(m => m.FluentMappings
                            .AddFromAssemblyOf<ProductMapper>()
                            .AddFromAssemblyOf<OrderMapper>()
                         )
                        .BuildConfiguration();

                    _sessionFactory = configuration.BuildSessionFactory();
                }
                return _sessionFactory;
            }
        }

        public static NHibernate.ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }

        public static void InitializeDatabase()
        {
            var configuration = Fluently.Configure()
                .Database(PostgreSQLConfiguration.PostgreSQL82
                    .ConnectionString(c => c
                        .Host("172.17.0.3")
                                .Port(5432)
                                .Username("root")
                                .Password("root")
                                .Database("postgres"))
                    .ShowSql())
                .Mappings(m => m.FluentMappings
                    .AddFromAssemblyOf<ProductMapper>()
                    .AddFromAssemblyOf<OrderMapper>())
                .BuildConfiguration();

            var update = new SchemaExport(configuration);

            update.Execute(true, true, false);
        }
    }

}
