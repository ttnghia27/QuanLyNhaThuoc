# Tài liệu API - Quản lý nhà thuốc

Tài liệu này mô tả các endpoint hiện có trong `QuanLyNhaThuoc.API/Controllers`.

## Thông tin chung

- Base URL khi chạy theo cấu hình Development: `http://localhost:5238`
- HTTPS: `https://localhost:7212`
- Tất cả route bắt đầu bằng `/api`; tên route không phân biệt chữ hoa/chữ thường.
- Request và response dùng JSON. Ví dụ bên dưới dùng camelCase; ASP.NET Core cũng nhận tên thuộc tính không phân biệt chữ hoa/chữ thường.
- Ngày giờ gửi lên nên dùng ISO 8601, ví dụ `2026-11-18` hoặc `2026-11-18T10:30:00`.
- Các mã (`maThuoc`, `maKH`, ...) được server tự sinh khi tạo dữ liệu, trừ khi API ghi rõ khác.
- Chưa có `[Authorize]` trên các controller. Dù login phát hành JWT và Swagger cấu hình Bearer, các endpoint hiện tại không bắt buộc gửi token.
- CORS hiện cho phép mọi origin, header và method.

## Xác thực

### `POST /api/Auth/login`

Đăng nhập và nhận JWT. Token có hiệu lực 8 giờ.

Request:

```json
{
  "tenDangNhap": "admin",
  "matKhau": "your-password"
}
```

Thành công `200 OK`:

```json
{
  "token": "<jwt>",
  "hoTen": "Nguyễn Văn A"
}
```

Sai thông tin `401 Unauthorized`:

```json
{
  "message": "Sai tên đăng nhập hoặc mật khẩu"
}
```

## Danh mục thuốc

### `GET /api/LoaiThuoc`

Lấy danh sách loại thuốc:

```json
[
  {
    "maLoai": "LT001",
    "tenLoai": "Thuốc giảm đau",
    "soLuongThuoc": 12
  }
]
```

### `GET /api/LoaiThuoc/{id}`

Lấy một loại theo `maLoai`. Trả về `maLoai`, `tenLoai`; không tìm thấy trả `404` với `message`.

### `POST /api/LoaiThuoc`

Tạo loại thuốc. `maLoai` do server sinh.

```json
{ "tenLoai": "Thuốc giảm đau" }
```

Thành công `200 OK`, trả về entity loại thuốc (`maLoai`, `tenLoai`).

### `PUT /api/LoaiThuoc/{id}`

Cập nhật tên loại. Body giống `POST`. Không tìm thấy trả `404`.

### `DELETE /api/LoaiThuoc/{id}`

Xóa loại thuốc. Thành công:

```json
{ "message": "Xóa thành công" }
```

Trả `404` nếu không tìm thấy; `400` nếu loại đang có thuốc trực thuộc.

### `GET /api/Thuoc`

Lấy danh sách thuốc. Query tùy chọn:

| Tham số | Ý nghĩa |
|---|---|
| `search` | Tìm gần đúng theo tên thuốc, mã thuốc hoặc công dụng |
| `maLoai` | Lọc chính xác theo mã loại |

Ví dụ: `/api/Thuoc?search=para&maLoai=LT001`

Mỗi phần tử gồm `maThuoc`, `tenThuoc`, `congDung`, `donGiaBan`, `soLuongTonKho`, `maLoai`, `tenLoai`.

### `GET /api/Thuoc/{id}`

Lấy thông tin thuốc theo mã, kèm loại thuốc và các lô còn tồn (`soLuongCon > 0`). Không tìm thấy trả `404`.

### `POST /api/Thuoc`

Tạo thuốc. Tồn kho ban đầu tự đặt bằng 0; kho tăng khi tạo phiếu nhập.

```json
{
  "tenThuoc": "Paracetamol 500mg",
  "congDung": "Hạ sốt, giảm đau",
  "donGiaBan": 2500,
  "maLoai": "LT001"
}
```

Thành công `200 OK`, trả về entity thuốc. `maThuoc` và `soLuongTonKho` do server thiết lập. `maLoai` không tồn tại trả `400`.

### `PUT /api/Thuoc/{id}`

Cập nhật `tenThuoc`, `congDung`, `donGiaBan`, `maLoai`; body giống `POST`. Không tìm thấy thuốc trả `404`; mã loại không tồn tại trả `400`. Tồn kho không được cập nhật qua endpoint này.

### `DELETE /api/Thuoc/{id}`

Xóa thuốc. Thành công trả `{ "message": "Xóa thành công" }`. Trả `404` nếu không tìm thấy; `400` nếu thuốc đã có lịch sử nhập lô.

## Khách hàng

### `GET /api/KhachHang`

Lấy danh sách khách hàng. Query `search` tùy chọn, tìm theo tên hoặc số điện thoại:

