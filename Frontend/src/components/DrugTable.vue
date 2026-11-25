<script setup lang="ts">
import { computed, ref } from "vue";
import { onClickOutside } from "@vueuse/core";
import type { Thuoc, LoaiThuoc } from "@/type";
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
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Badge } from "@/components/ui/badge";
import {
  Select,
  SelectTrigger,
  SelectValue,
  SelectContent,
  SelectItem,
} from "@/components/ui/select";
import {
  SearchIcon,
  PlusIcon,
  PencilIcon,
  Trash2Icon,
  PillIcon,
} from "@lucide/vue";

interface DrugTableProps {
  drugs: Thuoc[];
  categories: LoaiThuoc[];
  isLoading: boolean;
  selectedCategory: string;
  searchQuery: string;
}

const props = defineProps<DrugTableProps>();

type DrugSortOption =
  | "salePriceAsc"
  | "salePriceDesc"
  | "purchasePriceAsc"
  | "purchasePriceDesc"
  | "stockAsc"
  | "stockDesc";

const sortOptions: { value: DrugSortOption; label: string }[] = [
  { value: "salePriceAsc", label: "Giá bán: thấp đến cao" },
  { value: "salePriceDesc", label: "Giá bán: cao đến thấp" },
  { value: "purchasePriceAsc", label: "Giá nhập: thấp đến cao" },
  { value: "purchasePriceDesc", label: "Giá nhập: cao đến thấp" },
  { value: "stockAsc", label: "Tồn kho: thấp đến cao" },
  { value: "stockDesc", label: "Tồn kho: cao đến thấp" },
];

const sortBy = ref<DrugSortOption>("salePriceAsc");
const isSortMenuOpen = ref(false);
const sortMenuRef = ref<HTMLDivElement | null>(null);

const emit = defineEmits<{
  "update:searchQuery": [value: string];
  "update:selectedCategory": [value: string];
  search: [];
  createDrug: [];
  editDrug: [drug: Thuoc];
  deleteDrug: [drug: Thuoc];
  openCategoryManager: [];
}>();

const filteredDrugs = computed(() => {
  const q = props.searchQuery.trim().toLowerCase();
  if (!q) return props.drugs;
  return props.drugs.filter(
    (drug) =>
      drug.tenThuoc?.toLowerCase().includes(q) ||
      drug.maThuoc?.toLowerCase().includes(q) ||
      drug.congDung?.toLowerCase().includes(q) ||
      drug.tenLoai?.toLowerCase().includes(q),
  );
});

function compareDrugCode(a: Thuoc, b: Thuoc, direction: 1 | -1) {
  return (
    a.maThuoc.localeCompare(b.maThuoc, undefined, { numeric: true }) *
    direction
  );
}

const sortedDrugs = computed(() =>
  [...filteredDrugs.value].sort((a, b) => {
    switch (sortBy.value) {
      case "salePriceDesc":
        return b.donGiaBan - a.donGiaBan || compareDrugCode(a, b, -1);
      case "purchasePriceAsc":
        return a.giaNhapDuKien - b.giaNhapDuKien || compareDrugCode(a, b, 1);
      case "purchasePriceDesc":
        return b.giaNhapDuKien - a.giaNhapDuKien || compareDrugCode(a, b, -1);
      case "stockAsc":
        return a.soLuongTonKho - b.soLuongTonKho || compareDrugCode(a, b, 1);
      case "stockDesc":
        return b.soLuongTonKho - a.soLuongTonKho || compareDrugCode(a, b, -1);
      case "salePriceAsc":
      default:
        return a.donGiaBan - b.donGiaBan || compareDrugCode(a, b, 1);
    }
  }),
);

function selectSortOption(option: DrugSortOption) {
  sortBy.value = option;
  isSortMenuOpen.value = false;
}

onClickOutside(sortMenuRef, () => {
  isSortMenuOpen.value = false;
});

function formatCurrency(val: number): string {
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
  }).format(val);
}
</script>

