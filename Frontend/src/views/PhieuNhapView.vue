<script setup lang="ts">
import { ref, computed } from "vue";
import { onClickOutside } from "@vueuse/core";
import type {
  PhieuNhap,
  NhaCC,
  Thuoc,
  CreatePhieuNhapRequest,
  CreatePhieuNhapDetailItem,
} from "@/type";
import { pharmacyApi } from "@/services/api";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
} from "@/components/ui/card";
import {
  Table,
  TableHeader,
  TableRow,
  TableHead,
  TableBody,
  TableCell,
} from "@/components/ui/table";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from "@/components/ui/dialog";
import {
  Select,
  SelectTrigger,
  SelectValue,
  SelectContent,
  SelectItem,
} from "@/components/ui/select";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  FileDownIcon,
  PlusIcon,
  EyeIcon,
  Trash2Icon,
  InfoIcon,
} from "@lucide/vue";

interface PhieuNhapViewProps {
  receipts: PhieuNhap[];
  suppliers: NhaCC[];
  drugs: Thuoc[];
  isLoading: boolean;
}

const props = defineProps<PhieuNhapViewProps>();
type ReceiptSortOption = "dateDesc" | "dateAsc" | "totalAsc" | "totalDesc";

const sortOptions: { value: ReceiptSortOption; label: string }[] = [
  { value: "dateDesc", label: "Ngày nhập: mới nhất trước" },
  { value: "dateAsc", label: "Ngày nhập: cũ nhất trước" },
  { value: "totalAsc", label: "Tổng tiền: thấp đến cao" },
  { value: "totalDesc", label: "Tổng tiền: cao đến thấp" },
];

const sortBy = ref<ReceiptSortOption>("dateDesc");
const isSortMenuOpen = ref(false);
const sortMenuRef = ref<HTMLDivElement | null>(null);

const sortedReceipts = computed(() =>
  [...props.receipts].sort((a, b) => {
    switch (sortBy.value) {
      case "dateAsc":
        return new Date(a.ngayNhap).getTime() - new Date(b.ngayNhap).getTime();
      case "totalAsc":
        return a.tongTien - b.tongTien;
      case "totalDesc":
        return b.tongTien - a.tongTien;
      case "dateDesc":
      default:
        return new Date(b.ngayNhap).getTime() - new Date(a.ngayNhap).getTime();
    }
  }),
);

function selectSortOption(option: ReceiptSortOption) {
  sortBy.value = option;
  isSortMenuOpen.value = false;
}

onClickOutside(sortMenuRef, () => {
  isSortMenuOpen.value = false;
});
const emit = defineEmits<{
  createReceipt: [payload: CreatePhieuNhapRequest];
}>();

// Detail Modal
const isDetailOpen = ref(false);
const selectedReceipt = ref<PhieuNhap | null>(null);
const isLoadingDetail = ref(false);

async function openDetailModal(pn: PhieuNhap) {
  isDetailOpen.value = true;
  isLoadingDetail.value = true;
  selectedReceipt.value = pn;
  try {
    const full = await pharmacyApi.getPhieuNhapById(pn.maPhieuNhap);
    selectedReceipt.value = full;
  } catch (err) {
    console.error("Lỗi khi tải chi tiết phiếu nhập:", err);
  } finally {
    isLoadingDetail.value = false;
  }
}

// Create Modal
const isCreateOpen = ref(false);
const selectedSupplier = ref("");
const receiptDate = ref(new Date().toISOString().slice(0, 10));
const receiptItems = ref<CreatePhieuNhapDetailItem[]>([]);
const formError = ref("");

function defaultFutureDate(): string {
  const d = new Date();
  d.setFullYear(d.getFullYear() + 1);
  return d.toISOString().slice(0, 10);
}

function openCreateModal() {
  selectedSupplier.value =
    props.suppliers.length > 0 ? props.suppliers[0].maNhaCC : "";
  receiptDate.value = new Date().toISOString().slice(0, 10);
  receiptItems.value = [
    {
      maThuoc: props.drugs.length > 0 ? props.drugs[0].maThuoc : "",
      soLuongNhap: 100,
      donGiaNhap: 1000,
      hanSuDung: defaultFutureDate(),
    },
  ];
  formError.value = "";
  isCreateOpen.value = true;
}

function addItemRow() {
  receiptItems.value.push({
    maThuoc: props.drugs.length > 0 ? props.drugs[0].maThuoc : "",
    soLuongNhap: 100,
    donGiaNhap: 1000,
    hanSuDung: defaultFutureDate(),
  });
}

function removeItemRow(index: number) {
  if (receiptItems.value.length > 1) {
    receiptItems.value.splice(index, 1);
  }
}

