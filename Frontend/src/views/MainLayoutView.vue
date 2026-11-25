<script setup lang="ts">
import { ref, watch, onMounted } from "vue";
import type {
  Thuoc,
  LoaiThuoc,
  KhachHang,
  NhaCC,
  LoThuoc,
  HoaDon,
  PhieuNhap,
  ThongKeTongQuan,
  ThongKeDoanhThuItem,
  CreateThuocRequest,
  UpdateThuocRequest,
  CreateLoaiThuocRequest,
  CreateKhachHangRequest,
  CreateNhaCCRequest,
  CreateHoaDonRequest,
  CreatePhieuNhapRequest,
} from "@/type";
import { pharmacyApi } from "@/services/api";
import Sidebar, { type NavTab } from "@/components/Sidebar.vue";
import DashboardView from "@/views/DashboardView.vue";
import DrugView from "@/views/DrugView.vue";
import LoaiThuocView from "@/views/LoaiThuocView.vue";
import KhachHangView from "@/views/KhachHangView.vue";
import NhaCCView from "@/views/NhaCCView.vue";
import LoThuocView from "@/views/LoThuocView.vue";
import HoaDonView from "@/views/HoaDonView.vue";
import PhieuNhapView from "@/views/PhieuNhapView.vue";
import { Button } from "@/components/ui/button";
import {
  RefreshCwIcon,
  AlertTriangleIcon,
  CheckCircle2Icon,
} from "@lucide/vue";

// Navigation state
const currentTab = ref<NavTab>("dashboard");

// Data state
const stats = ref<ThongKeTongQuan | null>(null);
const isStatsLoading = ref(false);

const revenueChartData = ref<ThongKeDoanhThuItem[]>([]);
const isChartLoading = ref(false);

const drugs = ref<Thuoc[]>([]);
const isDrugsLoading = ref(false);
const drugSearchQuery = ref("");
const selectedDrugCategory = ref("ALL");

const categories = ref<LoaiThuoc[]>([]);
const isCategoriesLoading = ref(false);

const customers = ref<KhachHang[]>([]);
const isCustomersLoading = ref(false);
const customerSearchQuery = ref("");

const suppliers = ref<NhaCC[]>([]);
const isSuppliersLoading = ref(false);

const lots = ref<LoThuoc[]>([]);
const isLotsLoading = ref(false);
const conHangOnly = ref(false);

const invoices = ref<HoaDon[]>([]);
const isInvoicesLoading = ref(false);

const receipts = ref<PhieuNhap[]>([]);
const isReceiptsLoading = ref(false);

// Drug modal state
const isDrugModalOpen = ref(false);
const drugToEdit = ref<Thuoc | null>(null);
const isSavingDrug = ref(false);

// Notification state
const globalError = ref("");
const successMessage = ref("");

function showSuccess(msg: string) {
  successMessage.value = msg;
  setTimeout(() => {
    if (successMessage.value === msg) {
      successMessage.value = "";
    }
  }, 4000);
}

// Data loaders
async function loadStats() {
  isStatsLoading.value = true;
  try {
    stats.value = await pharmacyApi.getThongKeTongQuan();
  } catch (err) {
    console.error("Lỗi khi tải thống kê:", err);
  } finally {
    isStatsLoading.value = false;
  }
}

async function loadRevenueChart() {
  isChartLoading.value = true;
  try {
    revenueChartData.value = await pharmacyApi.getThongKeDoanhThu();
  } catch (err) {
    console.error("Lỗi khi tải biểu đồ doanh thu:", err);
  } finally {
    isChartLoading.value = false;
  }
}

async function loadCategories() {
  isCategoriesLoading.value = true;
  try {
    categories.value = await pharmacyApi.getLoaiThuoc();
  } catch (err) {
    console.error("Lỗi khi tải loại thuốc:", err);
  } finally {
    isCategoriesLoading.value = false;
  }
}

async function loadDrugs() {
  isDrugsLoading.value = true;
  try {
    drugs.value = await pharmacyApi.getThuoc({
      search: drugSearchQuery.value.trim() || undefined,
      maLoai: selectedDrugCategory.value !== "ALL" ? selectedDrugCategory.value : undefined,
    });
  } catch (err) {
    console.error("Lỗi khi tải danh sách thuốc:", err);
    throw err;
  } finally {
    isDrugsLoading.value = false;
  }
}

