<script setup lang="ts">
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
  FolderPlusIcon,
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

const emit = defineEmits<{
  "update:searchQuery": [value: string];
  "update:selectedCategory": [value: string];
  search: [];
  createDrug: [];
  editDrug: [drug: Thuoc];
  deleteDrug: [drug: Thuoc];
  openCategoryManager: [];
}>();

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
      <div class="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <CardTitle class="text-base font-semibold flex items-center gap-2">
            <PillIcon class="w-4 h-4 text-primary" />
            Danh mục Quản lý Thuốc
          </CardTitle>
          <CardDescription class="text-xs">
            Xem danh sách, tìm kiếm, lọc theo loại và thực hiện các thao tác CRUD thuốc
          </CardDescription>
        </div>

        <!-- Action buttons -->
        <div class="flex items-center gap-2">
          <Button
            variant="outline"
            size="sm"
            @click="emit('openCategoryManager')"
            class="text-xs"
          >
            <FolderPlusIcon class="w-3.5 h-3.5 mr-1" />
            Quản lý loại thuốc
          </Button>
          <Button
            size="sm"
            @click="emit('createDrug')"
            class="text-xs"
          >
            <PlusIcon class="w-3.5 h-3.5 mr-1" />
            Thêm thuốc mới
          </Button>
        </div>
      </div>

      <!-- Filters & Search -->
      <div class="grid grid-cols-1 sm:grid-cols-3 gap-3 pt-4">
        <!-- Search input -->
        <div class="sm:col-span-2 relative">
          <SearchIcon class="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" />
          <Input
            :model-value="searchQuery"
            @update:model-value="emit('update:searchQuery', String($event))"
            @keydown.enter="emit('search')"
            placeholder="Tìm theo tên thuốc, mã thuốc hoặc công dụng..."
            class="pl-9 text-sm h-9"
          />
        </div>

        <!-- Category filter -->
        <div>
          <Select
            :model-value="selectedCategory"
            @update:model-value="emit('update:selectedCategory', String($event))"
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
              <TableHead class="min-w-[180px]">Tên thuốc</TableHead>
              <TableHead class="w-36">Phân loại</TableHead>
              <TableHead class="min-w-[160px]">Công dụng</TableHead>
              <TableHead class="w-32 text-right">Đơn giá bán</TableHead>
              <TableHead class="w-28 text-center">Tồn kho</TableHead>
              <TableHead class="w-28 text-right">Hành động</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            <!-- Loading state -->
            <TableRow v-if="isLoading">
              <TableCell colspan="7" class="text-center py-10 text-muted-foreground text-sm">
                <div class="inline-flex items-center gap-2">
                  <div class="w-4 h-4 border-2 border-primary border-t-transparent rounded-full animate-spin" />
                  Đang tải dữ liệu thuốc...
                </div>
              </TableCell>
            </TableRow>

            <!-- Empty state -->
            <TableRow v-else-if="drugs.length === 0">
              <TableCell colspan="7" class="text-center py-10 text-muted-foreground text-sm">
                Không tìm thấy thuốc nào phù hợp với bộ lọc hiện tại.
              </TableCell>
            </TableRow>

            <!-- Data rows -->
            <TableRow v-else v-for="drug in drugs" :key="drug.maThuoc" class="hover:bg-muted/40 transition-colors">
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
              <TableCell class="text-xs text-muted-foreground line-clamp-1" :title="drug.congDung">
                {{ drug.congDung }}
              </TableCell>
              <TableCell class="text-right font-medium text-sm text-foreground">
                {{ formatCurrency(drug.donGiaBan) }}
              </TableCell>
              <TableCell class="text-center">
                <Badge
                  :variant="drug.soLuongTonKho <= 10 ? 'destructive' : 'secondary'"
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
      <div class="flex items-center justify-between text-xs text-muted-foreground pt-3 px-1">
        <span>Hiển thị {{ drugs.length }} sản phẩm</span>
        <span v-if="selectedCategory !== 'ALL'">Đang lọc theo phân loại</span>
      </div>
    </CardContent>
  </Card>
</template>
