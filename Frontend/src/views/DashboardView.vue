<script setup lang="ts">
import type { ThongKeTongQuan, ThongKeDoanhThuItem } from "@/type";
import StatCards from "@/components/StatCards.vue";
import RevenueChart from "@/components/RevenueChart.vue";

interface DashboardViewProps {
  stats: ThongKeTongQuan | null;
  isStatsLoading: boolean;
  revenueChartData: ThongKeDoanhThuItem[];
  isChartLoading: boolean;
}

defineProps<DashboardViewProps>();
</script>

<template>
  <div class="space-y-6">
    <div>
      <h2 class="text-xl font-bold tracking-tight">Tổng quan hệ thống</h2>
      <p class="text-xs text-muted-foreground mt-0.5">
        Các chỉ số hiệu quả kinh doanh, cảnh báo tồn kho và biểu đồ doanh thu
      </p>
    </div>

    <!-- Stat cards -->
    <StatCards :stats="stats" :is-loading="isStatsLoading" />

    <!-- Revenue chart -->
    <RevenueChart :data="revenueChartData" :is-loading="isChartLoading" />
  </div>
</template>
