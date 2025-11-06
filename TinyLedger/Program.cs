using Application.Dto;
using Application.Services.Interfaces;
using Data.Repository.Interfaces;
using Data.Repository.Repositories;
using Domain.Services;
using Domain.Services.Interfaces;
using ApplicationAdapterHelper = Application.Services.Helper.Adapterhelper;
using DomainAdapterHelper = Domain.Services.AdapterHelper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ITransactionRepository, TransactionRepository>();
builder.Services.AddSingleton<IBalanceRepository, BalanceRepository>();

builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IBalanceService, BalanceService>();

builder.Services.AddScoped<IToDboAdapter<Domain.Model.Transaction, Data.Dbo.TransactionDbo>, DomainAdapterHelper>();
builder.Services.AddScoped<IDboToDomainAdapter<Domain.Model.Balance, Data.Dbo.BalanceDbo>, DomainAdapterHelper>();
builder.Services.AddScoped<IDboToDomainAdapter<Domain.Model.Transaction, Data.Dbo.TransactionDbo>, DomainAdapterHelper>();
builder.Services.AddScoped<IToDomainAdapter<Domain.Model.Transaction, TransactionRequestDto>, ApplicationAdapterHelper>();
builder.Services.AddScoped<IToDtoAdapter<Domain.Model.Transaction, TransactionResponseDto>, ApplicationAdapterHelper>();
builder.Services.AddScoped<IToDtoAdapter<Domain.Model.Balance, BalanceDto>, ApplicationAdapterHelper>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
