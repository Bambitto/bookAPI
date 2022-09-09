using FluentValidation;

namespace CRUDApi.Models
{
    public class RequestBodies
    {
    }

    public class BookReqBody
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
    }

    public class UserReqBody
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class BookRequestValidator : AbstractValidator<BookReqBody>
    {
        public BookRequestValidator()
        {
            RuleFor(b => b.Name)
                .NotEmpty()
                .WithMessage("Name cannot be empty");

            RuleFor(b => b.Author)
                .NotEmpty()
                .WithMessage("Author cannot be empty");

        }
    }

    public class UserReqValidator : AbstractValidator<UserReqBody>
    {
        public UserReqValidator()
        {
            RuleFor(u => u.Username)
                .NotEmpty()
                .WithMessage("Username cannot be empty");

            RuleFor(u => u.Email)
                .EmailAddress()
                .WithMessage("This is not valid email address");

            RuleFor(u => u.Password)
                .NotEmpty()
                .WithMessage("Password cannot be empty");
        }
    }
}
