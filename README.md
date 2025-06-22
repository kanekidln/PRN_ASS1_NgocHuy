# Hotel System

Một hệ thống quản lý khách sạn được xây dựng trên nền tảng .NET 8 và WPF, sử dụng Entity Framework Core để tương tác với cơ sở dữ liệu.

## Yêu cầu cài đặt trước

* .NET 8 SDK
* SQL Server 2022 (hoặc mới hơn)
* Visual Studio 2022 (đề xuất)

## Thiết lập cơ sở dữ liệu

Ứng dụng sử dụng EF Core theo mô hình Code First, tự động khởi tạo và cập nhật database khi chạy lần đầu.

1. Đảm bảo SQL Server 2022 đang chạy.
2. Mở file `appsettings.json` trong cả hai project `BusinessObjects` và `huy`, kiểm tra và cập nhật chuỗi kết nối nếu cần.
3. Chuỗi kết nối mặc định:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=.;Database=hotel;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

## Chạy ứng dụng

1. Mở file solution `Huy-PRN.sln` bằng Visual Studio.
2. Đặt project `huy` làm startup project.
3. Build solution.
4. Chạy ứng dụng (F5).

Lần đầu chạy, ứng dụng sẽ:

* Tự động tạo database (nếu chưa tồn tại).
* Sinh các bảng cần thiết.
* Seeding dữ liệu mẫu ban đầu.

## Thông tin đăng nhập mẫu

* **Admin**

  * Email: `admin@FUMiniHotelSystem.com`
  * Mật khẩu: `@@abc123@@`

* **Khách hàng**

  1. `john.doe@example.com` / `password123`
  2. `jane.smith@example.com` / `password456`

## Tính năng chính

* Quản lý phòng (Rooms)
* Quản lý khách hàng (Customers)
* Đặt phòng và lịch sử booking (Bookings)
* Báo cáo thống kê (Reporting)

## Cấu trúc dự án

* **BusinessObjects**: Định nghĩa Entity Models và DbContext
* **DataAccessObjects**: Lớp truy xuất dữ liệu (DAL)
* **Repositories**: Interfaces và triển khai Repository Pattern
* **Services**: Xử lý logic nghiệp vụ (Business Logic)
* **huy**: Ứng dụng WPF (Giao diện người dùng)

## Hướng dẫn nhanh (CLI)

Trong thư mục project `huy`, bạn có thể dùng .NET CLI để tạo và áp dụng migration:

```bash
# Tạo migration
dotnet ef migrations add InitialCreate
# Cập nhật database
dotnet ef database update
# Chạy ứng dụng
dotnet run
```

Sau khi hoàn thành, bạn có thể đăng nhập và trải nghiệm đầy đủ các chức năng của hệ thống.
