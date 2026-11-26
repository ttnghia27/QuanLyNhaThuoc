--ThongKeTongQuanNew
use QuanLyNhaThuoc
GO

CREATE OR ALTER PROCEDURE sp_ThongKeTongQuan
    @Months INT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF @Months NOT IN (1, 3, 6, 12)
    BEGIN
        RAISERROR(N'Tham số months chỉ hỗ trợ các giá trị: 1, 3, 6, 12.', 16, 1);
        RETURN;
    END

    DECLARE @NgayHienTai DATE = CAST(GETDATE() AS DATE);
    DECLARE @TuNgay DATE = DATEADD(MONTH, -@Months, @NgayHienTai);
    DECLARE @NgayCanhBao DATE = DATEADD(DAY, 30, @NgayHienTai);

    DECLARE @TongDoanhThu DECIMAL(18, 0) = 0;
    DECLARE @GiaVonBanRa DECIMAL(18, 0) = 0;
    DECLARE @SoHoaDon INT = 0;
    DECLARE @TongGiaTriTonKho DECIMAL(18, 0) = 0;
    DECLARE @LoDaHetHan INT = 0;
    DECLARE @ThuocSapHetHang INT = 0;
    DECLARE @LoSapHetHan INT = 0;

    -- Doanh thu & số hóa đơn trong @Months tháng gần nhất
    SELECT
        @TongDoanhThu = ISNULL(SUM(hd.TongTien), 0),
        @SoHoaDon = COUNT(*)
    FROM HOADON hd
    WHERE CAST(hd.NgayLapHD AS DATE) BETWEEN @TuNgay AND @NgayHienTai;

    -- Giá vốn của các lô đã bán thuộc các hóa đơn trong @Months tháng gần nhất
    SELECT
        @GiaVonBanRa = ISNULL(SUM(CAST(ct.SoLuongBan AS DECIMAL(18, 0)) * ISNULL(l.DonGiaNhap, 0)), 0)
    FROM CHITIETHOADON ct
    INNER JOIN HOADON hd ON hd.MaHD = ct.MaHD
    INNER JOIN LOTHUC l ON l.MaLo = ct.MaLo
    WHERE CAST(hd.NgayLapHD AS DATE) BETWEEN @TuNgay AND @NgayHienTai;

    -- Tổng giá trị tồn kho còn hạn hiện tại
    SELECT
        @TongGiaTriTonKho = ISNULL(SUM(CAST(l.SoLuongCon AS DECIMAL(18, 0)) * ISNULL(l.DonGiaNhap, 0)), 0)
    FROM LOTHUC l
    WHERE l.SoLuongCon > 0
      AND l.HanSuDung >= @NgayHienTai;

      

    -- Số thuốc sắp hết hàng (chỉ tính lô còn hạn)
    ;WITH TonConHanTheoThuoc AS (
        SELECT
            t.MaThuoc,
            ISNULL(SUM(CASE
                WHEN l.SoLuongCon > 0 AND l.HanSuDung >= @NgayHienTai
                THEN l.SoLuongCon ELSE 0
            END), 0) AS TonConHan
        FROM THUOC t
        LEFT JOIN LOTHUC l ON l.MaThuoc = t.MaThuoc
        GROUP BY t.MaThuoc
    )
    SELECT @ThuocSapHetHang = COUNT(*)
    FROM TonConHanTheoThuoc
    WHERE TonConHan <= 10;

    -- Số lô sắp hết hạn (còn tồn > 0 và hạn trong 30 ngày tới)
    SELECT @LoSapHetHan = COUNT(*)
    FROM LOTHUC l
    WHERE l.SoLuongCon > 0
      AND l.HanSuDung >= @NgayHienTai
      AND l.HanSuDung <= @NgayCanhBao;

    SELECT @LoDaHetHan = COUNT(*)
    FROM LOTHUC l
    WHERE l.SoLuongCon > 0
      AND l.HanSuDung < @NgayHienTai

    SELECT
        @TongDoanhThu AS TongDoanhThu,
        (@TongDoanhThu - @GiaVonBanRa) AS LoiNhuanUocTinh,
        @TongGiaTriTonKho AS TongGiaTriTonKho,
        @SoHoaDon AS SoHoaDon,
        CAST(CASE WHEN @SoHoaDon > 0 THEN @TongDoanhThu / CAST(@SoHoaDon AS DECIMAL(18, 2)) ELSE 0 END AS DECIMAL(18, 2)) AS GiaTriTrungBinhDon,
        @ThuocSapHetHang AS ThuocSapHetHang,
        @LoSapHetHan AS LoSapHetHan,
        @LoDaHetHan AS LoDaHetHan;