function handleSaveReceipt() {
  formError.value = "";
  if (!selectedSupplier.value) {
    formError.value = "Vui lòng chọn nhà cung cấp.";
    return;
  }

  for (const item of receiptItems.value) {
    if (!item.maThuoc) {
      formError.value = "Vui lòng chọn thuốc cho tất cả các dòng.";
      return;
    }
    if (!item.soLuongNhap || item.soLuongNhap <= 0) {
      formError.value = "Số lượng nhập phải lớn hơn 0.";
      return;
    }
    if (!item.donGiaNhap || item.donGiaNhap <= 0) {
      formError.value = "Đơn giá nhập phải lớn hơn 0.";
      return;
    }
    if (!item.hanSuDung) {
      formError.value = "Vui lòng chọn hạn sử dụng cho lô nhập.";
      return;
    }
  }

  const payload: CreatePhieuNhapRequest = {
    maNhaCC: selectedSupplier.value,
    ngayNhap: receiptDate.value || null,
    chiTiet: receiptItems.value.map((i) => ({
      maThuoc: i.maThuoc,
      soLuongNhap: Number(i.soLuongNhap),
      donGiaNhap: Number(i.donGiaNhap),
      hanSuDung: i.hanSuDung,
    })),
  };

  emit("createReceipt", payload);
  isCreateOpen.value = false;
}

function formatCurrency(val?: number): string {
  if (val === undefined || val === null) return "0 ₫";
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
  }).format(val);
}

function formatDate(dateStr: string): string {
  try {
    const d = new Date(dateStr);
    return `${d.getDate().toString().padStart(2, "0")}/${(d.getMonth() + 1)
      .toString()
      .padStart(2, "0")}/${d.getFullYear()}`;
  } catch {
    return dateStr;
  }
}
</script>

