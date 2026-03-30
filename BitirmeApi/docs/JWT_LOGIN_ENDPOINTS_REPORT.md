# 🔐 JWT LOGIN ENDPOINTS - UYGULAMA RAPORU

## 📋 YAPILAN DEĞİŞİKLİKLERİN ÖZETİ

Presentation katmanında 3 farklı rol için ayrı login endpoint'leri oluşturuldu ve JWT authentication yapılandırıldı.

**Ana Özellikler:**
- ✅ 3 ayrı login endpoint (Admin, Teacher, Student)
- ✅ JWT token üretimi
- ✅ Rol bazlı authentication
- ✅ Swagger JWT desteği
- ✅ CORS yapılandırması
- ❌ Register endpoint'i YOK (sadece admin kullanıcı ekleyebilir)
- ❌ Password change endpoint'i YOK

---

## 📄 DEĞİŞTİRİLEN DOSYALAR

### 1. `appsettings.json`

**Eklenen Yapılandırmalar:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MudekDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyForJWT_MinimumLength32Characters!@#$%",
    "Issuer": "BitirmeApi",
    "Audience": "BitirmeApiUsers",
    "ExpiryInHours": 24
  }
}
```

**Özellikler:**
- ✅ Connection String yapılandırması
- ✅ JWT Key (minimum 32 karakter)
- ✅ Token süresi: 24 saat
- ⚠️ **ÖNEMLİ:** Production'da JWT Key'i değiştirin ve Environment Variables'da saklayın!

---

### 2. `Program.cs`

**Eklenen Yapılandırmalar:**

#### 🔹 DbContext
```csharp
builder.Services.AddDbContext<ProjectDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

#### 🔹 JWT Authentication
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});
```

#### 🔹 Authorization Policies
```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("TeacherOnly", policy => policy.RequireRole("Teacher"));
    options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
});
```

#### 🔹 CORS
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
```

#### 🔹 Swagger JWT Desteği
```csharp
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    // ... security requirement
});
```

#### 🔹 Middleware Sırası
```csharp
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();  // ← JWT Authentication
app.UseAuthorization();   // ← Authorization
app.MapControllers();
```

---

### 3. `BitirmeApi.Presentation.csproj`

**Eklenen NuGet Paketleri:**

```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.2" />
```

---

## 🎯 OLUŞTURULAN LOGIN CONTROLLER'LARI

### 1️⃣ AdminAuthController

**Endpoint:** `POST /api/AdminAuth/login`

**Request Body:**
```json
{
  "email": "admin@example.com",
  "password": "Admin123!"
}
```

**Success Response (200):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "fullName": "Admin User",
    "email": "admin@example.com",
    "role": "Admin"
  },
  "message": "Admin girişi başarılı"
}
```

**Error Responses:**

| Status | Message | Açıklama |
|--------|---------|----------|
| 400 | ModelState errors | Geçersiz request |
| 401 | "Email veya şifre hatalı" | Kimlik doğrulama başarısız |
| 401 | "Hesabınız aktif değil" | Kullanıcı pasif |
| 403 | "Bu endpoint sadece Admin kullanıcılar içindir" | Rol uyumsuz |

**JWT Claims:**
- `sub`: User ID
- `email`: Email
- `name`: Full Name
- `role`: "Admin"
- `jti`: Unique token ID

---

### 2️⃣ TeacherAuthController

**Endpoint:** `POST /api/TeacherAuth/login`

**Request Body:**
```json
{
  "email": "teacher@example.com",
  "password": "Teacher123!"
}
```

**Success Response (200):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "fullName": "Ahmet Yılmaz",
    "email": "teacher@example.com",
    "role": "Teacher",
    "title": "Doç. Dr.",
    "programId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "programName": "Bilgisayar Mühendisliği"
  },
  "message": "Öğretmen girişi başarılı"
}
```

**Error Responses:**

| Status | Message | Açıklama |
|--------|---------|----------|
| 400 | ModelState errors | Geçersiz request |
| 401 | "Email veya şifre hatalı" | Kimlik doğrulama başarısız |
| 401 | "Hesabınız aktif değil" | Kullanıcı pasif |
| 403 | "Bu endpoint sadece Öğretmen kullanıcılar içindir" | Rol uyumsuz |

**JWT Claims:**
- `sub`: User ID
- `email`: Email
- `name`: Full Name
- `role`: "Teacher"
- `Title`: Öğretmen unvanı
- `ProgramId`: Bağlı olduğu program ID
- `jti`: Unique token ID

---

### 3️⃣ StudentAuthController

**Endpoint:** `POST /api/StudentAuth/login`

**Request Body:**
```json
{
  "email": "student@example.com",
  "password": "Student123!"
}
```