END;
GO

CREATE OR ALTER FUNCTION fn_TinhTonKho(@MaThuoc NVARCHAR(10))
RETURNS INT
AS
BEGIN
    DECLARE @Ton INT;
    DECLARE @NgayHienTai DATE = CAST(GETDATE() AS DATE);

    SELECT @Ton = ISNULL(SUM(SoLuongCon), 0)
    FROM LOTHUC
    WHERE MaThuoc = @MaThuoc
      AND SoLuongCon > 0
      AND HanSuDung >= @NgayHienTai;

    RETURN @Ton;
END;
GO

CREATE OR ALTER TRIGGER trg_AfterInsertUpdate_LoThuoc_CapNhatGiaNhap
ON LOTHUC
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NgayHienTai DATE = CAST(GETDATE() AS DATE);

    ;WITH ThuocCanCapNhat AS (
        SELECT DISTINCT MaThuoc FROM inserted
        UNION
        SELECT DISTINCT MaThuoc FROM deleted
    ),
    LoMoiNhat AS (
        SELECT
            l.MaThuoc,
            l.DonGiaNhap,
            ROW_NUMBER() OVER (
                PARTITION BY l.MaThuoc
                ORDER BY l.NgayNhapKho DESC, l.MaLo DESC
            ) AS rn
        FROM LOTHUC l
        INNER JOIN ThuocCanCapNhat t ON t.MaThuoc = l.MaThuoc
        WHERE l.SoLuongCon > 0
          AND l.HanSuDung >= @NgayHienTai
    )
    UPDATE t
    SET t.GiaNhapDuKien = ISNULL(lm.DonGiaNhap, 0)
    FROM THUOC t
    INNER JOIN ThuocCanCapNhat tc ON tc.MaThuoc = t.MaThuoc
    LEFT JOIN LoMoiNhat lm ON lm.MaThuoc = t.MaThuoc AND lm.rn = 1;
END;
GO

