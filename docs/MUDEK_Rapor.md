# MÜDEK Değerlendirme Excel Dosyası — Detaylı Teknik Rapor

**Dosya:** `DENEME_Müdek Değerlendirme_v1.2_16.02.2026`  
**Sürüm:** 1.2 | **Tarih:** 16.02.2026  
**Kurum:** Bilgisayar Mühendisliği Bölümü

---

## İçindekiler

1. [Genel Yapı — Sayfalar](#1-genel-yapı--sayfalar)
2. [Sayfalar Arası Veri Akışı](#2-sayfalar-arası-veri-akışı)
3. [vize Sayfası](#3-vize-sayfası)
4. [final ve butunleme Sayfaları](#4-final-ve-butunleme-sayfaları)
5. [vizey / finaly / butunlemey Sayfaları](#5-vizey--finaly--butunlemey-sayfaları)
6. [geçme notu Sayfası](#6-geçme-notu-sayfası)
7. [Ciktilar Sayfası](#7-ciktilar-sayfası)
8. [DOC Sayfası](#8-doc-sayfası)
9. [PC Sayfası](#9-pc-sayfası)
10. [A5, B, C Sayfaları](#10-a5-b-c-sayfaları)
11. [Kritik Hesaplama Zinciri (Uçtan Uca)](#11-kritik-hesaplama-zinciri-uçtan-uca)
12. [Sürüm Geçmişi](#12-sürüm-geçmişi)
13. [Tasarım Kısıtları ve Kurallar](#13-tasarım-kısıtları-ve-kurallar)

---

## 1. Genel Yapı — Sayfalar

Bu Excel dosyası, bir MÜDEK (Mühendislik Eğitim Programları Değerlendirme ve Akreditasyon Derneği) akreditasyonu sürecinde ders değerlendirmesini otomatik olarak yürütmek üzere tasarlanmıştır. Her CSV dosyası, orijinal Excel'deki bir sekmeye (sheet) karşılık gelir.

| CSV Dosyası | Sekme Adı | Tür | Açıklama |
|---|---|---|---|
| `_vize.csv` | **vize** | Veri Girişi | Vize sınavı ham notları (öğrenci bazlı) |
| `_vizey.csv` | **vizey** | Ara Geçiş | Sadece geçen öğrencilerin vize verileri |
| `_final.csv` | **final** | Veri Girişi | Final sınavı ham notları |
| `_finaly.csv` | **finaly** | Ara Geçiş | Sadece geçen öğrencilerin final verileri |
| `_butunleme.csv` | **butunleme** | Veri Girişi | Bütünleme sınavı ham notları |
| `_butunlemey.csv` | **butunlemey** | Ara Geçiş | Bütünlemeye giren öğrenciler |
| `_geçme notu.csv` | **geçme notu** | Hesaplama | Başarı notu + Harf notu + İstatistikler |
| `_Ciktilar.csv` | **Ciktilar** | Çıktı Raporu | Sınav başarı oranları + Soru istatistikleri |
| `_DOC.csv` | **DOC** | Matris | DÖÇ-Soru matrisi + Anket karşılaştırması |
| `_PC.csv` | **PC** | Matris | PÇ-DÖÇ ağırlıklı ilişki matrisi |
| `_Aciklamalar.csv` | **Aciklamalar** | Bilgi | Kullanım kılavuzu (salt metin) |
| `_Version.log.csv` | **Version.log** | Bilgi | Sürüm geçmişi |
| `_A5.csv` | **A5** | Serbest Metin | Anket sonuçlarının tartışılması |
| `_B.csv` | **B** | Serbest Metin | PÇ Karşılanma İlişkileri başlığı |
| `_C.csv` | **C** | Serbest Metin | Dönem içi değerlendirme ve öneriler |

---

## 2. Sayfalar Arası Veri Akışı

Aşağıdaki diyagram, verilerin hangi sayfadan hangi sayfaya aktığını özetler:

```
┌─────────────────────────────────────────────────────────────────┐
│  VERİ GİRİŞİ (Sadece beyaz hücreler doldurulur)                │
│                                                                  │
│  [vize]        [final]        [butunleme]                       │
│  - Öğrenci no  - Soru notları - Soru notları                    │
│  - Ad/Soyad    (kimlik bilgisi vize'den gelir)                  │
│  - Soru notları                                                  │
└──────────────────────────────┬──────────────────────────────────┘
                               │
                               ▼
┌──────────────────────────────────────────────────────────────────┐
│  [geçme notu]                                                    │
│  - Vize × 0.40 + Final × 0.60 = Başarı Notu                    │
│  - Bütünleme varsa: Vize × 0.40 + Büt × 0.60                   │
│  - Başarı Notuna göre Harf Notu (AA → FF)                       │
│  - Harf dağılımı, sınav kişi sayıları                           │
└────────────────┬─────────────────────────────────────────────────┘
                 │ (hangi öğrenci geçti bilgisi)
                 ▼
┌─────────────────────────────────────────────────────────────────┐
│  ARA GEÇİŞ (Sadece geçen öğrencileri filtreler)                 │
│  [vizey]   [finaly]   [butunlemey]                              │
└────────────────┬────────────────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────────────────┐
│  [Ciktilar]                                                      │
│  - Her soru için ortalama puan ve başarı oranı                  │
│  - Vize / Final / Bütünleme ayrı bölümlerde                     │
│  - Sınıf istatistikleri (max, min, ortalama)                    │
└────────────────┬────────────────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────────────────┐
│  [DOC]                                                           │
│  - DÖÇ-Soru ağırlık matrisi (hoca girer)                       │
│  - DÖÇ skoru = SUMPRODUCT(başarı oranları × ağırlıklar)        │
│  - Anket skoru vs Ders ölçme skoru farkı                        │
└────────────────┬────────────────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────────────────┐
│  [PC]                                                            │
│  - PÇ-DÖÇ ağırlık matrisi (bölüm girer, kilitli)               │
│  - PÇ skoru = SUMPRODUCT(PÇ-DÖÇ ağırlıkları × DÖÇ skorları)   │
└─────────────────────────────────────────────────────────────────┘
```

---

## 3. `vize` Sayfası

### 3.1 Genel Açıklama

Vize sayfası, sistemin **tek ham veri giriş noktasıdır**. Öğrenci kimlik bilgileri (numara, ad, soyad) **yalnızca bu sayfaya** girilir; diğer sınavlar (final, bütünleme) bu bilgileri otomatik olarak buradan çeker.

### 3.2 Sütun Yapısı

| Sütun | Alan | Açıklama |
|---|---|---|
| A | OGRNO | Öğrenci numarası |
| B | OGRAD | Öğrenci adı |
| C | OGRSOYAD | Öğrenci soyadı |
| D | 1. soru | İlk soru puanı |
| E | 2. soru | İkinci soru puanı |
| ... | ... | ... (Soru 3–40) |
| AP | 40. soru | 40. soru puanı |
| AQ | PROJE vb. | Ek uygulama/ödev/proje notu (ayrı sütun) |
| AR | Proje notu | Proje notu (ayrı değerlendirme için) |
| AS | **Toplam (Vize Notu)** | Hesaplamalı — aşağıda açıklanmaktadır |
| AU | Soru etiketleri | Soru numarası açıklamaları |
| AV | Soru içerikleri | Soruların konu başlıkları |
| AV52 | Proje etkisi (%) | Projenin vize notuna katkı yüzdesi |

### 3.3 Satır Yapısı

| Satır | İçerik |
|---|---|
| 1 | Başlık satırı (`OGRNO`, `OGRAD`, `OGRSOYAD`, `Ders Adı`, ...) |
| 2 | Soru numarası etiketleri (`1.soru`, `2.soru`, ..., `PROJE vb.`) |
| 3 | **Soru puan değerleri** (örn: 20, 10, 10, 20, 10, ...) |
| 4–403 | Öğrenci verileri (max 400 öğrenci) |

### 3.4 Soru İçerikleri (Örnekler)

Vize sayfasının sağ tarafında (`AV` sütunu) her sorunun hangi konuyu test ettiği yazılır:

```
1. soru : ER Diyagramı çizilmesi
2. soru : Anahtar Türleri
3. soru : Zayıf ve Güçlü İlişki
4. soru : Varlık Hiyerarşisi
... (max 40 soru)
```

### 3.5 Toplam (Vize) Notu Hesabı — `AS` Sütunu

Her öğrenci için aşağıdaki formül uygulanır:

```excel
=IF(
    COUNTA(D4:AR4) = 0,
    "Girmedi",
    ROUND(
        SUM(D4:AP4) * (1 - $AV$52/100) + AR4 * $AV$52/100,
        0
    )
)
```

**Açıklama:**

| Durum | Sonuç |
|---|---|
| `D4:AR4` tamamen boşsa | `"Girmedi"` döndürür |
| Proje etkisi (`AV52`) = 0 | `ROUND(SUM(D4:AP4) * 1 + 0, 0)` → sadece sınav soruları toplamı |
| Proje etkisi (`AV52`) = 30 | `ROUND(SUM(D4:AP4) * 0.70 + AR4 * 0.30, 0)` |

**Formül Bileşenleri:**
- `COUNTA(D4:AR4)` → Dolu hücre sayısı; 0 ise öğrenci hiç sınava girmemiş
- `SUM(D4:AP4)` → Soru 1–40 arası tüm puanların toplamı
- `$AV$52` → Projenin toplama etkisi (% olarak, sabit referans)
- `AR4` → Proje notu
- `ROUND(..., 0)` → Sonucu tam sayıya yuvarla

---

## 4. `final` ve `butunleme` Sayfaları

### 4.1 Genel Açıklama

`final` ve `butunleme` sayfaları, `vize` sayfasıyla **birebir aynı yapıya ve aynı hesaplama formüllerine** sahiptir. Tek fark, öğrenci kimlik bilgilerinin kullanıcı tarafından girilmeyip `vize` sayfasından otomatik çekilmesidir.

### 4.2 Kimlik Bilgisi Çekme Formülü

```excel
=IF(vize!A4="", "", vize!A4)
=IF(vize!B4="", "", vize!B4)
=IF(vize!C4="", "", vize!C4)
```

**Mantık:** `vize` sayfasında ilgili satırda öğrenci varsa o öğrencinin bilgisini göster, yoksa boş bırak.

> **Önemli:** Bu sütunlara hiçbir şekilde manuel veri girilmez. Yalnızca sınav sorusu puanları (D:AP sütunları) doldurulur.

### 4.3 Toplam Not Hesabı

`vize` sayfasıyla aynı formül:

```excel
=IF(
    COUNTA(D4:AR4) = 0,
    "Girmedi",
    ROUND(
        SUM(D4:AP4) * (1 - $AV$52/100) + AR4 * $AV$52/100,
        0
    )
)
```

---

## 5. `vizey` / `finaly` / `butunlemey` Sayfaları

### 5.1 Genel Açıklama

Bu sayfalar **ara geçiş sayfalarıdır** ve içlerine hiçbir veri girilmez. Tek amaçları, `geçme notu` sayfasının belirlediği "geçen öğrenci" listesine göre ham verileri filtrelemektir.

Bu sayfalara filtrelenmiş veri aktarılmasının amacı şudur: `Ciktilar` sayfasındaki başarı oranı hesaplamaları **yalnızca geçen öğrenciler üzerinden** yapılır. Böylece dersi başaramamış öğrencilerin düşük puanları, başarı oranını negatif yönde etkilemez.

### 5.2 Filtreleme Mantığı

`vizey` sayfasında her öğrenci satırı için:
- Eğer `geçme notu` sayfasındaki harf notu AA / BA / BB / CB / CC / DC ise → öğrencinin puanları aktarılır
- Eğer DD veya FF ise → boş (`""`) bırakılır

### 5.3 `butunlemey` İçin Ek Koşul

```excel
=COUNTIFS(
    butunlemey!$AS$4:$AS$400, ">0",
    butunlemey!$AS$4:$AS$400, "<>""",
    butunlemey!$AS$4:$AS$400, "<>Girmedi"
)
```

Bütünleme için; notun sıfırdan büyük, boş olmayan ve "Girmedi" olmayan öğrenciler sayılır. Bu sayede büte girmeyen öğrenciler başarı hesabını etkilemez.

---

## 6. `geçme notu` Sayfası

### 6.1 Genel Açıklama

Sistemin merkezi hesaplama motorudur. Her öğrencinin üç sınavdaki notunu alır, başarı notunu ve harf notunu hesaplar. Ayrıca sınıf istatistiklerini üretir.

### 6.2 Sütun Yapısı

| Sütun | Alan | Kaynak | Açıklama |
|---|---|---|---|
| A | Öğrenci No | `=vize!A4` | vize'den otomatik |
| B | Ad | `=vize!B4` | vize'den otomatik |
| C | Soyad | `=vize!C4` | vize'den otomatik |
| D | **Vize Notu** | `=vize!AS4` | "Girmedi" ise 0 |
| E | **Final Notu** | `=final!AS4` | "Girmedi" ise 0 |
| F | **Bütünleme Notu** | `=butunleme!AS4` | "Girmedi" ise 0 |
| G | **Başarı Notu** | Hesaplamalı | Vize+Final veya Vize+Büt kombinasyonu |
| H | **Harf Notu** | Hesaplamalı | AA, BA, BB, CB, CC, DC, DD, FF |
| I–P | Harflendirme tablosu | Manuel/Sabit | Harf sınır değerleri |
| AD | Başarılı öğrenci no | Hesaplamalı | Geçen öğrencilerin numaraları |
| AE–AH | Sıralamalar | Hesaplamalı | SORT + TOCOL ile otomatik sıralama |

### 6.3 Sınav Notlarının Çekilmesi (D, E, F Sütunları)

```excel
-- D sütunu (Vize Notu):
=IF(vize!AS4 = "Girmedi", 0, IF(vize!AS4 = "", "", vize!AS4))

-- E sütunu (Final Notu):
=IF(final!AS4 = "Girmedi", 0, IF(final!AS4 = "", "", final!AS4))

-- F sütunu (Bütünleme Notu):
=IF(butunleme!AS4 = "Girmedi", 0, IF(butunleme!AS4 = "", "", butunleme!AS4))
```

**Açıklama:**
- `"Girmedi"` değeri → `0` olarak dönüştürülür (sınava girmeyen = 0 puan)
- `""` (boş) → boş bırakılır (öğrenci kaydı yok)
- Sayısal değer → aynen aktarılır

### 6.4 Başarı Notu Hesabı — `G` Sütunu

```excel
=IF(
    AND(A2="", B2="", C2=""),
    "",                                          -- Öğrenci yoksa boş
    IF(
        E2 = "F",
        "F",                                     -- Final = F ise başarı = F
        IF(
            AND(D2="", E2="", F2=""),
            "",                                  -- Hiç sınava girmediyse boş
            IF(
                F2 > 0,
                ROUND((D2 * 0.4 + F2 * 0.6), 0),  -- Bütünleme varsa
                ROUND((D2 * 0.4 + E2 * 0.6), 0)   -- Final varsa
            )
        )
    )
)
```

**Karar Ağacı:**

```
Öğrenci kaydı yok?       → ""
Final = "F"?             → "F"
Hiç sınava girmedi?      → ""
Bütünleme notu > 0?      → ROUND(Vize × 0.40 + Büt × 0.60, 0)
Bütünleme yok?           → ROUND(Vize × 0.40 + Final × 0.60, 0)
```

**Ağırlıklar:**
- Vize ağırlığı: **%40**
- Final / Bütünleme ağırlığı: **%60**

### 6.5 Harf Notu Hesabı — `H` Sütunu

```excel
=IF(
    AND(A2<>"", B2<>"", C2<>""),
    IF(
        E2 = "F",
        "F",                                     -- Final F ise harf = FF
        IF(
            OR(
                AND(E2 <> 0, E2 <> ""),
                AND(F2 <> 0, F2 <> "")
            ),
            IF(
                IF(F2>0, F2, E2) <= $P$18,
                $P$17,                           -- FF sınırının altında → FF
                IFERROR(
                    IF(AND(G2<=$I$18, G2>=$I$19), $I$17,    -- AA
                    IF(AND(G2<=$J$18, G2>=$J$19), $J$17,    -- BA
                    IF(AND(G2<=$K$18, G2>=$K$19), $K$17,    -- BB
                    IF(AND(G2<=$L$18, G2>=$L$19), $L$17,    -- CB
                    IF(AND(G2<=$M$18, G2>=$M$19), $M$17,    -- CC
                    IF(AND(G2<=$N$18, G2>=$N$19), $N$17,    -- DC
                    IF(AND(G2<=$O$18, G2>=$O$19), $O$17,    -- DD
                    $P$17))))))),                            -- FF (varsayılan)
                    ""
                )
            ),
            IF(D2 <> "", $P$17, "")              -- Final yoksa ve vize varsa FF
        )
    ),
    ""                                           -- Öğrenci yoksa boş
)
```

### 6.6 Harflendirme Tablosu

`$I$17:$P$19` aralığında tanımlı sabit değerler:

| Hücre Grubu | Harf | Alt Sınır | Üst Sınır |
|---|---|---|---|
| `$I$17`, `$I$18`, `$I$19` | AA | 82 | 100 |
| `$J$17`, `$J$18`, `$J$19` | BA | 74 | 81 |
| `$K$17`, `$K$18`, `$K$19` | BB | 65 | 73 |
| `$L$17`, `$L$18`, `$L$19` | CB | 58 | 64 |
| `$M$17`, `$M$18`, `$M$19` | CC | 50 | 57 |
| `$N$17`, `$N$18`, `$N$19` | DC | 40 | 49 |
| `$O$17`, `$O$18`, `$O$19` | DD | 35 | 39 |
| `$P$17`, `$P$18`, `$P$19` | FF | 0 | 34 |

> **Not:** `$P$14` hücresindeki sınıf başarı düzeyi (Çok İyi / İyi / Ortanın Üstü / Orta / Zayıf) parametresine göre AA üst sınırı dinamik olarak 100'e set edilebilir.

### 6.7 Harf Dağılımı İstatistikleri

```excel
-- Her harf için kişi sayısı (satır 24–32 arasında):
=COUNTIF(H:H, "AA")   -- AA alan kişi sayısı
=COUNTIF(H:H, "BA")   -- BA alan kişi sayısı
=COUNTIF(H:H, "BB")
=COUNTIF(H:H, "CB")
=COUNTIF(H:H, "CC")
=COUNTIF(H:H, "DC")
=COUNTIF(H:H, "DD")
=COUNTIF(H:H, "F")    -- Manuel F girilenlerin sayısı

-- Toplam:
=SUM(K24:K32)

-- Sınav katılım sayıları:
Vize      → =COUNTIF(D2:D400, ">0")
Final     → =COUNTIF(E2:E400, ">0")
Bütünleme → =COUNTIF(F2:F400, ">0")
```

### 6.8 Başarılı Öğrenci Sıralaması

```excel
-- AD sütunu: Geçen öğrencilerin numaraları
=IF(
    AND(A2 <> "",
        OR(H2=$I$17, H2=$J$17, H2=$K$17, H2=$L$17, H2=$M$17, H2=$N$17)
    ),
    A2,
    ""
)
-- (AA, BA, BB, CB, CC, DC → geçti; DD, FF → geçemedi)

-- AE sütunu: Büyükten küçüğe sıralama
=IFERROR(
    SORT(TOCOL($V$2:$AA$400, 1, FALSE), 1, -1, FALSE),
    ""
)

-- AF sütunu: Küçükten büyüğe sıralama
=IFERROR(
    SORT(TOCOL($V$2:$AA$400, 1, FALSE), 1, 1, FALSE),
    ""
)
```

---

## 7. `Ciktilar` Sayfası

### 7.1 Genel Açıklama

Sistemin **nihai çıktı rapor sayfasıdır**. Vize, final ve bütünleme sınavlarının her birini ayrı bölümlerde analiz eder. Bu sayfaya hiç veri girilmez; tamamen hesaplamalıdır.

Sayfada **3 bölüm** bulunur:

| Bölüm | Satır Aralığı | Kaynak |
|---|---|---|
| Ara Sınav (Vize) Sonuçları | 1 – 115 | `vizey` sayfası |
| Final Sınavı Sonuçları | 128 – 242 | `finaly` sayfası |
| Bütünleme Sınavı Sonuçları | 255 – 370 | `butunlemey` sayfası |

### 7.2 Bölüm Yapısı (Her Bölüm İçin Ortak)

Her bölüm aşağıdaki içerikleri barındırır:

#### 7.2.1 Katılım ve Başarı Sayıları

```excel
-- Değerlendirmeye alınan başarılı öğrenci sayısı:
=IF(G5 = 0, 0, COUNT(vizey!$AS$4:$AS$400))
-- (Eğer hiç öğrenci yoksa 0; varsa geçen öğrenci sayısını döndür)

-- Toplam sınava giren öğrenci sayısı (geçme notu sayfasından):
='geçme notu'!K34    -- Vize için
='geçme notu'!K36    -- Final için
='geçme notu'!K38    -- Bütünleme için

-- Tüm sorulara doğru cevap veren öğrenci sayısı (100 tam puan):
=COUNTIF(vizey!$AS$4:$AS$400, 100)
```

#### 7.2.2 Her Sorunun Ortalama Puanı

```excel
-- Her soru için (Vize, 1. soru örneği):
=IF(
    vizey!$D$3 = "",
    "---",                                         -- Soru puan değeri girilmediyse ---
    SUM(vizey!$D$4:$D$400) / $G$4                 -- Başarılı öğrenci sayısına böl
)
```

**Formül Mantığı:**
- `vizey!$D$3` → İlgili sorunun maksimum puan değeri (boşsa soru sorulmamış)
- `SUM(vizey!$D$4:$D$400)` → Tüm geçen öğrencilerin o sorudan aldığı puanların toplamı
- `$G$4` → Değerlendirmeye alınan başarılı öğrenci sayısı

Bu işlem soru sayısı kadar (1–40 arası + PROJE) tekrarlanır.

#### 7.2.3 Soru Başarı Oranları

```excel
-- 1. soru için başarı oranı:
=IFERROR(
    IF(G7 = "---", "---", G7 / vizey!$D$3),
    0
)
```

**Formül Mantığı:**
- `G7` → İlgili sorunun ortalama puanı
- `vizey!$D$3` → O sorunun maksimum puan değeri
- Sonuç: `ortalama_puan / maksimum_puan` → 0 ile 1 arasında oran
- Hata durumunda `0` döner

Bu değerler daha sonra `DOC` sayfasında DÖÇ başarı hesabında kullanılır.

#### 7.2.4 DÖÇ Başarı Oranı Bilgisi

```excel
-- Soru DÖÇ ağırlıklarının Ciktilar'daki başarı oranı satırları:
-- (Satır 95–115 vize, 222–242 final, 349–369 bütünleme için)
=vizey!AU3    -- Soru etiketini çek
=IF(vizey!AV3 = 0, "---", vizey!AV3)    -- Soru başarı oranını al
```

#### 7.2.5 Sınav İstatistikleri

```excel
-- Vize için:
Alınan maksimum puan   → =MAX(vizey!$AS$4:$AS$400)
Alınan minimum puan    → =MIN(vizey!$AS$4:$AS$400)
Toplam puanların ort.  → =AVERAGE(vizey!$AS$4:$AS$400)

-- Final için:
Alınan maksimum puan   → =MAX(finaly!$AS$4:$AS$400)
Alınan minimum puan    → =MIN(finaly!$AS$4:$AS$400)
Toplam puanların ort.  → =AVERAGE(finaly!$AS$4:$AS$400)

-- Bütünleme için:
Alınan maksimum puan   → =MAX(butunlemey!$AS$4:$AS$400)
Alınan minimum puan    → =MIN(butunlemey!$AS$4:$AS$400)
Toplam puanların ort.  → =IFERROR(AVERAGE(butunlemey!$AS$4:$AS$400), "")
```

---

## 8. `DOC` Sayfası

### 8.1 Genel Açıklama

DOC (Ders Öğrenim Çıktıları) sayfası iki temel işlevi yerine getirir:

1. **DÖÇ-Soru Matrisi:** Her sorunun hangi Ders Öğrenim Çıktısıyla ne kadar ilişkili olduğunu ağırlıklandırarak DÖÇ bazlı başarı skorunu hesaplar.
2. **Anket-Ders Farkı Analizi:** Öğrenci anket sonuçları ile ders ölçme başarısı arasındaki farkı gösterir.

### 8.2 Sayfa Bölümleri

DOC sayfası **3 ana blok**tan oluşur:

| Blok | Başlık | Ciktilar Kaynak Aralığı |
|---|---|---|
| Blok 1 | ARA SINAV | `Ciktilar!$B$95:$B$115` ve `$D$95:$D$114` |
| Blok 2 | FİNAL SINAVI | `Ciktilar!$B$222:$B$242` ve `$D$222:$D$241` |
| Blok 3 | BÜTÜNLEME SINAVI | `Ciktilar!$B$349:$B$369` ve `$D$349:$D$368` |

### 8.3 Soru Başarı Oranlarının Çekilmesi (Satır 4)

Her sütundaki soru için, `Ciktilar` sayfasındaki başarı oranı dinamik `LET` + `INDEX` formülüyle çekilir:

```excel
=LET(
    i, COLUMN() - COLUMN($C$1) + 1,           -- Sütun indeksi (1'den başlar)
    x, IF(
           i <= 21,
           INDEX(Ciktilar!$B$95:$B$115, i),   -- Soru 1–21 için sol sütun
           INDEX(Ciktilar!$D$95:$D$114, i-21) -- Soru 22–41 için sağ sütun
       ),
    IF(x = "---", 0, x)                       -- "---" ise 0 kabul et
)
```

**Açıklama:**
- `COLUMN() - COLUMN($C$1) + 1` → C sütunundan başlayarak kaçıncı soru olduğunu hesaplar
- `INDEX(...)` → `Ciktilar` sayfasındaki ilgili satırdaki başarı oranını getirir
- `"---"` değeri → Soru sorulmamış demektir; 0 olarak işlenir

### 8.4 DÖÇ Ağırlık Matrisi

Her DÖÇ satırına (DÖÇ-1 … DÖÇ-20), hoca tarafından her soru ile ilişki ağırlığı girilir:

- **Eski sistemde:** 0 veya 1 (ilişki var / yok)
- **Yeni sistemde:** 0 ile 1 arasında ondalıklı ağırlık

> **Kural:** Her soru sütununun DÖÇ ağırlıklarının **toplamı = 1** olmalıdır. Toplam 1'den farklıysa ilgili sütun **kırmızıya** döner ve uyarı verir.

**Örnek Dolgu (ARA SINAV, Vize Soruları → DÖÇ İlişkileri):**

| DÖÇ | S1 | S2 | S3 | S4 | S5 | S6 | S7 | S8 | S9 | ... |
|---|---|---|---|---|---|---|---|---|---|---|
| DÖÇ-1 | — | — | — | — | — | — | — | — | — | ... |
| DÖÇ-2 | 1 | 0.5 | 0.5 | 0.5 | 0.5 | — | — | — | — | ... |
| DÖÇ-3 | — | 0.5 | — | 0.5 | — | 1 | 1 | — | — | ... |
| DÖÇ-5 | — | — | 0.5 | — | 0.5 | — | — | 0.5 | — | ... |
| DÖÇ-7 | — | — | — | — | — | — | — | 0.5 | — | ... |

### 8.5 DÖÇ Başarı Skoru Hesabı — `AR` Sütunu

```excel
-- Her DÖÇ için (örnek: DÖÇ-2, satır 6):
=IF(
    COUNTA(C6:AQ6) = 0,
    "---",                                   -- Hiç ağırlık girilmemişse ---
    SUMPRODUCT(C6:AQ6, C$4:AQ$4) / SUM(C6:AQ6)
)
```

**Formül Açıklaması:**
- `C6:AQ6` → DÖÇ-2 için her soruya verilen ağırlıklar (hoca girer)
- `C$4:AQ$4` → Her sorunun başarı oranı (Ciktilar'dan çekilen, satır 4)
- `SUMPRODUCT(ağırlıklar, başarı_oranları)` → Her soru ağırlığı × başarı oranı çarpımlarının toplamı
- `SUM(C6:AQ6)` → Toplam ağırlık (normalizasyon için)

**Sonuç:** 0–1 arasında bir değer. O DÖÇ'ün ne ölçüde kazandırıldığını gösterir.

### 8.6 Üç Sınavı Birleştiren DÖÇ Ortalaması — `AV` Sütunu

```excel
-- DÖÇ-1 için (3 sınavın ortalaması):
=IFERROR(
    IF(
        DOC!AR55 <> 0,                          -- Bütünleme puanı varsa
        AVERAGE(DOC!AR5, DOC!AR30, DOC!AR55),   -- Vize + Final + Büt ortalaması
        AVERAGE(DOC!AR5, DOC!AR30)              -- Bütünleme yoksa Vize + Final
    ),
    "---"
)
```

**Satır Karşılıkları:**
- `AR5–AR24` → Vize DÖÇ skorları (DÖÇ-1 … DÖÇ-20)
- `AR30–AR49` → Final DÖÇ skorları
- `AR55–AR74` → Bütünleme DÖÇ skorları

### 8.7 Anket Soruları ve Ders Ölçme Farkı

DOC sayfasının sağ bölümünde (AV–AX sütunları), öğrenci anket sonuçları ile ders ölçme sonuçları karşılaştırılır.

**Anket Soruları:**
```
1.  Dersin amaç ve içeriği bilgilendirmesi
2.  Beklentinin karşılanması
3.  Çözümlerin sosyal, ekonomik vb. değinilmesi
4.  Öğrenilenlerin yaklaşım geliştirmeye katkısı
5.  Gerekli temel altyapı kazandırılması
6.  Sınav sorularının dersin içeriğini yansıtması
7.  Anlatım ve işleyişin verimli olması
8.  Ders içerik ve sürenin verimliliği
9.  Derste yeterince örnek verilmesi
10. Sınıf içi iletişim düzeyinin değerlendirilmesi
```

**Anket-Ders Farkı Formülü:**
```excel
-- AX sütunu (fark):
= [Anket başarı yüzdesi] - [Ders ölçme skoru (AV)]
```

- Sonuç **pozitifse** → Anket skoru daha yüksek (yeşil renk)
- Sonuç **negatifse** → Ders ölçme skoru daha yüksek (kırmızı renk)

### 8.8 DÖÇ Tanımları (Final Bloğu Sonrası)

Her DÖÇ'ün metin açıklaması `B` sütununda yer alır:

```
DÖÇ-1 : Sözlü ve yazılı sınavlara bireysel olarak hazırlanabilme
DÖÇ-2 : Açı ölçme aletlerini kullanabilme ve açı ölçme yöntemlerini öğrenebilme
DÖÇ-3 : Koordinat sistemi ve temel ödevleri öğrenebilme
DÖÇ-4 : Poligon noktalarının tesis, röper, ölçü ve hesabını yapabilme
DÖÇ-5 : Nokta koordinatlarını hesaplayabilme ve bunları birbirine dönüştürebilme
DÖÇ-6 : Takeometrik alım yapabilme, ölçüleri değerlendirebilme ve ölçü krokisi çizebilme
DÖÇ-7–20 : (Derse göre hoca tarafından tanımlanır)
```

---

## 9. `PC` Sayfası

### 9.1 Genel Açıklama

PC (Program Çıktıları) sayfası, Program Çıktıları (PÇ-1 … PÇ-30) ile Ders Öğrenim Çıktıları (DÖÇ-1 … DÖÇ-20) arasındaki **ağırlıklı ilişkiyi** gösterir ve her PÇ için başarı skoru hesaplar.

> **Önemli:** Bu matris bölüm tarafından önceden doldurulmuş ve **kilitlenmiştir**. Hocanın bu alana herhangi bir müdahalesi söz konusu değildir.

### 9.2 Başlık ve DÖÇ Skoru Kaynağı (Satır 4)

```excel
-- Her DÖÇ için skoru DOC sayfasından çek:
=IF(DOC!AV7 = "---", "0", DOC!AV7)    -- DÖÇ-1 skoru
=IF(DOC!AV8 = "---", "0", DOC!AV8)    -- DÖÇ-2 skoru
...
=IF(DOC!AV26 = "---", "0", DOC!AV26)  -- DÖÇ-20 skoru
```

**Açıklama:** `DOC!AV7` → DOC sayfasında DÖÇ-1'in üç sınavı birleştiren ortalama skorudur. `"---"` ise (DÖÇ tanımlanmamış) `"0"` olarak işlenir.

### 9.3 PÇ Skoru Hesabı

```excel
-- PÇ1 için (satır 5):
=IF(
    COUNT(C5:V5) = 0,
    "---",                              -- Hiçbir DÖÇ ilişkisi yoksa ---
    (C5*$C$4 + D5*$D$4 + E5*$E$4 + F5*$F$4 + G5*$G$4 + H5*$H$4 +
     I5*$I$4 + J5*$J$4 + K5*$K$4 + L5*$L$4 + M5*$M$4 + N5*$N$4 +
     O5*$O$4 + P5*$P$4 + Q5*$Q$4 + R5*$R$4 + S5*$S$4 + T5*$T$4 +
     U5*$U$4 + V5*$V$4)
    / SUM(C5:V5)
)
```

**Formül Açıklaması:**
- `C5:V5` → PÇ1 ile DÖÇ-1 … DÖÇ-20 arasındaki ağırlık değerleri (1–4 arası tamsayılar)
- `$C$4:$V$4` → Her DÖÇ'ün skoru (DOC!AV sütunundan çekilmiş, sabit referans)
- Her PÇ-DÖÇ ağırlığı × DÖÇ skoru → çarpımların toplamı / toplam ağırlık
- `COUNT(C5:V5) = 0` → Hiç ilişki kurulmamışsa `"---"` döndür

**Özet formül:**
```
PÇ_skoru = Σ(PÇ-DÖÇ_ağırlığı[i] × DÖÇ_skoru[i]) / Σ(PÇ-DÖÇ_ağırlığı[i])
```

### 9.4 Aktif PÇ-DÖÇ İlişkileri (Bu Derste Dolu Olanlar)

| PÇ | İlişkili DÖÇ'ler (ağırlık değerleriyle) |
|---|---|
| PÇ1 | DÖÇ-1(3), DÖÇ-2(3), DÖÇ-3(3), DÖÇ-4(2), DÖÇ-5(3), DÖÇ-7(3) |
| PÇ2 | DÖÇ-1(3), DÖÇ-2(3), DÖÇ-3(3), DÖÇ-4(3), DÖÇ-5(3), DÖÇ-6(3), DÖÇ-7(3) |
| PÇ3 | DÖÇ-1(4), DÖÇ-2(3), DÖÇ-3(3), DÖÇ-4(3), DÖÇ-5(3), DÖÇ-6(4), DÖÇ-7(3) |
| PÇ4 | DÖÇ-1(3), DÖÇ-2(3), DÖÇ-3(3), DÖÇ-4(4), DÖÇ-5(3), DÖÇ-6(3), DÖÇ-7(3) |
| PÇ5 | DÖÇ-2(3), DÖÇ-3(3), DÖÇ-4(3), DÖÇ-5(2), DÖÇ-6(4) |
| PÇ8 | DÖÇ-8(1) |
| PÇ9 | DÖÇ-8(1) |
| PÇ6, PÇ7, PÇ10–PÇ30 | İlişki yok → `"---"` döner |

---

## 10. `A5`, `B`, `C` Sayfaları

Bu sayfalar yalnızca **serbest metin girişi** içindir. İçlerinde hesaplama formülü bulunmaz.

| Sayfa | Başlık | Amaç |
|---|---|---|
| **A5** | A5-Öğrenci ders değerlendirme anket sonuçlarının tartışılması | Hoca, anket sonuçlarını yorumlar ve textbox'a girer |
| **B** | B-PÇ'lerin Karşılanma İlişkileri ve Düzeyleri | PÇ karşılanma düzeyi hakkında serbest değerlendirme |
| **C** | C-Dersin dönem içerisindeki işleyişi, yapılan uygulamaları... | Dönem özeti, farklılıklar, gelecek öneriler |

---

## 11. Kritik Hesaplama Zinciri (Uçtan Uca)

Aşağıda veri akışı, adım adım ve formüllerle birlikte gösterilmiştir:

```
ADIM 1: Hoca vize soru puan değerlerini girer
        [vize!D3:AP3] = 20, 10, 10, 20, 10, ...

ADIM 2: Hoca öğrenci vize puanlarını girer
        [vize!D4:AP4] = 5, 0, 4, 8, ...

ADIM 3: Vize toplam notu hesaplanır
        [vize!AS4] = ROUND(SUM(D4:AP4)*(1-AV52/100) + AR4*AV52/100, 0)

ADIM 4: Aynı işlem final ve bütünleme için tekrarlanır
        [final!AS4], [butunleme!AS4]

ADIM 5: Başarı notu hesaplanır
        [geçme notu!G2] = ROUND(vize*0.40 + (büt>0 ? büt : final)*0.60, 0)

ADIM 6: Harf notu atanır
        [geçme notu!H2] = IF(G2>=82,"AA", IF(G2>=74,"BA", ... "FF"))

ADIM 7: Geçen öğrenciler filtrelenir
        [vizey!AS] = Geçen öğrencinin vize puanı, geçemeyenin boş

ADIM 8: Her soru başarı oranı hesaplanır
        [Ciktilar!G7] = SUM(vizey!D4:D400) / başarılı_öğrenci_sayısı
        [Ciktilar!başarı_oranı] = ortalama_puan / maks_puan

ADIM 9: DÖÇ başarı skoru hesaplanır
        [DOC!AR6] = SUMPRODUCT(DÖÇ_ağırlıkları, başarı_oranları) / SUM(ağırlıklar)

ADIM 10: 3 sınav DÖÇ ortalaması birleştirilir
         [DOC!AV7] = AVERAGE(vize_DÖÇ, final_DÖÇ) veya
                     AVERAGE(vize_DÖÇ, final_DÖÇ, büt_DÖÇ)

ADIM 11: PÇ skoru hesaplanır
         [PC!W5] = SUMPRODUCT(PÇ-DÖÇ_ağırlıkları × DÖÇ_skorları) / SUM(ağırlıklar)
```

### 11.1 Özet Formül Tablosu

| Hesaplama | Formül Özeti | Bulunduğu Yer |
|---|---|---|
| Sınav notu | `ROUND(SUM(sorular)*(1-proje%) + PROJE*proje%, 0)` | vize/final/butunleme!AS |
| Başarı notu | `ROUND(vize*0.4 + final_veya_büt*0.6, 0)` | geçme notu!G |
| Harf notu | Başarı notunu sınırlara göre AA…FF'e eşle | geçme notu!H |
| Soru ortalaması | `SUM(geçen_öğrenci_puanları) / geçen_öğrenci_sayısı` | Ciktilar!G |
| Başarı oranı | `soru_ortalaması / maks_puan` | Ciktilar!başarı_oranı |
| DÖÇ skoru | `SUMPRODUCT(ağırlıklar, başarı_oranları) / SUM(ağırlıklar)` | DOC!AR |
| DÖÇ ort. (3 sınav) | `AVERAGE(vize_DÖÇ, final_DÖÇ [, büt_DÖÇ])` | DOC!AV |
| PÇ skoru | `SUMPRODUCT(PÇ-DÖÇ_ağırlık × DÖÇ_skor) / SUM(PÇ-DÖÇ_ağırlık)` | PC!W |

---

## 12. Sürüm Geçmişi

| Sürüm | Değişiklik |
|---|---|
| **1.0** | Temel bize-uyarlamalı hali |
| **1.1** | DOC sekmesinde 9. soruya kadar yapılan toplama 40. soruya kadar genişletildi |
| **1.1** | Manuel düzeltme için geçme notu ve çıktılar sayfasındaki korumalar kaldırıldı |
| **1.1** | Öğrenci adı varsa hesaplanmayan notun FF olması düzeltildi |
| **1.1** | DOC sekmesinde anket-ders farkı pozitifse yeşil, negatifse kırmızı renklendirme |
| **1.1** | Büt notu dikkate alınmayan hesaplama düzeltildi |
| **1.1** | Manuel girilen F notunun başarılı sayılması hatası giderildi |
| **1.1** | Vizeye girmeyen ama finalden geçer not alan öğrencinin geçememesi hatası giderildi |
| **1.1** | Çıktılar sayfasının vize, final ve büt kısımlarının soru değeri ve başarı oranları düzeltildi |
| **1.1** | DOC sekmesindeki yanlış değerler düzeltildi |
| **1.1** | Final notuna F girilen öğrencilerin harf notunun otomatik güncellenmesi eklendi |
| **1.2** | Soru isimlerindeki hata giderildi |
| **1.2** | **Büte girilmemesi durumunda başarıya etki etmemesi eklendi** |

---

## 13. Tasarım Kısıtları ve Kurallar

### 13.1 Kapasite Sınırları

| Parametre | Limit |
|---|---|
| Öğrenci sayısı | Max **400** (satır 4–403) |
| Soru sayısı (her sınav) | Max **40 soru** + **1 Proje/Uygulama** |
| DÖÇ sayısı | Max **20 DÖÇ** |
| PÇ sayısı | Max **30 PÇ** |

### 13.2 Veri Giriş Kuralları

- **Beyaz dolgulu** hücreler → Hoca tarafından doldurulur
- **Diğer hücreler** → Kilitlidir, değiştirilemez
- Öğrenci bilgileri (no/ad/soyad) **yalnızca `vize` sayfasına** girilir
- Soru sayısından az soru sorulduysa, kullanılmayan sütunlar **boş bırakılır** (silinmez)
- Proje notu her zaman **100 üzerinden** değerlendirilir

### 13.3 DÖÇ Ağırlık Kuralı

Her soru sütununun DÖÇ ağırlıklarının toplamı tam olarak **1** olmalıdır:
```
Σ DÖÇ_ağırlık[soru_i] = 1
```
Aksi hâlde ilgili sütun **kırmızıya** döner ve hata uyarısı verir.

### 13.4 Proje Etkisi Kuralı

```
vize_toplam = SUM(sorular) * (1 - proje_etki%) + PROJE * proje_etki%
```
- `proje_etki% = 0` → Yalnızca sınav soruları toplamı kullanılır
- `proje_etki% = 30` → Sınav %70, Proje %30

### 13.5 Notlara Özel Renklendirme (`vize`/`final`/`butunleme` sayfalarında)

| Renk | Anlamı |
|---|---|
| Kırmızı | En düşük toplam not |
| Sarı | Ortalamaya en yakın not |
| Yeşil | En yüksek toplam not |

Bu renklendirme, fotokopi/fotoğraf için örnek kağıt seçiminde kolaylık sağlamak üzere tasarlanmıştır.

### 13.6 Birden Fazla Şube

Eğer ders A ve B şubelerinde okutuluyorsa ve soru içerikleri aynıysa:
- A şubesi verileri girildikten sonra B şubesinin öğrenci bilgileri **aynı sayfanın devamına** eklenir
- İki şube için **tek dosya** yeterlidir

---

*Bu rapor, `ExcelToCsv` klasöründeki CSV dosyalarının (`DENEME_Müdek Değerlendirme_v1.2_16.02.2026_*.csv`) incelenmesiyle otomatik olarak üretilmiştir.*
