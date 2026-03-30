# MÜDEK Değerlendirme Sistemi - İyileştirme Özeti

Bu dokümantasyon, MÜDEK değerlendirme sistemine yapılan minimal ama eksiksiz şema değişikliklerini açıklar.

## 📋 Yapılan Değişiklikler Özeti

### ✅ 1. Yeni Eklenen Tablolar (5 Adet)

#### 1.1. ExamQuestionOutcomeMappings
- **Amaç:** Bir sınav sorusunun birden fazla ders öğrenme çıktısı ile ağırlıklı ilişkilendirilmesi
- **Özellikler:**
  - Çoklu öğrenme çıktısı desteği
  - Ağırlık (Weight) bazlı eşleme
  - Unique constraint ile aynı soru-çıktı çifti bir kez tanımlanır

#### 1.2. AssessmentComponents
- **Amaç:** Quiz, ödev, proje, sunum, uygulama, lab gibi tüm değerlendirme bileşenlerini tek yapıda toplamak
- **Desteklenen Bileşen Tipleri:**
  - Quiz
  - Homework (Ödev)
  - Project (Proje)
  - Presentation (Sunum)
  - Practice (Uygulama)
  - Lab (Laboratuvar)
  - ExamQuestion (Klasik sınav sorusu)
  - Other (Diğer)
- **Özellikler:**
  - Maksimum puan
  - Katkı yüzdesi
  - Aktif/Pasif durumu
  - Sıralama indeksi

#### 1.3. AssessmentComponentOutcomeMappings
- **Amaç:** Değerlendirme bileşenlerinin öğrenme çıktıları ile ağırlıklı ilişkilendirilmesi
- **Özellikler:**
  - Her bileşen birden fazla öğrenme çıktısına bağlanabilir
  - Ağırlık bazlı eşleme

#### 1.4. StudentAssessmentComponentScores
- **Amaç:** Öğrencilerin değerlendirme bileşenlerinden aldıkları puanları tutmak
- **Özellikler:**
  - Öğrenci bazlı puan takibi
  - Değerlendiren bilgisi
  - Değerlendirme tarihi
  - Notlar alanı
  - Unique constraint ile bir öğrenci bir bileşenden bir kez puan alabilir

#### 1.5. CourseEvaluationLetterGradeRules
- **Amaç:** Harf notu kurallarını dönem/değerlendirme bazlı saklamak
- **Özellikler:**
  - Dönem bazlı harf notu sistemi
  - Min/Max puan aralıkları
  - Geçer not bilgisi
  - Minimum final puanı şartı (opsiyonel)
  - Check constraints ile puan aralığı validasyonu

---

### ✅ 2. Güncellenmiş Tablolar

#### 2.1. ExamQuestions
**Eklenen Kolonlar:**
- `Title` (nvarchar(500), nullable) - Soru başlığı/kısa açıklama
- `Description` (nvarchar(max), nullable) - Detaylı açıklama
- `QuestionType` (nvarchar(50), nullable) - Soru tipi
- `UpdatedAt` (datetime2, nullable) - Güncellenme tarihi

**Değiştirilen Kolonlar:**
- `CourseLearningOutcomeId` artık **nullable** (geriye dönük uyumluluk için)
  - ⚠️ **DEPRECATED:** Artık `ExamQuestionOutcomeMappings` tablosu kullanılıyor

#### 2.2. CourseEvaluations
**Eklenen Kolonlar:**
- `StudentFeedbackEvaluation` (nvarchar(max), nullable) - Öğrenci geri bildirimi değerlendirmesi
- `ProgramOutcomeEvaluation` (nvarchar(max), nullable) - Program çıktıları değerlendirmesi
- `GeneralEvaluation` (nvarchar(max), nullable) - Genel değerlendirme
- `ImprovementSuggestions` (nvarchar(max), nullable) - İyileştirme önerileri
- `UpdatedAt` (datetime2, nullable) - Güncellenme tarihi

---

### ✅ 3. Yeni Servisler ve Repository'ler

Tüm yeni entity'ler için aşağıdaki yapılar oluşturuldu:

**Data Access Layer (Repository):**
- `IExamQuestionOutcomeMappingDal` / `EfExamQuestionOutcomeMappingDal`
- `IAssessmentComponentDal` / `EfAssessmentComponentDal`
- `IAssessmentComponentOutcomeMappingDal` / `EfAssessmentComponentOutcomeMappingDal`
- `IStudentAssessmentComponentScoreDal` / `EfStudentAssessmentComponentScoreDal`
- `ICourseEvaluationLetterGradeRuleDal` / `EfCourseEvaluationLetterGradeRuleDal`

