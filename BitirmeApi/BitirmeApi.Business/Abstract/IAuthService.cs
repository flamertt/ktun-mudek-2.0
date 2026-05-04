using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface IAuthService
    {
        /// <summary>Admin: local DB şifre kontrolü</summary>
        Task<AuthResult> AdminLoginAsync(LoginDto loginDto);

        /// <summary>Öğretmen: üniversite API ile doğrulama</summary>
        Task<AuthResult> TeacherLoginAsync(LoginDto loginDto);

        /// <summary>Öğrenci: üniversite API ile doğrulama</summary>
        Task<AuthResult> StudentLoginAsync(LoginDto loginDto);

        /// <summary>Herhangi bir role: üniversite API ile doğrulama, rol önceden bilinmiyorsa</summary>
        Task<AuthResult> UniversityLoginAsync(LoginDto loginDto);
    }
}