async function loadCustomers() {
  isCustomersLoading.value = true;
  try {
    customers.value = await pharmacyApi.getKhachHang(customerSearchQuery.value.trim() || undefined);
  } catch (err) {
    console.error("Lỗi khi tải khách hàng:", err);
  } finally {
    isCustomersLoading.value = false;
  }
}

async function loadSuppliers() {
  isSuppliersLoading.value = true;
  try {
    suppliers.value = await pharmacyApi.getNhaCC();
  } catch (err) {
    console.error("Lỗi khi tải nhà cung cấp:", err);
  } finally {
    isSuppliersLoading.value = false;
  }
}

async function loadLots() {
  isLotsLoading.value = true;
  try {
    lots.value = await pharmacyApi.getLoThuoc({ conHangOnly: conHangOnly.value });
  } catch (err) {
    console.error("Lỗi khi tải lô thuốc:", err);
  } finally {
    isLotsLoading.value = false;
  }
}

async function loadInvoices() {
  isInvoicesLoading.value = true;
  try {
    invoices.value = await pharmacyApi.getHoaDon();
  } catch (err) {
    console.error("Lỗi khi tải hóa đơn:", err);
  } finally {
    isInvoicesLoading.value = false;
  }
}

async function loadReceipts() {
  isReceiptsLoading.value = true;
  try {
    receipts.value = await pharmacyApi.getPhieuNhap();
  } catch (err) {
    console.error("Lỗi khi tải phiếu nhập:", err);
  } finally {
    isReceiptsLoading.value = false;
  }
}

async function loadAllData() {
  globalError.value = "";
  try {
    await Promise.all([
      loadStats(),
      loadRevenueChart(),
      loadCategories(),
      loadDrugs(),
      loadCustomers(),
      loadSuppliers(),
      loadLots(),
      loadInvoices(),
      loadReceipts(),
    ]);
  } catch (err: unknown) {
    globalError.value =
      err instanceof Error
        ? err.message
        : "Không thể kết nối đến máy chủ Backend C# (http://localhost:5238).";
  }
}

// CRUD Operations: Thuốc
function openCreateDrugModal() {
  drugToEdit.value = null;
  isDrugModalOpen.value = true;
}

function openEditDrugModal(drug: Thuoc) {
  drugToEdit.value = drug;
  isDrugModalOpen.value = true;
}

async function handleSaveDrug(payload: CreateThuocRequest, id?: string) {
  isSavingDrug.value = true;
  try {
    if (id) {
      await pharmacyApi.updateThuoc(id, payload as UpdateThuocRequest);
      showSuccess(`Cập nhật thuốc "${payload.tenThuoc}" thành công!`);
    } else {
      await pharmacyApi.createThuoc(payload);
      showSuccess(`Thêm thuốc "${payload.tenThuoc}" thành công!`);
    }
    isDrugModalOpen.value = false;
    await Promise.all([loadDrugs(), loadStats()]);
  } catch (err: unknown) {
    alert(err instanceof Error ? err.message : "Có lỗi xảy ra khi lưu thông tin thuốc.");
  } finally {
    isSavingDrug.value = false;
  }
}

async function handleDeleteDrug(drug: Thuoc) {
  if (!confirm(`Bạn có chắc muốn xóa thuốc "${drug.tenThuoc}" (${drug.maThuoc})?`)) return;
  try {
    await pharmacyApi.deleteThuoc(drug.maThuoc);
    showSuccess(`Đã xóa thuốc "${drug.tenThuoc}" thành công!`);
    await Promise.all([loadDrugs(), loadStats()]);
  } catch (err: unknown) {
    alert(err instanceof Error ? err.message : "Không thể xóa thuốc (có thể đã có phiếu nhập/hóa đơn).");
  }
}

