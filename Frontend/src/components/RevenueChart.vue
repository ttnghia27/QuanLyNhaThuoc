<script setup lang="ts">
import { computed } from "vue";
import type { ThongKeDoanhThuItem } from "@/type";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
} from "@/components/ui/card";
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale,
  type ChartOptions,
} from "chart.js";
import { Bar } from "vue-chartjs";
import { TrendingUpIcon } from "@lucide/vue";

ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale);

interface RevenueChartProps {
  data: ThongKeDoanhThuItem[];
  isLoading: boolean;
}

const props = defineProps<RevenueChartProps>();

function formatDate(dateStr: string): string {
  try {
    const d = new Date(dateStr);
    return `${d.getDate().toString().padStart(2, "0")}/${(d.getMonth() + 1)
      .toString()
      .padStart(2, "0")}`;
  } catch {
    return dateStr;
  }
}

const chartData = computed(() => {
  const items = [...props.data].sort(
    (a, b) => new Date(a.ngay).getTime() - new Date(b.ngay).getTime()
  );

  const labels = items.map((i) => formatDate(i.ngay));
  const revenues = items.map((i) => i.doanhThu);
  const invoices = items.map((i) => i.soHoaDon);

  return {
    labels,
    datasets: [
      {
        label: "Doanh thu (₫)",
        data: revenues,
        backgroundColor: "rgba(59, 130, 246, 0.75)",
        borderColor: "rgb(37, 99, 235)",
        borderWidth: 1,
        borderRadius: 6,
        yAxisID: "y",
      },
      {
        label: "Số hóa đơn",
        data: invoices,
        backgroundColor: "rgba(16, 185, 129, 0.65)",
        borderColor: "rgb(5, 150, 105)",
        borderWidth: 1,
        borderRadius: 6,
        yAxisID: "y1",
      },
    ],
  };
});

const chartOptions: ChartOptions<"bar"> = {
  responsive: true,
  maintainAspectRatio: false,
  interaction: {
    mode: "index",
    intersect: false,
  },
  plugins: {
    legend: {
      position: "top",
      labels: {
        font: {
          family: "Inter, sans-serif",
          size: 12,
        },
        usePointStyle: true,
      },
    },
    tooltip: {
      callbacks: {
        label: (context) => {
          if (context.datasetIndex === 0) {
            return ` Doanh thu: ${new Intl.NumberFormat("vi-VN", {
              style: "currency",
              currency: "VND",
            }).format(context.parsed.y ?? 0)}`;
          }
          return ` Số hóa đơn: ${context.parsed.y ?? 0} đơn`;
        },
      },
    },
  },
  scales: {
    x: {
      grid: {
        display: false,
      },
    },
    y: {
      type: "linear",
      display: true,
      position: "left",
      title: {
        display: true,
        text: "Doanh thu (₫)",
        font: { size: 11 },
      },
      ticks: {
        callback: (value) => {
          if (typeof value === "number") {
            if (value >= 1_000_000) return `${(value / 1_000_000).toFixed(1)}M`;
            if (value >= 1_000) return `${(value / 1_000).toFixed(0)}k`;
          }
          return value;
        },
      },
      grid: {
        color: "rgba(200, 200, 200, 0.2)",
      },
    },
    y1: {
      type: "linear",
      display: true,
      position: "right",
      title: {
        display: true,
        text: "Số hóa đơn",
        font: { size: 11 },
      },
      grid: {
        drawOnChartArea: false,
      },
      ticks: {
        precision: 0,
      },
    },
  },
};
</script>

<template>
  <Card class="border-border/60 shadow-xs">
    <CardHeader class="flex flex-row items-center justify-between pb-3">
      <div>
        <CardTitle class="text-base font-semibold flex items-center gap-2">
          <TrendingUpIcon class="w-4 h-4 text-primary" />
          Biểu đồ Doanh thu & Hóa đơn theo ngày
        </CardTitle>
        <CardDescription class="text-xs">
          Dữ liệu thống kê doanh số bán hàng trong các ngày gần nhất
        </CardDescription>
      </div>
    </CardHeader>
    <CardContent>
      <div v-if="isLoading" class="h-64 flex items-center justify-center">
        <div class="flex items-center gap-2 text-muted-foreground text-sm">
          <div class="w-4 h-4 border-2 border-primary border-t-transparent rounded-full animate-spin" />
          Đang tải biểu đồ...
        </div>
      </div>
      <div v-else-if="props.data.length === 0" class="h-64 flex flex-col items-center justify-center text-muted-foreground text-sm">
        <p>Chưa có dữ liệu giao dịch trong khoảng thời gian này.</p>
        <p class="text-xs text-muted-foreground/80 mt-1">Các hóa đơn mới được lập sẽ tự động hiển thị tại đây.</p>
      </div>
      <div v-else class="h-72 w-full">
        <Bar :data="chartData" :options="chartOptions" />
      </div>
    </CardContent>
  </Card>
</template>
