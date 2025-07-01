using Microsoft.EntityFrameworkCore;
using PagueVeloz.Challenge.Application.Handlers;
using PagueVeloz.Challenge.Domain.Interfaces;
using PagueVeloz.Challenge.Infrastructure.Data;
using PagueVeloz.Challenge.Infrastructure.Repositories;
using FluentValidation; 
using FluentValidation.AspNetCore; 
using PagueVeloz.Challenge.Application.Validators;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
{
    // Use um arquivo SQLite no diretório da aplicação
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Adicionar e configurar FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

builder.Services.AddValidatorsFromAssemblyContaining<CriarClienteDTOValidator>();


// Registrar os Repositórios
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IContaRepository, ContaRepository>(); 
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IContaRepository, ContaRepository>();
builder.Services.AddScoped<ITransacaoRepository, TransacaoRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Registrar os Handlers da camada de aplicação
builder.Services.AddScoped<CriarClienteCommandHandler>();
builder.Services.AddScoped<RealizarVendaCommandHandler>();
builder.Services.AddScoped<CriarClienteCommandHandler>();
builder.Services.AddScoped<RealizarVendaCommandHandler>();
builder.Services.AddScoped<RealizarEstornoCommandHandler>();
builder.Services.AddScoped<RealizarTransferenciaCommandHandler>();
builder.Services.AddScoped<GetSaldoContaQueryHandler>(); 
builder.Services.AddScoped<GetHistoricoTransacoesQueryHandler>(); 
builder.Services.AddScoped<GetTransacaoByIdQueryHandler>();
builder.Services.AddScoped<RealizarDepositoCommandHandler>();
builder.Services.AddValidatorsFromAssemblyContaining<CriarClienteCommandValidator>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
