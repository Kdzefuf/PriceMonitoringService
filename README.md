# PriceMonitorService
## Технологии

- ASP.NET Core 8.0 - веб-фреймворк
- Entity Framework Core - ORM для работы с базой данных
- SQLite - встраиваемая база данных
- MailKit - отправка электронной почты
- Docker - контейнеризация

## Запуск приложения

Сборка docker-образа с помощью:
```
docker build -t pricemonitorservice -f PriceMonitorService/Dockerfile .
```
Запуск контейнера:
```
docker run -p 8080:8080 --name price-monitor -e ASPNETCORE_ENVIRONMENT=Development -e ASPNETCORE_HTTP_PORTS=8080 -v ${PWD}//app/data pricemonitorservice
```

## Эндпоинты
1. Подписка на отслеживание цены
POST /api/subscribe
Подписывает пользователя на отслеживание цены квартиры.
Request:
```
{
  "listingUrl": "https://prinzip.su/apartments/shartashpark/65056/",
  "email": "user@example.com"
}
```
Response (200 OK):
```
{
  "message": "Подписка успешно создана",
  "subscriptionId": 1,
  "currentPrice": 490644000,
  "apartmentUrl": "https://prinzip.su/apartments/shartashpark/65056/"
}
```
Response (400 Bad Request):
```
{
  "message": "Подписка на этот объект уже существует"
}
```
2. Получение актуальных цен
GET /api/price/current
Возвращает текущие цены для всех активных подписок.
Response (200 OK):
```
[
  {
    "listingUrl": "https://prinzip.su/apartments/shartashpark/65056/",
    "currentPrice": 4906440,
    "previousPrice": 5000000,
    "priceChanged": true,
    "lastChecked": "2026-02-03T09:13:49.208207Z"
  }
]
```
3. История цен
GET /api/price/history/{subscriptionId}
Возвращает историю цен для конкретной подписки.
Response (200 OK):
```

  {
    "id": 3,
    "subscriptionId": 1,
    "price": 490644000,
    "recordedAt": "2026-02-03T09:08:46.1777176"
  },
  {
    "id": 2,
    "subscriptionId": 1,
    "price": 500000000,
    "recordedAt": "2026-02-02T18:43:17.8650738"
  }
]
```
## Слои архитектуры:
1. Слой представления

  Контроллеры SubscribeController для управления подписками пользователей и PriceController для получения информации о ценах
  
  Обрабатывают входящие запросы HTTP-запросы и формируют ответы.

3. Бизнес-логика
  Сервисы:
  - PriceMonitoringService для проверки и получение цен

    В нем реализованы методы:
      - CheckAllPricesAsync() - проверка цен всех активных подписок
      - CheckPriceForSubscriptionAsync() - проверка цены для конкретной подписки
      - GetCurrentPriceAsync() - получение текущей цены по URL
      - GetCurrentPriceInfoForSubscriptionAsync() - получение информации о цене для существующей подписки

    Сервис получает список всех активных подписок из репозитория
    Для каждой подписки:
      - Запрашивает текущую цену через ApartmentPriceService
      - Получает последнюю записанную цену из базы данных
      - Сравнивает цены
      - Если цена изменилась:
        * Сохраняет новую цену в базу данных
        * Отправляет уведомление на почту через EmailNotificationService
 
  - ApartmentPriceService получение актуальных цен с сайта prinzip.su через публичное API

    Метод: GetPriceFromListingAsync(string listingUrl) - получение цены по ссылке на квартиру
      1. Извлекает ID квартиры из URL (например, из "https://prinzip.su/apartments/shartashpark/65056/" → "65056")
      2. Формирует запрос к API: "https://prinzip.su/api/v1/public/apartments/{id}" и выполняет HTTP-запрос
      3. Извлекает цену из поля "price"
 
  - EmailNotificationService - отправка email-уведомлений о изменении цен
  
    Метод SendPriceChangeNotificationAsync(string email, string listingUrl, decimal oldPrice, decimal newPrice)
      1. Формирует HTML-письмо с информацией об изменении цены
      2. Подключается к локальному SMTP-серверу (проверял с помощью Papercut SMTP)
      3. Отправляет письмо получателю
      4. Отключается от сервера

  - PriceMonitoringBackgroundService - автоматическая проверка цен с периодом в 1 час
  
    В бесконечном цикле:
      - Вызывает PriceMonitoringService.CheckAllPricesAsync()
      - Ждет интервал времени
      - Повторяет проверку

3. Доступ к данным

  - AppDbContext - контекст базы данных

    Сущности: Subscriptions - таблица подписок, PriceRecords - таблица истории цен
    
  - SubscriptionRepository - операции с подписками

    Методы:
      - AddAsync() - добавление новой подписки
      - GetByIdAsync() - получение подписки по ID
      - GetByUrlAsync() - получение подписки по URL квартиры
      - GetAllActiveAsync() - получение всех активных подписок

  - PriceRepository - опреации с записями цен

    Методы:
      - AddPriceRecordAsync() - добавление новой записи о цене
      - GetLatestPriceAsync() - получение последней цены
      - GetPreviousPriceAsync() - получение предыдущей цены
      - GetPriceHistoryAsync() - получение полной истории цен

Модели данных:
- Subscription состоит из id, url квартиры, email для уведомлений и даты создания
- PriceRecord - id записи, id подписки, цена квартиры, дата записи
- ListingPriceInfo - url квартиры, текущая цена, предыдущая цена, дата последней проверки

Возможные улучшения:
- Реализовать функции изменения и удаления подписок
- Добавление интерфейса приложения
- Добавление тестирования приложения
- Реализация работы для нескольких сайтов

Проблемы:
- Не смог првоерить автматическую рассылку писем при изменении цены на сайте, изменял цены в бд и перезапускал приложение, в таком случае письмо отправлялось.
