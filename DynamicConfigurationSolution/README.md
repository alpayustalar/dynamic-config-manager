# 🧩 Dynamic Configuration Management System (.NET 8)

Bu proje, .NET 8 ile geliştirilmiş dinamik konfigürasyon yönetim sistemidir. Amaç, `appsettings.json`, `web.config` gibi sabit yapıların yerine, servis bazlı merkezi bir yapı ile konfigürasyonların **canlı olarak** (restart gerektirmeden) yönetilmesini sağlamaktır.

---

## 📂 Proje Yapısı

| Proje Adı                     | Açıklama                                                  |
|------------------------------|------------------------------------------------------------|
| `ConfigurationLibrary`       | Dinamik konfigürasyonları yöneten kütüphane                |
| `Configuration.WebApi`       | Konfigürasyon kayıtlarını yöneten RESTful API              |
| `Configuration.WebApp`       | Web arayüzü (MVC) ile listeleme, ekleme, güncelleme, silme |
| `Configuration.UnitTests`    | xUnit ile `ConfigurationReader` unit testleri              |

---

## 🎯 Özellikler

- [x] Her servis sadece kendi konfigürasyonlarını görür (`ApplicationName`)
- [x] `GetValue<T>(string key)` ile tip güvenli erişim
- [x] EF Core üzerinden SQL tabanlı storage
- [x] MemoryCache ile performanslı erişim
- [x] Belirli aralıklarla (timer) storage güncelleme
- [x] Storage erişilemiyorsa son bilinen cache ile devam etme
- [x] Web UI ile konfigürasyonları filtreleme, ekleme, silme, güncelleme
- [x] SweetAlert ile başarılı/başarısız işlem bildirimleri
- [x] xUnit testleri ile temel kontrol

---

## 🧪 Kurulum & Çalıştırma

### 💻 Gereksinimler

- .NET 8 SDK
- SQL Server 2019+ (Express yeterlidir)
- Visual Studio 2022 (Community uygun)

### 🧱 Veritabanı Scripti

```sql
CREATE DATABASE ConfigurationDb;

USE ConfigurationDb;

CREATE TABLE ConfigurationRecords (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100),
    Type NVARCHAR(50),
    Value NVARCHAR(200),
    IsActive BIT,
    ApplicationName NVARCHAR(100),
    LastUpdated DATETIME
);

🚀 Projeyi Başlatma
Visual Studio → Multiple Startup Projects ayarla:

Configuration.WebApi ve Configuration.WebApp → Start

Web API Swagger → https://localhost:{port}/swagger

Web UI → https://localhost:{port}/Configuration

🧪 Unit Test Çalıştırma
Visual Studio > Test > Test Explorer
Veya Ctrl + R, A
