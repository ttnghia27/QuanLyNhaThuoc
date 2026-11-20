<script setup lang="ts">
import { ref, computed } from "vue";
import type { KhachHang, CreateKhachHangRequest } from "@/type";
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
import { UsersIcon, PlusIcon, PencilIcon, Trash2Icon, SearchIcon, XIcon } from "@lucide/vue";

interface KhachHangViewProps {
  customers: KhachHang[];
  isLoading: boolean;
  searchQuery: string;
}

const props = defineProps<KhachHangViewProps>();

const emit = defineEmits<{
  "update:searchQuery": [value: string];
  search: [];
  createCustomer: [payload: CreateKhachHangRequest];
  updateCustomer: [id: string, payload: CreateKhachHangRequest];
  deleteCustomer: [id: string];
}>();

const isModalOpen = ref(false);
const editingCustomer = ref<KhachHang | null>(null);
const tenKH = ref("");
const sdtKH = ref("");
const diaChiKH = ref("");
const formError = ref("");

const filteredCustomers = computed(() => {
  const q = props.searchQuery.trim().toLowerCase();
  if (!q) return props.customers;
  return props.customers.filter(
    (c) =>
      c.tenKH?.toLowerCase().includes(q) ||
      c.maKH?.toLowerCase().includes(q) ||
      (c.sdtKH && c.sdtKH.toLowerCase().includes(q)) ||
      (c.diaChiKH && c.diaChiKH.toLowerCase().includes(q))
  );
});

function handleClearSearch() {
  emit("update:searchQuery", "");
  emit("search");
}

function openCreateModal() {
  editingCustomer.value = null;
  tenKH.value = "";
  sdtKH.value = "";
  diaChiKH.value = "";
  formError.value = "";
  isModalOpen.value = true;
}

function openEditModal(c: KhachHang) {
  editingCustomer.value = c;
  tenKH.value = c.tenKH;
  sdtKH.value = c.sdtKH || "";
  diaChiKH.value = c.diaChiKH || "";
  formError.value = "";
  isModalOpen.value = true;
}

function handleSave() {
  if (!tenKH.value.trim()) {
    formError.value = "Vui lòng nhập tên khách hàng.";
    return;
  }

  const payload: CreateKhachHangRequest = {
    tenKH: tenKH.value.trim(),
    sdtKH: sdtKH.value.trim() || null,
    diaChiKH: diaChiKH.value.trim() || null,
  };

  if (editingCustomer.value) {
    emit("updateCustomer", editingCustomer.value.maKH, payload);
  } else {
    emit("createCustomer", payload);
  }
  isModalOpen.value = false;
}

function handleDelete(c: KhachHang) {
  if (confirm(`Bạn có chắc muốn xóa khách hàng "${c.tenKH}" (${c.maKH})?`)) {
    emit("deleteCustomer", c.maKH);
  }
}
</script>

