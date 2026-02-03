# PriceMonitorService
Технологии
ASP.NET Core 8.0 - веб-фреймворк
Entity Framework Core - ORM для работы с базой данных
SQLite - встраиваемая база данных
MailKit - отправка электронной почты
Docker - контейнеризация

Эндпоинты
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
