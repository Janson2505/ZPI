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

// Repositories
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();

// Queries
builder.Services.AddTransient<ICategoryQueries, CategoriesQueries>();

// Unit of work
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

// UseCases / Services
builder.Services.AddTransient<ICategoryUseCases, CategoryServices>();
builder.Services.AddTransient<IContactUseCases, ContactServices>();
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

// Categories
app.MapPost("/api/categories",
    (AddCategoryDTO dto, ICategoryUseCases useCases) =>
    {
        try
        {
            var category = useCases.AddCategory(dto);
            return Results.Created($"/api/categories/{category.Id}", category);
        }
        catch (ServiceException ex)
        {
            return Results.Problem(title: "Błąd biznesowy", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
        catch
        {
            return Results.Problem(
                detail: "Wystąpił błąd podczas realizacji tego żądania",
                title: "Błąd",
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }
)
.WithOpenApi(operation =>
{
    operation.Summary = "Tworzy nową kategorię";
    operation.Description = "Dodaje kategorię. Zwraca 201 oraz utworzoną kategorię.";
    operation.Responses["201"].Description = "Kategoria została dodana";
    operation.Responses["400"].Description = "Błąd walidacji/biznesowy";
    operation.Responses["500"].Description = "Wystąpił błąd";
    return operation;
})
.WithTags("Kategoria")
.Produces<AddCategoryResult>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest)
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
            detail: "Wystąpił błąd podczas realizacji tego żądania",
            title: "Błąd",
            statusCode: StatusCodes.Status500InternalServerError
        );
    }
})
.WithOpenApi(operation =>
{
    operation.Summary = "Lista kategorii";
    operation.Description = "Pobiera listę wszystkich kategorii.";
    operation.Responses["200"].Description = "Lista kategorii";
    operation.Responses["500"].Description = "Wystąpił błąd";
    return operation;
})
.WithTags("Kategoria")
.Produces<IEnumerable<Contacts.Application.Queries.DTOs.CategoryDTO>>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status500InternalServerError);

