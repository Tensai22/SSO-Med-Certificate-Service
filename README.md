# MedicalSertificatesSSO
System for automating the submission and verification of student medical certificates within a university SSO portal.  This project allows students to upload medical certificates online, while university staff can review, approve, or reject them. The system ensures transparency, security, and traceability of all certificate processing steps.


Commands:
Asp.Net:
# Произвести миграцию в базу данных
Add-Migration (Название миграции) -Project MedicalCertificate.Infrastructure -StartupProject MedicalCertificate.WebAPI
# Внедрение и обновление миграции в базу данных
Update-Database -Project MedicalCertificate.Infrastructure -StartupProject MedicalCertificate.WebAPI
# s3 Minio:
minio server C:\minio-data
React:
# Запуск React.js проекта
npm start server

## Configuration

### Backend (`MedicalCertificate.WebAPI/appsettings.json`)
- Укажите безопасные значения для:
  - `ConnectionStrings:DefaultConnection`
  - `JwtConfigurationOptions:Key`
  - `Minio:AccessKey`
  - `Minio:SecretKey`
- Настройте CORS в `Cors:AllowedOrigins`.

### Frontend (`med-sert-react/forcertificate-main/.env`)
- Создайте `.env` из `.env.example`.
- Укажите `REACT_APP_API_BASE_URL`.
