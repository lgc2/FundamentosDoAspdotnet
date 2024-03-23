using Blog.Data;
using Blog.Services;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });
builder.Services.AddDbContext<BlogDataContext>();
builder.Services.AddTransient<TokenService>(); // sempre cria um novo
//builder.Services.AddScoped(); // instancia uma vez por transação/requisição
//builder.Services.AddSingleton(); // instancia uma vez por app

var app = builder.Build();
app.MapControllers();

app.Run();
