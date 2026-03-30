# 🌱 SEED DATA - TEST KULLANICILARI

## 📋 OLUŞTURULAN TEST KULLANICILARI

ProjectDbContext'e 3 test kullanıcısı seed data olarak eklendi.

**Tüm kullanıcılar için ortak şifre:** `1234`  
**Hash değeri:** `A6xnQhbz4gLh9j4ZDj6RjW6M8M6m8vM2qXfXk6K9n6Y=`

---

## 👥 KULLANICI DETAYLARI

### 1️⃣ Admin Kullanıcı

| Alan | Değer |
|------|-------|
| **ID** | `11111111-1111-1111-1111-111111111111` |
| **Tam Ad** | Admin User |
| **Email** | admin@mudek.com |
| **Şifre** | 1234 |
| **Rol** | Admin |
| **Telefon** | +90 555 000 0001 |
| **Aktif** | ✅ true |
| **Oluşturulma** | 01.01.2024 00:00:00 UTC |

**Login Endpoint:** `POST /api/AdminAuth/login`

**Login Request:**
```json
{
  "email": "admin@mudek.com",
  "password": "1234"
}
```

---

### 2️⃣ Teacher Kullanıcı

| Alan | Değer |
|------|-------|
| **ID** | `22222222-2222-2222-2222-222222222222` |
| **Tam Ad** | Ahmet Yılmaz |
| **Email** | teacher@mudek.com |
| **Şifre** | 1234 |
| **Rol** | Teacher |
| **Unvan** | Doç. Dr. |
| **Telefon** | +90 555 000 0002 |
| **Aktif** | ✅ true |
| **Oluşturulma** | 01.01.2024 00:00:00 UTC |

**Login Endpoint:** `POST /api/TeacherAuth/login`

**Login Request:**
```json
{
  "email": "teacher@mudek.com",
  "password": "1234"
}
```

---

### 3️⃣ Student Kullanıcı

| Alan | Değer |
|------|-------|
| **ID** | `33333333-3333-3333-3333-333333333333` |
| **Tam Ad** | Mehmet Demir |
| **Email** | student@mudek.com |
| **Şifre** | 1234 |
| **Rol** | Student |
| **Öğrenci No** | 20200001 |
| **Telefon** | +90 555 000 0003 |
| **Aktif** | ✅ true |
| **Oluşturulma** | 01.01.2024 00:00:00 UTC |

**Login Endpoint:** `POST /api/StudentAuth/login`

**Login Request:**
```json
{
  "email": "student@mudek.com",
  "password": "1234"
}
```

---

## 💾 SEED DATA KODU

Aşağıdaki kod `ProjectDbContext.OnModelCreating` metoduna eklendi:

```csharp
// ========== SEED DATA - TEST KULLANICILARI ==========
// Şifre: 1234 (Hash: A6xnQhbz4gLh9j4ZDj6RjW6M8M6m8vM2qXfXk6K9n6Y=)

var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
var teacherId = Guid.Parse("22222222-2222-2222-2222-222222222222");
var studentId = Guid.Parse("33333333-3333-3333-3333-333333333333");

b.Entity<AppUser>().HasData(
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
        CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
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
        CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
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
        CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
    }
);
```

---

## 🚀 MİGRATION VE DATABASE UPDATE

### 1. Migration Oluşturma

```bash
cd BitirmeApi.DataAccess
dotnet ef migrations add AddSeedDataForTestUsers --context ProjectDbContext
```

### 2. Database'i Güncelleme

```bash
dotnet ef database update --context ProjectDbContext
```

Migration sonrası veritabanına 3 test kullanıcısı otomatik olarak eklenecek.

---

## 🧪 TEST SENARYOLARI

### Senaryo 1: Admin Login

**Request:**
```http
POST https://localhost:7000/api/AdminAuth/login
Content-Type: application/json

{
  "email": "admin@mudek.com",
  "password": "1234"
}
```

**Beklenen Response (200):**
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

### Senaryo 2: Teacher Login

**Request:**
```http
POST https://localhost:7000/api/TeacherAuth/login
Content-Type: application/json

{
  "email": "teacher@mudek.com",
  "password": "1234"
}
```

