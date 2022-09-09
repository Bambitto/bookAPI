using CRUDApi.Models;
using CRUDApi.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CRUDApi.Requests
{
    public static class Requests
    {
        public static WebApplication RegisterEndpoins(this WebApplication app)
        {
            app.MapPost("/register", async (BooksDbContext context, UserReqBody user) => await Register(context, user))
                .WithValidator<UserReqBody>();

            app.MapPost("/create",
               [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
            async (BooksDbContext context, BookReqBody newBook, IBookService service) => await Create(context, newBook, service))
            .WithValidator<BookReqBody>();

            app.MapGet("/get",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, standard")]
            async (BooksDbContext context, Guid id, IBookService service) => await Get(context, id, service));

            app.MapGet("/list", async (BooksDbContext context) => await List(context));

            app.MapPut("/update",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
            (BooksDbContext context, BookReqBody newBook, Guid id, IBookService service) => Update(context, newBook, id, service));

            app.MapDelete("/delete",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
            async (BooksDbContext context, Guid id, IBookService service) => await Delete(context, id, service));

            return app;

            static async Task<IResult> Register(BooksDbContext context, UserReqBody user)
            {
                var newUser = new User
                {
                    Name = user.Name,
                    Email = user.Email,
                    Username = user.Username,
                    Surname = user.Surname,
                    Password = user.Password,
                    Role = "standard",
                    Id = new Guid(),
                };
                await context.AddAsync(newUser);
                await context.SaveChangesAsync();
                return Results.Ok(user);
            }

            static async Task<IResult> Create(BooksDbContext context, BookReqBody newBook, IBookService service)
            {
                var book = service.Create(newBook);
                await context.Books.AddAsync(book);
                await context.SaveChangesAsync();
                return Results.Ok(book);
            }

            static async Task<IResult> Get(BooksDbContext context, Guid id, IBookService service)
            {
                var result = await service.Get(context, id);
                if (result is null)
                {
                    return Results.NotFound("Book not found");
                }
                return Results.Ok(result);
            }

            static async Task<IResult> List(BooksDbContext context)
            {
                var result = await context.Books.ToListAsync();
                return Results.Ok(result);
            }


            static async Task<IResult> Update(BooksDbContext context, BookReqBody newBook, Guid id, IBookService service)
            {
                var result = service.Update(context, newBook, id);
                if (result is null) return Results.NotFound("Book not found");

                await context.SaveChangesAsync();

                return Results.Ok(result);
            }

            static async Task<IResult> Delete(BooksDbContext context, Guid id, IBookService service)
            {
                var book = await service.Get(context, id);

                if (book is null) return Results.NotFound("Book not found");

                context.Books.Remove(book);
                await context.SaveChangesAsync();

                return Results.Ok();
            }
        }
    }
}
