using Microsoft.EntityFrameworkCore;
using ProjetoTechStore_Volvo_2026.Data;
using ProjetoTechStore_Volvo_2026.Service;
using Microsoft.OpenApi;

// Cria o construtor da aplica��o
// Al�m de ler as configura��es, preparar o DI, logging...
var construtor = WebApplication.CreateBuilder(args);


// Registra suporte para controllers
construtor.Services.AddControllers();


// Registra o Dbcontext no sistema de inje��o de dependencia
// Basicamente ele ta dizendo pro EF "Usa o sql server e pega a connectionstring TechStore/DefaultConnection)
//obs: mudei para default connection para funcionar melhor com o user-secrets.
construtor.Services.AddDbContext<TechStoreContext>
    (options => options.UseSqlServer(construtor.Configuration.GetConnectionString("DefaultConnection"),
    sqlOptions => sqlOptions.EnableRetryOnFailure()));


construtor.Services.AddScoped<PedidoService>();


// Para o swagger descobrir os endpoints
construtor.Services.AddEndpointsApiExplorer();
construtor.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TechStore Volvo 2026",
        Version = "v1",
        Description = "Projeto feito por Willian e Felipe para o Curso Volvo-PUCPR 2026",
        Contact = new OpenApiContact
        {
            Name = "GitHub Repo",
            Url = new Uri("https://github.com/willian14551/ProjetoTechStore-Volvo-2026"),
        }
    });
});


// Cria a aplica��o final
var app = construtor.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redireciona HTTP pro HTTPS
app.UseHttpsRedirection();

// Diz pro asp.net mapear as rotas dos controllers
app.MapControllers();

app.Run();