CREATE OR ALTER PROCEDURE sp_TaoHoaDon
    @MaKH       NVARCHAR(10),
    @ChiTiet    NVARCHAR(MAX)   -- JSON: [{"MaThuoc":"T001","SoLuongBan":10}, ...]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @MaHD NVARCHAR(10) = dbo.fn_SinhMaHD();
        DECLARE @TongTien DECIMAL(18, 0) = 0;
        DECLARE @Today DATE = CAST(GETDATE() AS DATE);
        DECLARE @Now DATETIME = GETDATE();

        INSERT INTO HOADON (MaHD, NgayLapHD, TongTien, MaKH)
        VALUES (@MaHD, @Now, 0, @MaKH);

        DECLARE @MaThuoc NVARCHAR(10), @SoLuongCanBan INT, @GiaBan DECIMAL(18, 0);
        DECLARE @MaLo NVARCHAR(15), @SoLuongCon INT, @SoLuongLay INT;

        DECLARE curItem CURSOR FOR
            SELECT MaThuoc, SoLuongBan
            FROM OPENJSON(@ChiTiet)
            WITH (
                MaThuoc NVARCHAR(10) '$.MaThuoc',
                SoLuongBan INT '$.SoLuongBan'
            );

        OPEN curItem;
        FETCH NEXT FROM curItem INTO @MaThuoc, @SoLuongCanBan;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- Reset biến trước mỗi lần đọc thuốc mới để tránh dính dữ liệu cũ
            SET @GiaBan = NULL;

            -- Lấy giá bán
            SELECT @GiaBan = DonGiaBan FROM THUOC WHERE MaThuoc = @MaThuoc;
            IF @GiaBan IS NULL
            BEGIN
                RAISERROR(N'Không tìm thấy thuốc %s', 16, 1, @MaThuoc);
            END

            -- Kiểm tra đủ hàng (nhờ fn_TinhTonKho đã lọc SoLuongCon > 0 và còn hạn)
            IF dbo.fn_TinhTonKho(@MaThuoc) < @SoLuongCanBan
            BEGIN
                RAISERROR(N'Thuốc %s không đủ số lượng tồn kho còn hạn', 16, 1, @MaThuoc);
            END

            -- Duyệt các lô còn hạn theo hạn sử dụng gần nhất (FEFO)
            DECLARE curLo CURSOR LOCAL FAST_FORWARD FOR
                SELECT MaLo, SoLuongCon
                FROM LOTHUC
                WHERE MaThuoc = @MaThuoc
                  AND SoLuongCon > 0
                  AND HanSuDung >= @Today
                ORDER BY HanSuDung ASC, NgayNhapKho ASC, MaLo ASC;

            OPEN curLo;
            FETCH NEXT FROM curLo INTO @MaLo, @SoLuongCon;

            WHILE @@FETCH_STATUS = 0 AND @SoLuongCanBan > 0
            BEGIN
                SET @SoLuongLay = CASE
                    WHEN @SoLuongCon >= @SoLuongCanBan THEN @SoLuongCanBan
                    ELSE @SoLuongCon
                END;

                INSERT INTO CHITIETHOADON (MaHD, MaLo, SoLuongBan, GiaBan)
                VALUES (@MaHD, @MaLo, @SoLuongLay, @GiaBan);

                -- Trigger trên CHITIETHOADON / LOTHUC sẽ tự trừ SoLuongCon

                SET @TongTien = @TongTien + (CAST(@SoLuongLay AS DECIMAL(18, 0)) * @GiaBan);
                SET @SoLuongCanBan = @SoLuongCanBan - @SoLuongLay;

                FETCH NEXT FROM curLo INTO @MaLo, @SoLuongCon;
            END

            CLOSE curLo;
            DEALLOCATE curLo;

            -- Phòng trường hợp dữ liệu thay đổi giữa lúc kiểm tra tồn và lúc duyệt cursor
            IF @SoLuongCanBan > 0
            BEGIN
                RAISERROR(N'Không thể xuất đủ số lượng còn hạn cho thuốc %s', 16, 1, @MaThuoc);
            END

            FETCH NEXT FROM curItem INTO @MaThuoc, @SoLuongCanBan;
        END

        CLOSE curItem;
        DEALLOCATE curItem;

        UPDATE HOADON
        SET TongTien = @TongTien
        WHERE MaHD = @MaHD;

        COMMIT TRANSACTION;

        SELECT @MaHD AS MaHD, @TongTien AS TongTien;
    END TRY
    BEGIN CATCH
        IF CURSOR_STATUS('local', 'curLo') >= -1
        BEGIN
            CLOSE curLo;
            DEALLOCATE curLo;
        END

        IF CURSOR_STATUS('local', 'curItem') >= -1
        BEGIN
            CLOSE curItem;
            DEALLOCATE curItem;
        END

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE sp_TaoHoaDon_SeedRandomDate
    @MaKH       NVARCHAR(10),
    @ChiTiet    NVARCHAR(MAX)   -- JSON: [{"MaThuoc":"T01","SoLuongBan":3}, ...]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @MaHD NVARCHAR(10) = dbo.fn_SinhMaHD();
        DECLARE @TongTien DECIMAL(18, 0) = 0;

        -- Ngày lập hóa đơn ngẫu nhiên trong 365 ngày gần nhất
        DECLARE @NgayLapHD DATETIME = DATEADD(
            SECOND,
            ABS(CHECKSUM(NEWID())) % 86400,
            CAST(DATEADD(DAY, -(ABS(CHECKSUM(NEWID())) % 366), CAST(GETDATE() AS DATE)) AS DATETIME)
        );

        -- Mốc ngày kiểm tra hạn sử dụng lô: dùng ngày lập hóa đơn (hoặc đổi thành CAST(GETDATE() AS DATE) nếu muốn so với hôm nay)
        DECLARE @NgayKiemTraHan DATE = CAST(@NgayLapHD AS DATE);

        INSERT INTO HOADON (MaHD, NgayLapHD, TongTien, MaKH)
        VALUES (@MaHD, @NgayLapHD, 0, @MaKH);

        DECLARE @MaThuoc NVARCHAR(10), @SoLuongCanBan INT, @GiaBan DECIMAL(18, 0);
        DECLARE @MaLo NVARCHAR(15), @SoLuongCon INT, @SoLuongLay INT;

        DECLARE curItem CURSOR LOCAL FAST_FORWARD FOR
            SELECT MaThuoc, SoLuongBan
            FROM OPENJSON(@ChiTiet)
            WITH (
                MaThuoc NVARCHAR(10) '$.MaThuoc',
                SoLuongBan INT '$.SoLuongBan'
            );

        OPEN curItem;
        FETCH NEXT FROM curItem INTO @MaThuoc, @SoLuongCanBan;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @GiaBan = NULL;

            SELECT @GiaBan = DonGiaBan FROM THUOC WHERE MaThuoc = @MaThuoc;
            IF @GiaBan IS NULL
            BEGIN
                RAISERROR(N'Không tìm thấy thuốc %s', 16, 1, @MaThuoc);
            END

            IF dbo.fn_TinhTonKho(@MaThuoc) < @SoLuongCanBan
            BEGIN
                RAISERROR(N'Thuốc %s không đủ số lượng tồn kho còn hạn', 16, 1, @MaThuoc);
            END

            DECLARE curLo CURSOR LOCAL FAST_FORWARD FOR
                SELECT MaLo, SoLuongCon
                FROM LOTHUC
                WHERE MaThuoc = @MaThuoc
                  AND SoLuongCon > 0
                  AND HanSuDung >= @NgayKiemTraHan
                ORDER BY HanSuDung ASC, NgayNhapKho ASC, MaLo ASC;

            OPEN curLo;
            FETCH NEXT FROM curLo INTO @MaLo, @SoLuongCon;

            WHILE @@FETCH_STATUS = 0 AND @SoLuongCanBan > 0
            BEGIN
                SET @SoLuongLay = CASE
                    WHEN @SoLuongCon >= @SoLuongCanBan THEN @SoLuongCanBan
                    ELSE @SoLuongCon
                END;

                INSERT INTO CHITIETHOADON (MaHD, MaLo, SoLuongBan, GiaBan)
                VALUES (@MaHD, @MaLo, @SoLuongLay, @GiaBan);

                SET @TongTien = @TongTien + (CAST(@SoLuongLay AS DECIMAL(18, 0)) * @GiaBan);
                SET @SoLuongCanBan = @SoLuongCanBan - @SoLuongLay;

                FETCH NEXT FROM curLo INTO @MaLo, @SoLuongCon;
            END

            CLOSE curLo;
            DEALLOCATE curLo;

            IF @SoLuongCanBan > 0
            BEGIN
                RAISERROR(N'Không thể xuất đủ số lượng còn hạn cho thuốc %s', 16, 1, @MaThuoc);
            END

            FETCH NEXT FROM curItem INTO @MaThuoc, @SoLuongCanBan;
        END

        CLOSE curItem;
        DEALLOCATE curItem;

        UPDATE HOADON
        SET TongTien = @TongTien
        WHERE MaHD = @MaHD;

        COMMIT TRANSACTION;

        SELECT @MaHD AS MaHD, @NgayLapHD AS NgayLapHD, @TongTien AS TongTien;
    END TRY
    BEGIN CATCH
        IF CURSOR_STATUS('local', 'curLo') >= -1
        BEGIN
            CLOSE curLo;
            DEALLOCATE curLo;
        END

        IF CURSOR_STATUS('local', 'curItem') >= -1
        BEGIN
            CLOSE curItem;
            DEALLOCATE curItem;
        END

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END;
GO

