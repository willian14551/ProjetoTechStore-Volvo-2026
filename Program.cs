using Microsoft.EntityFrameworkCore;
using ProjetoTechStore_Volvo_2026.Data;
using ProjetoTechStore_Volvo_2026.Service;
using Microsoft.OpenApi;


var construtor = WebApplication.CreateBuilder(args);

construtor.Services.AddControllers();


construtor.Services.AddDbContext<TechStoreContext>
    (options => options.UseSqlServer(construtor.Configuration.GetConnectionString("DefaultConnection"),
    sqlOptions => sqlOptions.EnableRetryOnFailure()));


construtor.Services.AddScoped<PedidoService>();


construtor.Services.AddEndpointsApiExplorer();
construtor.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Projeto TechStore API 2026",
        Version = "v1",
        Description = "Projeto feito e produzido por Willian Anderson da Rocha e Felipe da Silva Mossato",
        Contact = new OpenApiContact
        {
            Name = "GitHub Repo",
            Url = new Uri("https://github.com/willian14551/ProjetoTechStore-Volvo-2026"),
        }
    });
});


var app = construtor.Build();

app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "swagger";
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TechStore API v1");

        c.IndexStream = () => typeof(Program).Assembly
            .GetManifestResourceStream("ProjetoTechStore_Volvo_2026.wwwroot.index.html");
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