// CRUD Operations: Loại thuốc
async function handleCreateCategory(payload: CreateLoaiThuocRequest) {
  try {
    await pharmacyApi.createLoaiThuoc(payload);
    showSuccess(`Thêm loại thuốc "${payload.tenLoai}" thành công!`);
    await loadCategories();
  } catch (err: unknown) {
    alert(err instanceof Error ? err.message : "Không thể thêm loại thuốc.");
  }
}

async function handleUpdateCategory(id: string, payload: CreateLoaiThuocRequest) {
  try {
    await pharmacyApi.updateLoaiThuoc(id, payload);
    showSuccess(`Cập nhật loại thuốc thành công!`);
    await loadCategories();
  } catch (err: unknown) {
    alert(err instanceof Error ? err.message : "Không thể cập nhật loại thuốc.");
  }
}

async function handleDeleteCategory(id: string) {
  try {
    await pharmacyApi.deleteLoaiThuoc(id);
    showSuccess(`Đã xóa loại thuốc ${id} thành công!`);
    await loadCategories();
  } catch (err: unknown) {
    alert(err instanceof Error ? err.message : "Không thể xóa loại thuốc này (có thể đang có thuốc trực thuộc).");
  }
}

// CRUD Operations: Khách hàng
async function handleCreateCustomer(payload: CreateKhachHangRequest) {
  try {
    await pharmacyApi.createKhachHang(payload);
    showSuccess(`Thêm khách hàng "${payload.tenKH}" thành công!`);
    await loadCustomers();
  } catch (err: unknown) {
    alert(err instanceof Error ? err.message : "Không thể thêm khách hàng.");
  }
}

async function handleUpdateCustomer(id: string, payload: CreateKhachHangRequest) {
  try {
    await pharmacyApi.updateKhachHang(id, payload);
    showSuccess(`Cập nhật thông tin khách hàng thành công!`);
    await loadCustomers();
  } catch (err: unknown) {
    alert(err instanceof Error ? err.message : "Không thể cập nhật khách hàng.");
  }
}

async function handleDeleteCustomer(id: string) {
  try {
    await pharmacyApi.deleteKhachHang(id);
    showSuccess(`Đã xóa khách hàng ${id} thành công!`);
    await loadCustomers();
  } catch (err: unknown) {
    alert(err instanceof Error ? err.message : "Không thể xóa khách hàng (đã có hóa đơn mua thuốc).");
  }
}

// CRUD Operations: Nhà cung cấp
async function handleCreateSupplier(payload: CreateNhaCCRequest) {
  try {
    await pharmacyApi.createNhaCC(payload);
    showSuccess(`Thêm nhà cung cấp "${payload.tenNhaCC}" thành công!`);
    await loadSuppliers();
  } catch (err: unknown) {
    alert(err instanceof Error ? err.message : "Không thể thêm nhà cung cấp.");
  }
}

async function handleUpdateSupplier(id: string, payload: CreateNhaCCRequest) {
  try {
    await pharmacyApi.updateNhaCC(id, payload);
    showSuccess(`Cập nhật nhà cung cấp thành công!`);
    await loadSuppliers();
  } catch (err: unknown) {
    alert(err instanceof Error ? err.message : "Không thể cập nhật nhà cung cấp.");
  }
}

async function handleDeleteSupplier(id: string) {
  try {
    await pharmacyApi.deleteNhaCC(id);
    showSuccess(`Đã xóa nhà cung cấp ${id} thành công!`);
    await loadSuppliers();
  } catch (err: unknown) {
    alert(err instanceof Error ? err.message : "Không thể xóa nhà cung cấp (đã có phiếu nhập kho).");
  }
}

// CRUD Operations: Lô thuốc (Tiêu hủy)
async function handleTieuHuyLot(maLo: string, tenThuoc: string, soLuong: number) {
  try {
    await pharmacyApi.tieuHuyLoThuoc(maLo);
    showSuccess(`Đã tiêu hủy ${soLuong} sản phẩm từ lô ${maLo} (${tenThuoc}) và trừ tồn kho!`);
    await Promise.all([loadLots(), loadDrugs(), loadStats()]);
  } catch (err: unknown) {
    alert(err instanceof Error ? err.message : "Có lỗi khi tiêu hủy lô thuốc.");
  }
}