SET NOCOUNT ON;

DECLARE @i INT = 1;
DECLARE @TotalCalls INT = 100;
DECLARE @MaKH NVARCHAR(10) = N'KH01';

DECLARE @QtyT01 INT;
DECLARE @QtyT02 INT;
DECLARE @QtyT03 INT;
DECLARE @ChiTiet NVARCHAR(MAX);

WHILE @i <= @TotalCalls
BEGIN
    -- Sinh ngẫu nhiên số lượng từ 2 đến 4 cho từng thuốc
    SET @QtyT01 = 2 + (ABS(CHECKSUM(NEWID())) % 3);
    SET @QtyT02 = 2 + (ABS(CHECKSUM(NEWID())) % 3);
    SET @QtyT03 = 2 + (ABS(CHECKSUM(NEWID())) % 3);

    SET @ChiTiet = (
        SELECT
            MaThuoc,
            SoLuongBan
        FROM (
            VALUES
                (N'T01', @QtyT01),
                (N'T02', @QtyT02),
                (N'T03', @QtyT03)
        ) AS v(MaThuoc, SoLuongBan)
        FOR JSON AUTO
    );

    EXEC dbo.sp_TaoHoaDon_SeedRandomDate
        @MaKH = @MaKH,
        @ChiTiet = @ChiTiet;

    SET @i = @i + 1;
