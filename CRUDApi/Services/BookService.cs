using CRUDApi.Models;
using System.Security.Claims;

namespace CRUDApi.Services
{
    public class BookService : IBookService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public BookService(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }


        public Book Create(BookReqBody newBook)
        {
            var username = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = new Book
            {
                Name = newBook.Name,
                Author = newBook.Author,
                Description = newBook.Description,
                CreatedBy = username,
            };
            return result;
            
        }

        public async Task<Book> Get(BooksDbContext context, Guid id)
        {
            var result = await context.Books.FindAsync(id);
            if (result == null) { return null; }
            return result;
        }

        public async Task<Book> Update(BooksDbContext context, BookReqBody newBook, Guid id)
        {
            var outdatedBook = await context.Books.FindAsync(id);
            if (outdatedBook == null) { return null; }

            outdatedBook.Name = newBook.Name;
            outdatedBook.Author = newBook.Author;
            outdatedBook.Description = newBook.Description;

            return outdatedBook;

        }
    }
}