**Beklenen Response (200):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "22222222-2222-2222-2222-222222222222",
    "fullName": "Ahmet Yılmaz",
    "email": "teacher@mudek.com",
    "role": "Teacher",
    "title": "Doç. Dr.",
    "programId": null,
    "programName": null
  },
  "message": "Öğretmen girişi başarılı"
}
```

---

### Senaryo 3: Student Login

**Request:**
```http
POST https://localhost:7000/api/StudentAuth/login
Content-Type: application/json

{
  "email": "student@mudek.com",
  "password": "1234"
}
```

**Beklenen Response (200):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "33333333-3333-3333-3333-333333333333",
    "fullName": "Mehmet Demir",
    "email": "student@mudek.com",
    "role": "Student",
    "studentNumber": "20200001",
    "programId": null,
    "programName": null
  },
  "message": "Öğrenci girişi başarılı"
}
```

---

### Senaryo 4: Yanlış Şifre

**Request:**
```http
POST https://localhost:7000/api/AdminAuth/login
Content-Type: application/json

{
  "email": "admin@mudek.com",
  "password": "wrongpassword"
}
```

**Beklenen Response (401):**
```json
{
  "message": "Email veya şifre hatalı"
}
```

---

### Senaryo 5: Yanlış Rol ile Login

**Request:**
```http
POST https://localhost:7000/api/AdminAuth/login
Content-Type: application/json

{
  "email": "student@mudek.com",
  "password": "1234"
}
```

**Beklenen Response (403):**
```json
{
  "message": "Bu endpoint sadece Admin kullanıcılar içindir"
}
```

---

## 📊 SWAGGER İLE TEST

### 1. Uygulamayı Başlatın

```bash
cd BitirmeApi.Presentation
dotnet run
```

### 2. Swagger UI'a Gidin

```
https://localhost:7000/swagger
```

### 3. Login Test Adımları

1. **AdminAuth/login**, **TeacherAuth/login** veya **StudentAuth/login** endpoint'ini açın
2. **"Try it out"** butonuna tıklayın
3. Request body'ye test kullanıcı bilgilerini girin:
   ```json
   {
     "email": "admin@mudek.com",
     "password": "1234"
   }
   ```
4. **"Execute"** butonuna tıklayın
5. Response'dan `token` değerini kopyalayın
6. Sayfanın sağ üstündeki **"Authorize"** butonuna tıklayın
7. Token'ı yapıştırın (başına "Bearer " eklemeyin, Swagger otomatik ekler)
8. **"Authorize"** butonuna tıklayın
9. Artık korumalı endpoint'leri test edebilirsiniz

---

## 🔐 GÜVENLİK NOTLARI

### ⚠️ ÖNEMLİ UYARILAR

1. **Test Kullanıcıları Sadece Development İçin!**
   - Bu kullanıcılar test amaçlıdır
   - Production ortamında **mutlaka silin** veya şifrelerini değiştirin

2. **Basit Şifre**
   - Şifre: `1234` sadece test için kullanılıyor
   - Production'da güçlü şifreler kullanın

3. **Seed Data**
   - Seed data sadece development veritabanında olmalı
   - Production migration'ında seed data olmamalı

---

## 📝 POSTMAN COLLECTION

### Admin Login
```javascript
{
  "name": "Admin Login",
  "request": {
    "method": "POST",
    "header": [
      {
        "key": "Content-Type",
        "value": "application/json"
      }
    ],
    "body": {
      "mode": "raw",
      "raw": "{\n  \"email\": \"admin@mudek.com\",\n  \"password\": \"1234\"\n}"
    },
    "url": {
      "raw": "https://localhost:7000/api/AdminAuth/login",
      "protocol": "https",
      "host": ["localhost"],
      "port": "7000",
      "path": ["api", "AdminAuth", "login"]
    }
  }
}
```

### Teacher Login
```javascript
{
  "name": "Teacher Login",
  "request": {
    "method": "POST",
    "header": [
      {
        "key": "Content-Type",
        "value": "application/json"
      }
    ],
    "body": {
      "mode": "raw",
      "raw": "{\n  \"email\": \"teacher@mudek.com\",\n  \"password\": \"1234\"\n}"
    },
    "url": {
      "raw": "https://localhost:7000/api/TeacherAuth/login",
      "protocol": "https",
      "host": ["localhost"],
      "port": "7000",
      "path": ["api", "TeacherAuth", "login"]
    }
  }
}
```

