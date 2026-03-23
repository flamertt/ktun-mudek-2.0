using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Helpers
{
    public interface IJwtService
    {
        string GenerateToken(AppUserDto user);
    }
}