**Success Response (200):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "fullName": "Mehmet Demir",
    "email": "student@example.com",
    "role": "Student",
    "studentNumber": "20200001",
    "programId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "programName": "Bilgisayar Mühendisliği"
  },
  "message": "Öğrenci girişi başarılı"
}
```

**Error Responses:**

| Status | Message | Açıklama |
|--------|---------|----------|
| 400 | ModelState errors | Geçersiz request |
| 401 | "Email veya şifre hatalı" | Kimlik doğrulama başarısız |
| 401 | "Hesabınız aktif değil" | Kullanıcı pasif |
| 403 | "Bu endpoint sadece Öğrenci kullanıcılar içindir" | Rol uyumsuz |

**JWT Claims:**
- `sub`: User ID
- `email`: Email
- `name`: Full Name
- `role`: "Student"
- `StudentNumber`: Öğrenci numarası
- `ProgramId`: Bağlı olduğu program ID
- `jti`: Unique token ID

---

## 🔐 JWT TOKEN YAPISI

### Token Özellikleri

| Özellik | Değer |
|---------|-------|
| **Algorithm** | HS256 (HMAC-SHA256) |
| **Token Type** | Bearer |
| **Expiry** | 24 saat |
| **Clock Skew** | 0 (tam süre kontrolü) |

### Token Kullanımı

**1. Login İsteği:**
```bash
curl -X POST "https://localhost:7000/api/StudentAuth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "student@example.com",
    "password": "Student123!"
  }'
```

**2. Token Alımı:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**3. Korumalı Endpoint'e İstek:**
```bash
curl -X GET "https://localhost:7000/api/SomeProtectedEndpoint" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

---

## 🔒 AUTHORIZATION KULLANIMI

### Controller Seviyesinde

```csharp
[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminOnlyController : ControllerBase
{
    // Sadece Admin erişebilir
}
```

### Action Seviyesinde

```csharp
[HttpGet("admin-only")]
[Authorize(Roles = "Admin")]
public IActionResult AdminOnlyAction()
{
    return Ok("Admin erişimi başarılı");
}

[HttpGet("teacher-or-admin")]
[Authorize(Roles = "Admin,Teacher")]
public IActionResult TeacherOrAdminAction()
{
    return Ok("Teacher veya Admin erişimi başarılı");
}
```

### Policy Kullanımı

```csharp
[HttpGet("policy-based")]
[Authorize(Policy = "AdminOnly")]
public IActionResult PolicyBasedAction()
{
    return Ok("Policy bazlı erişim başarılı");
}
```

### Token'dan Bilgi Alma

```csharp
[HttpGet("my-profile")]
[Authorize]
public IActionResult GetMyProfile()
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var email = User.FindFirst(ClaimTypes.Email)?.Value;
    var role = User.FindFirst(ClaimTypes.Role)?.Value;
    var name = User.FindFirst(ClaimTypes.Name)?.Value;
    
    // Öğrenci için ekstra claim
    var studentNumber = User.FindFirst("StudentNumber")?.Value;
    
    return Ok(new { userId, email, role, name, studentNumber });
}
```

---

## 📊 SWAGGER KULLANIMI

### 1. Swagger UI'a Erişim
```
https://localhost:7000/swagger
```

### 2. JWT Token ile Test

1. İlgili login endpoint'ini çağırın (Admin/Teacher/Student)
2. Response'dan `token` değerini kopyalayın
3. Swagger sayfasında sağ üstteki **"Authorize"** butonuna tıklayın
4. Açılan popup'a token'ı yapıştırın:
   ```
   Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
   ```
   ⚠️ **NOT:** "Bearer " prefix'i manuel eklemeyin, Swagger otomatik ekler!
5. **"Authorize"** butonuna tıklayın
6. Artık korumalı endpoint'leri test edebilirsiniz

---

## 🧪 TEST ÖRNEKLERİ

### Postman ile Test

#### 1. Admin Login
```http
POST https://localhost:7000/api/AdminAuth/login
Content-Type: application/json

{
  "email": "admin@example.com",
  "password": "Admin123!"
}
```

#### 2. Teacher Login
```http
POST https://localhost:7000/api/TeacherAuth/login
Content-Type: application/json

{
  "email": "teacher@example.com",
  "password": "Teacher123!"
}
```

#### 3. Student Login
```http
POST https://localhost:7000/api/StudentAuth/login
Content-Type: application/json

{
  "email": "student@example.com",
  "password": "Student123!"
}
```

