# HƯỚNG DẪN SỬ DỤNG CÁC FUNCTION VÀ STORED PROCEDURE

**Cơ sở dữ liệu:** `QuanLyNhaThuoc`

Tài liệu này tổng hợp toàn bộ các Hàm (Functions), Trigger và Thủ tục (Stored Procedures) được thiết lập trong cơ sở dữ liệu Quản Lý Nhà Thuốc, đi kèm mô tả, danh sách tham số và ví dụ câu lệnh gọi cụ thể.

---

## 📌 MỤC LỤC

1. [Các Hàm Sinh Mã Tự Động (Scalar Functions)](#1-các-hàm-sinh-mã-tự-động)
2. [Các Hàm Nghiệp Vụ & Tính Toán](#2-các-hàm-nghiệp-vụ--tính-toán)
3. [Tóm Tắt Các Trigger Tự Động (Triggers)](#3-các-trigger-xử-lý-tự-động)
4. [Các Stored Procedure Nghiệp Vụ & Thống Kê](#4-các-stored-procedure-nghiệp-vụ--thống-kê)

---

## 1. CÁC HÀM SINH MÃ TỰ ĐỘNG

Các hàm này không nhận tham số truyền vào và đều trả về kiểu `NVARCHAR`. Chuỗi mã trả về gồm tiền tố (Prefix) + 3 chữ số tự động tăng (ví dụ: `001`, `002`).

| Tên Hàm                    | Mô Tả                              | Định Dạng Trả Về        | Bảng Tham Chiếu |
| :------------------------- | :--------------------------------- | :---------------------- | :-------------- |
| `dbo.fn_SinhMaLoai()`      | Sinh mã loại thuốc tiếp theo       | `LTxxx` (vd: `LT001`)   | `LOAITHUOC`     |
| `dbo.fn_SinhMaThuoc()`     | Sinh mã thuốc tiếp theo            | `Txxx` (vd: `T001`)     | `THUOC`         |
| `dbo.fn_SinhMaNhaCC()`     | Sinh mã nhà cung cấp tiếp theo     | `NCCxxx` (vd: `NCC001`) | `NHACC`         |
| `dbo.fn_SinhMaPhieuNhap()` | Sinh mã phiếu nhập tiếp theo       | `PNxxx` (vd: `PN001`)   | `PHIEUNHAP`     |
| `dbo.fn_SinhMaLo()`        | Sinh mã lô thuốc tiếp theo         | `LOxxx` (vd: `LO001`)   | `LOTHUC`        |
| `dbo.fn_SinhMaKH()`        | Sinh mã khách hàng tiếp theo       | `KHxxx` (vd: `KH001`)   | `KHACHHANG`     |
| `dbo.fn_SinhMaHD()`        | Sinh mã hóa đơn bán hàng tiếp theo | `HDxxx` (vd: `HD001`)   | `HOADON`        |

### 💡 Cú pháp sử dụng: SQL EXPRESS 2022

---

## 2. CÁC HÀM NGHIỆP VỤ & TÍNH TOÁN

### 2.1. `dbo.fn_TinhTonKho`

Tính tổng số lượng còn tồn kho thực tế của một mặt hàng thuốc từ tất cả các lô thuốc còn hàng (`SoLuongCon > 0`).

- **Tham số:**
    - `@MaThuoc` (`NVARCHAR(10)`): Mã thuốc cần kiểm tra tồn kho.
- **Kiểu trả về:** `INT` (Số lượng tồn kho tổng cộng).

```sql
-- Ví dụ: Kiểm tra tồn kho của thuốc có mã 'T001'
SELECT dbo.fn_TinhTonKho('T001') AS TonKhoThucTe;
```

---

### 2.2. `dbo.fn_SoNgayConHan`

Tính số ngày còn lại đến hạn sử dụng của một lô thuốc cụ thể tính từ ngày hiện tại (`GETDATE()`).

- **Tham số:**
    - `@MaLo` (`NVARCHAR(15)`): Mã lô thuốc cần kiểm tra.
- **Kiểu trả về:** `INT`
    - Số dương ($> 0$): Số ngày còn hạn.
    - Số 0 ($= 0$): Hết hạn hôm nay.
    - Số âm ($< 0$): Đã hết hạn $N$ ngày.
    - Trả về `-999` nếu không tìm thấy mã lô.

```sql
-- Ví dụ: Kiểm tra số ngày còn hạn của lô 'LO001'
SELECT dbo.fn_SoNgayConHan('LO001') AS SoNgayConHan;
```

---

## 3. CÁC TRIGGER XỬ LÝ TỰ ĐỘNG

_(Người dùng không cần gọi trực tiếp các Trigger, hệ thống tự kích hoạt khi thao tác dữ liệu)_

- **`trg_AfterInsert_ChiTietPhieuNhap`**:
    - Tự động sinh mã lô mới (`LOxxx`) và thêm bản ghi vào `LOTHUC`.
    - Tự động cập nhật `SoLuongTonKho` trong bảng `THUOC`.
    - Tính toán và cập nhật lại `TongTien` của `PHIEUNHAP`.
- **`trg_AfterInsert_ChiTietHoaDon`**:
    - Trừ số lượng còn (`SoLuongCon`) của lô thuốc được chọn mua.
    - Chặn bán (`ROLLBACK`) và thông báo lỗi nếu bán quá số lượng còn của lô.
    - Cập nhật lại tồn kho tổng của `THUOC` và `TongTien` của `HOADON`.
- **`trg_InsteadOfDelete_Thuoc`**:
    - Chặn việc xóa thông tin thuốc nếu thuốc đó đã phát sinh lô nhập kho.

---

## 4. CÁC STORED PROCEDURE NGHIỆP VỤ & THỐNG KÊ

### 4.1. `sp_TaoPhieuNhap`

Tạo một phiếu nhập hàng hoàn chỉnh gồm: thông tin phiếu nhập, các dòng chi tiết phiếu nhập (đọc từ dữ liệu JSON), tự động khởi tạo các Lô thuốc tương ứng và cập nhật tổng tiền phiếu nhập.

- **Danh sách Tham số:**
    - `@MaNhaCC` (`NVARCHAR(10)`): Mã nhà cung cấp. _(Bắt buộc)_
    - `@NgayNhap` (`DATE`): Ngày nhập kho. _(Mặc định là ngày hiện tại nếu truyền `NULL`)_
    - `@ChiTiet` (`NVARCHAR(MAX)`): Chuỗi danh sách mặt hàng nhập dạng JSON array. _(Bắt buộc)_

- **Cấu trúc JSON đầu vào (`@ChiTiet`):**

    ```json
    [
        { "MaThuoc": "T001", "SoLuongNhap": 100, "DonGiaNhap": 5000, "HanSuDung": "2027-12-31" },
        { "MaThuoc": "T002", "SoLuongNhap": 50, "DonGiaNhap": 12000, "HanSuDung": "2026-10-15" }
    ]
    ```

- **Ví dụ thực thi:**
    ```sql
    EXEC sp_TaoPhieuNhap
        @MaNhaCC = 'NCC001',
        @NgayNhap = '2026-03-30',
        @ChiTiet = N'[
            {"MaThuoc":"T001", "SoLuongNhap":100, "DonGiaNhap":5000, "HanSuDung":"2027-12-31"},
            {"MaThuoc":"T002", "SoLuongNhap":200, "DonGiaNhap":15000, "HanSuDung":"2026-06-30"}
        ]';
    ```

---

### 4.2. `sp_TaoHoaDon`

Tạo hóa đơn bán hàng cho khách hàng. Hệ thống **tự động áp dụng nguyên tắc FIFO (First In, First Out)**: ưu tiên chọn trừ số lượng ở các lô thuốc có **Hạn sử dụng gần nhất** và ngày nhập kho cũ nhất.

- **Danh sách Tham số:**
    - `@MaKH` (`NVARCHAR(10)`): Mã khách hàng mua. _(Bắt buộc)_
    - `@ChiTiet` (`NVARCHAR(MAX)`): Chuỗi danh sách thuốc cần mua dạng JSON array. _(Bắt buộc)_

- **Cấu trúc JSON đầu vào (`@ChiTiet`):**

    ```json
    [
        { "MaThuoc": "T001", "SoLuongBan": 5 },
        { "MaThuoc": "T002", "SoLuongBan": 10 }
    ]
    ```

- **Ví dụ thực thi:**
    ```sql
    EXEC sp_TaoHoaDon
        @MaKH = 'KH001',
        @ChiTiet = N'[
            {"MaThuoc":"T001", "SoLuongBan":12},
            {"MaThuoc":"T002", "SoLuongBan":5}
        ]';
    ```

---

### 4.3. `sp_CanhBaoSapHetHan`

Liệt kê danh sách các lô thuốc còn hàng trong kho bị hết hạn hoặc sắp hết hạn trong vòng $N$ ngày tới, đi kèm mức độ cảnh báo.

- **Danh sách Tham số:**
    - `@SoNgay` (`INT`): Ngưỡng số ngày sắp hết hạn cần kiểm tra. _(Mặc định = `30` ngày nếu không truyền)_

- **Cột trả về:** `MaLo`, `MaThuoc`, `TenThuoc`, `HanSuDung`, `SoNgayConLai`, `SoLuongCon`, `MucDo`.

- **Ví dụ thực thi:**

    ```sql
    -- Kiểm tra các lô sắp hết hạn trong 30 ngày tới (Mặc định)
    EXEC sp_CanhBaoSapHetHan;

    -- Kiểm tra các lô sắp hết hạn trong 60 ngày tới
    EXEC sp_CanhBaoSapHetHan @SoNgay = 60;
    ```

---

### 4.4. `sp_ThongKeDoanhThu`

Báo cáo thống kê chi tiết doanh thu và số lượng hóa đơn theo từng ngày, kèm theo một bảng tổng kết doanh thu trong khoảng thời gian truy vấn.

- **Danh sách Tham số:**
    - `@TuNgay` (`DATE`): Ngày bắt đầu thống kê. _(Mặc định = 1 tháng trước)_
    - `@DenNgay` (`DATE`): Ngày kết thúc thống kê. _(Mặc định = Ngày hiện tại)_

- **Ví dụ thực thi:**

    ```sql
    -- Xem doanh thu từ đầu tháng đến nay
    EXEC sp_ThongKeDoanhThu @TuNgay = '2026-03-01', @DenNgay = '2026-03-31';

    -- Xem doanh thu 30 ngày gần nhất
    EXEC sp_ThongKeDoanhThu;
    ```

---

### 4.5. `sp_ThongKeThuocBanChay`

Báo cáo danh sách Top các mặt hàng thuốc có tổng số lượng bán ra cao nhất trong một khoảng thời gian chỉ định.

- **Danh sách Tham số:**
    - `@TuNgay` (`DATE`): Ngày bắt đầu thống kê. _(Mặc định = 1 tháng trước)_
    - `@DenNgay` (`DATE`): Ngày kết thúc thống kê. _(Mặc định = Ngày hiện tại)_
    - `@Top` (`INT`): Số lượng kết quả top cần lấy. _(Mặc định = `10`)_

- **Cột trả về:** `MaThuoc`, `TenThuoc`, `TongSoLuongBan`, `TongDoanhThu`.

- **Ví dụ thực thi:**

    ```sql
    -- Lấy Top 10 thuốc bán chạy nhất trong 30 ngày gần đây
    EXEC sp_ThongKeThuocBanChay;

    -- Lấy Top 5 thuốc bán chạy nhất trong tháng 3/2026
    EXEC sp_ThongKeThuocBanChay
        @TuNgay = '2026-03-01',
        @DenNgay = '2026-03-31',
        @Top = 5;
    ```

---

### 4.6. `sp_XemTonKhoTheoLo`

Truy vấn chi tiết tình trạng tồn kho, hạn sử dụng và đơn giá nhập của từng lô thuốc còn hàng trong kho (`SoLuongCon > 0`).

- **Danh sách Tham số:**
    - `@MaThuoc` (`NVARCHAR(10)`): Mã thuốc cụ thể muốn xem. _(Mặc định là `NULL` - hiển thị toàn bộ thuốc trong kho)_

- **Cột trả về:** `MaLo`, `MaThuoc`, `TenThuoc`, `NgayNhapKho`, `HanSuDung`, `SoNgayConLai`, `SoLuongNhap`, `SoLuongCon`, `DonGiaNhap`.

- **Ví dụ thực thi:**

    ```sql
    -- Xem tồn kho theo lô của tất cả các thuốc
    EXEC sp_XemTonKhoTheoLo;

    -- Chỉ xem các lô còn tồn của thuốc có mã 'T001'
    EXEC sp_XemTonKhoTheoLo @MaThuoc = 'T001';
    ```