**Business Layer (Service):**
- `IExamQuestionOutcomeMappingService` / `ExamQuestionOutcomeMappingService`
- `IAssessmentComponentService` / `AssessmentComponentService`
- `IAssessmentComponentOutcomeMappingService` / `AssessmentComponentOutcomeMappingService`
- `IStudentAssessmentComponentScoreService` / `StudentAssessmentComponentScoreService`
- `ICourseEvaluationLetterGradeRuleService` / `CourseEvaluationLetterGradeRuleService`

**Dependency Injection:**
- Tüm servisler `BusinessStartup.cs` dosyasına eklendi

---

### ✅ 4. Migration Dosyası

**Dosya:** `20260306200000_MudekEnhancements.cs`

Migration dosyası aşağıdaki işlemleri içerir:
1. Mevcut kolonların nullable yapılması
2. Yeni kolonların eklenmesi
3. Yeni tabloların oluşturulması
4. Foreign key ilişkilerinin kurulması
5. Unique index'lerin eklenmesi
6. Check constraint'lerin eklenmesi

---

## 🎯 Tasarım Kararları ve Gerekçeler

### 1. Tek Değerlendirme Bileşeni Tablosu
**Karar:** Quiz, ödev, proje vb. için ayrı ayrı tablo yerine `AssessmentComponents` tablosu kullanıldı.

**Gerekçe:**
- Kod tekrarını önler
- Yeni değerlendirme türleri kolayca eklenebilir
- Tüm bileşenler aynı veri yapısını kullanır
- Sürdürülebilirlik artar

### 2. Çoklu Öğrenme Çıktısı İlişkisi
**Karar:** ExamQuestions tablosundaki tekil `CourseLearningOutcomeId` yerine `ExamQuestionOutcomeMappings` ara tablosu kullanıldı.

**Gerekçe:**
- Bir soru birden fazla öğrenme çıktısını ölçebilir
- Her çıktının katkısı ağırlıkla belirlenebilir
- Daha esnek ve gerçekçi bir yapı

### 3. Dönem Bazlı Harf Notu Sistemi
**Karar:** Harf notu kuralları ders seviyesi yerine `CourseEvaluation` seviyesinde tanımlandı.

**Gerekçe:**
- Aynı dersin farklı dönemlerde farklı harf notu sistemi olabilir
- Her dönem için ayrı kurallar belirlenebilir
- Daha esnek ve gerçekçi bir yapı

### 4. Metinsel Değerlendirme Alanları
**Karar:** Anket yapısına zorlamak yerine doğrudan `CourseEvaluations` tablosuna kolon eklendi.

**Gerekçe:**
- Daha basit ve performanslı
- Anket sistemi öğrenci cevapları için kullanılır
- Akademik değerlendirme metinleri farklı bir amaç taşır

---

## 🔧 Kullanım Örnekleri

### Örnek 1: Bir Sınav Sorusunu Birden Fazla Çıktıya Bağlama

```csharp
// Sınav sorusu oluştur
var examQuestion = new ExamQuestion
{
    ExamId = examId,
    QuestionNumber = 1,
    MaxScore = 20,
    Title = "Nesne Yönelimli Programlama İlkeleri",
    QuestionType = "Essay"
};

// Soruyu birden fazla öğrenme çıktısına bağla
var mappings = new List<ExamQuestionOutcomeMapping>
{
    new ExamQuestionOutcomeMapping
    {
        ExamQuestionId = examQuestion.Id,
        CourseLearningOutcomeId = outcome1Id,
        Weight = 60  // %60 ağırlık
    },
    new ExamQuestionOutcomeMapping
    {
        ExamQuestionId = examQuestion.Id,
        CourseLearningOutcomeId = outcome2Id,
        Weight = 40  // %40 ağırlık
    }
};
```

### Örnek 2: Ödev/Proje Bileşeni Oluşturma

