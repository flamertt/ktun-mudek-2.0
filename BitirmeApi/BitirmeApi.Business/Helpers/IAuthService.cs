using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Helpers
{
    public interface IAuthService
    {
        Task<AuthResult> AdminLoginAsync(LoginDto loginDto);
        Task<AuthResult> TeacherLoginAsync(LoginDto loginDto);
        Task<AuthResult> StudentLoginAsync(LoginDto loginDto);
    }
}