// CRUD Operations: Hóa đơn (Bán hàng & tự trừ kho theo FEFO)
async function handleCreateInvoice(payload: CreateHoaDonRequest) {
  try {
    const created = await pharmacyApi.createHoaDon(payload);
    showSuccess(`Lập hóa đơn ${created.maHD} thành công! Kho hàng đã được tự động trừ theo FEFO.`);
    await Promise.all([
      loadInvoices(),
      loadDrugs(),
      loadLots(),
      loadStats(),
      loadRevenueChart(),
    ]);
  } catch (err: unknown) {
    alert(err instanceof Error ? err.message : "Không thể lập hóa đơn (kiểm tra tồn kho hoặc hạn sử dụng).");
  }
}

// CRUD Operations: Phiếu nhập (Nhập hàng, sinh lô & tăng tồn kho)
async function handleCreateReceipt(payload: CreatePhieuNhapRequest) {
  try {
    const created = await pharmacyApi.createPhieuNhap(payload);
    showSuccess(`Tạo phiếu nhập ${created.maPhieuNhap} thành công! Các lô mới đã được sinh và tăng tồn kho.`);
    await Promise.all([
      loadReceipts(),
      loadLots(),
      loadDrugs(),
      loadStats(),
    ]);
  } catch (err: unknown) {
    alert(err instanceof Error ? err.message : "Không thể tạo phiếu nhập kho.");
  }
}

// Watchers
let drugSearchTimer: ReturnType<typeof setTimeout> | null = null;
watch(drugSearchQuery, () => {
  if (drugSearchTimer) clearTimeout(drugSearchTimer);
  drugSearchTimer = setTimeout(() => {
    loadDrugs();
  }, 300);
});

let customerSearchTimer: ReturnType<typeof setTimeout> | null = null;
watch(customerSearchQuery, () => {
  if (customerSearchTimer) clearTimeout(customerSearchTimer);
  customerSearchTimer = setTimeout(() => {
    loadCustomers();
  }, 300);
});

watch(selectedDrugCategory, () => {
  loadDrugs();
});

watch(conHangOnly, () => {
  loadLots();
});

onMounted(() => {
  loadAllData();
});
</script>

