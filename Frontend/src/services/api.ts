import type {
  LoaiThuoc,
  Thuoc,
  CreateThuocRequest,
  UpdateThuocRequest,
  CreateLoaiThuocRequest,
  KhachHang,
  CreateKhachHangRequest,
  NhaCC,
  CreateNhaCCRequest,
  LoThuoc,
  HoaDon,
  CreateHoaDonRequest,
  PhieuNhap,
  CreatePhieuNhapRequest,
  CanhBaoSapHetHanItem,
  ThongKeTongQuan,
  ThongKeDoanhThuItem,
  ApiMessageResponse,
} from "@/type";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || "http://localhost:5238/api";

async function request<T>(endpoint: string, options?: RequestInit): Promise<T> {
  const url = `${API_BASE_URL}${endpoint}`;
  try {
    const response = await fetch(url, {
      headers: {
        "Content-Type": "application/json",
        ...options?.headers,
      },
      ...options,
    });

    if (!response.ok) {
      let errorMessage = `Yêu cầu thất bại (${response.status})`;
      try {
        const errorData = await response.json();
        if (errorData?.message) {
          errorMessage = errorData.message;
        } else if (errorData?.title) {
          errorMessage = errorData.title;
        }
      } catch {
        // Response is not JSON
      }
      throw new Error(errorMessage);
    }

    return (await response.json()) as T;
  } catch (error: unknown) {
    if (error instanceof TypeError && error.message.includes("fetch")) {
      throw new Error(
        `Không thể kết nối đến máy chủ Backend (${API_BASE_URL}). Vui lòng đảm bảo Backend C# đang chạy.`
      );
    }
    throw error;
  }
}

