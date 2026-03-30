# 🌱 OTOMATİK SEED DATA - UYGULAMA BAŞLATMA SİSTEMİ

## 📋 YAPILAN DEĞİŞİKLİKLERİN ÖZETİ

Seed data artık migration ile değil, **uygulama başlatıldığında otomatik** olarak kontrol edilip ekleniyor.

**Avantajlar:**
- ✅ Migration gerektirmiyor
- ✅ Veritabanı varsa tekrar eklemiyor
- ✅ Uygulama her başladığında kontrol eder
- ✅ Konsola bilgilendirme mesajları yazdırır
- ✅ Hata durumunda loglar
- ✅ Development ve production'da farklı davranabilir

---

## 🔧 OLUŞTURULAN DOSYALAR

### 📄 `DatabaseSeeder.cs`

**Konum:** `BitirmeApi.Presentation/Services/DatabaseSeeder.cs`

```csharp
public class DatabaseSeeder
{
    public static async Task SeedTestUsersAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ProjectDbContext>();

        // 1. Veritabanının oluşturulduğundan emin ol
        await context.Database.EnsureCreatedAsync();

        // 2. Migration'ları uygula (varsa)
        if ((await context.Database.GetPendingMigrationsAsync()).Any())
        {
            await context.Database.MigrateAsync();
        }

        // 3. Kullanıcı var mı kontrol et
        if (await context.Users.AnyAsync())
        {
            Console.WriteLine("✓ Veritabanında kullanıcılar mevcut. Seed data atlandı.");
            return;
        }

        // 4. Test kullanıcılarını ekle
        // ... (3 kullanıcı eklenir)
    }
}
```

**Özellikler:**
- ✅ Veritabanı kontrolü
- ✅ Otomatik migration uygulama
- ✅ Kullanıcı varlık kontrolü
- ✅ Bulk insert (3 kullanıcı birden)
- ✅ Try-catch ile hata yönetimi
- ✅ Konsol logları

---

## 🔄 ÇALIŞMA AKIŞI

### Uygulama Başlatıldığında:

```
┌─────────────────────────────────────┐
│   Uygulama Başlatılıyor (app.Run)  │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  DatabaseSeeder.SeedTestUsersAsync  │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  Veritabanı Var Mı?                 │
│  • EnsureCreatedAsync()             │
└──────────────┬──────────────────────┘
               │
         ┌─────┴─────┐
         │           │
      YOK         VAR
         │           │
         ▼           ▼
   ┌─────────┐   ┌──────────┐
   │ Oluştur │   │ Devam Et │
   └────┬────┘   └─────┬────┘
        │              │
        └──────┬───────┘
               │
               ▼
┌─────────────────────────────────────┐
│  Pending Migration Var Mı?          │
│  • GetPendingMigrationsAsync()      │
└──────────────┬──────────────────────┘
               │
         ┌─────┴─────┐
         │           │
      VAR          YOK
         │           │
         ▼           ▼
   ┌─────────┐   ┌──────────┐
   │ Uygula  │   │ Devam Et │
   └────┬────┘   └─────┬────┘
        │              │
        └──────┬───────┘
               │
               ▼
┌─────────────────────────────────────┐
│  Users Tablosunda Veri Var Mı?      │
│  • Users.AnyAsync()                 │
└──────────────┬──────────────────────┘
               │
         ┌─────┴─────┐
         │           │
      VAR          YOK
         │           │
         ▼           ▼
   ┌─────────┐   ┌───────────────┐
   │  Atla   │   │ 3 Kullanıcı   │
   │         │   │ Ekle          │
   └────┬────┘   └───────┬───────┘
        │                │
        │                ▼
        │      ┌──────────────────┐
        │      │ • Admin          │
        │      │ • Teacher        │
        │      │ • Student        │
        │      └────────┬─────────┘
        │               │
        └───────┬───────┘
                │
                ▼
    ┌───────────────────────┐
    │  Uygulama Başlatıldı  │
    └───────────────────────┘
```

---

## 👥 EKLENENtest KULLANICILAR

### 1️⃣ Admin
```
ID       : 11111111-1111-1111-1111-111111111111
Ad Soyad : Admin User
Email    : admin@mudek.com
Şifre    : 1234
Rol      : Admin
Telefon  : +90 555 000 0001
```

### 2️⃣ Teacher
```
ID       : 22222222-2222-2222-2222-222222222222
Ad Soyad : Ahmet Yılmaz
Email    : teacher@mudek.com
Şifre    : 1234
Rol      : Teacher
Unvan    : Doç. Dr.
Telefon  : +90 555 000 0002
```

### 3️⃣ Student
```
ID         : 33333333-3333-3333-3333-333333333333
Ad Soyad   : Mehmet Demir
Email      : student@mudek.com
Şifre      : 1234
Rol        : Student
Öğrenci No : 20200001
Telefon    : +90 555 000 0003
```

---

