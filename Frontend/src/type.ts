// Reusable application-wide types according to AGENTS.md and API-DOCS.md

export interface LoaiThuoc {
    maLoai: string;
    tenLoai: string;
    soLuongThuoc?: number;
}

export interface Thuoc {
    maThuoc: string;
    tenThuoc: string;
    congDung: string;
    donGiaBan: number;
    giaNhapDuKien: number;
    soLuongTonKho: number;
    maLoai: string;
    tenLoai?: string;
}

export interface CreateThuocRequest {
    tenThuoc: string;
    congDung: string;
    donGiaBan: number;
    giaNhapDuKien: number;
    maLoai: string;
}

export interface UpdateThuocRequest {
    tenThuoc: string;
    congDung: string;
    donGiaBan: number;
    giaNhapDuKien: number;
    maLoai: string;
}

export interface CreateLoaiThuocRequest {
    tenLoai: string;
}

export interface KhachHang {
    maKH: string;
    tenKH: string;
    sdtkh?: string | null;
    diaChiKH?: string | null;
}

export interface CreateKhachHangRequest {
    tenKH: string;
    sdtKH?: string | null;
    diaChiKH?: string | null;
}

export interface NhaCC {
    maNhaCC: string;
    tenNhaCC: string;
    sdtNhaCC?: string | null;
}

export interface CreateNhaCCRequest {
    tenNhaCC: string;
    sdtNhaCC?: string | null;
}

export interface LoThuoc {
    maLo: string;
    maThuoc: string;
    tenThuoc: string;
    maPhieuNhap: string;
    ngayNhapKho: string;
    hanSuDung: string;
    soLuongNhap: number;
    soLuongCon: number;
    donGiaNhap: number;
    daHetHan: boolean;
}

export interface ChiTietHoaDonItem {
    maLo: string;
    maThuoc: string;
    tenThuoc: string;
    hanSuDung: string;
    soLuongBan: number;
    giaBan: number;
    thanhTien: number;
}

export interface HoaDon {
    maHD: string;
    ngayLapHD: string;
    tongTien: number;
    maKH: string;
    tenKH: string;
    chiTiet?: ChiTietHoaDonItem[];
}

export interface CreateHoaDonDetailItem {
    maThuoc: string;
    soLuongBan: number;
}

export interface CreateHoaDonRequest {
    maKH: string;
    chiTiet: CreateHoaDonDetailItem[];
}

export interface ChiTietPhieuNhapItem {
    maLo: string;
    maThuoc: string;
    tenThuoc: string;
    hanSuDung: string;
    soLuongNhap: number;
    soLuongCon: number;
    donGiaNhap: number;
    thanhTien: number;
}

export interface PhieuNhap {
    maPhieuNhap: string;
    ngayNhap: string;
    tongTien: number;
    maNhaCC: string;
    tenNhaCC: string;
    chiTietLo?: ChiTietPhieuNhapItem[];
}

export interface CreatePhieuNhapDetailItem {
    maThuoc: string;
    soLuongNhap: number;
    donGiaNhap: number;
    hanSuDung: string;
}

export interface CreatePhieuNhapRequest {
    maNhaCC: string;
    ngayNhap?: string | null;
    chiTiet: CreatePhieuNhapDetailItem[];
}

export interface CanhBaoSapHetHanItem {
    maLo: string;
    maThuoc: string;
    tenThuoc: string;
    hanSuDung: string;
    soNgayConLai: number;
    soLuongCon: number;
}

export interface ThongKeTongQuan {
    tongDoanhThu: number;
    loiNhuanUocTinh: number;
    tongGiaTriTonKho: number;
    soHoaDon: number;
    giaTriTrungBinhDon: number;
    thuocSapHetHang: number;
    loSapHetHan: number;
}

export interface ThongKeDoanhThuItem {
    ngay: string;
    soHoaDon: number;
    doanhThu: number;
}

export interface ApiMessageResponse {
    message: string;
}

export interface LoginRequest {
    tenDangNhap: string;
    matKhau: string;
}

export interface LoginResponse {
    token: string;
    hoTen: string;
}
