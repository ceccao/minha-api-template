using MinhaApi.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIoC(builder.Configuration);

// Captive Dependency (README §7.6): valida em desenvolvimento que nenhum Singleton
// esta capturando algo Scoped (como ISession) — isso quebraria em producao de um
// jeito sutil, sem erro de compilacao.
builder.Host.UseDefaultServiceProvider((ctx, opts) =>
{
    opts.ValidateScopes = ctx.HostingEnvironment.IsDevelopment();
    opts.ValidateOnBuild = ctx.HostingEnvironment.IsDevelopment();
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
