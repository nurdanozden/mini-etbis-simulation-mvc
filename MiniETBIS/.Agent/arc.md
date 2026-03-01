# Mini ETBÝS Simülasyon Platformu
ASP.NET Core MVC Tabanlý Teknik Dokümantasyon

---

## 1. Proje Amacý

Mini ETBÝS Simülasyon Platformu, e-ticaret yapan iþletmelerin kayýt altýna alýnmasý, satýþ verilerinin toplanmasý ve bu verilerin analiz edilerek istatistiksel çýktýlar üretilmesini simüle eden bir sistemdir.

Amaç:
- ETBÝS benzeri bir kamu veri toplama sisteminin temel mimarisini modellemek
- Yüksek hacimli veri senaryosuna uygun bir backend tasarýmý oluþturmak
- Analitik dashboard üretmek
- Performans ve veri modelleme yetkinliði göstermek

---

## 2. Teknoloji Yýðýný (Tech Stack)

Backend:
- ASP.NET Core 8 MVC
- Entity Framework Core (Code First)
- PostgreSQL

Frontend:
- Razor View Engine
- Bootstrap 5
- Chart.js

Authentication:
- ASP.NET Core Identity
- Role-based Authorization

Diðer:
- AutoMapper
- FluentValidation
- Serilog (loglama)
- Docker (opsiyonel containerization)

---

## 3. Sistem Mimarisi

Katmanlý Mimari (Layered Architecture)

- Presentation Layer (MVC)
- Business Layer (Service Classes)
- Data Access Layer (Repository Pattern)
- Database Layer (PostgreSQL)

Opsiyonel:
- DTO Pattern
- Unit of Work

---

## 4. Kullanýcý Rolleri

1. Admin
   - Tüm firmalarý görüntüleme
   - Ýstatistik dashboard eriþimi
   - Sistem loglarýný görüntüleme

2. Firma (E-Ticaret Ýþletmesi)
   - Firma profil yönetimi
   - Satýþ verisi ekleme
   - Kendi satýþ analizini görüntüleme

---

## 5. Veritabaný Tasarýmý

### 5.1 Tablolar

Users
- Id (PK)
- Email
- PasswordHash
- Role

Companies
- Id (PK)
- Name
- TaxNumber
- City
- Sector
- CreatedDate
- UserId (FK)

Products
- Id (PK)
- Name
- Category
- Price
- CompanyId (FK)

Sales
- Id (PK)
- ProductId (FK)
- Quantity
- TotalAmount
- SaleDate
- City

AuditLogs
- Id (PK)
- UserId
- Action
- Timestamp
- IPAddress

### 5.2 Ýliþkiler

Company 1 - N Products  
Product 1 - N Sales  
User 1 - 1 Company  

### 5.3 Indexleme

Performans için:

- Sales(SaleDate)
- Sales(ProductId)
- Companies(City)
- Companies(Sector)

---

## 6. Temel Modüller

### 6.1 Firma Kayýt Modülü
- Firma oluþturma
- Vergi numarasý validasyonu
- Sektör seçimi

### 6.2 Satýþ Veri Giriþ Modülü
- Ürün seçimi
- Satýþ miktarý girme
- Otomatik toplam hesaplama

### 6.3 Dashboard Modülü

Grafikler:
- Aylýk toplam satýþ
- Þehre göre satýþ daðýlýmý
- Sektöre göre firma sayýsý
- En çok satan ürünler

Chart.js ile dinamik grafik üretimi.

---

## 7. Ýþ Kurallarý

- Satýþ miktarý negatif olamaz.
- Firma sadece kendi verisini görebilir.
- Admin tüm sistem verisine eriþebilir.
- Ayný vergi numarasý ile ikinci firma kaydý yapýlamaz.

---

## 8. Performans Stratejileri

- AsNoTracking() kullanýmý (sadece okuma iþlemleri için)
- Pagination
- Lazy Loading devre dýþý
- Projection (Select ile DTO mapleme)
- Index optimizasyonu

---

## 9. Güvenlik

- Role-based authorization
- Anti-forgery token
- Input validation (FluentValidation)
- SQL Injection korumasý (EF Core)
- Audit log mekanizmasý

---

## 10. API Opsiyonu (Geliþtirilebilir)

MVC projesine ek olarak:

- RESTful API endpointleri
- /api/sales
- /api/companies
- JWT tabanlý authentication

---

## 11. Raporlama Özellikleri

- CSV export
- PDF rapor çýktýsý
- Tarih aralýðý filtreleme
- KPI hesaplama:
  - Toplam satýþ hacmi
  - Ortalama sipariþ tutarý
  - Aylýk büyüme oraný

---

## 12. Gelecek Geliþtirmeler

- Fake data generator (yük testi için)
- Background service ile otomatik veri üretimi
- Mini data warehouse yapýsý
- Redis cache
- Docker + CI/CD pipeline

---

## 13. Deployment

- Dockerfile
- PostgreSQL container
- Environment variable bazlý connection string
- Production ortamýnda Nginx reverse proxy

---

## 14. Projenin Teknik Güçlü Yanlarý

- Normalize edilmiþ veri modeli
- Rol bazlý eriþim kontrolü
- Performans optimizasyonu
- Analitik dashboard
- Audit log sistemi

---

## 15. Sonuç

Bu proje, kamu entegrasyonlu e-ticaret veri toplama sistemlerinin temel mimarisini simüle eder. 
Kurumsal ölçekli veri yönetimi, analiz ve performans odaklý backend geliþtirme yetkinliði gösterir.