#### 4. Korumalı Endpoint
```http
GET https://localhost:7000/api/SomeProtectedEndpoint
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## ⚠️ ÖNEMLİ GÜVENLİK NOTLARI

### 1. JWT Secret Key

**❌ YAPMYIN:**
```json
{
  "Jwt": {
    "Key": "123456"  // ← Çok kısa ve basit
  }
}
```

**✅ YAPIN:**
```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyForJWT_MinimumLength32Characters!@#$%"
  }
}
```

**🔒 PRODUCTION:**
- Environment Variables kullanın
- Azure Key Vault, AWS Secrets Manager vb. kullanın
- Asla GitHub'a commit etmeyin

```bash
# appsettings.Production.json'da
{
  "Jwt": {
    "Key": "${JWT_SECRET_KEY}"  // Environment variable'dan gelecek
  }
}
```

### 2. HTTPS Kullanımı

Production'da **mutlaka HTTPS** kullanın:

```csharp
// Program.cs
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseHsts();
}
```

### 3. CORS Yapılandırması

**❌ Development için (şu anki):**
```csharp
builder.AllowAnyOrigin()  // Tüm origin'lere izin
```

**✅ Production için:**
```csharp
builder.WithOrigins("https://yourdomain.com", "https://admin.yourdomain.com")
       .AllowAnyMethod()
       .AllowAnyHeader();
```

### 4. Token Süresi

Development için 24 saat uygun, ancak production'da daha kısa tutun:

```json
{
  "Jwt": {
    "ExpiryInHours": 2  // Production için 1-2 saat
  }
}
```

Ayrıca **Refresh Token** mekanizması ekleyin.

### 5. Rate Limiting

Brute force attack'leri önlemek için rate limiting ekleyin:

```bash
dotnet add package AspNetCoreRateLimit
```

---

## 📝 EKSİK OLAN ÖZELLİKLER (İSTENMEDİ)

### ❌ Register Endpoint
Kullanıcı ekleme sadece admin tarafından yapılacak, bu yüzden public register endpoint'i yok.

### ❌ Password Change
Şifre değiştirme endpoint'i istek dışı bırakıldı.

### ❌ Forgot Password
Şifre sıfırlama endpoint'i istek dışı bırakıldı.

### ❌ Email Verification
Email doğrulama istek dışı bırakıldı.

### ❌ Refresh Token
Token yenileme mekanizması istek dışı bırakıldı.

---

## 🎯 SONRAKI ADIMLAR (ÖNERİLER)

### 1. Test Kullanıcıları Oluşturma

Migration sonrası test kullanıcıları ekleyin:

```sql
-- Admin kullanıcı
INSERT INTO Users (Id, FullName, Email, PasswordHash, Role, IsActive, CreatedAt)
VALUES (NEWID(), 'Admin User', 'admin@example.com', 
        'hashed_password_here', 'Admin', 1, GETUTCDATE());

-- Teacher kullanıcı
INSERT INTO Users (Id, FullName, Email, PasswordHash, Role, Title, IsActive, CreatedAt)
VALUES (NEWID(), 'Ahmet Yılmaz', 'teacher@example.com', 
        'hashed_password_here', 'Teacher', 'Doç. Dr.', 1, GETUTCDATE());

-- Student kullanıcı
INSERT INTO Users (Id, FullName, Email, PasswordHash, Role, StudentNumber, IsActive, CreatedAt)
VALUES (NEWID(), 'Mehmet Demir', 'student@example.com', 
        'hashed_password_here', 'Student', '20200001', 1, GETUTCDATE());
```

⚠️ **NOT:** `PasswordHash` değerlerini `AppUserService` kullanarak hash'leyin.

### 2. Admin Panel için User Management Endpoint'leri

Admin'in kullanıcı eklemesi/düzenlemesi için:

```csharp
[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class UserManagementController : ControllerBase
{
    [HttpPost("add-student")]
    public async Task<IActionResult> AddStudent([FromBody] CreateAppUserDto dto)
    {
        // Student ekleme
    }
    
    [HttpPost("add-teacher")]
    public async Task<IActionResult> AddTeacher([FromBody] CreateAppUserDto dto)
    {
        // Teacher ekleme
    }
}
```

### 3. Refresh Token Mekanizması

Token süresinin dolmasından önce yenileme:

```csharp
[HttpPost("refresh-token")]
public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
{
    // Refresh token logic
}
```

---

## 📄 ÖZET

| Özellik | Durum |
|---------|-------|
| **JWT Authentication** | ✅ Yapıldı |
| **3 Farklı Login Endpoint** | ✅ Admin, Teacher, Student |
| **Rol Bazlı Authorization** | ✅ Policies oluşturuldu |
| **Swagger JWT Desteği** | ✅ Bearer token ile test |
| **CORS Yapılandırması** | ✅ AllowAll (dev için) |
| **Token Expiry** | ✅ 24 saat |
| **Last Login Tracking** | ✅ Her login'de güncelleniyor |
| **Register Endpoint** | ❌ İstek dışı |
| **Password Change** | ❌ İstek dışı |
| **Refresh Token** | ❌ İstek dışı |

---

**Oluşturulma Tarihi:** 6 Mart 2026  
**Durum:** ✅ Hazır ve Test Edilmeye Uygun

**Kullanıma Hazır Endpoint'ler:**
- `POST /api/AdminAuth/login`
- `POST /api/TeacherAuth/login`
- `POST /api/StudentAuth/login`