<template>
  <Card class="border-border/60 shadow-xs">
    <CardHeader class="pb-3">
      <div
        class="flex flex-col sm:flex-row sm:items-center justify-between gap-4"
      >
        <div>
          <CardTitle class="text-base font-semibold flex items-center gap-2">
            <FileDownIcon class="w-4 h-4 text-primary" />
            Bảng Dữ liệu: Phiếu nhập kho (PhieuNhap & ChiTietPhieuNhap)
          </CardTitle>
          <CardDescription class="text-xs">
            Tạo phiếu nhập từ nhà cung cấp, sinh lô thuốc mới và tự động tăng
            tồn kho sản phẩm
          </CardDescription>
        </div>

        <Button size="sm" @click="openCreateModal" class="text-xs">
          <PlusIcon class="w-3.5 h-3.5 mr-1" />
          Tạo phiếu nhập mới
        </Button>
      </div>
    </CardHeader>

    <CardContent>
      <div class="mb-4 flex flex-col gap-3 sm:flex-row sm:items-end">
        <div
          ref="sortMenuRef"
          class="relative sm:ml-auto"
          @keydown.esc="isSortMenuOpen = false"
        >
          <Button
            type="button"
            variant="outline"
            size="sm"
            aria-haspopup="menu"
            aria-controls="receipt-sort-menu"
            :aria-expanded="isSortMenuOpen"
            @click="isSortMenuOpen = !isSortMenuOpen"
          >
            Sắp xếp
          </Button>
          <div
            v-if="isSortMenuOpen"
            id="receipt-sort-menu"
            role="menu"
            aria-label="Sắp xếp phiếu nhập"
            class="absolute right-0 z-50 mt-2 min-w-56 rounded-md border bg-popover p-1 text-popover-foreground shadow-md"
          >
            <button
              v-for="option in sortOptions"
              :key="option.value"
              type="button"
              role="menuitemradio"
              :aria-checked="sortBy === option.value"
              class="flex w-full items-center justify-between rounded-sm px-2 py-1.5 text-left text-sm hover:bg-accent hover:text-accent-foreground"
              @click="selectSortOption(option.value)"
            >
              {{ option.label }}
              <span v-if="sortBy === option.value" aria-hidden="true">✓</span>
            </button>
          </div>
        </div>
      </div>
      <div class="border rounded-md overflow-x-auto">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead class="w-28">Mã PN</TableHead>
              <TableHead class="w-36">Ngày nhập</TableHead>
              <TableHead>Nhà cung cấp</TableHead>
              <TableHead class="w-36 text-right">Tổng tiền</TableHead>
              <TableHead class="w-24 text-right">Chi tiết</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            <TableRow v-if="isLoading">
              <TableCell
                colspan="5"
                class="text-center py-10 text-muted-foreground text-sm"
              >
                Đang tải danh sách phiếu nhập...
              </TableCell>
            </TableRow>
            <TableRow v-else-if="receipts.length === 0">
              <TableCell
                colspan="5"
                class="text-center py-10 text-muted-foreground text-sm"
              >
                Chưa có phiếu nhập kho nào.
              </TableCell>
            </TableRow>
            <TableRow
              v-else
              v-for="pn in sortedReceipts"
              :key="pn.maPhieuNhap"
              class="hover:bg-muted/40 transition-colors"
            >
              <TableCell class="font-mono text-xs font-semibold text-primary">
                {{ pn.maPhieuNhap }}
              </TableCell>
              <TableCell class="text-xs">
                {{ formatDate(pn.ngayNhap) }}
              </TableCell>
              <TableCell class="font-medium text-xs">
                {{ pn.tenNhaCC }}
                <span class="font-mono text-muted-foreground"
                  >({{ pn.maNhaCC }})</span
                >
              </TableCell>
              <TableCell
                class="text-right font-semibold text-xs text-blue-600 dark:text-blue-400"
              >
                {{ formatCurrency(pn.tongTien) }}
              </TableCell>
              <TableCell class="text-right">
                <Button
                  variant="ghost"
                  size="icon-xs"
                  class="hover:text-primary"
                  title="Xem chi tiết các lô tạo bởi phiếu nhập này"
                  @click="openDetailModal(pn)"
                >
                  <EyeIcon class="w-3.5 h-3.5" />
                </Button>
              </TableCell>
            </TableRow>
          </TableBody>
        </Table>
      </div>
    </CardContent>

    <!-- Dialog: Xem chi tiết phiếu nhập -->
    <Dialog :open="isDetailOpen" @update:open="isDetailOpen = $event">
      <DialogContent class="sm:max-w-[640px]">
        <DialogHeader>
          <DialogTitle
            >Chi tiết Phiếu nhập:
            {{ selectedReceipt?.maPhieuNhap }}</DialogTitle
          >
          <DialogDescription>
            Nhà cung cấp: {{ selectedReceipt?.tenNhaCC }} &bull; Ngày nhập:
            {{
              selectedReceipt?.ngayNhap
                ? formatDate(selectedReceipt.ngayNhap)
                : ""
            }}
          </DialogDescription>
        </DialogHeader>

        <div class="py-2">
          <div class="border rounded-md overflow-x-auto max-h-72">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead class="w-20">Mã lô</TableHead>
                  <TableHead>Tên thuốc</TableHead>
                  <TableHead class="w-24 text-center">Hạn dùng</TableHead>
                  <TableHead class="w-20 text-center">SL Nhập</TableHead>
                  <TableHead class="w-24 text-right">Đơn giá nhập</TableHead>
                  <TableHead class="w-28 text-right">Thành tiền</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                <TableRow v-if="isLoadingDetail">
                  <TableCell
                    colspan="6"
                    class="text-center py-6 text-sm text-muted-foreground"
                  >
                    Đang tải chi tiết phiếu nhập...
                  </TableCell>
                </TableRow>
                <TableRow
                  v-else
                  v-for="(item, idx) in selectedReceipt?.chiTietLo || []"
                  :key="idx"
                >
                  <TableCell
                    class="font-mono text-xs font-semibold text-primary"
                    >{{ item.maLo }}</TableCell
                  >
                  <TableCell class="font-medium text-xs">{{
                    item.tenThuoc
                  }}</TableCell>
                  <TableCell class="text-center text-xs font-mono">{{
                    formatDate(item.hanSuDung)
                  }}</TableCell>
                  <TableCell class="text-center text-xs font-semibold">{{
                    item.soLuongNhap
                  }}</TableCell>
                  <TableCell class="text-right text-xs">{{
                    formatCurrency(item.donGiaNhap)
                  }}</TableCell>
                  <TableCell
                    class="text-right text-xs font-semibold text-blue-600"
                    >{{ formatCurrency(item.thanhTien) }}</TableCell
                  >
                </TableRow>
              </TableBody>
            </Table>
          </div>

          <div
            class="flex justify-between items-center pt-3 text-sm font-semibold"
          >
            <span>Tổng tiền nhập hàng:</span>
            <span class="text-base text-blue-600 dark:text-blue-400">
              {{ formatCurrency(selectedReceipt?.tongTien) }}
            </span>
          </div>
        </div>

        <DialogFooter>
          <Button variant="outline" @click="isDetailOpen = false">Đóng</Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>

    <!-- Dialog: Tạo phiếu nhập mới -->
    <Dialog :open="isCreateOpen" @update:open="isCreateOpen = $event">
      <DialogContent class="sm:max-w-[620px]">
        <DialogHeader>
          <DialogTitle>Tạo Phiếu nhập kho mới</DialogTitle>
          <DialogDescription>
            Nhập hàng từ nhà cung cấp. Mỗi dòng sẽ tự động tạo một lô thuốc
            trong kho và cộng dồn tồn kho sản phẩm.
          </DialogDescription>
        </DialogHeader>

        <form @submit.prevent="handleSaveReceipt" class="space-y-4 py-2">
          <div
            v-if="formError"
            class="p-2.5 text-xs text-destructive bg-destructive/10 rounded"
          >
            {{ formError }}
          </div>

          <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <div class="space-y-1.5">
              <Label for="maNhaCC"
                >Nhà cung cấp <span class="text-destructive">*</span></Label
              >
              <Select v-model="selectedSupplier">
                <SelectTrigger id="maNhaCC" class="w-full">
                  <SelectValue placeholder="Chọn nhà cung cấp" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem
                    v-for="ncc in suppliers"
                    :key="ncc.maNhaCC"
                    :value="ncc.maNhaCC"
                  >
                    {{ ncc.tenNhaCC }} ({{ ncc.maNhaCC }})
                  </SelectItem>
                </SelectContent>
              </Select>
            </div>

            <div class="space-y-1.5">
              <Label for="ngayNhap">Ngày nhập kho</Label>
              <Input id="ngayNhap" type="date" v-model="receiptDate" />
            </div>
          </div>

          <!-- Items list -->
          <div class="space-y-2">
            <div class="flex items-center justify-between">
              <Label>Các mặt hàng & lô nhập</Label>
              <Button
                type="button"
                variant="outline"
                size="xs"
                @click="addItemRow"
              >
                <PlusIcon class="w-3 h-3 mr-1" />
                Thêm dòng
              </Button>
            </div>

            <div class="space-y-2 max-h-56 overflow-y-auto pr-1">
              <div
                v-for="(row, idx) in receiptItems"
                :key="idx"
                class="flex flex-col sm:flex-row items-center gap-2 border p-2.5 rounded-md bg-muted/20"
              >
                <!-- Drug Select -->
                <div class="w-full sm:flex-1">
                  <Select v-model="row.maThuoc">
                    <SelectTrigger class="h-8 text-xs">
                      <SelectValue placeholder="Chọn thuốc" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem
                        v-for="d in drugs"
                        :key="d.maThuoc"
                        :value="d.maThuoc"
                      >
                        {{ d.tenThuoc }} ({{ d.maThuoc }})
                      </SelectItem>
                    </SelectContent>
                  </Select>
                </div>

                <!-- Quantity -->
                <div class="w-full sm:w-20">
                  <Input
                    type="number"
                    min="1"
                    v-model="row.soLuongNhap"
                    placeholder="SL"
                    class="h-8 text-xs text-center"
                    required
                  />
                </div>

                <!-- Unit Cost -->
                <div class="w-full sm:w-24">
                  <Input
                    type="number"
                    min="1000"
                    step="1000"
                    v-model="row.donGiaNhap"
                    placeholder="Giá nhập"
                    class="h-8 text-xs text-right"
                    required
                  />
                </div>

                <!-- Expiry Date -->
                <div class="w-full sm:w-28">
                  <Input
                    type="date"
                    v-model="row.hanSuDung"
                    class="h-8 text-xs"
                    required
                  />
                </div>

                <!-- Remove button -->
                <Button
                  type="button"
                  variant="ghost"
                  size="icon-xs"
                  class="text-destructive hover:bg-destructive/10"
                  :disabled="receiptItems.length <= 1"
                  @click="removeItemRow(idx)"
                >
                  <Trash2Icon class="w-3.5 h-3.5" />
                </Button>
              </div>
            </div>
          </div>

          <div
            class="p-2.5 rounded-md bg-blue-50 dark:bg-blue-950/40 text-blue-700 dark:text-blue-300 text-xs flex gap-2 items-start"
          >
            <InfoIcon class="w-4 h-4 shrink-0 mt-0.5" />
            <span>
              Mỗi dòng thuốc nhập trên phiếu sẽ tự động sinh một mã lô (LoThuoc)
              và cập nhật số lượng tồn kho của thuốc tương ứng.
            </span>
          </div>

          <DialogFooter class="pt-2">
            <Button
              type="button"
              variant="outline"
              @click="isCreateOpen = false"
            >
              Hủy
            </Button>
            <Button type="submit"> Lưu phiếu nhập & Tăng tồn kho </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  </Card>
</template>
