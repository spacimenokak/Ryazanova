# Система занятий пилатесом

REST API сервис для управления системой занятий пилатесом, разработанный на ASP.NET Core Web API.

## Технологический стек

- ASP.NET Core Web API (.NET 10.0)
- PostgreSQL
- Redis
- Docker + docker-compose
- Liquibase (миграции БД)
- Entity Framework Core
- Dapper
- FluentValidation
- Swagger (OpenAPI)
- JWT (Bearer authentication)
- Prometheus (метрики)

## Архитектура проекта

```
projectTest/
├── Controllers/          # API контроллеры
├── Services/            # Бизнес-логика
│   └── Interfaces/
├── Repositories/        # Доступ к данным
│   └── Interfaces/
├── Data/                # DbContext и конфигурация БД
├── Models/             # Модели данных
│   ├── Entities/       # Сущности БД
│   └── DTO/           # DTO для API
├── Middleware/         # Middleware компоненты
├── Validators/         # FluentValidation валидаторы
└── Liquibase/          # Миграции БД
```

## Сущности

- **User** - Пользователи системы (Admin, Manager, User)
- **Instructor** - Тренеры
- **Class** - Занятия
- **Booking** - Записи на занятия (many-to-many между User и Class)
- **Subscription** - Абонементы
- **ApiKey** - API ключи для системных клиентов

## Запуск проекта

### Требования

- Docker и Docker Compose
- .NET 10.0 SDK (для локальной разработки)

### Запуск через Docker Compose

```bash
cd project
docker-compose up -d
```

Сервисы будут доступны:
- API: http://localhost:5000
- Swagger UI: http://localhost:5000/swagger
- Health Checks: http://localhost:5000/health
- Prometheus Metrics: http://localhost:9090/metrics
- PostgreSQL: localhost:5438
- Redis: localhost:6379

### Локальный запуск

1. Запустите PostgreSQL и Redis через Docker Compose:
```bash
docker-compose up -d db redis migrations
```

2. Запустите приложение:
```bash
cd projectTest
dotnet run
```

## API Endpoints

### Аутентификация

- `POST /api/auth/login` - Вход в систему (получение JWT токена)

### Пользователи

- `GET /api/users` - Получить всех пользователей (Admin, Manager)
- `GET /api/users/{id}` - Получить пользователя по ID
- `POST /api/users` - Создать пользователя (Admin)
- `PUT /api/users/{id}` - Обновить пользователя
- `DELETE /api/users/{id}` - Удалить пользователя (Admin)

### Тренеры

- `GET /api/instructors` - Получить всех тренеров
- `GET /api/instructors/{id}` - Получить тренера по ID
- `POST /api/instructors` - Создать тренера (Admin, Manager)
- `PUT /api/instructors/{id}` - Обновить тренера (Admin, Manager)
- `DELETE /api/instructors/{id}` - Удалить тренера (Admin)

### Занятия

- `GET /api/classes?page=1&pageSize=10&search=...` - Получить занятия с пагинацией и фильтрацией
- `GET /api/classes/{id}` - Получить занятие по ID
- `POST /api/classes` - Создать занятие (Admin, Manager)
- `PUT /api/classes/{id}` - Обновить занятие (Admin, Manager)
- `DELETE /api/classes/{id}` - Удалить занятие (Admin)

### Записи на занятия

- `GET /api/bookings` - Получить все записи (Admin, Manager)
- `GET /api/bookings/{id}` - Получить запись по ID
- `GET /api/bookings/user/{userId}` - Получить записи пользователя
- `POST /api/bookings` - Создать запись (с поддержкой Idempotency-Key)
- `PUT /api/bookings/{id}` - Обновить запись
- `DELETE /api/bookings/{id}` - Удалить запись

### Абонементы

- `GET /api/subscriptions` - Получить все абонементы (Admin, Manager)
- `GET /api/subscriptions/{id}` - Получить абонемент по ID
- `GET /api/subscriptions/user/{userId}` - Получить абонементы пользователя
- `POST /api/subscriptions` - Создать абонемент (Admin, Manager)
- `PUT /api/subscriptions/{id}` - Обновить абонемент (Admin, Manager)
- `DELETE /api/subscriptions/{id}` - Удалить абонемент (Admin)

## Авторизация

### JWT Bearer Token

Для пользователей системы используется JWT Bearer Token:

```
Authorization: Bearer <token>
```

Токен содержит:
- userId
- роль (Admin, Manager, User)
- email

### API Key

Для системных клиентов используется API Key:

```
X-API-KEY: <key>
```

API ключи хранятся в БД и могут иметь срок действия.

## Матрица доступа

| Роль | Read | Create | Update | Delete |
|------|------|--------|--------|--------|
| Admin | ✅ | ✅ | ✅ | ✅ |
| Manager | ✅ | ✅ | ✅ | ❌ |
| User | ✅ | ❌ | ❌ | ❌ |

## Особенности реализации

- **EF Core** используется для CRUD операций и many-to-many связей
- **Dapper** используется для создания записей с транзакциями (BookingRepository.CreateBookingWithTransactionAsync)
- **Redis** кэширование для GET запросов занятий
- **Пагинация и фильтрация** для занятий
- **Idempotency** для POST запросов на создание записей (через заголовок Idempotency-Key)
- **Rate Limiting** - 100 запросов в минуту на IP
- **Prometheus метрики** доступны на /metrics
- **Health Checks** для API, PostgreSQL и Redis

## Тестирование

Запуск unit-тестов:

```bash
cd projectTest.Tests
dotnet test
```

## Логирование

Все запросы и ошибки логируются через ILogger. Логи структурированы и содержат:
- Входящие HTTP-запросы
- Ошибки
- Бизнес-события

## Обработка ошибок

Глобальный middleware обрабатывает все ошибки и возвращает единый формат:

```json
{
  "error": "NotFound",
  "message": "Entity not found",
  "traceId": "..."
}
```

## Swagger

Swagger UI доступен по адресу `/swagger` и содержит полную документацию API с описанием всех эндпоинтов, DTO и возможных HTTP-кодов ответа.
