# C1Soft B2B

Basit bir ASP.NET Core MVC B2B ürün ve sipariş uygulamasıdır.

## Kullanılan yapılar

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Türkçe Razor sayfaları
- Basit cookie tabanlı giriş sistemi

## Veritabanını çalıştırma

SQL Server’ın varsayılan instance’ı çalışıyor olmalıdır. SSMS veya sqlcmd ile şu dosya çalıştırılır:

```text
database_schema.sql
```

Bağlantı ayarı `appsettings.json` dosyasındadır:

```text
Server=.;Database=C1SoftB2B;Trusted_Connection=True;TrustServerCertificate=True;
```

## Projeyi çalıştırma

```text
dotnet restore
dotnet build
dotnet run
```

## Test kullanıcıları

```text
Admin:    admin / Admin123!
Müşteri:  customer / Customer123!
```

Admin ürün, kategori ve sipariş ekranlarını görür. Müşteri ürünleri inceler, sepete ekler ve sipariş oluşturur.

## Önemli klasörler

- `Controllers`: Sayfa ve form işlemleri
- `Models`: Veritabanı sınıfları
- `Data`: Entity Framework bağlantısı
- `Views`: Razor ekranları
- `Services`: Şifre gibi yardımcı işlemler
- `database_schema.sql`: SQL Server tabloları ve başlangıç verileri
