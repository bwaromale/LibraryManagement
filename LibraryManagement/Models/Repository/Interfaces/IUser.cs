using LibraryManagement.Models.DTO;

namespace LibraryManagement.Models.Repository.Interfaces
{
    public interface IUser: IRepository<User>
    {
        Task<LoginResponse> Login(LoginRequest loginRequest);
    }
}
