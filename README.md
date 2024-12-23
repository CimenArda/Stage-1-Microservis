# Microservice
- Bu repo Microservis Mimarisi öğrenirken 1. Etapta yaptığım pratik çalışmaları tanıtmak ve işlediğim teorik kısımları(CalisilanKonuBasliklari.txt) göstermek amacıyla oluşturulmuştur.

## BasicMicroservisOperation

Bu çalışma, mikroservis mimarisine giriş için temel bir uygulamadır. RabbitMQ ve MassTransit kullanılarak olay tabanlı mesajlaşma, MongoDB ve Entity Framework Core ile veri yönetimi gerçekleştirilmiştir. Sipariş, stok ve ödeme süreçleri bağımsız servisler halinde geliştirilmiş, mikroservisler arası iletişim asenkron bir şekilde sağlanmıştır.

### Çalışmada Yapılanlar
- **MassTransit ve RabbitMQ Entegrasyonu**: Mikroservisler arasında asenkron iletişim sağlandı.
- **Sipariş Yönetimi**: Siparişlerin durumu olaylara göre güncellendi.
- **Stok Yönetimi**: MongoDB kullanılarak stok kontrolü ve rezervasyon işlemleri yapıldı.
- **Ödeme Süreci**: Stok rezervasyonuna göre ödeme işlemleri simüle edildi.
- **Ortak Mesaj ve Olay Yapıları**: Mikroservisler arası standart mesajlaşma sağlandı.
- **API Dokümantasyonu**: Swagger ile servislerin test edilebilirliği artırıldı.

---

## MicroservisArchitecture_CommunicationModels

Bu çalışma, Mikroservis İletişim Modelleri üzerine odaklanmaktadır ve gRPC, RabbitMQ, ve HTTP tabanlı iletişim yöntemlerini incelemektedir. Farklı mikroservislerin iletişim protokollerini uygulayarak hem olay tabanlı hem de istemci-sunucu tabanlı senaryolar geliştirilmiştir. Bu çalışmalar, mikroservis mimarisindeki iletişim yapılarını anlamak ve farklı kullanım durumlarına uygun çözümler oluşturmak için hazırlanmıştır.

### gRPC ile İletişim
- İstemci-sunucu arasında çift yönlü mesaj akışı sağlandı.
- Protobuf ile veri modelleri tanımlandı.

### HTTP API Kullanımı
- HTTP üzerinden veri istekleri ve yanıtları işlendi.
- Gecikme senaryoları simüle edildi.

### RabbitMQ ile Mesajlaşma
- Üretici ve tüketici uygulamaları geliştirildi.
- Direct exchange modeliyle mesaj iletimi sağlandı.

---

## Data_Synchronization_Examples

Bu çalışma, mikroservislerde veri senkronizasyonu üzerine iki farklı yaklaşımı göstermektedir: API tabanlı ve event tabanlı iletişim. İki farklı senaryo üzerinde, veritabanlarında senkronize veri güncellemelerinin nasıl yapılabileceği ele alınmıştır. RabbitMQ, MassTransit ve HTTP gibi araçlar kullanılarak, mikroservislerin birbirleriyle nasıl uyumlu çalıştığı pratik edilmiştir.

### API Tabanlı Veri Senkronizasyonu
- Bir serviste güncellenen veri, diğer bir servise HTTP istekleriyle iletilerek senkronize edildi.
- Veriler, Service_A ve Service_B arasındaki RESTful çağrılarla güncellendi.

### Event Tabanlı Veri Senkronizasyonu
- RabbitMQ ve MassTransit kullanılarak, veri değişiklikleri için event tabanlı bir iletişim modeli uygulandı.
- `UpdatedPersonNameEvent` kullanılarak ServiceA tarafından yapılan güncellemeler ServiceB'ye bildirildi.

### Veritabanı Kullanımı
- Service_A ve Service_B, MSSQL veritabanlarını kullandı ve EF Core ile veri yönetimi sağlandı.

### RabbitMQ Entegrasyonu
- Event tabanlı senaryoda, RabbitMQ kuyruğu üzerinden mesajlaşma sağlandı.
- Event tüketicileri (Consumer) ile mesajlar işlendi ve ilgili veritabanı güncellendi.

### Kod Organizasyonu
- API-based ve Event-based senaryolar ayrı projeler altında düzenlendi.
- Ortak veri ve mesaj yapıları `Shared` katmanında tanımlandı.

---

## Two-PhaseCommit (2PC) Protocol

Bu çalışma, Two-Phase Commit (2PC) Protokolü ile mikroservisler arasında dağıtık işlem yönetimini ele almaktadır. Sipariş, stok ve ödeme gibi servislerin, tutarlılığı koruyarak iki aşamalı bir işlem sürecini nasıl koordine edebileceği incelenmiştir. Bu yöntemle, tüm servisler ya işlemi başarıyla tamamlar ya da tamamlanamayan bir durumda geri alınır. Protokol, dağıtık sistemlerde veri bütünlüğünü sağlamayı hedefler.

