using ECommerceApp;
using ECommerceApp.Mappers;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using StackExchange.Redis;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure ISessionFactory
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

var sessionFactory = configuration.BuildSessionFactory();

builder.Services.AddSingleton<ISessionFactory>(sessionFactory);

// Initialize Redis
var redisConnectionString = "172.17.0.2:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
NHibernateHelper.InitializeDatabase();

app.MapControllers();

app.Run();
