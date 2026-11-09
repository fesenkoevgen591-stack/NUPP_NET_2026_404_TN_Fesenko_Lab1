# Лабораторна робота №4 · SmartHome

Проєкти використовують моделі, контекст SQLite та міграції з [Lab3](../Lab3/SmartHome.Infrastructure/). REST-контролери отримують наявний асинхронний CRUD-сервіс через впровадження залежностей. MVC-застосунок використовує контекст Lab3 у згенерованих контролерах і Razor-представленнях.

[Усі чотири лабораторні](../README.md) · [Окремий PR №4](https://github.com/fesenkoevgen591-stack/NUPP_NET_2026_404_TN_Fesenko_Lab1/pull/4)

## Збірка і запуск

Потрібен .NET 8 SDK. Виконайте команди з кореня репозиторію.

```sh
dotnet build SmartHome.sln
dotnet test SmartHome.sln
dotnet run --project Lab4/SmartHome.REST
```

Відкрийте [Swagger UI](http://localhost:5083/swagger). Приклади запитів є у [SmartHome.REST.http](SmartHome.REST/SmartHome.REST.http).

Щоб запустити додаткове завдання, відкрийте окремий термінал у корені репозиторію.

```sh
dotnet run --project Lab4/SmartHome.MVC
```

Відкрийте [вебінтерфейс SmartHome](http://localhost:5084) і перейдіть до кімнат або пристроїв.

## База даних

За наведених команд обидва застосунки використовують локальну базу `Lab4/SmartHome.Lab4.db` і застосовують до неї міграції Lab3. Під час першого запуску дочекайтеся, поки запуститься один застосунок, і тоді запускайте другий.

Щоб переглянути записи з Lab3, після завершення її роботи скопіюйте `Lab3/SmartHome.Infrastructure/SmartHome.db` в окремий файл. У кожному терміналі перед запуском задайте `ConnectionStrings__SmartHome` зі значенням `Data Source=<абсолютний шлях до копії>;Foreign Keys=True`. Приклад для PowerShell.

```powershell
$env:ConnectionStrings__SmartHome = 'Data Source=C:/шлях/до/копії/SmartHome.db;Foreign Keys=True'
```

## REST API

| Метод | Кімнати | Пристрої | Успішна відповідь |
| --- | --- | --- | --- |
| GET | `/api/rooms` | `/api/devices` | `200 OK`, список |
| GET | `/api/rooms/{id}` | `/api/devices/{id}` | `200 OK`, запис |
| POST | `/api/rooms` | `/api/devices` | `201 Created`, запис і заголовок `Location` |
| PUT | `/api/rooms/{id}` | `/api/devices/{id}` | `204 No Content` |
| DELETE | `/api/rooms/{id}` | `/api/devices/{id}` | `204 No Content` |

Для створення та оновлення кімнати потрібні `name` і `floor`, а для пристрою — `name`, `isOnline` і `roomId` наявної кімнати. Списки можна запитати без параметрів або передати обидва параметри пагінації, наприклад `?page=1&amount=10`. Нумерація починається з 1, розмір сторінки обмежено 100 записами. Невалідні дані повертають `400 Bad Request`, відсутній запис за ідентифікатором — `404 Not Found`. За схемою Lab3 видалення кімнати каскадно видаляє її пристрої.

[Звіт із результатами перевірки та відповідями на контрольні запитання](Lab4_Report.pdf)
