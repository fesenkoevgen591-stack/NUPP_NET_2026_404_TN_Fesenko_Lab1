# Лабораторні роботи з .NET

Три етапи проєкту SmartHome зібрано в одному рішенні [SmartHome.sln](SmartHome.sln). Для збірки потрібен .NET 8 SDK.

| Робота | Тема | Матеріали |
| --- | --- | --- |
| [Lab1](#lab1) | Моделі SmartHome і синхронний CRUD | [Код](Lab1/SmartHome.Console/) · [Звіт PDF](Lab1/Lab1_Report.pdf) |
| [Lab2](#lab2) | Асинхронний CRUD, LINQ і паралельна обробка | [Код](Lab2/Lab2_NET/) · [Звіт PDF](Lab2/Lab2_Report.pdf) |
| [Lab3](#lab3) | EF Core, SQLite і міграції | [Код](Lab3/SmartHome.Lab3/) · [Інфраструктура](Lab3/SmartHome.Infrastructure/) |

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

[Початковий PR №2](https://github.com/fesenkoevgen591-stack/NUPP_NET_2026_404_TN_Fesenko_Lab1/pull/2)

## Lab3

Застосунок використовує EF Core і SQLite та демонструє асинхронні CRUD-операції через репозиторій. Моделі охоплюють зв’язки 1:N між кімнатою та пристроями, 1:1 між пристроєм і налаштуваннями та N:M між пристроями й тегами.

```sh
dotnet run --project Lab3/SmartHome.Lab3
```

Запускайте з кореня репозиторію. Застосунок використовує [SmartHome.db](Lab3/SmartHome.Infrastructure/SmartHome.db), застосовує наявні [міграції](Lab3/SmartHome.Infrastructure/Migrations/) і додає демонстраційні записи під час запуску.

[Початковий PR №3](https://github.com/fesenkoevgen591-stack/NUPP_NET_2026_404_TN_Fesenko_Lab1/pull/3)
