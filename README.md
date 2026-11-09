# Лабораторні роботи 1–4 · SmartHome

Чотири лабораторні роботи зібрано в одному рішенні [SmartHome.sln](SmartHome.sln). У кожної роботи є власна папка, а Visual Studio показує проєкти за тими самими групами. Для збірки потрібен .NET 8 SDK.

| Робота | Тема | Матеріали |
| --- | --- | --- |
| [Lab1](#lab1) | Моделі SmartHome і синхронний CRUD | [Папка Lab1](Lab1/) · [Звіт PDF](Lab1/Lab1_Report.pdf) |
| [Lab2](#lab2) | Асинхронний CRUD, LINQ і паралельна обробка | [Папка Lab2](Lab2/) · [Звіт PDF](Lab2/Lab2_Report.pdf) |
| [Lab3](#lab3) | EF Core, SQLite і міграції | [Папка Lab3](Lab3/) · [Інфраструктура](Lab3/SmartHome.Infrastructure/) |
| [Lab4](#lab4) | ASP.NET Core REST API, Swagger і MVC | [Папка Lab4](Lab4/) · [Звіт PDF](Lab4/Lab4_Report.pdf) · [PR №4](https://github.com/fesenkoevgen591-stack/NUPP_NET_2026_404_TN_Fesenko_Lab1/pull/4) |

Спільні моделі та сервіси CRUD містяться в [Shared/SmartHome.Common](Shared/SmartHome.Common/).

## Збірка і тести

Виконайте з кореня репозиторію.

```sh
dotnet build SmartHome.sln
dotnet test SmartHome.sln
```

## Lab1

Консольний застосунок демонструє роботу з кімнатою та пристроями SmartHome, подію зміни стану, синхронні CRUD-операції та збереження ламп у JSON.

З кореня репозиторію перейдіть до `Lab1` і запустіть застосунок. Він оновить [lamps.json](Lab1/lamps.json) у цій папці.

```sh
cd Lab1
dotnet run --project SmartHome.Console
cd ..
```

[Початковий PR №1](https://github.com/fesenkoevgen591-stack/NUPP_NET_2026_404_TN_Fesenko_Lab1/pull/1)

## Lab2

Застосунок демонструє асинхронні CRUD-операції на прикладах ламп і автобусів, паралельне створення об’єктів, статистику LINQ, пагінацію, синхронізацію потоків і збереження в JSON.

```sh
dotnet run --project Lab2/Lab2_NET
```

Сім [тестів xUnit](Lab2/SmartHome.Tests/) покривають асинхронний CRUD для ламп, повторні ідентифікатори, пагінацію та збереження в JSON. Окремо збережено [результати запуску](Lab2/Lab2_Results.txt) та [журнал тестів](Lab2/Lab2_Tests.txt).

Під час запуску Lab2 записує `lamps_lab2.json` і `buses.json` у папку збірки `Lab2/Lab2_NET/bin`.

[Початковий PR №2](https://github.com/fesenkoevgen591-stack/NUPP_NET_2026_404_TN_Fesenko_Lab1/pull/2)

## Lab3

Застосунок використовує EF Core і SQLite та демонструє асинхронні CRUD-операції через репозиторій. Моделі охоплюють зв’язки 1:N між кімнатою та пристроями, 1:1 між пристроєм і налаштуваннями та N:M між пристроями й тегами.

```sh
dotnet run --project Lab3/SmartHome.Lab3
```

Запускайте з кореня репозиторію. Застосунок використовує [SmartHome.db](Lab3/SmartHome.Infrastructure/SmartHome.db), застосовує наявні [міграції](Lab3/SmartHome.Infrastructure/Migrations/) і додає демонстраційні записи під час запуску.

[Початковий PR №3](https://github.com/fesenkoevgen591-stack/NUPP_NET_2026_404_TN_Fesenko_Lab1/pull/3)

## Lab4

REST API і вебінтерфейс MVC дають змогу створювати, переглядати, редагувати й видаляти кімнати та пристрої. Обидва застосунки використовують ті самі моделі, контекст і міграції [Lab3](Lab3/SmartHome.Infrastructure/).

З кореня репозиторію запустіть REST API та відкрийте [Swagger](http://localhost:5083/swagger).

```sh
dotnet run --project Lab4/SmartHome.REST
```

Коли REST API запуститься, відкрийте другий термінал у корені репозиторію. Запустіть MVC і відкрийте [вебінтерфейс](http://localhost:5084).

```sh
dotnet run --project Lab4/SmartHome.MVC
```

REST і MVC спільно використовують локальну базу `Lab4/SmartHome.Lab4.db`. Щоб переглянути в Lab4 записи, створені консоллю Lab3, можна підключити копію бази Lab3 за інструкцією в [README Lab4](Lab4/README.md#база-даних).

Результати запитів можна переглянути на GitHub у [PDF-звіті Lab4](Lab4/Lab4_Report.pdf). Зміни четвертої роботи зібрано в [окремому PR №4](https://github.com/fesenkoevgen591-stack/NUPP_NET_2026_404_TN_Fesenko_Lab1/pull/4).
