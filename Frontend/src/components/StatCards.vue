<script setup lang="ts">
import { computed } from "vue";
import type { ThongKeTongQuan } from "@/type";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
} from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import {
  DollarSignIcon,
  TrendingUpIcon,
  AlertTriangleIcon,
  PillIcon,
} from "@lucide/vue";

interface StatCardsProps {
  stats: ThongKeTongQuan | null;
  isLoading: boolean;
}

const props = defineProps<StatCardsProps>();

function formatCurrency(value?: number): string {
  if (value === undefined || value === null) return "0 ₫";
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
  }).format(value);
}

const formattedStats = computed(() => {
  const s = props.stats;
  return {
    doanhThuHomNay: formatCurrency(s?.doanhThuHomNay),
    tongDoanhThu: formatCurrency(s?.tongDoanhThu),
    loiNhuanUocTinh: formatCurrency(s?.loiNhuanUocTinh),
    tongTienNhap: formatCurrency(s?.tongTienNhap),
    soHoaDon: s?.soHoaDon ?? 0,
    soLoaiThuoc: s?.soLoaiThuoc ?? 0,
    thuocSapHetHang: s?.thuocSapHetHang ?? 0,
    loSapHetHan: s?.loSapHetHan ?? 0,
  };
});
</script>

<template>
  <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
    <!-- Doanh thu hôm nay -->
    <Card class="border-border/60 shadow-xs transition-shadow hover:shadow-sm">
      <CardHeader class="flex flex-row items-center justify-between pb-2 space-y-0">
        <CardTitle class="text-sm font-medium text-muted-foreground">
          Doanh thu hôm nay
        </CardTitle>
        <div class="p-2 rounded-lg bg-emerald-50 text-emerald-600 dark:bg-emerald-950/40 dark:text-emerald-400">
          <DollarSignIcon class="w-4 h-4" />
        </div>
      </CardHeader>
      <CardContent>
        <div v-if="isLoading" class="h-8 w-28 bg-muted animate-pulse rounded" />
        <div v-else class="text-2xl font-bold tracking-tight text-emerald-600 dark:text-emerald-400">
          {{ formattedStats.doanhThuHomNay }}
        </div>
        <CardDescription class="mt-1 text-xs">
          Cập nhật theo hóa đơn bán trong ngày
        </CardDescription>
      </CardContent>
    </Card>

    <!-- Tổng doanh thu -->
    <Card class="border-border/60 shadow-xs transition-shadow hover:shadow-sm">
      <CardHeader class="flex flex-row items-center justify-between pb-2 space-y-0">
        <CardTitle class="text-sm font-medium text-muted-foreground">
          Tổng doanh thu
        </CardTitle>
        <div class="p-2 rounded-lg bg-blue-50 text-blue-600 dark:bg-blue-950/40 dark:text-blue-400">
          <TrendingUpIcon class="w-4 h-4" />
        </div>
      </CardHeader>
      <CardContent>
        <div v-if="isLoading" class="h-8 w-32 bg-muted animate-pulse rounded" />
        <div v-else class="text-2xl font-bold tracking-tight text-blue-600 dark:text-blue-400">
          {{ formattedStats.tongDoanhThu }}
        </div>
        <CardDescription class="mt-1 text-xs flex items-center justify-between">
          <span>{{ formattedStats.soHoaDon }} đơn đã hoàn thành</span>
          <span class="font-medium text-foreground">LN: {{ formattedStats.loiNhuanUocTinh }}</span>
        </CardDescription>
      </CardContent>
    </Card>

    <!-- Tổng số thuốc trong kho -->
    <Card class="border-border/60 shadow-xs transition-shadow hover:shadow-sm">
      <CardHeader class="flex flex-row items-center justify-between pb-2 space-y-0">
        <CardTitle class="text-sm font-medium text-muted-foreground">
          Mặt hàng thuốc
        </CardTitle>
        <div class="p-2 rounded-lg bg-indigo-50 text-indigo-600 dark:bg-indigo-950/40 dark:text-indigo-400">
          <PillIcon class="w-4 h-4" />
        </div>
      </CardHeader>
      <CardContent>
        <div v-if="isLoading" class="h-8 w-16 bg-muted animate-pulse rounded" />
        <div v-else class="text-2xl font-bold tracking-tight">
          {{ formattedStats.soLoaiThuoc }}
          <span class="text-sm font-normal text-muted-foreground">mặt hàng</span>
        </div>
        <CardDescription class="mt-1 text-xs">
          Tiền nhập kho: {{ formattedStats.tongTienNhap }}
        </CardDescription>
      </CardContent>
    </Card>

    <!-- Cảnh báo thuốc sắp hết / lô hết hạn -->
    <Card class="border-border/60 shadow-xs transition-shadow hover:shadow-sm">
      <CardHeader class="flex flex-row items-center justify-between pb-2 space-y-0">
        <CardTitle class="text-sm font-medium text-muted-foreground">
          Cảnh báo tồn & hạn
        </CardTitle>
        <div class="p-2 rounded-lg bg-amber-50 text-amber-600 dark:bg-amber-950/40 dark:text-amber-400">
          <AlertTriangleIcon class="w-4 h-4" />
        </div>
      </CardHeader>
      <CardContent>
        <div v-if="isLoading" class="h-8 w-24 bg-muted animate-pulse rounded" />
        <div v-else class="flex items-center gap-2">
          <Badge
            :variant="formattedStats.thuocSapHetHang > 0 ? 'destructive' : 'secondary'"
            class="text-xs font-semibold px-2 py-0.5"
          >
            {{ formattedStats.thuocSapHetHang }} sắp hết tồn
          </Badge>
          <Badge
            :variant="formattedStats.loSapHetHan > 0 ? 'outline' : 'secondary'"
            class="text-xs font-semibold px-2 py-0.5 border-amber-500 text-amber-600"
          >
            {{ formattedStats.loSapHetHan }} lô sắp hạn
          </Badge>
        </div>
        <CardDescription class="mt-2 text-xs">
          Thuốc tồn &le; 10 hoặc lô hạn trong 30 ngày
        </CardDescription>
      </CardContent>
    </Card>
  </div>
</template>