<template>
  <div class="flex min-h-screen bg-neutral-50/60 dark:bg-neutral-950 text-foreground">
    <!-- Sidebar Navigation -->
    <Sidebar :current-tab="currentTab" @select-tab="currentTab = $event" />

    <!-- Main Workspace -->
    <div class="flex-1 flex flex-col min-w-0">
      <!-- Top Header -->
      <header class="sticky top-0 z-20 border-b bg-background/95 backdrop-blur-md px-6 py-3 flex items-center justify-between">
        <div class="flex items-center gap-3">
          <span class="inline-block w-2.5 h-2.5 rounded-full" :class="globalError ? 'bg-destructive animate-ping' : 'bg-emerald-500'" />
          <span class="text-xs font-medium text-muted-foreground">
            {{ globalError ? 'Mất kết nối Backend C#' : 'Backend C# kết nối thành công (http://localhost:5238)' }}
          </span>
        </div>

        <Button
          variant="outline"
          size="sm"
          class="text-xs"
          :disabled="isStatsLoading || isDrugsLoading"
          @click="loadAllData"
        >
          <RefreshCwIcon
            class="w-3.5 h-3.5 mr-1.5"
            :class="{ 'animate-spin': isStatsLoading || isDrugsLoading }"
          />
          Làm mới toàn bộ dữ liệu
        </Button>
      </header>

      <!-- Content Area -->
      <main class="flex-1 p-6 max-w-7xl w-full mx-auto space-y-6">
        <!-- Error notification banner -->
        <div
          v-if="globalError"
          class="border border-destructive/40 bg-destructive/10 text-destructive rounded-lg p-4 flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3 text-sm"
        >
          <div class="flex items-center gap-3">
            <AlertTriangleIcon class="w-5 h-5 shrink-0" />
            <div>
              <p class="font-medium">Lỗi kết nối API Backend</p>
              <p class="text-xs text-destructive/90 mt-0.5">{{ globalError }}</p>
            </div>
          </div>
          <Button size="sm" variant="destructive" @click="loadAllData">
            Thử kết nối lại
          </Button>
        </div>

        <!-- Success toast -->
        <div
          v-if="successMessage"
          class="border border-emerald-500/40 bg-emerald-50 dark:bg-emerald-950/40 text-emerald-700 dark:text-emerald-300 rounded-lg p-3.5 flex items-center gap-2 text-sm shadow-xs"
        >
          <CheckCircle2Icon class="w-4 h-4 shrink-0 text-emerald-600 dark:text-emerald-400" />
          <span>{{ successMessage }}</span>
        </div>

        <!-- Tab 1: Dashboard Overview & Chart -->
        <DashboardView
          v-if="currentTab === 'dashboard'"
          :stats="stats"
          :is-stats-loading="isStatsLoading"
          :revenue-chart-data="revenueChartData"
          :is-chart-loading="isChartLoading"
        />

        <!-- Tab 2: Thuốc CRUD -->
        <DrugView
          v-else-if="currentTab === 'thuoc'"
          :drugs="drugs"
          :categories="categories"
          :is-loading="isDrugsLoading"
          :search-query="drugSearchQuery"
          :selected-category="selectedDrugCategory"
          :is-modal-open="isDrugModalOpen"
          :drug-to-edit="drugToEdit"
          :is-saving="isSavingDrug"
          @update:search-query="drugSearchQuery = $event"
          @update:selected-category="selectedDrugCategory = $event"
          @update:modal-open="isDrugModalOpen = $event"
          @search="loadDrugs"
          @create-drug="openCreateDrugModal"
          @edit-drug="openEditDrugModal"
          @delete-drug="handleDeleteDrug"
          @open-category-manager="currentTab = 'loai-thuoc'"
          @save="handleSaveDrug"
        />

        <!-- Tab 3: Loại thuốc CRUD -->
        <LoaiThuocView
          v-else-if="currentTab === 'loai-thuoc'"
          :categories="categories"
          :is-loading="isCategoriesLoading"
          @create-category="handleCreateCategory"
          @update-category="handleUpdateCategory"
          @delete-category="handleDeleteCategory"
        />

        <!-- Tab 4: Khách hàng CRUD -->
        <KhachHangView
          v-else-if="currentTab === 'khach-hang'"
          :customers="customers"
          :is-loading="isCustomersLoading"
          v-model:search-query="customerSearchQuery"
          @search="loadCustomers"
          @create-customer="handleCreateCustomer"
          @update-customer="handleUpdateCustomer"
          @delete-customer="handleDeleteCustomer"
        />

        <!-- Tab 5: Nhà cung cấp CRUD -->
        <NhaCCView
          v-else-if="currentTab === 'nha-cc'"
          :suppliers="suppliers"
          :is-loading="isSuppliersLoading"
          @create-supplier="handleCreateSupplier"
          @update-supplier="handleUpdateSupplier"
          @delete-supplier="handleDeleteSupplier"
        />

        <!-- Tab 6: Lô thuốc & Tiêu hủy -->
        <LoThuocView
          v-else-if="currentTab === 'lo-thuoc'"
          :lots="lots"
          :is-loading="isLotsLoading"
          v-model:con-hang-only="conHangOnly"
          @tieu-huy="handleTieuHuyLot"
        />

        <!-- Tab 7: Hóa đơn bán lẻ & Tự trừ tồn kho (FEFO) -->
        <HoaDonView
          v-else-if="currentTab === 'hoa-don'"
          :invoices="invoices"
          :customers="customers"
          :drugs="drugs"
          :is-loading="isInvoicesLoading"
          @create-invoice="handleCreateInvoice"
        />

        <!-- Tab 8: Phiếu nhập kho & Tăng tồn kho -->
        <PhieuNhapView
          v-else-if="currentTab === 'phieu-nhap'"
          :receipts="receipts"
          :suppliers="suppliers"
          :drugs="drugs"
          :is-loading="isReceiptsLoading"
          @create-receipt="handleCreateReceipt"
        />
      </main>
    </div>
  </div>
</template>
