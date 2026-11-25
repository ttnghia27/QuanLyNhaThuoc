<script setup lang="ts">
import type { ThongKeTongQuan, ThongKeDoanhThuItem } from "@/type";
import type { ThongKeMonths } from "@/services/api";
import StatCards from "@/components/StatCards.vue";
import RevenueChart from "@/components/RevenueChart.vue";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

interface DashboardViewProps {
  stats: ThongKeTongQuan | null;
  isStatsLoading: boolean;
  revenueChartData: ThongKeDoanhThuItem[];
  isChartLoading: boolean;
  months: ThongKeMonths;
}

defineProps<DashboardViewProps>();

const emit = defineEmits<{
  "update:months": [months: ThongKeMonths];
}>();

const monthOptions: { value: ThongKeMonths; label: string }[] = [
  { value: 1, label: "1 tháng" },
  { value: 3, label: "3 tháng" },
  { value: 6, label: "6 tháng" },
  { value: 12, label: "12 tháng" },
];

function handleMonthsChange(value: string) {
  const months = Number(value);
  if (months === 1 || months === 3 || months === 6 || months === 12) {
    emit("update:months", months);
  }
}
</script>

<template>
  <div class="space-y-6">
    <div class="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <h2 class="text-xl font-bold tracking-tight">Tổng quan hệ thống</h2>
        <p class="text-xs text-muted-foreground mt-0.5">
          Các chỉ số hiệu quả kinh doanh, cảnh báo tồn kho và biểu đồ doanh thu
        </p>
      </div>
      <Select
        :model-value="String(months)"
        @update:model-value="handleMonthsChange(String($event))"
      >
        <SelectTrigger class="h-9 w-full sm:w-36" aria-label="Khoảng thời gian thống kê">
          <SelectValue placeholder="Chọn thời gian" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem v-for="option in monthOptions" :key="option.value" :value="String(option.value)">
            {{ option.label }}
          </SelectItem>
        </SelectContent>
      </Select>
    </div>

    <!-- Stat cards -->
    <StatCards :stats="stats" :is-loading="isStatsLoading" />

    <!-- Revenue chart -->
    <RevenueChart :data="revenueChartData" :is-loading="isChartLoading" />
  </div>
</template>