app.MapGet("/api/categories/{id:int}", (int id, ICategoryQueries queries) =>
    {
        try
        {
            var category = queries.GetCategory(id);
            return category is not null ? Results.Ok(category) : Results.NotFound();
        }
        catch
        {
            return Results.Problem(
                detail: "Wystąpił błąd podczas realizacji tego żądania",
                title: "Błąd",
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }
)
.WithOpenApi(operation =>
{
    operation.Summary = "Szczegóły kategorii";
    operation.Description = "Pobiera kategorię po Id.";
    operation.Responses["200"].Description = "Znaleziono";
    operation.Responses["404"].Description = "Nie znaleziono";
    operation.Responses["500"].Description = "Wystąpił błąd";
    return operation;
})
.WithTags("Kategoria")
.Produces<Contacts.Application.Queries.DTOs.CategoryDTO>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status500InternalServerError);

app.MapPut("/api/categories/{id:int}", (int id, UpdateCategoryDTO dto, ICategoryUseCases useCases) =>
    {
        try
        {
            dto.Id = id;
            useCases.UpdateCategory(dto);
            return Results.NoContent();
        }
        catch (ServiceException ex)
        {
            return Results.Problem(title: "Błąd biznesowy", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
        catch
        {
            return Results.Problem(
                detail: "Wystąpił błąd podczas realizacji tego żądania",
                title: "Błąd",
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }
)
.WithOpenApi(operation =>
{
    operation.Summary = "Aktualizacja kategorii";
    operation.Description = "Aktualizuje nazwę kategorii.";
    operation.Responses["204"].Description = "Zaktualizowano";
    operation.Responses["400"].Description = "Błąd walidacji/biznesowy";
    operation.Responses["500"].Description = "Wystąpił błąd";
    return operation;
})
.WithTags("Kategoria")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status500InternalServerError);

app.MapDelete("/api/categories/{id:int}", (int id, ICategoryUseCases useCases) =>
    {
        try
        {
            useCases.RemoveCategory(id);
            return Results.NoContent();
        }
        catch (ServiceException ex)
        {
            return Results.Problem(title: "Błąd biznesowy", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
        catch
        {
            return Results.Problem(
                detail: "Wystąpił błąd podczas realizacji tego żądania",
                title: "Błąd",
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }
)
.WithOpenApi(operation =>
{
    operation.Summary = "Usunięcie kategorii";
    operation.Description = "Usuwa kategorię po Id (zabezpieczenie przed usunięciem, jeśli są emaile w tej kategorii).";
    operation.Responses["204"].Description = "Usunięto";
    operation.Responses["400"].Description = "Błąd biznesowy (np. kategoria w użyciu)";
    operation.Responses["500"].Description = "Wystąpił błąd";
    return operation;
})
.WithTags("Kategoria")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status500InternalServerError);

// Contacts
app.MapPost("/api/contacts", (AddContactDTO dto, IContactUseCases useCases) =>
    {
        try
        {
            var created = useCases.AddContact(dto);
            return Results.Created($"/api/contacts/{created.Id}", created);
        }
        catch (ServiceException ex)
        {
            return Results.Problem(title: "Błąd biznesowy", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
        catch
        {
            return Results.Problem(detail: "Wystąpił błąd podczas realizacji tego żądania", title: "Błąd", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
)
.WithOpenApi(op =>
{
    op.Summary = "Tworzy nowy kontakt";
    op.Description = "Dodaje kontakt (bez powiązanych elementów).";
    op.Responses["201"].Description = "Kontakt został dodany";
    op.Responses["400"].Description = "Błąd walidacji/biznesowy";
    op.Responses["500"].Description = "Wystąpił błąd";
    return op;
})
.WithTags("Kontakt")
.Produces<AddContactResult>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status500InternalServerError);

app.MapPut("/api/contacts/{id:int}", (int id, UpdateContactDTO dto, IContactUseCases useCases) =>
    {
        try
        {
            dto.Id = id;
            useCases.UpdateContact(dto);
            return Results.NoContent();
        }
        catch (ServiceException ex)
        {
            return Results.Problem(title: "Błąd biznesowy", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
        catch
        {
            return Results.Problem(detail: "Wystąpił błąd podczas realizacji tego żądania", title: "Błąd", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
)
.WithOpenApi(op =>
{
    op.Summary = "Aktualizacja kontaktu";
    op.Description = "Aktualizuje podstawowe dane kontaktu.";
    op.Responses["204"].Description = "Zaktualizowano";
    op.Responses["400"].Description = "Błąd walidacji/biznesowy";
    op.Responses["500"].Description = "Wystąpił błąd";
    return op;
})
.WithTags("Kontakt")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status500InternalServerError);

app.MapDelete("/api/contacts/{id:int}", (int id, IContactUseCases useCases) =>
    {
        try
        {
            useCases.RemoveContact(id);
            return Results.NoContent();
        }
        catch (ServiceException ex)
        {
            return Results.Problem(title: "Błąd biznesowy", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
        catch
        {
            return Results.Problem(detail: "Wystąpił błąd podczas realizacji tego żądania", title: "Błąd", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
)
.WithOpenApi(op =>
{
    op.Summary = "Usunięcie kontaktu";
    op.Description = "Usuwa kontakt wraz z powiązaniami (kaskadowo).";
    op.Responses["204"].Description = "Usunięto";
    op.Responses["400"].Description = "Błąd biznesowy";
    op.Responses["500"].Description = "Wystąpił błąd";
    return op;
})
.WithTags("Kontakt")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status500InternalServerError);

// Emails
app.MapPost("/api/contacts/{contactId:int}/emails", (int contactId, AddEmailDTO dto, IContactUseCases useCases) =>
    {
        try
        {
            dto = new AddEmailDTO { ContactId = contactId, Address = dto.Address, CategoryId = dto.CategoryId };
            useCases.AddEmail(dto);
            return Results.NoContent();
        }
        catch (ServiceException ex)
        {
            return Results.Problem(title: "Błąd biznesowy", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
        catch
        {
            return Results.Problem(detail: "Wystąpił błąd podczas realizacji tego żądania", title: "Błąd", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
)
.WithOpenApi(op =>
{
    op.Summary = "Dodanie adresu email do kontaktu";
    op.Description = "Dodaje email do wskazanego kontaktu w wybranej kategorii.";
    op.Responses["204"].Description = "Dodano";
    op.Responses["400"].Description = "Błąd biznesowy";
    op.Responses["500"].Description = "Wystąpił błąd";
    return op;
})
.WithTags("Email")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status500InternalServerError);

app.MapDelete("/api/contacts/{contactId:int}/emails/{emailId:int}", (int contactId, int emailId, IContactUseCases useCases) =>
    {
        try
        {
            useCases.RemoveEmail(new RemoveEmailDTO { ContactId = contactId, EmailId = emailId });
            return Results.NoContent();
        }
        catch (ServiceException ex)
        {
            return Results.Problem(title: "Błąd biznesowy", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
        catch
        {
            return Results.Problem(detail: "Wystąpił błąd podczas realizacji tego żądania", title: "Błąd", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
)
.WithOpenApi(op =>
{
    op.Summary = "Usunięcie adresu email (po Id)";
    op.Description = "Usuwa email przypisany do kontaktu po Id emaila.";
    op.Responses["204"].Description = "Usunięto";
    op.Responses["400"].Description = "Błąd biznesowy";
    op.Responses["500"].Description = "Wystąpił błąd";
    return op;
})
.WithTags("Email")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status500InternalServerError);

// Optional: remove email by address via query ?address=
app.MapDelete("/api/contacts/{contactId:int}/emails", (int contactId, string address, IContactUseCases useCases) =>
    {
        try
        {
            useCases.RemoveEmail(new RemoveEmailDTO { ContactId = contactId, Address = address });
            return Results.NoContent();
        }
        catch (ServiceException ex)
        {
            return Results.Problem(title: "Błąd biznesowy", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
        catch
        {
            return Results.Problem(detail: "Wystąpił błąd podczas realizacji tego żądania", title: "Błąd", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
)
.WithOpenApi(op =>
{
    op.Summary = "Usunięcie adresu email (po adresie)";
    op.Description = "Usuwa email przypisany do kontaktu po adresie (parametr zapytania 'address').";
    op.Responses["204"].Description = "Usunięto";
    op.Responses["400"].Description = "Błąd biznesowy";
    op.Responses["500"].Description = "Wystąpił błąd";
    return op;
})
.WithTags("Email")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status500InternalServerError);

// Phones
app.MapPost("/api/contacts/{contactId:int}/phones", (int contactId, AddPhoneDTO dto, IContactUseCases useCases) =>
    {
        try
        {
            dto = new AddPhoneDTO { ContactId = contactId, Number = dto.Number, CategoryId = dto.CategoryId };
            useCases.AddPhone(dto);
            return Results.NoContent();
        }
        catch (ServiceException ex)
        {
            return Results.Problem(title: "Błąd biznesowy", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
        catch
        {
            return Results.Problem(detail: "Wystąpił błąd podczas realizacji tego żądania", title: "Błąd", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
)
.WithOpenApi(op =>
{
    op.Summary = "Dodanie numeru telefonu do kontaktu";
    op.Description = "Dodaje numer telefonu do wskazanego kontaktu.";
    op.Responses["204"].Description = "Dodano";
    op.Responses["400"].Description = "Błąd biznesowy";
    op.Responses["500"].Description = "Wystąpił błąd";
    return op;
})
.WithTags("Telefon")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status500InternalServerError);

app.MapDelete("/api/contacts/{contactId:int}/phones/{phoneId:int}", (int contactId, int phoneId, IContactUseCases useCases) =>
    {
        try
        {
            useCases.RemovePhone(new RemovePhoneDTO { ContactId = contactId, PhoneId = phoneId });
            return Results.NoContent();
        }
        catch (ServiceException ex)
        {
            return Results.Problem(title: "Błąd biznesowy", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
        catch
        {
            return Results.Problem(detail: "Wystąpił błąd podczas realizacji tego żądania", title: "Błąd", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
)
.WithOpenApi(op =>
{
    op.Summary = "Usunięcie numeru telefonu (po Id)";
    op.Description = "Usuwa numer telefonu przypisany do kontaktu po Id numeru.";
    op.Responses["204"].Description = "Usunięto";
    op.Responses["400"].Description = "Błąd biznesowy";
    op.Responses["500"].Description = "Wystąpił błąd";
    return op;
})
.WithTags("Telefon")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status500InternalServerError);

// Important dates
app.MapPost("/api/contacts/{contactId:int}/dates", (int contactId, AddImportantDateDTO dto, IContactUseCases useCases) =>
    {
        try
        {
            dto = new AddImportantDateDTO { ContactId = contactId, Date = dto.Date, Type = dto.Type, Description = dto.Description };
            useCases.AddImportantDate(dto);
            return Results.NoContent();
        }
        catch (ServiceException ex)
        {
            return Results.Problem(title: "Błąd biznesowy", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
        catch
        {
            return Results.Problem(detail: "Wystąpił błąd podczas realizacji tego żądania", title: "Błąd", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
)
.WithOpenApi(op =>
{
    op.Summary = "Dodanie ważnej daty";
    op.Description = "Dodaje ważną datę do kontaktu.";
    op.Responses["204"].Description = "Dodano";
    op.Responses["400"].Description = "Błąd biznesowy";
    op.Responses["500"].Description = "Wystąpił błąd";
    return op;
})
.WithTags("Ważne daty")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status500InternalServerError);

app.MapPut("/api/contacts/{contactId:int}/dates/{dateId:int}", (int contactId, int dateId, UpdateImportantDateDTO dto, IContactUseCases useCases) =>
    {
        try
        {
            dto = new UpdateImportantDateDTO { ContactId = contactId, DateId = dateId, Date = dto.Date, Type = dto.Type, Description = dto.Description };
            useCases.UpdateImportantDate(dto);
            return Results.NoContent();
        }
        catch (ServiceException ex)
        {
            return Results.Problem(title: "Błąd biznesowy", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
        catch
        {
            return Results.Problem(detail: "Wystąpił błąd podczas realizacji tego żądania", title: "Błąd", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
)
.WithOpenApi(op =>
{
    op.Summary = "Aktualizacja ważnej daty";
    op.Description = "Aktualizuje ważną datę kontaktu.";
    op.Responses["204"].Description = "Zaktualizowano";
    op.Responses["400"].Description = "Błąd biznesowy";
    op.Responses["500"].Description = "Wystąpił błąd";
    return op;
})
.WithTags("Ważne daty")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status500InternalServerError);

app.MapDelete("/api/contacts/{contactId:int}/dates/{dateId:int}", (int contactId, int dateId, IContactUseCases useCases) =>
    {
        try
        {
            useCases.RemoveImportantDate(new RemoveImportantDateDTO { ContactId = contactId, DateId = dateId });
            return Results.NoContent();
        }
        catch (ServiceException ex)
        {
            return Results.Problem(title: "Błąd biznesowy", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
        catch
        {
            return Results.Problem(detail: "Wystąpił błąd podczas realizacji tego żądania", title: "Błąd", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
)
.WithOpenApi(op =>
{
    op.Summary = "Usunięcie ważnej daty";
    op.Description = "Usuwa ważną datę kontaktu.";
    op.Responses["204"].Description = "Usunięto";
    op.Responses["400"].Description = "Błąd biznesowy";
    op.Responses["500"].Description = "Wystąpił błąd";
    return op;
})
.WithTags("Ważne daty")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status500InternalServerError);
// Uruchomienie aplikacji
app.Run();
