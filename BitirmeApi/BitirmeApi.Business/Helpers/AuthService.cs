using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Helpers
{
    public class AuthService : IAuthService
    {
        private readonly IAppUserService _userService;
        private readonly IJwtService _jwtService;

        public AuthService(IAppUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        public async Task<AuthResult> AdminLoginAsync(LoginDto loginDto)
        {
            var user = await _userService.ValidateUserAsync(loginDto.Email, loginDto.Password);

            if (user == null)
                return AuthResult.Unauthorized("Email veya şifre hatalı");

            if (user.Role != "Admin")
                return AuthResult.Forbidden("Bu endpoint sadece Admin kullanıcılar içindir");

            if (!user.IsActive)
                return AuthResult.Unauthorized("Hesabınız aktif değil");

            var token = _jwtService.GenerateToken(user);
            await _userService.UpdateLastLoginAsync(user.Id);

            return AuthResult.Ok(new AuthResponseDto
            {
                Token = token,
                User = new AuthUserResponseDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role
                },
                Message = "Admin girişi başarılı"
            });
        }

        public async Task<AuthResult> TeacherLoginAsync(LoginDto loginDto)
        {
            var user = await _userService.ValidateUserAsync(loginDto.Email, loginDto.Password);

            if (user == null)
                return AuthResult.Unauthorized("Email veya şifre hatalı");

            if (user.Role != "Teacher")
                return AuthResult.Forbidden("Bu endpoint sadece Öğretmen kullanıcılar içindir");

            if (!user.IsActive)
                return AuthResult.Unauthorized("Hesabınız aktif değil");

            var token = _jwtService.GenerateToken(user);
            await _userService.UpdateLastLoginAsync(user.Id);

            return AuthResult.Ok(new AuthResponseDto
            {
                Token = token,
                User = new AuthUserResponseDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role,
                    Title = user.Title,
                    ProgramEntityId = user.ProgramEntityId,
                    ProgramName = user.ProgramName
                },
                Message = "Öğretmen girişi başarılı"
            });
        }

        public async Task<AuthResult> StudentLoginAsync(LoginDto loginDto)
        {
            var user = await _userService.ValidateUserAsync(loginDto.Email, loginDto.Password);

            if (user == null)
                return AuthResult.Unauthorized("Email veya şifre hatalı");

            if (user.Role != "Student")
                return AuthResult.Forbidden("Bu endpoint sadece Öğrenci kullanıcılar içindir");

            if (!user.IsActive)
                return AuthResult.Unauthorized("Hesabınız aktif değil");

            var token = _jwtService.GenerateToken(user);
            await _userService.UpdateLastLoginAsync(user.Id);

            return AuthResult.Ok(new AuthResponseDto
            {
                Token = token,
                User = new AuthUserResponseDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role,
                    StudentNumber = user.StudentNumber,
                    ProgramEntityId = user.ProgramEntityId,
                    ProgramName = user.ProgramName
                },
                Message = "Öğrenci girişi başarılı"
            });
        }
    }
}
