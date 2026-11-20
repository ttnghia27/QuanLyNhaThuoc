<script setup lang="ts">
import {
  LayoutDashboardIcon,
  PillIcon,
  TagsIcon,
  UsersIcon,
  TruckIcon,
  BoxesIcon,
  ReceiptIcon,
  FileDownIcon,
} from "@lucide/vue";

export type NavTab =
  | "dashboard"
  | "thuoc"
  | "loai-thuoc"
  | "khach-hang"
  | "nha-cc"
  | "lo-thuoc"
  | "hoa-don"
  | "phieu-nhap";

interface SidebarProps {
  currentTab: NavTab;
}

defineProps<SidebarProps>();

const emit = defineEmits<{
  selectTab: [tab: NavTab];
}>();

const menuItems: { id: NavTab; label: string; icon: any; countKey?: string; description: string }[] = [
  {
    id: "dashboard",
    label: "Tổng quan & Doanh thu",
    icon: LayoutDashboardIcon,
    description: "Thống kê KPI & Biểu đồ doanh thu",
  },
  {
    id: "thuoc",
    label: "Quản lý Thuốc",
    icon: PillIcon,
    description: "Danh mục thuốc, giá bán, tồn kho",
  },
  {
    id: "loai-thuoc",
    label: "Loại thuốc",
    icon: TagsIcon,
    description: "Nhóm & phân loại dược phẩm",
  },
  {
    id: "khach-hang",
    label: "Khách hàng",
    icon: UsersIcon,
    description: "Hồ sơ & số điện thoại khách hàng",
  },
  {
    id: "nha-cc",
    label: "Nhà cung cấp",
    icon: TruckIcon,
    description: "Công ty dược phẩm & đối tác",
  },
  {
    id: "lo-thuoc",
    label: "Lô thuốc & Hạn dùng",
    icon: BoxesIcon,
    description: "Theo dõi hạn sử dụng, tiêu hủy",
  },
  {
    id: "hoa-don",
    label: "Hóa đơn bán lẻ",
    icon: ReceiptIcon,
    description: "Bán hàng & xuất trừ kho (FEFO)",
  },
  {
    id: "phieu-nhap",
    label: "Phiếu nhập kho",
    icon: FileDownIcon,
    description: "Nhập hàng từ NCC & tăng tồn kho",
  },
];
</script>

<template>
  <aside class="w-64 bg-card border-r border-border shrink-0 flex flex-col justify-between h-screen sticky top-0">
    <div class="p-4 flex flex-col gap-6">
      <!-- Brand Logo -->
      <div class="flex items-center gap-3 px-2">
        <div class="w-9 h-9 rounded-lg bg-primary text-primary-foreground flex items-center justify-center font-bold shadow-xs">
          <PillIcon class="w-5 h-5" />
        </div>
        <div>
          <h2 class="text-sm font-bold tracking-tight">PharmaCare</h2>
          <p class="text-[11px] text-muted-foreground">Hệ thống Quản lý Nhà thuốc</p>
        </div>
      </div>

      <!-- Navigation Links -->
      <nav class="space-y-1">
        <p class="px-2 text-[10px] font-semibold text-muted-foreground uppercase tracking-wider mb-2">
          Danh mục & Dữ liệu
        </p>

        <button
          v-for="item in menuItems"
          :key="item.id"
          type="button"
          @click="emit('selectTab', item.id)"
          class="w-full flex items-center gap-3 px-3 py-2.5 rounded-lg text-xs font-medium transition-colors text-left"
          :class="[
            currentTab === item.id
              ? 'bg-primary text-primary-foreground shadow-xs font-semibold'
              : 'text-muted-foreground hover:bg-muted hover:text-foreground',
          ]"
        >
          <component :is="item.icon" class="w-4 h-4 shrink-0" />
          <div class="flex-1 truncate">
            <div>{{ item.label }}</div>
          </div>
        </button>
      </nav>
    </div>

    <!-- Sidebar footer -->
    <div class="p-4 border-t border-border/80 text-[11px] text-muted-foreground/80 flex items-center justify-between">
      <span>API: http://localhost:5238</span>
      <span class="inline-block w-2 h-2 rounded-full bg-emerald-500" />
    </div>
  </aside>
</template>