export const pharmacyApi = {
  // Thống kê
  async getThongKeTongQuan(): Promise<ThongKeTongQuan> {
    return request<ThongKeTongQuan>("/ThongKe/tong-quan");
  },

  async getThongKeDoanhThu(tuNgay?: string, denNgay?: string): Promise<ThongKeDoanhThuItem[]> {
    const params = new URLSearchParams();
    if (tuNgay) params.append("tuNgay", tuNgay);
    if (denNgay) params.append("denNgay", denNgay);
    const queryString = params.toString() ? `?${params.toString()}` : "";
    return request<ThongKeDoanhThuItem[]>(`/ThongKe/doanh-thu${queryString}`);
  },

  // Cảnh báo
  async getCanhBaoSapHetHan(soNgay = 30): Promise<CanhBaoSapHetHanItem[]> {
    return request<CanhBaoSapHetHanItem[]>(`/CanhBao/sap-het-han?soNgay=${soNgay}`);
  },

  // Loại thuốc
  async getLoaiThuoc(): Promise<LoaiThuoc[]> {
    return request<LoaiThuoc[]>("/LoaiThuoc");
  },

  async getLoaiThuocById(id: string): Promise<LoaiThuoc> {
    return request<LoaiThuoc>(`/LoaiThuoc/${encodeURIComponent(id)}`);
  },

  async createLoaiThuoc(data: CreateLoaiThuocRequest): Promise<LoaiThuoc> {
    return request<LoaiThuoc>("/LoaiThuoc", {
      method: "POST",
      body: JSON.stringify(data),
    });
  },

  async updateLoaiThuoc(id: string, data: CreateLoaiThuocRequest): Promise<LoaiThuoc> {
    return request<LoaiThuoc>(`/LoaiThuoc/${encodeURIComponent(id)}`, {
      method: "PUT",
      body: JSON.stringify(data),
    });
  },

  async deleteLoaiThuoc(id: string): Promise<ApiMessageResponse> {
    return request<ApiMessageResponse>(`/LoaiThuoc/${encodeURIComponent(id)}`, {
      method: "DELETE",
    });
  },

  // Thuốc
  async getThuoc(filters?: { search?: string; maLoai?: string }): Promise<Thuoc[]> {
    const params = new URLSearchParams();
    if (filters?.search) params.append("search", filters.search);
    if (filters?.maLoai && filters.maLoai !== "ALL") params.append("maLoai", filters.maLoai);
    const queryString = params.toString() ? `?${params.toString()}` : "";
    return request<Thuoc[]>(`/Thuoc${queryString}`);
  },

  async getThuocById(id: string): Promise<Thuoc> {
    return request<Thuoc>(`/Thuoc/${encodeURIComponent(id)}`);
  },

  async createThuoc(data: CreateThuocRequest): Promise<Thuoc> {
    return request<Thuoc>("/Thuoc", {
      method: "POST",
      body: JSON.stringify(data),
    });
  },

  async updateThuoc(id: string, data: UpdateThuocRequest): Promise<Thuoc> {
    return request<Thuoc>(`/Thuoc/${encodeURIComponent(id)}`, {
      method: "PUT",
      body: JSON.stringify(data),
    });
  },

  async deleteThuoc(id: string): Promise<ApiMessageResponse> {
    return request<ApiMessageResponse>(`/Thuoc/${encodeURIComponent(id)}`, {
      method: "DELETE",
    });
  },

  // Khách hàng
  async getKhachHang(search?: string): Promise<KhachHang[]> {
    const params = new URLSearchParams();
    if (search) params.append("search", search);
    const queryString = params.toString() ? `?${params.toString()}` : "";
    return request<KhachHang[]>(`/KhachHang${queryString}`);
  },

  async getKhachHangById(id: string): Promise<KhachHang> {
    return request<KhachHang>(`/KhachHang/${encodeURIComponent(id)}`);
  },

  async createKhachHang(data: CreateKhachHangRequest): Promise<KhachHang> {
    return request<KhachHang>("/KhachHang", {
      method: "POST",
      body: JSON.stringify(data),
    });
  },

  async updateKhachHang(id: string, data: CreateKhachHangRequest): Promise<KhachHang> {
    return request<KhachHang>(`/KhachHang/${encodeURIComponent(id)}`, {
      method: "PUT",
      body: JSON.stringify(data),
    });
  },

  async deleteKhachHang(id: string): Promise<ApiMessageResponse> {
    return request<ApiMessageResponse>(`/KhachHang/${encodeURIComponent(id)}`, {
      method: "DELETE",
    });
  },

  // Nhà cung cấp
  async getNhaCC(): Promise<NhaCC[]> {
    return request<NhaCC[]>("/NhaCC");
  },

  async getNhaCCById(id: string): Promise<NhaCC> {
    return request<NhaCC>(`/NhaCC/${encodeURIComponent(id)}`);
  },

  async createNhaCC(data: CreateNhaCCRequest): Promise<NhaCC> {
    return request<NhaCC>("/NhaCC", {
      method: "POST",
      body: JSON.stringify(data),
    });
  },

  async updateNhaCC(id: string, data: CreateNhaCCRequest): Promise<NhaCC> {
    return request<NhaCC>(`/NhaCC/${encodeURIComponent(id)}`, {
      method: "PUT",
      body: JSON.stringify(data),
    });
  },

  async deleteNhaCC(id: string): Promise<ApiMessageResponse> {
    return request<ApiMessageResponse>(`/NhaCC/${encodeURIComponent(id)}`, {
      method: "DELETE",
    });
  },

  // Lô thuốc
  async getLoThuoc(filters?: { maThuoc?: string; conHangOnly?: boolean }): Promise<LoThuoc[]> {
    const params = new URLSearchParams();
    if (filters?.maThuoc) params.append("maThuoc", filters.maThuoc);
    if (filters?.conHangOnly !== undefined) params.append("conHangOnly", String(filters.conHangOnly));
    const queryString = params.toString() ? `?${params.toString()}` : "";
    return request<LoThuoc[]>(`/LoThuoc${queryString}`);
  },

  async tieuHuyLoThuoc(maLo: string): Promise<ApiMessageResponse> {
    return request<ApiMessageResponse>(`/LoThuoc/${encodeURIComponent(maLo)}/tieu-huy`, {
      method: "PUT",
    });
  },

  // Hóa đơn
  async getHoaDon(): Promise<HoaDon[]> {
    return request<HoaDon[]>("/HoaDon");
  },

  async getHoaDonById(id: string): Promise<HoaDon> {
    return request<HoaDon>(`/HoaDon/${encodeURIComponent(id)}`);
  },

  async createHoaDon(data: CreateHoaDonRequest): Promise<HoaDon> {
    return request<HoaDon>("/HoaDon", {
      method: "POST",
      body: JSON.stringify(data),
    });
  },

  // Phiếu nhập
  async getPhieuNhap(): Promise<PhieuNhap[]> {
    return request<PhieuNhap[]>("/PhieuNhap");
  },

  async getPhieuNhapById(id: string): Promise<PhieuNhap> {
    return request<PhieuNhap>(`/PhieuNhap/${encodeURIComponent(id)}`);
  },

  async createPhieuNhap(data: CreatePhieuNhapRequest): Promise<PhieuNhap> {
    return request<PhieuNhap>("/PhieuNhap", {
      method: "POST",
      body: JSON.stringify(data),
    });
  },
};