`/api/KhachHang?search=0901`

Mỗi phần tử là entity khách hàng: `maKH`, `tenKH`, `sdtKH`, `diaChiKH`.

### `GET /api/KhachHang/{id}`

Lấy khách hàng theo mã, kèm danh sách hóa đơn liên quan. Không tìm thấy trả `404`.

### `POST /api/KhachHang`

```json
{
  "tenKH": "Nguyễn Văn A",
  "sdtKH": "0901234567",
  "diaChiKH": "Quận 1, TP.HCM"
}
```

`tenKH` bắt buộc; `sdtKH`, `diaChiKH` có thể bỏ qua hoặc gửi `null`. Mã khách hàng do server sinh. Thành công trả entity khách hàng (`200 OK`).

### `PUT /api/KhachHang/{id}`

Cập nhật các trường thông tin khách hàng. Body giống `POST`. Không tìm thấy trả `404`.

### `DELETE /api/KhachHang/{id}`

Xóa khách hàng. Thành công trả `{ "message": "Xóa thành công" }`. Trả `404` nếu không tìm thấy; `400` nếu khách hàng đã có hóa đơn.

## Nhà cung cấp

### `GET /api/NhaCC`

Lấy danh sách nhà cung cấp. Mỗi phần tử gồm `maNhaCC`, `tenNhaCC`, `sdtNhaCC`.

### `GET /api/NhaCC/{id}`

Lấy nhà cung cấp theo mã. Không tìm thấy trả `404`.

### `POST /api/NhaCC`

```json
{
  "tenNhaCC": "Công ty Dược ABC",
  "sdtNhaCC": "0281234567"
}
```

`tenNhaCC` bắt buộc; `sdtNhaCC` tùy chọn. Mã nhà cung cấp do server sinh. Thành công trả entity nhà cung cấp (`200 OK`).

### `PUT /api/NhaCC/{id}`

Cập nhật tên và số điện thoại. Body giống `POST`. Không tìm thấy trả `404`.

### `DELETE /api/NhaCC/{id}`

Xóa nhà cung cấp. Thành công trả `{ "message": "Xóa thành công" }`. Trả `404` nếu không tìm thấy; `400` nếu nhà cung cấp đã có phiếu nhập.

## Lô thuốc và cảnh báo

### `GET /api/LoThuoc`

Lấy danh sách lô, sắp xếp theo hạn sử dụng. Query tùy chọn:

| Tham số | Mặc định | Ý nghĩa |
|---|---:|---|
| `maThuoc` | — | Lọc chính xác theo mã thuốc |
| `conHangOnly` | `false` | Nếu `true`, chỉ lấy lô có `soLuongCon > 0` |

Mỗi phần tử gồm `maLo`, `maThuoc`, `tenThuoc`, `maPhieuNhap`, `ngayNhapKho`, `hanSuDung`, `soLuongNhap`, `soLuongCon`, `donGiaNhap`, `daHetHan`.

### `PUT /api/LoThuoc/{maLo}/tieu-huy`

Tiêu hủy toàn bộ số còn lại của lô và trừ số đó khỏi tồn kho thuốc. Không cần body.

Thành công:

```json
{ "message": "Đã tiêu hủy 5 sản phẩm từ lô LO001" }
```

Trả `404` nếu không tìm thấy lô; `400` nếu lô đã hết số lượng tồn. Endpoint hiện không kiểm tra lô đã quá hạn trước khi tiêu hủy.

### `GET /api/CanhBao/sap-het-han`

Lấy lô còn hàng có hạn sử dụng trước hoặc bằng ngày cảnh báo, sắp theo hạn gần nhất. Query `soNgay` mặc định `30`; ngày mốc tính từ hôm nay cộng số ngày này. Mỗi phần tử gồm `maLo`, `maThuoc`, `tenThuoc`, `hanSuDung`, `soNgayConLai`, `soLuongCon`.

Ví dụ: `/api/CanhBao/sap-het-han?soNgay=14`

## Phiếu nhập

### `GET /api/PhieuNhap`

Danh sách phiếu nhập, mới nhất trước. Mỗi phần tử gồm `maPhieuNhap`, `ngayNhap`, `tongTien`, `maNhaCC`, `tenNhaCC`.

### `GET /api/PhieuNhap/{id}`

Chi tiết phiếu nhập theo mã, gồm thông tin phiếu và `chiTietLo`. Mỗi lô chi tiết gồm `maLo`, `maThuoc`, `tenThuoc`, `hanSuDung`, `soLuongNhap`, `soLuongCon`, `donGiaNhap`, `thanhTien`.

Không tìm thấy trả `404`.

### `POST /api/PhieuNhap`

Tạo phiếu nhập và tự tạo một lô cho từng dòng chi tiết. Ngày nhập mặc định hôm nay. Mã phiếu và mã lô do server sinh; tồn kho thuốc được tăng tương ứng.