<template>
  <Card class="border-border/60 shadow-xs">
    <CardHeader class="pb-3">
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <CardTitle class="text-base font-semibold flex items-center gap-2">
            <UsersIcon class="w-4 h-4 text-primary" />
            Bảng Dữ liệu: Khách hàng (KhachHang)
          </CardTitle>
          <CardDescription class="text-xs">
            Quản lý thông tin hồ sơ và số điện thoại liên lạc của khách hàng
          </CardDescription>
        </div>

        <Button size="sm" @click="openCreateModal" class="text-xs">
          <PlusIcon class="w-3.5 h-3.5 mr-1" />
          Thêm khách hàng
        </Button>
      </div>

      <!-- Search bar with clear button and search button -->
      <div class="pt-3 max-w-lg flex items-center gap-2">
        <div class="relative flex-1">
          <SearchIcon class="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" />
          <Input
            :model-value="searchQuery"
            @update:model-value="emit('update:searchQuery', String($event))"
            @keydown.enter="emit('search')"
            placeholder="Tìm theo tên khách hàng, số điện thoại, mã KH..."
            class="pl-9 pr-8 text-sm h-9"
          />
          <button
            v-if="searchQuery"
            type="button"
            @click="handleClearSearch"
            class="absolute right-2.5 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground"
            title="Xóa tìm kiếm"
          >
            <XIcon class="w-3.5 h-3.5" />
          </button>
        </div>

        <Button
          type="button"
          variant="secondary"
          size="sm"
          class="h-9 px-3 text-xs shrink-0"
          @click="emit('search')"
        >
          Tìm kiếm
        </Button>
      </div>
    </CardHeader>

    <CardContent>
      <div class="border rounded-md overflow-x-auto">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead class="w-28">Mã KH</TableHead>
              <TableHead>Họ và tên</TableHead>
              <TableHead class="w-40">Số điện thoại</TableHead>
              <TableHead>Địa chỉ</TableHead>
              <TableHead class="w-28 text-right">Thao tác</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            <TableRow v-if="isLoading">
              <TableCell colspan="5" class="text-center py-10 text-muted-foreground text-sm">
                Đang tải danh sách khách hàng...
              </TableCell>
            </TableRow>
            <TableRow v-else-if="customers.length === 0">
              <TableCell colspan="5" class="text-center py-10 text-muted-foreground text-sm">
                Chưa có khách hàng nào.
              </TableCell>
            </TableRow>
            <TableRow v-else-if="filteredCustomers.length === 0">
              <TableCell colspan="5" class="text-center py-10 text-muted-foreground text-sm">
                Không tìm thấy khách hàng nào phù hợp với từ khóa "{{ searchQuery }}".
              </TableCell>
            </TableRow>
            <TableRow v-else v-for="c in filteredCustomers" :key="c.maKH">
              <TableCell class="font-mono text-xs font-semibold text-primary">
                {{ c.maKH }}
              </TableCell>
              <TableCell class="font-medium text-sm">
                {{ c.tenKH }}
              </TableCell>
              <TableCell class="text-xs font-mono">
                {{ c.sdtKH || "—" }}
              </TableCell>
              <TableCell class="text-xs text-muted-foreground">
                {{ c.diaChiKH || "—" }}
              </TableCell>
              <TableCell class="text-right">
                <div class="inline-flex items-center justify-end gap-1">
                  <Button
                    variant="ghost"
                    size="icon-xs"
                    class="hover:text-primary"
                    title="Chỉnh sửa thông tin"
                    @click="openEditModal(c)"
                  >
                    <PencilIcon class="w-3.5 h-3.5" />
                  </Button>
                  <Button
                    variant="ghost"
                    size="icon-xs"
                    class="text-destructive hover:text-destructive hover:bg-destructive/10"
                    title="Xóa khách hàng"
                    @click="handleDelete(c)"
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
      <div class="flex items-center justify-between text-xs text-muted-foreground pt-3 px-1">
        <span>Hiển thị {{ filteredCustomers.length }} / {{ customers.length }} khách hàng</span>
        <span v-if="searchQuery">Đang lọc theo: "{{ searchQuery }}"</span>
      </div>
    </CardContent>

    <!-- Dialog Add/Edit -->
    <Dialog :open="isModalOpen" @update:open="isModalOpen = $event">
      <DialogContent class="sm:max-w-[460px]">
        <DialogHeader>
          <DialogTitle>{{ editingCustomer ? "Chỉnh sửa Khách hàng" : "Thêm Khách hàng mới" }}</DialogTitle>
          <DialogDescription>
            Điền các thông tin liên lạc của khách hàng.
          </DialogDescription>
        </DialogHeader>

        <form @submit.prevent="handleSave" class="space-y-4 py-2">
          <div v-if="formError" class="p-2 text-xs text-destructive bg-destructive/10 rounded">
            {{ formError }}
          </div>

          <div class="space-y-1.5">
            <Label for="tenKH">Họ và tên <span class="text-destructive">*</span></Label>
            <Input id="tenKH" v-model="tenKH" placeholder="Ví dụ: Nguyễn Văn A" required />
          </div>

          <div class="space-y-1.5">
            <Label for="sdtKH">Số điện thoại</Label>
            <Input id="sdtKH" v-model="sdtKH" placeholder="Ví dụ: 0901234567" />
          </div>

          <div class="space-y-1.5">
            <Label for="diaChiKH">Địa chỉ</Label>
            <Input id="diaChiKH" v-model="diaChiKH" placeholder="Ví dụ: Quận 1, TP.HCM" />
          </div>

          <DialogFooter class="pt-2">
            <Button type="button" variant="outline" @click="isModalOpen = false">
              Hủy
            </Button>
            <Button type="submit">
              {{ editingCustomer ? "Lưu thay đổi" : "Thêm mới" }}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  </Card>
</template>
