using Microsoft.EntityFrameworkCore;
using Contacts.Application;
using Contacts.Application.Queries;
using Contacts.Application.Repositories;
using Contacts.Application.Services;
using Contacts.Application.UseCases;
using Contacts.Data;
using Contacts.Application.UseCases.DTOs;

// Przygotowanie konfiguratora naszej aplikacji
var builder = WebApplication.CreateBuilder(args);
//Dodanie kontekstu danych
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlite(builder.Configuration.GetConnectionString("default"))
);
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<ICategoryQueries, CategoriesQueries>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<ICategoryUseCases, CategoryServices>();
// Dodanie usług koniecznych dla Swagger’a
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Zbudowanie aplikacji na podstawie konfiguracji
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Uruchomienie przekierowania żądań z HTTP do HTTPSS
app.UseHttpsRedirection();

app.MapPost("/api/categories",
    (AddCategoryDTO dto, ICategoryUseCases useCases) =>{
    try
    {
        var category = useCases.AddCategory(dto);
        return Results.Created($"/categories/{category.Id}", category);
    }
    catch
    {
        return Results.Problem(
            detail: "Wystpił błąd podczas realizacji tego żądania",
            title: "Błąd"
        );
    }
})
.WithOpenApi((operation) =>
{
    operation.Responses["201"].Description = "Kategoria została dodana";
    operation.Responses["500"].Description = "Wystąpił błąd";
    return operation;
})
.WithDescription("Tworzy nową kategorię")
.WithSummary("Tworzy nową kategorię")
.WithTags("Kategoria")
.Produces<AddCategoryResult>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status500InternalServerError);


app.MapGet("/api/categories", (ICategoryQueries queries) => {
try
    {
        var categories = queries.GetCategories();
        return Results.Ok(categories);
    }
  
    catch
    {
        return Results.Problem(
            detail: "Wystpił błąd podczas realizacji tego żądania",
            title: "Błąd"
        );
    }
});
// Uruchomienie aplikacji
app.Run();