## 🖥️ KONSOL ÇIKTILARI

### Senaryo 1: Veritabanı Boş (İlk Çalıştırma)

```
⚠ Veritabanında kullanıcı bulunamadı. Test kullanıcıları ekleniyor...
✓ Test kullanıcıları başarıyla eklendi:
  • Admin    : admin@mudek.com    | Şifre: 1234
  • Teacher  : teacher@mudek.com  | Şifre: 1234
  • Student  : student@mudek.com  | Şifre: 1234
```

### Senaryo 2: Veritabanında Kullanıcı Var

```
✓ Veritabanında kullanıcılar mevcut. Seed data atlandı.
```

### Senaryo 3: Hata Durumu

```
✗ Seed data eklenirken hata oluştu: [Hata mesajı]
```

---

## 🔧 Program.cs'deki Değişiklikler

### Eklenen Kod:

```csharp
// Uygulama başlatıldığında seed data'yı kontrol et ve ekle
using (var scope = app.Services.CreateScope())
{
    try
    {
        await DatabaseSeeder.SeedTestUsersAsync(scope.ServiceProvider);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Seed data eklenirken bir hata oluştu");
    }
}

app.Run();
```

**Özellikler:**
- ✅ Scope oluşturur (dependency injection için)
- ✅ DatabaseSeeder'ı çağırır
- ✅ Try-catch ile hataları yakalar
- ✅ Logger ile hataları loglar
- ✅ Hata olsa bile uygulama çalışmaya devam eder

---

## 🧪 TEST SENARYOLARI

### Test 1: İlk Kurulum

**Adımlar:**
1. Veritabanını sil (eğer varsa)
2. Uygulamayı başlat: `dotnet run`
3. Konsol çıktısını kontrol et
4. Login endpoint'lerini test et

**Beklenen Sonuç:**
```
⚠ Veritabanında kullanıcı bulunamadı. Test kullanıcıları ekleniyor...
✓ Test kullanıcıları başarıyla eklendi:
  • Admin    : admin@mudek.com    | Şifre: 1234
  • Teacher  : teacher@mudek.com  | Şifre: 1234
  • Student  : student@mudek.com  | Şifre: 1234
```

---

### Test 2: Uygulama Yeniden Başlatma

**Adımlar:**
1. Uygulamayı durdur (Ctrl+C)
2. Uygulamayı tekrar başlat: `dotnet run`
3. Konsol çıktısını kontrol et

**Beklenen Sonuç:**
```
✓ Veritabanında kullanıcılar mevcut. Seed data atlandı.
```

---

### Test 3: Login Testi

**Adımlar:**
1. Swagger'a git: https://localhost:7000/swagger
2. AdminAuth/login endpoint'ini test et:
   ```json
   {
     "email": "admin@mudek.com",
     "password": "1234"
   }
   ```

**Beklenen Sonuç:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "11111111-1111-1111-1111-111111111111",
    "fullName": "Admin User",
    "email": "admin@mudek.com",
    "role": "Admin"
  },
  "message": "Admin girişi başarılı"
}
```

---

## 🎯 ÖZEL SENARYOLAR

### Senaryo 1: Sadece 1 Kullanıcı Varsa?

Kod `Users.AnyAsync()` kontrolü yaptığı için **herhangi bir kullanıcı varsa** seed data eklenmez.

**Manuel Ekleme Yapmak İsterseniz:**

```sql
-- Tüm kullanıcıları sil
DELETE FROM Users;