<template>
  <Card class="border-border/60 shadow-xs">
    <CardHeader class="pb-3">
      <div
        class="flex flex-col md:flex-row md:items-center justify-between gap-4"
      >
        <div>
          <CardTitle class="text-base font-semibold flex items-center gap-2">
            <PillIcon class="w-4 h-4 text-primary" />
            Danh mục Quản lý Thuốc
          </CardTitle>
          <CardDescription class="text-xs">
            Xem danh sách, tìm kiếm, lọc theo loại và thực hiện các thao tác
            CRUD thuốc
          </CardDescription>
        </div>

        <!-- Action buttons -->
        <div class="flex items-center gap-2">
          <div
            ref="sortMenuRef"
            class="relative"
            @keydown.esc="isSortMenuOpen = false"
          >
            <Button
              type="button"
              variant="outline"
              size="sm"
              aria-haspopup="menu"
              aria-controls="drug-sort-menu"
              :aria-expanded="isSortMenuOpen"
              @click="isSortMenuOpen = !isSortMenuOpen"
            >
              Sắp xếp
            </Button>
            <div
              v-if="isSortMenuOpen"
              id="drug-sort-menu"
              role="menu"
              aria-label="Sắp xếp thuốc"
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
          <Button size="sm" @click="emit('createDrug')" class="text-xs">
            <PlusIcon class="w-3.5 h-3.5 mr-1" />
            Thêm thuốc mới
          </Button>
        </div>
      </div>

      <!-- Filters & Search -->
      <div class="grid grid-cols-1 sm:grid-cols-3 gap-3">
        <div class="sm:col-span-2 flex items-center gap-2">
          <div class="relative flex-1">
            <SearchIcon
              class="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground"
            />
            <Input
              :model-value="searchQuery"
              @update:model-value="emit('update:searchQuery', String($event))"
              @keydown.enter="emit('search')"
              placeholder="Tìm theo tên thuốc, mã thuốc, công dụng..."
              class="pl-9 pr-8 text-sm h-9"
            />
          </div>

          <!-- <Button
                        type="button"
                        variant="secondary"
                        size="sm"
                        class="h-9 px-3 text-xs shrink-0"
                        @click="emit('search')"
                    >
                        Tìm kiếm
                    </Button> -->
        </div>

        <!-- Category filter -->
        <div>
          <Select
            :model-value="selectedCategory"
            @update:model-value="
              emit('update:selectedCategory', String($event))
            "
          >
            <SelectTrigger class="h-9 text-sm w-full">
              <SelectValue placeholder="Lọc theo tất cả phân loại" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="ALL">Tất cả phân loại</SelectItem>
              <SelectItem
                v-for="cat in categories"
                :key="cat.maLoai"
                :value="cat.maLoai"
              >
                {{ cat.tenLoai }}
              </SelectItem>
            </SelectContent>
          </Select>
        </div>
      </div>
    </CardHeader>

    <CardContent>
      <!-- Table wrapper -->
      <div class="border rounded-md overflow-x-auto">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead class="w-24">Mã thuốc</TableHead>
              <TableHead class="min-w-45">Tên thuốc</TableHead>
              <TableHead class="w-36">Phân loại</TableHead>
              <TableHead class="min-w-45">Công dụng</TableHead>
              <TableHead class="w-32 text-right">Đơn giá bán</TableHead>
              <TableHead class="w-32 text-right"
                >Đơn giá nhập gần nhất</TableHead
              >
              <TableHead class="w-28 text-center">Tồn kho</TableHead>
              <TableHead class="w-28 text-right">Hành động</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            <!-- Loading state -->
            <TableRow v-if="isLoading">
              <TableCell
                colspan="7"
                class="text-center py-10 text-muted-foreground text-sm"
              >
                <div class="inline-flex items-center gap-2">
                  <div
                    class="w-4 h-4 border-2 border-primary border-t-transparent rounded-full animate-spin"
                  />
                  Đang tải dữ liệu thuốc...
                </div>
              </TableCell>
            </TableRow>

            <!-- Empty state -->
            <TableRow v-else-if="drugs.length === 0">
              <TableCell
                colspan="7"
                class="text-center py-10 text-muted-foreground text-sm"
              >
                Chưa có thuốc nào trong hệ thống.
              </TableCell>
            </TableRow>

            <TableRow v-else-if="filteredDrugs.length === 0">
              <TableCell
                colspan="7"
                class="text-center py-10 text-muted-foreground text-sm"
              >
                Không tìm thấy thuốc nào khớp với từ khóa "{{ searchQuery }}".
              </TableCell>
            </TableRow>

            <!-- Data rows -->
            <TableRow
              v-else
              v-for="drug in sortedDrugs"
              :key="drug.maThuoc"
              class="hover:bg-muted/40 transition-colors"
            >
              <TableCell class="font-mono text-xs font-semibold text-primary">
                {{ drug.maThuoc }}
              </TableCell>
              <TableCell class="font-medium text-sm">
                {{ drug.tenThuoc }}
              </TableCell>
              <TableCell>
                <Badge variant="outline" class="text-xs font-normal">
                  {{ drug.tenLoai || drug.maLoai }}
                </Badge>
              </TableCell>
              <TableCell
                class="text-xs text-muted-foreground line-clamp-1"
                :title="drug.congDung"
              >
                {{ drug.congDung }}
              </TableCell>
              <TableCell class="text-right font-medium text-sm text-foreground">
                {{ formatCurrency(drug.donGiaBan) }}
              </TableCell>
              <TableCell class="text-right font-medium text-sm text-foreground">
                {{ formatCurrency(drug.giaNhapDuKien) }}
              </TableCell>
              <TableCell class="text-center">
                <Badge
                  :variant="
                    drug.soLuongTonKho <= 10 ? 'destructive' : 'secondary'
                  "
                  class="text-xs font-semibold"
                >
                  {{ drug.soLuongTonKho }} sp
                </Badge>
              </TableCell>
              <TableCell class="text-right">
                <div class="inline-flex items-center justify-end gap-1">
                  <Button
                    variant="ghost"
                    size="icon-xs"
                    class="hover:text-primary"
                    title="Chỉnh sửa thông tin"
                    @click="emit('editDrug', drug)"
                  >
                    <PencilIcon class="w-3.5 h-3.5" />
                  </Button>
                  <Button
                    variant="ghost"
                    size="icon-xs"
                    class="text-destructive hover:text-destructive hover:bg-destructive/10"
                    title="Xóa thuốc"
                    @click="emit('deleteDrug', drug)"
                  >
                    <Trash2Icon class="w-3.5 h-3.5" />
                  </Button>
                </div>
              </TableCell>
            </TableRow>
          </TableBody>
        </Table>
      </div>

      <!-- Table footer count -->
      <div
        class="flex items-center justify-between text-xs text-muted-foreground pt-3 px-1"
      >
        <span
          >Hiển thị {{ filteredDrugs.length }} / {{ drugs.length }} sản
          phẩm</span
        >
        <span v-if="searchQuery">Đang lọc theo: "{{ searchQuery }}"</span>
        <span v-else-if="selectedCategory !== 'ALL'"
          >Đang lọc theo phân loại</span
        >
      </div>
    </CardContent>
  </Card>
</template>
