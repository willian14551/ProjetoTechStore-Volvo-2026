using Microsoft.EntityFrameworkCore;
using ProjetoTechStore_Volvo_2026.Data;
using ProjetoTechStore_Volvo_2026.Service;

// Cria o construtor da aplica��o
// Al�m de ler as configura��es, preparar o DI, logging...
var construtor = WebApplication.CreateBuilder(args);


// Registra suporte para controllers
construtor.Services.AddControllers();


// Registra o Dbcontext no sistema de inje��o de dependencia
// Basicamente ele ta dizendo pro EF "Usa o sql server e pega a connectionstring TechStore/DefaultConnection)
//obs: mudei para default connection para funcionar melhor com o user-secrets.
construtor.Services.AddDbContext<TechStoreContext>
    (options => options.UseSqlServer(construtor.Configuration.GetConnectionString("DefaultConnection")));

construtor.Services.AddScoped<PedidoService>();


// Para o swagger descobrir os endpoints
construtor.Services.AddEndpointsApiExplorer();

//
construtor.Services.AddSwaggerGen();


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

