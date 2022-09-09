using CRUDApi.Models;
using System.Security.Claims;

namespace CRUDApi.Services
{
    public interface IBookService
    {
        public Book Create(BookReqBody newBook);
        public Task<Book> Get(BooksDbContext context, Guid id);
        public Task<Book> Update(BooksDbContext context, BookReqBody newBook, Guid id);
    }
}
