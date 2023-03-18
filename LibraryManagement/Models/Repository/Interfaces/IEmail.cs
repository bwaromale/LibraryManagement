using LibraryManagement.Models.DTO;

namespace LibraryManagement.Models.Repository.Interfaces
{
    public interface IEmail
    {
        void SendEmail(EmailDto emailRequest);
    }
}