```csharp
// Final projesi tanımla
var projectComponent = new AssessmentComponent
{
    ExamId = finalExamId,
    Name = "Yazılım Geliştirme Projesi",
    ComponentType = "Project",
    MaxScore = 100,
    WeightPercentage = 30,  // Final notunun %30'u
    OrderIndex = 1
};

// Projeyi öğrenme çıktılarına bağla
var outcomeMapping = new AssessmentComponentOutcomeMapping
{
    AssessmentComponentId = projectComponent.Id,
    CourseLearningOutcomeId = outcomeId,
    Weight = 100
};

// Öğrenci puanı gir
var studentScore = new StudentAssessmentComponentScore
{
    AssessmentComponentId = projectComponent.Id,
    StudentEnrollmentId = studentId,
    Score = 85,
    Notes = "Çok başarılı bir proje",
    EvaluatedBy = "Prof. Dr. ...",
    EvaluatedAt = DateTime.Now
};
```

### Örnek 3: Dönem Bazlı Harf Notu Kuralları Tanımlama

```csharp
var letterGradeRules = new List<CourseEvaluationLetterGradeRule>
{
    new CourseEvaluationLetterGradeRule
    {
        CourseEvaluationId = evaluationId,
        LetterGrade = "AA",
        MinScore = 90,
        MaxScore = 100,
        IsPassing = true
    },
    new CourseEvaluationLetterGradeRule
    {
        CourseEvaluationId = evaluationId,
        LetterGrade = "BA",
        MinScore = 85,
        MaxScore = 89.99m,
        IsPassing = true
    },
    new CourseEvaluationLetterGradeRule
    {
        CourseEvaluationId = evaluationId,
        LetterGrade = "FF",
        MinScore = 0,
        MaxScore = 49.99m,
        IsPassing = false,
        MinimumFinalScore = 40  // Final şartı
    }
};
```

---

## 📊 Veritabanı İlişki Diyagramı (Özet)

```
CourseEvaluation (1) ----< (N) Exam (1) ----< (N) AssessmentComponent
                                                         |
                                                         | (1)
                                                         |
                                                         < (N) StudentAssessmentComponentScore
                                                         |
                                                         | (N)
                                                         |
                                                         > (N) AssessmentComponentOutcomeMapping
                                                                |
                                                                | (N)
                                                                |
                                                                > (1) CourseLearningOutcomeEntity

CourseEvaluation (1) ----< (N) CourseEvaluationLetterGradeRule

Exam (1) ----< (N) ExamQuestion (1) ----< (N) ExamQuestionOutcomeMapping
                                                         |
                                                         | (N)
                                                         |
                                                         > (1) CourseLearningOutcomeEntity
```

---

## 🚀 Migration Uygulama

Migration'ı uygulamak için:

```bash
cd BitirmeApi.DataAccess
dotnet ef database update --context ProjectDbContext
```

---

## ⚠️ Önemli Notlar

1. **Geriye Dönük Uyumluluk:** `ExamQuestions.CourseLearningOutcomeId` alanı nullable yapıldı, mevcut veriler korundu.

2. **Veri Migrasyonu:** Mevcut soru-çıktı ilişkileri `ExamQuestionOutcomeMappings` tablosuna taşınmalıdır:
   ```sql
   INSERT INTO ExamQuestionOutcomeMappings (Id, ExamQuestionId, CourseLearningOutcomeId, Weight, CreatedAt)
   SELECT NEWID(), Id, CourseLearningOutcomeId, 100, GETDATE()
   FROM ExamQuestions
   WHERE CourseLearningOutcomeId IS NOT NULL
   ```

3. **Validation:** Ağırlık toplamlarının kontrollü olması için uygulama katmanında validasyon eklenmeli.

4. **Index'ler:** Performans için gerekli unique ve foreign key index'leri otomatik oluşturuldu.

---

## 📝 Yapılması Gerekenler (İsteğe Bağlı)

1. ✅ Entity'ler oluşturuldu
2. ✅ DbContext güncellendi
3. ✅ Migration oluşturuldu
4. ✅ Repository'ler oluşturuldu
5. ✅ Servisler oluşturuldu
6. ✅ Dependency Injection yapılandırıldı
7. ⏳ Controller'lar oluşturulmalı (isteğe bağlı)
8. ⏳ DTO'lar ve Mapper'lar oluşturulmalı (isteğe bağlı)
9. ⏳ Frontend entegrasyonu yapılmalı (isteğe bağlı)
10. ⏳ Validation kuralları eklenebilir (isteğe bağlı)

---

## 📞 İletişim ve Destek

Bu değişiklikler minimal ve eksiksiz bir şekilde tasarlanmıştır. Herhangi bir sorunuz için dokümantasyonu inceleyin.

---

**Oluşturulma Tarihi:** 6 Mart 2026  
**Versiyon:** 1.0  
**Migration Numarası:** 20260306200000_MudekEnhancements
