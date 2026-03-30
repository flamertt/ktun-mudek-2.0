# 🔐 IDENTITY'DEN JWT'YE GEÇİŞ - DETAYLI RAPOR

## 📋 YAPILAN DEĞİŞİKLİKLERİN ÖZETİ

Bu migration ile sistemden ASP.NET Identity tamamen kaldırıldı ve JWT tabanlı basit kullanıcı yönetimine geçildi.

**Ana Değişiklikler:**
- ✅ ASP.NET Identity kaldırıldı
- ✅ Tek kullanıcı tablosu (`Users`) oluşturuldu
- ✅ StudentProfile ve TeacherProfile tabloları kaldırıldı
- ✅ Tüm foreign key'ler `string` → `Guid` yapıldı
- ✅ Rol yönetimi tek kolon ile yapılıyor

---

## 1. YENİ `AppUser` ENTITY'Sİ

```csharp
public class AppUser : IEntity
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; } // UNIQUE
    public string PasswordHash { get; set; }
    public string Role { get; set; } // Admin, Teacher, Student
    public string? StudentNumber { get; set; } // Sadece öğrenciler için
    public string? Title { get; set; } // Sadece öğretmenler için
    public string? PhoneNumber { get; set; }
    public Guid? ProgramEntityId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    
    public ProgramEntity? Program { get; set; }
}
```

### Özellikler:
- **Email unique olarak indexlendi**
- **Role alanı ile öğrenci/öğretmen/admin ayrımı**
- **StudentNumber sadece öğrenciler için dolu**
- **Title sadece öğretmenler için dolu**
- **Tüm kullanıcı tipleri tek tabloda**

---

## 2. GÜNCELLENMIŞ `ProjectDbContext`

### Öncesi:
```csharp
public class ProjectDbContext : IdentityDbContext<ApplicationUser>
{
    // OnConfiguring içinde connection string tanımı
}
```

### Sonrası:
```csharp
public class ProjectDbContext : DbContext
{
    public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options) { }
    
    public DbSet<AppUser> Users => Set<AppUser>();
    // Diğer DbSet'ler...
}
```

