using ApiPeliculas.Data;
using ApiPeliculas.Repositorio;
using ApiPeliculas.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ApiPeliculas.PeliculasMappers;

var builder = WebApplication.CreateBuilder(args);

//Configuramos la conexion a sqlServer
builder.Services.AddDbContext<ApplicationDbContext>(optiones => 
{
    optiones.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionDb")); 
});

//Agregamos los repositorios.
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<IPeliculaRepositorio, PeliculaRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

//Agregando Automapper
builder.Services.AddAutoMapper(typeof(PeliculasMapper).Assembly);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Soporte para CORS
builder.Services.AddCors( P => P.AddPolicy("PolicyCors", build =>
{
    build.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
}));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("PolicyCors");
app.UseAuthorization();

app.MapControllers();

app.Run();
