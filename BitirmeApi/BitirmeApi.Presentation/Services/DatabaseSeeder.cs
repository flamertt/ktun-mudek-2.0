using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.Presentation.Services
{
    /// <summary>
    /// Uygulama başlatıldığında veritabanını kontrol eder ve seed data ekler
    /// </summary>
    public class DatabaseSeeder
    {
        /// <summary>
        /// Test kullanıcılarını veritabanına ekler (yoksa)
        /// Şifre: 1234 (Hash: A6xnQhbz4gLh9j4ZDj6RjW6M8M6m8vM2qXfXk6K9n6Y=)
        /// </summary>
        public static async Task SeedTestUsersAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ProjectDbContext>();

            try
            {
                // Veritabanının oluşturulduğundan emin ol
                await context.Database.EnsureCreatedAsync();

                // Migration'ları uygula (varsa)
                if ((await context.Database.GetPendingMigrationsAsync()).Any())
                {
                    await context.Database.MigrateAsync();
                }

                // Kullanıcı var mı kontrol et
                if (await context.Users.AnyAsync())
                {
                    Console.WriteLine("✓ Veritabanında kullanıcılar mevcut. Seed data atlandı.");
                    return;
                }

                Console.WriteLine("⚠ Veritabanında kullanıcı bulunamadı. Test kullanıcıları ekleniyor...");

                // Sabit ID'ler (test için kolaylık)
                var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
                var teacherId = Guid.Parse("22222222-2222-2222-2222-222222222222");
                var studentId = Guid.Parse("33333333-3333-3333-3333-333333333333");

                var testUsers = new List<AppUser>
                {
                    // Admin kullanıcı
                    new AppUser
                    {
                        Id = adminId,
                        FullName = "Admin User",
                        Email = "admin@mudek.com",
                        PasswordHash = "A6xnQhbz4gLh9j4ZDj6RjW6M8M6m8vM2qXfXk6K9n6Y=",
                        Role = "Admin",
                        PhoneNumber = "+90 555 000 0001",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    // Teacher kullanıcı
                    new AppUser
                    {
                        Id = teacherId,
                        FullName = "Ahmet Yılmaz",
                        Email = "teacher@mudek.com",
                        PasswordHash = "A6xnQhbz4gLh9j4ZDj6RjW6M8M6m8vM2qXfXk6K9n6Y=",
                        Role = "Teacher",
                        Title = "Doç. Dr.",
                        PhoneNumber = "+90 555 000 0002",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    // Student kullanıcı
                    new AppUser
                    {
                        Id = studentId,
                        FullName = "Mehmet Demir",
                        Email = "student@mudek.com",
                        PasswordHash = "A6xnQhbz4gLh9j4ZDj6RjW6M8M6m8vM2qXfXk6K9n6Y=",
                        Role = "Student",
                        StudentNumber = "20200001",
                        PhoneNumber = "+90 555 000 0003",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await context.Users.AddRangeAsync(testUsers);
                await context.SaveChangesAsync();

                Console.WriteLine("✓ Test kullanıcıları başarıyla eklendi:");
                Console.WriteLine("  • Admin    : admin@mudek.com    | Şifre: 1234");
                Console.WriteLine("  • Teacher  : teacher@mudek.com  | Şifre: 1234");
                Console.WriteLine("  • Student  : student@mudek.com  | Şifre: 1234");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Seed data eklenirken hata oluştu: {ex.Message}");
                throw;
            }
        }
    }
}