### Değişiklikler:
- ❌ `IdentityDbContext<ApplicationUser>` kaldırıldı
- ✅ `DbContext` kullanılıyor
- ✅ Constructor ile options injection yapılıyor
- ✅ `OnConfiguring` temizlendi (connection string Program.cs'de)
- ❌ `DbSet<StudentProfile>` kaldırıldı
- ❌ `DbSet<TeacherProfile>` kaldırıldı
- ✅ `DbSet<AppUser> Users` eklendi

---

## 3. GÜNCELLENMESİ GEREKEN ENTITY ALANLARI

### 3.1. **Course Entity**

#### Değişiklik:
```csharp
// ❌ ÖNCESI
[MaxLength(450)]
public string? TeacherId { get; set; }

// ✅ SONRASI
public Guid? TeacherId { get; set; }
[ForeignKey("TeacherId")]
public AppUser? Teacher { get; set; }
```

**Tip Değişimi:** `string?` → `Guid?`  
**Navigation Property Eklendi:** `AppUser? Teacher`

---

### 3.2. **Submission Entity**

#### Değişiklik:
```csharp
// ❌ ÖNCESI
[Required, MaxLength(450)]
public string UserId { get; set; } = default!;

// ✅ SONRASI
[Required]
public Guid UserId { get; set; }
[ForeignKey("UserId")]
public AppUser User { get; set; } = default!;
```

**Tip Değişimi:** `string` → `Guid`  
**Navigation Property Eklendi:** `AppUser User`

---

### 3.3. **StudentEnrollment Entity**

#### Değişiklik:
```csharp
// ❌ ÖNCESI
[MaxLength(450)]
public string? ApplicationUserId { get; set; }
[ForeignKey("ApplicationUserId")]
public ApplicationUser? ApplicationUser { get; set; }

// ✅ SONRASI
public Guid? StudentUserId { get; set; }
[ForeignKey("StudentUserId")]
public AppUser? StudentUser { get; set; }
```

**Alan Adı Değişti:** `ApplicationUserId` → `StudentUserId`  
**Tip Değişimi:** `string?` → `Guid?`  
**Navigation Property Adı Değişti:** `ApplicationUser` → `StudentUser`

---

### 3.4. **CourseStudent Entity**

#### Değişiklik:
```csharp
// ❌ ÖNCESI
[Required, MaxLength(450)]
public string StudentId { get; set; } = default!;
public ApplicationUser Student { get; set; } = default!;

// ✅ SONRASI
[Required]
public Guid StudentId { get; set; }
[ForeignKey("StudentId")]
public AppUser Student { get; set; } = default!;
```

**Tip Değişimi:** `string` → `Guid`  
**Navigation Property Tipi Değişti:** `ApplicationUser` → `AppUser`

---

## 4. KALDIRILAN ENTITY VE DBSET'LER

### Kaldırılan Entity'ler:
- ❌ `ApplicationUser : IdentityUser`
- ❌ `StudentProfile`
- ❌ `TeacherProfile`

### Kaldırılan DbSet'ler:
```csharp
// ❌ KALDIRILAN
public DbSet<StudentProfile> StudentProfiles => Set<StudentProfile>();
public DbSet<TeacherProfile> TeacherProfiles => Set<TeacherProfile>();
```

### Kaldırılan Veritabanı Tabloları:
- ❌ `AspNetUsers`
- ❌ `AspNetRoles`
- ❌ `AspNetUserRoles`
- ❌ `AspNetUserClaims`
- ❌ `AspNetUserLogins`
- ❌ `AspNetUserTokens`
- ❌ `AspNetRoleClaims`
- ❌ `StudentProfiles`
- ❌ `TeacherProfiles`

**Toplam:** 9 tablo kaldırıldı, 1 yeni tablo eklendi (`Users`)

---

## 5. `OnModelCreating` İÇİNDE YAPILAN DEĞİŞİKLİKLER

### 5.1. **Yeni AppUser Konfigürasyonu Eklendi**

```csharp
b.Entity<AppUser>(e =>
{
    e.Property(x => x.FullName).IsRequired().HasMaxLength(256);
    e.Property(x => x.Email).IsRequired().HasMaxLength(256);
    e.Property(x => x.PasswordHash).IsRequired().HasMaxLength(512);
    e.Property(x => x.Role).IsRequired().HasMaxLength(50);
    e.Property(x => x.StudentNumber).HasMaxLength(64);
    e.Property(x => x.Title).HasMaxLength(256);
    e.Property(x => x.PhoneNumber).HasMaxLength(20);
    e.Property(x => x.IsActive).HasDefaultValue(true);

    // Email benzersiz olmalı
    e.HasIndex(x => x.Email).IsUnique();

    // Program ilişkisi
    e.HasOne(x => x.Program)
     .WithMany()
     .HasForeignKey(x => x.ProgramEntityId)
     .OnDelete(DeleteBehavior.SetNull);
});
```

---

### 5.2. **Course - Teacher İlişkisi Güncellendi**

```csharp
// ❌ ÖNCESI
e.HasOne<ApplicationUser>()
 .WithMany()
 .HasForeignKey(x => x.TeacherId)
 .OnDelete(DeleteBehavior.SetNull);

// ✅ SONRASI
e.HasOne(x => x.Teacher)
 .WithMany()
 .HasForeignKey(x => x.TeacherId)
 .OnDelete(DeleteBehavior.SetNull);
```

---

### 5.3. **Submission - User İlişkisi Güncellendi**

```csharp
// ❌ ÖNCESI
e.Property(x => x.UserId).IsRequired().HasMaxLength(450);
e.HasOne<ApplicationUser>()
 .WithMany()
 .HasForeignKey(x => x.UserId)
 .OnDelete(DeleteBehavior.Restrict);

// ✅ SONRASI
e.HasOne(x => x.User)
 .WithMany()
 .HasForeignKey(x => x.UserId)
 .OnDelete(DeleteBehavior.Restrict);
```

---

### 5.4. **StudentEnrollment - StudentUser İlişkisi Güncellendi**

```csharp
// ❌ ÖNCESI
e.HasOne(x => x.ApplicationUser)
 .WithMany()
 .HasForeignKey(x => x.ApplicationUserId)
 .OnDelete(DeleteBehavior.SetNull);

// ✅ SONRASI
e.HasOne(x => x.StudentUser)
 .WithMany()
 .HasForeignKey(x => x.StudentUserId)
 .OnDelete(DeleteBehavior.SetNull);
```

---

### 5.5. **CourseStudent - Student İlişkisi Güncellendi**

```csharp
// ❌ ÖNCESI
e.HasOne(x => x.Student)
 .WithMany()
 .HasForeignKey(x => x.StudentId)
 .OnDelete(DeleteBehavior.Cascade);
// StudentId tipi: string

// ✅ SONRASI
e.HasOne(x => x.Student)
 .WithMany()
 .HasForeignKey(x => x.StudentId)
 .OnDelete(DeleteBehavior.Cascade);
// StudentId tipi: Guid
```

---

### 5.6. **Kaldırılan Konfigürasyonlar**

```csharp
// ❌ KALDIRILAN - ApplicationUser konfigürasyonu
b.Entity<ApplicationUser>(e => { ... });

// ❌ KALDIRILAN - StudentProfile konfigürasyonu
b.Entity<StudentProfile>(e => { ... });

// ❌ KALDIRILAN - TeacherProfile konfigürasyonu
b.Entity<TeacherProfile>(e => { ... });
```

---

## 6. MİGRATION SIRASINDA OLUŞABİLECEK BREAKING CHANGE'LER

### 🔴 **CRITICAL BREAKING CHANGES**

#### 6.1. **Tüm Identity Tabloları Kaldırılacak**

**Etkilenen Tablolar:**
- `AspNetUsers`
- `AspNetRoles`
- `AspNetUserRoles`
- `AspNetUserClaims`
- `AspNetUserLogins`
- `AspNetUserTokens`
- `AspNetRoleClaims`

**⚠️ ETKİ:** Mevcut tüm kullanıcı verisi kaybolacak!

**✅ ÇÖZÜM:** Migration'dan önce mevcut kullanıcı verilerini yeni `Users` tablosuna taşıyın:

```sql
-- Örnek veri taşıma scripti
INSERT INTO Users (Id, FullName, Email, PasswordHash, Role, StudentNumber, ProgramEntityId, IsActive, CreatedAt)
SELECT 
    NEWID(),
    ISNULL(FullName, Email),
    Email,
    PasswordHash,
    CASE UserType
        WHEN 0 THEN 'Student'
        WHEN 1 THEN 'Teacher'
        WHEN 2 THEN 'Admin'
    END,
    StudentNumber,
    ProgramEntityId,
    1,
    GETDATE()
FROM AspNetUsers;
```

---

#### 6.2. **StudentProfile ve TeacherProfile Tabloları Kaldırılacak**

**⚠️ ETKİ:** Profil tabloların verisi kaybolacak!

**✅ ÇÖZÜM:** Gerekli bilgiler zaten `ApplicationUser` içinde varsa sorun yok. Yoksa önce migration'dan önce veri birleştirme yapın.

---

#### 6.3. **Foreign Key Tip Değişimi (string → Guid)**

**Etkilenen Alanlar:**
- `Courses.TeacherId` → string → Guid
- `Submissions.UserId` → string → Guid
- `StudentEnrollments.ApplicationUserId` → string → Guid → StudentUserId
- `CourseStudents.StudentId` → string → Guid

**⚠️ ETKİ:** 
- Mevcut string ID'ler Guid'e otomatik çevrilemez
- İlişkili veriler kaybolabilir

**✅ ÇÖZÜM:** 
1. Migration öncesi veri mapping tablosu oluşturun
2. Eski string ID → Yeni Guid ID eşleştirmesi yapın
3. İlişkili verileri yeni ID'lerle güncelleyin

**Örnek Veri Taşıma:**
```sql
-- 1. Mapping tablosu oluştur
CREATE TABLE #UserIdMapping (
    OldStringId NVARCHAR(450),
    NewGuidId UNIQUEIDENTIFIER
);

-- 2. Her eski kullanıcı için yeni Guid oluştur
INSERT INTO #UserIdMapping (OldStringId, NewGuidId)
SELECT Id, NEWID() FROM AspNetUsers;

-- 3. Courses.TeacherId güncelle
UPDATE Courses
SET TeacherId = (SELECT NewGuidId FROM #UserIdMapping WHERE OldStringId = Courses.TeacherId);
```

---

#### 6.4. **Kod Seviyesi Breaking Changes**

**1. Using Direktifleri:**
```csharp
// ❌ KALDIRIN
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

// ✅ ARTIK GEREK YOK
```

**2. Repository/Service'lerde:**
```csharp
// ❌ ESKİ
UserManager<ApplicationUser> _userManager;
SignInManager<ApplicationUser> _signInManager;

// ✅ YENİ - Kendi auth servisinizi yazın
IAuthService _authService; // JWT üreten servis
```

**3. Controller'larda:**
```csharp
// ❌ ESKİ
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase

// ✅ YENİ - JWT claim'lerini kontrol edin
[Authorize]
public class AdminController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (role != "Admin") return Forbid();
        // ...
    }
}
```

**4. LINQ Sorguları:**
```csharp
// ❌ ESKİ
var teacher = context.Courses
    .Include(c => c.Teacher) // ApplicationUser tipinde
    .FirstOrDefault();

// ✅ YENİ
var teacher = context.Courses
    .Include(c => c.Teacher) // AppUser tipinde
    .FirstOrDefault();
```

---

### 🟡 **UYARI: Dikkat Edilmesi Gerekenler**

#### 1. **Connection String Değişikliği**

**Öncesi (`ProjectDbContext`):**
```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseSqlServer((@"Server=(localdb)\mssqllocaldb;database=MudekDb;Trusted_Connection=True;"));
}
```

**Sonrası (`Program.cs` veya `appsettings.json`):**
```csharp
// Program.cs
builder.Services.AddDbContext<ProjectDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MudekDb;Trusted_Connection=True;"
  }
}
```

---

#### 2. **JWT Authentication Yapılandırması**

Migration'dan sonra `Program.cs`'e eklenmelidir:

```csharp
// JWT ayarları
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
    options.AddPolicy("TeacherOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Teacher"));
    options.AddPolicy("StudentOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Student"));
});
```

---

#### 3. **Password Hashing**

Identity'nin built-in password hasher'ı olmayacağı için:

```csharp
// Basit password hashing servisi
public class PasswordHasher
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}

// Veya
using System.Security.Cryptography;
```

---

## 7. NİHAİ MİMARİ ÖZETİ

### 🎯 **Hedeflenen Mimari**

```
┌──────────────────────────────────────────────┐
│            ASP.NET Core API                  │
│                                              │
│  ┌────────────────────────────────────────┐ │
│  │       JWT Authentication              │ │
│  │   (No ASP.NET Identity)               │ │
│  └────────────────────────────────────────┘ │
│                                              │
│  ┌────────────────────────────────────────┐ │
│  │          Controllers                   │ │
│  │  [Authorize] attribute ile korumalı   │ │
│  └────────────────────────────────────────┘ │
│                                              │
│  ┌────────────────────────────────────────┐ │
│  │          Business Layer                │ │
│  │  - AuthService (JWT üretir)           │ │
│  │  - UserService (CRUD)                  │ │
│  └────────────────────────────────────────┘ │
│                                              │
│  ┌────────────────────────────────────────┐ │
│  │          Data Access Layer             │ │
│  │  - ProjectDbContext (DbContext)        │ │
│  │  - Repositories                        │ │
│  └────────────────────────────────────────┘ │
│                                              │
│  ┌────────────────────────────────────────┐ │
│  │           Database                     │ │
│  │                                        │ │
│  │  ✅ Users (AppUser)                    │ │
│  │  ✅ Courses                            │ │
│  │  ✅ StudentEnrollments                 │ │
│  │  ✅ Submissions                        │ │
│  │  ✅ CourseStudents                     │ │
│  │  ✅ ...diğer tablolar                  │ │
│  │                                        │ │
│  │  ❌ AspNetUsers (REMOVED)              │ │
│  │  ❌ AspNetRoles (REMOVED)              │ │
│  │  ❌ StudentProfiles (REMOVED)          │ │
│  │  ❌ TeacherProfiles (REMOVED)          │ │
│  └────────────────────────────────────────┘ │
└──────────────────────────────────────────────┘
```

---

### ✅ **Kazanılanlar**

1. **Basitlik**
   - Tek kullanıcı tablosu
   - Karmaşık Identity yapısı yok
   - Kolay yönetim

2. **Performans**
   - 9 tablo yerine 1 tablo
   - Daha az JOIN
   - Daha hızlı sorgular

3. **Esneklik**
   - JWT ile stateless authentication
   - Özelleştirilebilir rol yönetimi
   - Mikroservis mimarisine uygun

4. **Bakım Kolaylığı**
   - Identity bağımlılığı yok
   - Daha az kod
   - Daha kolay debug

---

### 🔧 **Gereken İlaveler**

Migration'dan sonra eklenmelidir:

1. ✅ **AuthService** - JWT token üretimi
2. ✅ **PasswordHasher** - Şifre hashing
3. ✅ **UserService** - CRUD işlemleri
4. ✅ **JWT middleware** yapılandırması
5. ✅ **Role-based authorization** policy'leri

---

### 📊 **Tablo Karşılaştırması**

| Önceki Yapı (Identity) | Yeni Yapı (JWT) |
|------------------------|-----------------|
| AspNetUsers | ✅ Users |
| AspNetRoles | ❌ (Role kolonu) |
| AspNetUserRoles | ❌ (Role kolonu) |
| AspNetUserClaims | ❌ |
| AspNetUserLogins | ❌ |
| AspNetUserTokens | ❌ |
| AspNetRoleClaims | ❌ |
| StudentProfiles | ❌ (Users içinde) |
| TeacherProfiles | ❌ (Users içinde) |
| **TOPLAM: 9 tablo** | **TOPLAM: 1 tablo** |

---

### 🎯 **Sonuç**

Bu migration ile sistem:
- ✅ ASP.NET Identity'den tamamen arındırıldı
- ✅ Tek kullanıcı tablosu (`Users`) kullanıyor
- ✅ JWT tabanlı authentication'a hazır
- ✅ Daha basit ve performanslı
- ✅ Mikroservis mimarisine uygun

**⚠️ UYARI:** Bu migration **geri döndürülemez** bir değişiklik. Mutlaka:
1. Veritabanı yedeği alın
2. Mevcut kullanıcı verilerini migration öncesi taşıyın
3. Test ortamında deneyin

---

**Migration Dosyası:** `20260306220000_RemoveIdentityAndMigrateToAppUser.cs`  
**Oluşturulma Tarihi:** 6 Mart 2026  
**Durum:** ✅ Hazır (Veri taşıma işlemi manuel yapılmalı)
