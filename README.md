# MedSpravka (MedicalSertificatesSSO)

Система для подачи, проверки и обработки медицинских справок студентов в рамках университетского портала.

## Что умеет система

- **Студент**
  - Авторизация в системе
  - Загрузка справки в форматах PDF/PNG/JPG/JPEG (до 10 МБ)
  - Отправка заявки на обработку
  - Просмотр статуса своих заявок
- **Регистратура (Office Registrar)**
  - Просмотр входящих заявок
  - Фильтрация, поиск, просмотр вложений
  - Подтверждение или отклонение справки с комментарием
  - Просмотр истории обработанных заявок

## Архитектура

- `MedicalCertificate.WebAPI` — ASP.NET Core Web API (контроллеры, JWT, middleware, Swagger)
- `MedicalCertificate.Application` — CQRS/бизнес-логика (MediatR, DTO, команды/запросы)
- `MedicalCertificate.Domain` — доменные сущности, константы, опции
- `MedicalCertificate.Infrastructure` — EF Core, PostgreSQL, репозитории, MinIO, JWT provider
- `med-sert-react/forcertificate-main` — React-клиент

## Технологии

- Backend: **.NET 8**, ASP.NET Core, EF Core, MediatR, JWT, Npgsql
- Frontend: **React 19**, React Router, Axios, React Toastify
- База данных: **PostgreSQL**
- Файловое хранилище: **MinIO (S3-совместимое)**

## Роли и статусы

- Роли (seed):
  - `1` — `Office Registrar`
  - `2` — `Student`
- Статусы справки:
  - `1` — В обработке (`Pending`)
  - `2` — Принято (`Approved`)
  - `3` — Отклонено (`Rejected`)

## Требования

- .NET SDK 8.0+
- Node.js 18+ и npm
- PostgreSQL 14+
- MinIO

## Быстрый старт

### 1. Настроить backend-конфиг

Файл: `MedicalCertificate.WebAPI\appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=medsert;Username=postgres;Password=your_password"
  },
  "JwtConfigurationOptions": {
    "Key": "your-very-strong-secret-key-min-32-chars",
    "Issuer": "MedSpravka",
    "Audience": "MedSpravkaClient",
    "ExpirationHours": 2
  },
  "Minio": {
    "Endpoint": "localhost:9000",
    "AccessKey": "your_access_key",
    "SecretKey": "your_secret_key",
    "BucketName": "medical-files",
    "UseSSL": false
  },
  "Cors": {
    "AllowedOrigins": [ "http://localhost:3000" ]
  }
}
```

### 2. Запустить PostgreSQL и MinIO

Пример запуска MinIO на Windows:

```powershell
minio server C:\minio-data --console-address ":9001"
```

### 3. Применить миграции БД

Из корня проекта:

```powershell
dotnet restore MedicalSertificate.sln
dotnet ef database update --project MedicalCertificate.Infrastructure --startup-project MedicalCertificate.WebAPI
```

Если `dotnet ef` не установлен:

```powershell
dotnet tool install --global dotnet-ef
```

### 4. Запустить backend

```powershell
dotnet run --project MedicalCertificate.WebAPI
```

- API: `http://localhost:5280`
- Swagger: `http://localhost:5280/swagger`

### 5. Настроить и запустить frontend

```powershell
cd med-sert-react\forcertificate-main
copy .env.example .env
npm install
npm start
```

`.env.example`:

```env
REACT_APP_API_BASE_URL=http://localhost:5280/api
```

Frontend по умолчанию: `http://localhost:3000`

## Основные API endpoints

### Auth

| Метод | Endpoint | Доступ | Описание |
|---|---|---|---|
| POST | `/api/Auth/register` | Анонимный | Регистрация |
| POST | `/api/Auth/login` | Анонимный | Логин, выдача JWT |
| GET | `/api/Auth/me` | Авторизованный | Данные текущего пользователя |
| POST | `/api/Auth/logout` | Авторизованный | Выход |

### Certificates

| Метод | Endpoint | Доступ | Описание |
|---|---|---|---|
| GET | `/api/Certificate` | Только регистратура | Все справки (опц. `statusId`) |
| GET | `/api/Certificate/{id}` | Авторизованный | Справка по ID |
| GET | `/api/Certificate/user/{userId}` | Регистратура или владелец | Справки пользователя |
| GET | `/api/Certificate/{id}/history` | Только регистратура | История статусов |
| POST | `/api/Certificate` | Авторизованный | Создать справку |
| PUT | `/api/Certificate/{id}` | Только регистратура | Обновить справку |
| POST | `/api/Certificate/{id}/approve` | Только регистратура | Подтвердить справку |
| POST | `/api/Certificate/{id}/reject` | Только регистратура | Отклонить справку |
| DELETE | `/api/Certificate/{id}` | Только регистратура | Удалить справку |

### Files

| Метод | Endpoint | Доступ | Описание |
|---|---|---|---|
| POST | `/api/File/upload` | Авторизованный | Загрузка PDF/PNG/JPG/JPEG (до 10 МБ) |
| GET | `/api/File/{id}` | Регистратура или владелец | Скачать/просмотреть файл |
| GET | `/api/File/download/{fileName}` | Только регистратура | Скачивание по имени |
| DELETE | `/api/File/delete/{fileName}` | Только регистратура | Пометить файл удалённым |

### Users

Все endpoints `UserController` доступны только роли регистратуры:

- `GET /api/User`
- `GET /api/User/{id}`
- `GET /api/User/email/{email}`
- `POST /api/User`
- `PUT /api/User/{id}`
- `DELETE /api/User/{id}`

## UI-маршруты фронтенда

- `/login` — страница входа
- `/sertificate` — кабинет студента
- `/registrar` — кабинет регистратуры

## Миграции через Package Manager Console (Visual Studio)

```powershell
Add-Migration <MigrationName> -Project MedicalCertificate.Infrastructure -StartupProject MedicalCertificate.WebAPI
Update-Database -Project MedicalCertificate.Infrastructure -StartupProject MedicalCertificate.WebAPI
```