### Transaction Yönetimi
- Dağıtık işlemleri yönetmek için `Coordinator` servisi oluşturuldu.
- İlk aşamada (Prepare), tüm servislerin işlem için hazır olup olmadığı kontrol edildi.
- İkinci aşamada (Commit), tüm servislerin işlemi başarıyla tamamlaması sağlandı.

### Transaction Durumlarının Takibi
- Her bir servisin işlem durumunu takip etmek için `Node` ve `NodeState` modelleri kullanıldı.
- Servislerin hazır olup olmadıkları (`ReadyType`) ve işlem sonuçları (`TransactionState`) kaydedildi.

### Servis İletişimi
- HTTP istemcileri ile sipariş (`Order.API`), stok (`Stock.API`) ve ödeme (`Payment.API`) servisleri arasında iletişim sağlandı.
- Servislerden "hazır", "işlem tamamlandı" veya "işlem geri alındı" bilgileri alındı.

### Rollback Mekanizması
- İşlem sırasında bir hata meydana geldiğinde, tüm servislerin işlemi geri alması için `Rollback` çağrısı yapıldı.

### Veritabanı Entegrasyonu
- EF Core ile veritabanı işlemleri gerçekleştirildi ve işlem durumları kaydedildi.

### Simülasyon ve Test
- `Order.API`, `Stock.API`, ve `Payment.API` servisleri üzerinde `Ready`, `Commit`, ve `Rollback` uç noktaları ile işlem simülasyonu yapıldı.

---

## SagaPattern.ChoreographyExample

Bu çalışma, Saga Pattern - Choreography Modelini kullanarak mikroservisler arasında koordinasyonu sağlamayı amaçlamaktadır. Choreography modeli, mikroservislerin birbirleriyle doğrudan haberleştiği, bağımsız çalıştığı ve olay tabanlı bir akış ile sistemin işlediği bir yapı sunar. RabbitMQ ve MassTransit kullanılarak, sipariş, stok ve ödeme servisleri arasında mesajlaşma ve süreç yönetimi gerçekleştirilmiştir.

### Saga Pattern - Choreography Modeli
- Her mikroservis, kendi sorumluluğundaki işlemleri gerçekleştirdikten sonra ilgili diğer mikroservisleri olaylar (events) aracılığıyla bilgilendirdi.
- Merkezi bir koordinasyon bulunmaksızın olaylar üzerinden akış sağlandı.

### RabbitMQ ile Olay Tabanlı İletişim
- RabbitMQ kuyrukları, olayların (event) mikroservisler arasında taşınmasında kullanıldı.
- Mikroservisler, birbirlerinin durumlarını takip etmek için RabbitMQ'da ilgili olayları dinledi.

### Sipariş Süreci Yönetimi
- Sipariş oluşturulduktan sonra stok ve ödeme işlemleri tetiklendi.
- Stok veya ödeme işlemi başarısız olduğunda, sipariş durumu (`OrderStatus`) güncellendi.

---

## SagaPattern.OrchestrationExample

Bu çalışma, Saga Pattern - Orchestration Modelini kullanarak mikroservisler arasında koordinasyonu sağlamayı amaçlamaktadır. Orchestration modeli, bir merkezi orkestratör (state machine) aracılığıyla mikroservislerin süreçlerini yöneten bir yapı sunar. RabbitMQ, MassTransit ve Entity Framework Core gibi araçlar kullanılarak, sipariş, stok ve ödeme servisleri arasında süreçlerin başarılı bir şekilde yönetilmesi hedeflenmiştir.

### Saga Pattern - Orchestration Modeli
- Merkezi bir `State Machine` ile mikroservislerin akışı kontrol edildi.
- Sipariş süreci, stok ve ödeme işlemlerinin sıralı bir şekilde tamamlanmasıyla yönetildi.

### RabbitMQ ile Olay Yönetimi
- Olayların kuyruklar üzerinden taşınması ve ilgili servislere iletilmesi sağlandı.
- Olay tabanlı asenkron iletişim mekanizması kuruldu.

### Sipariş Süreci Yönetimi
- Sipariş oluşturulduktan sonra stok kontrolü ve ödeme işlemleri için ilgili servislerle haberleşme sağlandı.
- Başarılı işlemlerde sipariş durumu "Tamamlandı" olarak güncellendi, başarısız durumlarda "Başarısız" durumuna geçirildi.

### Hata Yönetimi ve Rollback
- Ödeme veya stok işlemlerinde hata oluştuğunda, işlemler geri alındı (rollback).
- Rollback işlemleri sırasında stok miktarları yeniden artırıldı ve ilgili servislere bilgi verildi.

---