### Student Login
```javascript
{
  "name": "Student Login",
  "request": {
    "method": "POST",
    "header": [
      {
        "key": "Content-Type",
        "value": "application/json"
      }
    ],
    "body": {
      "mode": "raw",
      "raw": "{\n  \"email\": \"student@mudek.com\",\n  \"password\": \"1234\"\n}"
    },
    "url": {
      "raw": "https://localhost:7000/api/StudentAuth/login",
      "protocol": "https",
      "host": ["localhost"],
      "port": "7000",
      "path": ["api", "StudentAuth", "login"]
    }
  }
}
```

---

## 🎯 ÖZELLİKLER

### ✅ Seed Data Avantajları

1. **Otomatik Test Verisi**
   - Her migration sonrası test kullanıcıları hazır
   - Manuel kullanıcı eklemeye gerek yok

2. **Tutarlılık**
   - Her ortamda aynı test kullanıcıları
   - Tüm geliştiriciler aynı verilerle çalışır

3. **Hızlı Başlangıç**
   - Projeyi ilk clone eden kişi hemen test edebilir
   - Onboarding süreci hızlanır

4. **CI/CD Uyumlu**
   - Otomatik testler için hazır veriler
   - Integration testlerinde kullanılabilir

---

## 📊 VERİTABANI KONTROLÜ

Migration sonrası veritabanında kontrol:

```sql
-- Tüm kullanıcıları listele
SELECT 
    Id,
    FullName,
    Email,
    Role,
    StudentNumber,
    Title,
    IsActive,
    CreatedAt
FROM Users
ORDER BY Role;

-- Sadece seed data kullanıcıları
SELECT 
    Id,
    FullName,
    Email,
    Role
FROM Users
WHERE Id IN (
    '11111111-1111-1111-1111-111111111111',
    '22222222-2222-2222-2222-222222222222',
    '33333333-3333-3333-3333-333333333333'
);
```

**Beklenen Sonuç:**
| Id | FullName | Email | Role |
|----|----------|-------|------|
| 11111111-... | Admin User | admin@mudek.com | Admin |
| 22222222-... | Ahmet Yılmaz | teacher@mudek.com | Teacher |
| 33333333-... | Mehmet Demir | student@mudek.com | Student |

---

## 🔄 SEED DATA'YI GÜNCELLEME

Seed data'da değişiklik yapmanız gerekirse:

### 1. ProjectDbContext'i Düzenleyin

```csharp
// Örnek: Email değiştirme
new AppUser
{
    Id = adminId,
    Email = "admin@newdomain.com",  // ← Değişiklik
    // ... diğer alanlar
}
```

### 2. Yeni Migration Oluşturun

```bash
dotnet ef migrations add UpdateSeedData --context ProjectDbContext
```

### 3. Migration'ı Uygulayın

```bash
dotnet ef database update --context ProjectDbContext
```

---

## ⚠️ PRODUCTION NOTLARI

### Seed Data'yı Production'dan Kaldırma

**Yöntem 1: Conditional Seed Data**

```csharp
// OnModelCreating içinde
#if DEBUG
    b.Entity<AppUser>().HasData(
        // Seed data buraya
    );
#endif
```

**Yöntem 2: Environment Check**

```csharp
// Sadece Development ortamında
if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
{
    b.Entity<AppUser>().HasData(
        // Seed data buraya
    );
}
```

**Yöntem 3: Ayrı Migration**

Development ve Production için farklı migration branch'leri kullanın.

---

## 🎉 ÖZET

| Özellik | Değer |
|---------|-------|
| **Toplam Kullanıcı** | 3 |
| **Admin** | ✅ 1 (admin@mudek.com) |
| **Teacher** | ✅ 1 (teacher@mudek.com) |
| **Student** | ✅ 1 (student@mudek.com) |
| **Ortak Şifre** | 1234 |
| **Tüm Kullanıcılar Aktif** | ✅ true |
| **Sabit ID'ler** | ✅ Guid pattern |

---

**Oluşturulma Tarihi:** 6 Mart 2026  
**Durum:** ✅ Seed Data Eklendi - Migration Bekleniyor

**Sonraki Adım:**
```bash
cd BitirmeApi.DataAccess
dotnet ef migrations add AddSeedDataForTestUsers --context ProjectDbContext
dotnet ef database update --context ProjectDbContext
```