END;
GO

--SET NOCOUNT ON;
--BEGIN TRY
--    BEGIN TRANSACTION;

--    DECLARE @SoLuongHoaDonCanRevert INT = 100;
--    DECLARE @MaKH NVARCHAR(10) = N'KH01';

--    -- Lấy danh sách N hóa đơn mới nhất của KH01 vừa tạo
--    DECLARE @TargetHD TABLE (
--        MaHD NVARCHAR(10) PRIMARY KEY
--    );

--    INSERT INTO @TargetHD (MaHD)
--    SELECT TOP (@SoLuongHoaDonCanRevert) hd.MaHD
--    FROM HOADON hd
--    WHERE hd.MaKH = @MaKH
--    ORDER BY hd.NgayLapHD DESC, hd.MaHD DESC;

--    -- Tổng hợp số lượng cần hoàn lại theo từng lô
--    DECLARE @RefundLo TABLE (
--        MaLo NVARCHAR(15) PRIMARY KEY,
--        SoLuongHoan INT
--    );

--    INSERT INTO @RefundLo (MaLo, SoLuongHoan)
--    SELECT
--        ct.MaLo,
--        SUM(ct.SoLuongBan) AS SoLuongHoan
--    FROM CHITIETHOADON ct
--    INNER JOIN @TargetHD t ON t.MaHD = ct.MaHD
--    GROUP BY ct.MaLo;

--    -- Hoàn lại SoLuongCon vào LOTHUC
--    UPDATE l
--    SET l.SoLuongCon = l.SoLuongCon + r.SoLuongHoan
--    FROM LOTHUC l
--    INNER JOIN @RefundLo r ON r.MaLo = l.MaLo;

--    -- Xóa chi tiết hóa đơn và hóa đơn
--    DELETE ct
--    FROM CHITIETHOADON ct
--    INNER JOIN @TargetHD t ON t.MaHD = ct.MaHD;

--    DELETE hd
--    FROM HOADON hd
--    INNER JOIN @TargetHD t ON t.MaHD = hd.MaHD;

--    COMMIT TRANSACTION;

--    SELECT COUNT(*) AS SoHoaDonDaRevert
--    FROM @TargetHD;
--END TRY
--BEGIN CATCH
--    IF @@TRANCOUNT > 0
--        ROLLBACK TRANSACTION;

--    THROW;
--END CATCH;

--GO

--DECLARE @Today DATE = CAST(GETDATE() AS DATE);

---- Tính tổng tồn kho còn hạn theo từng thuốc
--;WITH TonConHan AS (
--    SELECT
--        lt.MaThuoc,
--        ISNULL(SUM(lt.SoLuongCon), 0) AS TongTonConHan
--    FROM LOTHUC lt
--    WHERE lt.SoLuongCon > 0
--      AND lt.HanSuDung >= @Today
--    GROUP BY lt.MaThuoc
--)

--UPDATE t
--SET t.SoLuongTonKho = ISNULL(tch.TongTonConHan, 0) -- Đổi tên cột cho khớp bảng THUOC của bạn
--FROM THUOC t
--LEFT JOIN TonConHan tch ON tch.MaThuoc = t.MaThuoc;
--GO
