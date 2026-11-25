<script setup lang="ts">
import { ref, computed } from "vue";
import type { LoaiThuoc, CreateLoaiThuocRequest } from "@/type";
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
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";
import {
  PlusIcon,
  PencilIcon,
  Trash2Icon,
  TagsIcon,
  SearchIcon,
} from "@lucide/vue";

interface LoaiThuocViewProps {
  categories: LoaiThuoc[];
  isLoading: boolean;
}

const props = defineProps<LoaiThuocViewProps>();

const emit = defineEmits<{
  createCategory: [payload: CreateLoaiThuocRequest];
  updateCategory: [id: string, payload: CreateLoaiThuocRequest];
  deleteCategory: [id: string];
}>();

const searchQuery = ref("");
const isModalOpen = ref(false);
const editingCategory = ref<LoaiThuoc | null>(null);
const tenLoai = ref("");
const formError = ref("");

const filteredCategories = computed(() => {
  const q = searchQuery.value.trim().toLowerCase();
  if (!q) return props.categories;
  return props.categories.filter(
    (cat) =>
      cat.tenLoai.toLowerCase().includes(q) ||
      cat.maLoai.toLowerCase().includes(q),
  );
});

function openCreateModal() {
  editingCategory.value = null;
  tenLoai.value = "";
  formError.value = "";
  isModalOpen.value = true;
}

function openEditModal(cat: LoaiThuoc) {
  editingCategory.value = cat;
  tenLoai.value = cat.tenLoai;
  formError.value = "";
  isModalOpen.value = true;
}

function handleSave() {
  if (!tenLoai.value.trim()) {
    formError.value = "Vui lòng nhập tên loại thuốc.";
    return;
  }

  const payload: CreateLoaiThuocRequest = {
    tenLoai: tenLoai.value.trim(),
  };

  if (editingCategory.value) {
    emit("updateCategory", editingCategory.value.maLoai, payload);
  } else {
    emit("createCategory", payload);
  }
  isModalOpen.value = false;
}

function handleDelete(cat: LoaiThuoc) {
  if (
    confirm(`Bạn có chắc muốn xóa loại thuốc "${cat.tenLoai}" (${cat.maLoai})?`)
  ) {
    emit("deleteCategory", cat.maLoai);
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
            <TagsIcon class="w-4 h-4 text-primary" />
            Bảng Dữ liệu: Phân loại Thuốc (LoaiThuoc)
          </CardTitle>
          <CardDescription class="text-xs">
            Quản lý các nhóm danh mục thuốc trong nhà thuốc (CRUD bảng
            LoaiThuoc)
          </CardDescription>
        </div>

        <Button size="sm" @click="openCreateModal" class="text-xs">
          <PlusIcon class="w-3.5 h-3.5 mr-1" />
          Thêm phân loại
        </Button>
      </div>

      Ô tìm kiếm loại thuốc
      <div class="pt-3 max-w-md relative">
        <SearchIcon
          class="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground"
        />
        <Input
          v-model="searchQuery"
          placeholder="Tìm theo tên loại thuốc hoặc mã loại..."
          class="pl-9 text-sm h-9"
        />
      </div>
    </CardHeader>

    <CardContent>
      <div class="border rounded-md overflow-x-auto">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead class="w-28">Mã loại</TableHead>
              <TableHead>Tên loại thuốc</TableHead>
              <TableHead class="w-36 text-center">Số lượng mặt hàng</TableHead>
              <TableHead class="w-28 text-right">Thao tác</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            <TableRow v-if="isLoading">
              <TableCell
                colspan="4"
                class="text-center py-10 text-muted-foreground text-sm"
              >
                Đang tải danh mục loại thuốc...
              </TableCell>
            </TableRow>
            <TableRow v-else-if="categories.length === 0">
              <TableCell
                colspan="4"
                class="text-center py-10 text-muted-foreground text-sm"
              >
                Chưa có loại thuốc nào.
              </TableCell>
            </TableRow>
            <TableRow v-else-if="filteredCategories.length === 0">
              <TableCell
                colspan="4"
                class="text-center py-10 text-muted-foreground text-sm"
              >
                Không tìm thấy loại thuốc nào phù hợp với từ khóa "{{
                  searchQuery
                }}".
              </TableCell>
            </TableRow>
            <TableRow
              v-else
              v-for="cat in filteredCategories"
              :key="cat.maLoai"
              class="hover:bg-muted/40 transition-colors"
            >
              <TableCell class="font-mono text-xs font-semibold text-primary">
                {{ cat.maLoai }}
              </TableCell>
              <TableCell class="font-medium text-sm">
                {{ cat.tenLoai }}
              </TableCell>
              <TableCell class="text-center">
                <Badge variant="secondary" class="text-xs">
                  {{ cat.soLuongThuoc ?? 0 }} mặt hàng
                </Badge>
              </TableCell>
              <TableCell class="text-right">
                <div class="inline-flex items-center justify-end gap-1">
                  <Button
                    variant="ghost"
                    size="icon-xs"
                    class="hover:text-primary"
                    title="Chỉnh sửa tên loại"
                    @click="openEditModal(cat)"
                  >
                    <PencilIcon class="w-3.5 h-3.5" />
                  </Button>
                  <Button
                    variant="ghost"
                    size="icon-xs"
                    class="text-destructive hover:text-destructive hover:bg-destructive/10"
                    title="Xóa loại thuốc"
                    @click="handleDelete(cat)"
                  >
                    <Trash2Icon class="w-3.5 h-3.5" />
                  </Button>
                </div>
              </TableCell>
            </TableRow>
          </TableBody>
        </Table>
      </div>

      <!-- Count footer -->
      <div
        class="flex items-center justify-between text-xs text-muted-foreground pt-3 px-1"
      >
        <span
          >Hiển thị {{ filteredCategories.length }} /
          {{ categories.length }} loại thuốc</span
        >
        <span v-if="searchQuery">Đang lọc theo: "{{ searchQuery }}"</span>
      </div>
    </CardContent>

    <!-- Dialog Add/Edit -->
    <Dialog :open="isModalOpen" @update:open="isModalOpen = $event">
      <DialogContent class="sm:max-w-[420px]">
        <DialogHeader>
          <DialogTitle>{{
            editingCategory ? "Chỉnh sửa Loại thuốc" : "Thêm Loại thuốc mới"
          }}</DialogTitle>
          <DialogDescription>
            Nhập tên phân loại thuốc để quản lý danh mục.
          </DialogDescription>
        </DialogHeader>

        <form @submit.prevent="handleSave" class="space-y-4 py-2">
          <div
            v-if="formError"
            class="p-2 text-xs text-destructive bg-destructive/10 rounded"
          >
            {{ formError }}
          </div>

          <div class="space-y-1.5">
            <Label for="tenLoai"
              >Tên loại thuốc <span class="text-destructive">*</span></Label
            >
            <Input
              id="tenLoai"
              v-model="tenLoai"
              placeholder="Ví dụ: Thuốc kháng sinh, Giảm đau..."
              required
            />
          </div>

          <DialogFooter class="pt-2">
            <Button
              type="button"
              variant="outline"
              @click="isModalOpen = false"
            >
              Hủy
            </Button>
            <Button type="submit">
              {{ editingCategory ? "Lưu thay đổi" : "Thêm mới" }}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  </Card>
</template>
