# Dynamic Form Builder

Jotform benzeri dinamik form oluşturma sistemi. ASP.NET Core MVC ile katmanlı mimari kullanılarak geliştirilmiştir.

## Teknolojiler

- C# / .NET 8
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Bootstrap 5
- Sortable.js
- ClosedXML (Excel export)

## Solution Yapısı

```
DynamicFormBuilder/
├── DynamicFormBuilder.Entity/       # Entity ve enum tanımları
├── DynamicFormBuilder.DataAccess/   # DbContext, Repository Pattern
├── DynamicFormBuilder.Business/     # Service Layer, iş kuralları
└── DynamicFormBuilder.WebUI/        # MVC UI, Controller, Views
```

## Özellikler

### Dashboard
- Toplam / aktif form sayısı
- Toplam yanıt sayısı
- Son formlar ve yanıtlar

### Form Yönetimi
- CRUD işlemleri
- Kopyalama, aktif/pasif, yayınlama
- Public link üretimi (`/PublicForm/Fill/{slug}`)
- Tema seçimi (Light, Dark, Rounded, Flat)

### Form Builder
- Sürükle-bırak alan ekleme (Sortable.js)
- 24 farklı alan tipi
- Alan özellikleri: label, placeholder, validation, width, CSS class
- Koşullu mantık (show/hide)
- JSON tabanlı kayıt

### Public Form
- Yayınlanan formlar public link ile doldurulabilir
- Backend validasyonu
- Dosya yükleme desteği

### Yanıt Yönetimi
- Liste ve detay görüntüleme
- Silme
- Excel export
- PDF export altyapısı (iskelet)

### E-posta Bildirimi
- `SmtpSettings` ile yapılandırılabilir
- Aktif/pasif kontrolü

## Kurulum

### Gereksinimler
- .NET 8 SDK
- SQL Server veya LocalDB

### Adımlar

1. Repoyu klonlayın veya solution klasörüne gidin.

2. `appsettings.json` içinde connection string'i düzenleyin:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DynamicFormBuilderDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

3. Migration uygulayın (uygulama ilk çalıştırmada otomatik migrate eder):

```bash
dotnet ef database update --project DynamicFormBuilder.DataAccess --startup-project DynamicFormBuilder.WebUI
```

4. Uygulamayı çalıştırın:

```bash
dotnet run --project DynamicFormBuilder.WebUI
```

5. Tarayıcıda `https://localhost:5xxx` adresine gidin.

## Kullanım Akışı

1. **Dashboard** → genel özet
2. **Yeni Form** → başlık, slug, tema ayarları
3. **Form Builder** → alanları sürükle-bırak ile ekle, kaydet
4. **Önizleme** → formu kontrol et
5. **Yayınla** → public link oluşur
6. **Yanıtlar** → gelen verileri görüntüle / Excel'e aktar

## Public URL Örneği

```
https://localhost:5001/PublicForm/Fill/contact-form
```

## SMTP Ayarları

```json
"SmtpSettings": {
  "Enabled": false,
  "Host": "smtp.example.com",
  "Port": 587,
  "EnableSsl": true,
  "Username": "your-email@example.com",
  "Password": "your-password",
  "FromEmail": "noreply@example.com",
  "FromName": "Dynamic Form Builder"
}
```

## Dosya Yükleme

- Dosyalar `wwwroot/uploads/` altında saklanır
- Maksimum boyut ve izin verilen uzantılar `appsettings.json` → `FileUpload` bölümünden yapılandırılır

## Mimari Notlar

- **Controller**: İnce katman, yönlendirme ve model binding
- **Business**: İş kuralları, validasyon, DTO dönüşümleri
- **DataAccess**: EF Core, Generic Repository, özel repository'ler
- **Entity**: Saf domain modelleri

## Geliştirme Notları

- PDF export şu an metin tabanlı iskelet servistir; üretimde QuestPDF veya iTextSharp entegre edilebilir
- Form Builder kaydetme endpoint'i JSON API olarak çalışır
- Koşullu mantık client-side (preview/public) ve server-side kayıt ile desteklenir

## Lisans

MIT (veya proje sahibinin belirlediği lisans)