```json
{
  "maNhaCC": "NCC001",
  "ngayNhap": "2026-11-18",
  "chiTiet": [
    {
      "maThuoc": "T001",
      "soLuongNhap": 100,
      "donGiaNhap": 1200,
      "hanSuDung": "2027-11-18"
    }
  ]
}
```

`ngayNhap` có thể là `null` hoặc bỏ qua. Mỗi dòng cần `maThuoc`, `soLuongNhap`, `donGiaNhap`, `hanSuDung`. Thành công trả entity phiếu nhập (`200 OK`), gồm `maPhieuNhap`, `ngayNhap`, `tongTien`, `maNhaCC`.

Lỗi nghiệp vụ/ghi dữ liệu được trả `400` dạng `{ "message": "..." }`. Tổng tiền được tính bằng tổng `soLuongNhap * donGiaNhap` của các dòng.

## Hóa đơn

### `GET /api/HoaDon`

Danh sách hóa đơn, mới nhất trước. Mỗi phần tử gồm `maHD`, `ngayLapHD`, `tongTien`, `maKH`, `tenKH`.

### `GET /api/HoaDon/{id}`

Chi tiết hóa đơn theo mã, gồm `maHD`, `ngayLapHD`, `tongTien`, `maKH`, `tenKH` và `chiTiet`. Mỗi dòng chi tiết gồm `maLo`, `maThuoc`, `tenThuoc`, `hanSuDung`, `soLuongBan`, `giaBan`, `thanhTien`.

Không tìm thấy trả `404`.

### `POST /api/HoaDon`

Tạo hóa đơn. Giá bán lấy theo giá hiện tại của thuốc. Hệ thống tự chọn lô chưa hết hạn, còn tồn, ưu tiên hạn dùng gần nhất (FEFO); có thể lấy số lượng từ nhiều lô.

```json
{
  "maKH": "KH001",
  "chiTiet": [
    { "maThuoc": "T001", "soLuongBan": 2 },
    { "maThuoc": "T002", "soLuongBan": 1 }
  ]
}
```

`chiTiet` phải có ít nhất một dòng; tổng số lượng bán của mỗi thuốc phải lớn hơn 0. Thành công trả entity hóa đơn (`200 OK`), gồm `maHD`, `ngayLapHD`, `tongTien`, `maKH`.

Lỗi nghiệp vụ trả `400` dạng `{ "message": "..." }`, ví dụ khách hàng không tồn tại, thuốc hết hàng/chỉ còn lô quá hạn, hoặc tồn khả dụng không đủ. Tạo hóa đơn cập nhật tồn kho và số lượng tồn từng lô.

## Thống kê

### `GET /api/ThongKe/tong-quan`

Không cần query. Response:

```json
{
  "doanhThuHomNay": 0,
  "tongDoanhThu": 0,
  "tongTienNhap": 0,
  "loiNhuanUocTinh": 0,
  "soHoaDon": 0,
  "soLoaiThuoc": 0,
  "thuocSapHetHang": 0,
  "loSapHetHan": 0
}
```

`soLoaiThuoc` trong implementation hiện đếm bản ghi thuốc (không phải danh mục loại thuốc). `thuocSapHetHang` đếm thuốc có tồn kho `<= 10`; `loSapHetHan` đếm lô còn hàng có hạn sử dụng trong vòng 30 ngày tới hoặc đã quá hạn.

### `GET /api/ThongKe/doanh-thu`

Thống kê doanh thu và số hóa đơn theo ngày. Query tùy chọn:

| Tham số | Mặc định |
|---|---|
| `tuNgay` | 7 ngày trước, tính từ đầu ngày |
| `denNgay` | Hôm nay, tính hết ngày |

Ví dụ: `/api/ThongKe/doanh-thu?tuNgay=2026-11-01&denNgay=2026-11-18`

Response là mảng:

```json
[
  {
    "ngay": "2026-11-18T00:00:00",
    "soHoaDon": 3,
    "doanhThu": 125000
  }
]
```

Ngày không có hóa đơn sẽ không xuất hiện trong mảng.

## Quy ước lỗi

- `400 Bad Request`: lỗi nghiệp vụ hoặc dữ liệu không hợp lệ; các lỗi được controller xử lý thường có dạng `{ "message": "..." }`.
- `404 Not Found`: không tìm thấy bản ghi theo mã; response có dạng `{ "message": "..." }`.
- `401 Unauthorized`: hiện dùng cho đăng nhập sai. Các endpoint còn lại chưa gắn yêu cầu JWT.
- Với lỗi binding/validation tự động của `[ApiController]`, ASP.NET Core có thể trả `400` dạng `ValidationProblemDetails` thay vì đối tượng `message`.