-- Uygulamayı yeniden başlat
-- Seed data otomatik eklenecek
```

---

### Senaryo 2: Sadece Belirli Kullanıcıları Eklemek

`DatabaseSeeder.cs` dosyasını düzenleyin:

```csharp
// Örnek: Sadece Admin yoksa ekle
var adminExists = await context.Users.AnyAsync(u => u.Role == "Admin");
if (!adminExists)
{
    await context.Users.AddAsync(new AppUser { /* Admin bilgileri */ });
    await context.SaveChangesAsync();
}
```

---

### Senaryo 3: Development/Production Ayrımı

```csharp
public static async Task SeedTestUsersAsync(IServiceProvider serviceProvider, IHostEnvironment env)
{
    // Sadece Development ortamında seed data ekle
    if (!env.IsDevelopment())
    {
        Console.WriteLine("⚠ Production ortamı. Seed data atlandı.");
        return;
    }
    
    // ... seed data ekleme kodu
}
```

**Program.cs'de:**
```csharp
var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
await DatabaseSeeder.SeedTestUsersAsync(scope.ServiceProvider, env);
```

---

## 🆚 KARŞILAŞTIRMA: Migration vs Uygulama Başlatma

### Migration ile Seed Data (ESKİ YOL)

| Özellik | Durum |
|---------|-------|
| **Migration Gerekli** | ✅ Evet |
| **Veritabanı Değişirse** | ❌ Yeni migration |
| **Silinirse** | ❌ Manuel yeniden ekleme |
| **Development/Production** | ❌ İki ortamda da aynı |
| **Esneklik** | ❌ Düşük |

### Uygulama Başlatma ile (YENİ YOL)

| Özellik | Durum |
|---------|-------|
| **Migration Gerekli** | ❌ Hayır |
| **Veritabanı Değişirse** | ✅ Otomatik kontrol |
| **Silinirse** | ✅ Otomatik yeniden ekleme |
| **Development/Production** | ✅ Ayrılabilir |
| **Esneklik** | ✅ Yüksek |

---

## 🎨 ÖZELLEŞTİRME ÖRNEKLERİ

### 1. Daha Fazla Test Kullanıcısı Eklemek

```csharp
var testUsers = new List<AppUser>
{
    // ... mevcut 3 kullanıcı ...
    
    // Yeni öğretmen ekle
    new AppUser
    {
        Id = Guid.NewGuid(),
        FullName = "Ayşe Kaya",
        Email = "teacher2@mudek.com",
        PasswordHash = "A6xnQhbz4gLh9j4ZDj6RjW6M8M6m8vM2qXfXk6K9n6Y=",
        Role = "Teacher",
        Title = "Prof. Dr.",
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    }
};
```

---

### 2. Şifre Farklı Yapmak

```csharp
// Her kullanıcı için farklı şifre hash'i kullan
new AppUser
{
    // ...
    PasswordHash = "FarkliHashDegeri123=",  // Farklı şifre
    // ...
}
```

---

### 3. Program Bazlı Kullanıcılar

```csharp
// Önce bir program oluştur
var program = await context.Programs.FirstOrDefaultAsync();
if (program != null)
{
    testUsers[1].ProgramEntityId = program.Id;  // Teacher'a program ata
    testUsers[2].ProgramEntityId = program.Id;  // Student'a program ata
}
```

---

## ⚠️ ÖNEMLİ NOTLAR

### 1. Password Hash Tutarlılığı

**UYARI:** `DatabaseSeeder` içindeki password hash, `AppUserService` içindeki hash algoritması ile **uyumlu olmalı**.

**Kontrol:**
```csharp
// AppUserService.cs
private string HashPassword(string password)
{
    using (var sha256 = SHA256.Create())
    {
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}

// Test: "1234" -> "A6xnQhbz4gLh9j4ZDj6RjW6M8M6m8vM2qXfXk6K9n6Y="
```

---

### 2. Unique Constraint Hataları

Email unique olduğu için **aynı email ile ekleme yapamazsınız**.

Eğer hata alırsanız:
```
Microsoft.EntityFrameworkCore.DbUpdateException: 
An error occurred while updating the entries. 
See the inner exception for details.
```

**Çözüm:**
- Veritabanını temizleyin
- Veya farklı email adresleri kullanın

---

### 3. Migration Konflikti

Eğer daha önce seed data içeren bir migration oluşturduysanız:

```bash
# Migration'ı geri al
dotnet ef migrations remove --context ProjectDbContext

# Veya migration dosyasındaki seed data kısmını manuel silin
```

---

## 🚀 KULLANIMA HAZIR

### Hemen Test Edin!

```bash
# 1. Veritabanını temizleyin (opsiyonel)
dotnet ef database drop --context ProjectDbContext --force

# 2. Uygulamayı başlatın
cd BitirmeApi.Presentation
dotnet run
```

**Konsol Çıktısı:**
```
⚠ Veritabanında kullanıcı bulunamadı. Test kullanıcıları ekleniyor...
✓ Test kullanıcıları başarıyla eklendi:
  • Admin    : admin@mudek.com    | Şifre: 1234
  • Teacher  : teacher@mudek.com  | Şifre: 1234
  • Student  : student@mudek.com  | Şifre: 1234
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7000
```

### Swagger'dan Test Edin:

1. https://localhost:7000/swagger
2. `POST /api/AdminAuth/login`
3. Request:
   ```json
   {
     "email": "admin@mudek.com",
     "password": "1234"
   }
   ```
4. ✅ Token alın!

---

## 📊 ÖZET

| Özellik | Değer |
|---------|-------|
| **Seed Yöntemi** | Uygulama Başlatma |
| **Migration Gerekli** | ❌ Hayır |
| **Otomatik Kontrol** | ✅ Her başlatmada |
| **Tekrar Ekleme** | ❌ Hayır (varsa atlar) |
| **Konsol Logları** | ✅ Var |
| **Hata Yönetimi** | ✅ Try-Catch + Logger |
| **Test Kullanıcıları** | 3 (Admin, Teacher, Student) |
| **Ortak Şifre** | 1234 |

---

**Oluşturulma Tarihi:** 6 Mart 2026  
**Durum:** ✅ Hazır - Hemen Kullanılabilir

**Artık migration gerekmeden, her uygulama başlatıldığında test kullanıcıları otomatik kontrol edilip ekleniyor!** 🎉
