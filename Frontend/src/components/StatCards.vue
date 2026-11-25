<script setup lang="ts">
import { computed } from "vue";
import type { ThongKeTongQuan } from "@/type";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";

interface StatCardsProps {
    stats: ThongKeTongQuan | null;
    isLoading: boolean;
}

const props = defineProps<StatCardsProps>();

function formatCurrency(value?: number): string {
    return new Intl.NumberFormat("vi-VN", {
        style: "currency",
        currency: "VND",
    }).format(value ?? 0);
}

function formatCount(value?: number): string {
    return new Intl.NumberFormat("vi-VN").format(value ?? 0);
}

const financialStats = computed(() => {
    const s = props.stats;
    return [
        {
            title: "Tổng doanh thu",
            value: formatCurrency(s?.tongDoanhThu),
            description: "Trong kỳ đã chọn",
            cardClass:
                "border-emerald-500/30 bg-gradient-to-br from-emerald-500/10 via-card to-card",
            titleClass: "text-emerald-700 dark:text-emerald-400",
            valueClass: "text-emerald-950 dark:text-emerald-100",
        },
        {
            title: "Lợi nhuận ước tính",
            value: formatCurrency(s?.loiNhuanUocTinh),
            description: "Trong kỳ đã chọn",
            cardClass: "border-sky-500/30 bg-gradient-to-br from-sky-500/10 via-card to-card",
            titleClass: "text-sky-700 dark:text-sky-400",
            valueClass: "text-sky-950 dark:text-sky-100",
        },
        {
            title: "Giá trị tồn kho",
            value: formatCurrency(s?.tongGiaTriTonKho),
            description: "Giá trị hàng tồn hiện tại",
            cardClass: "border-violet-500/30 bg-gradient-to-br from-violet-500/10 via-card to-card",
            titleClass: "text-violet-700 dark:text-violet-400",
            valueClass: "text-violet-950 dark:text-violet-100",
        },
    ];
});

const orderStats = computed(() => {
    const s = props.stats;
    return [
        {
            title: "Số hóa đơn",
            value: formatCount(s?.soHoaDon),
            description: "Trong kỳ đã chọn",
            cardClass:
                "border-slate-300/80 bg-slate-50/80 dark:border-slate-700 dark:bg-slate-900/50",
            valueClass: "text-slate-900 dark:text-slate-100",
        },
        {
            title: "Giá trị trung bình đơn",
            value: formatCurrency(s?.giaTriTrungBinhDon),
            description: "Trung bình mỗi hóa đơn",
            cardClass:
                "border-slate-300/80 bg-slate-50/80 dark:border-slate-700 dark:bg-slate-900/50",
            valueClass: "text-slate-900 dark:text-slate-100",
        },
    ];
});

const alertStats = computed(() => {
    const s = props.stats;
    const lowStock = s?.thuocSapHetHang ?? 0;
    const nearExpiry = s?.loSapHetHan ?? 0;
    const expired = s?.loDaHetHan ?? 0;

    return [
        {
            title: "Thuốc sắp hết hàng",
            value: formatCount(lowStock),
            description: "Tồn kho không quá 10",
            cardClass:
                lowStock > 0
                    ? "border-amber-400 bg-amber-50/90 shadow-sm dark:border-amber-500/60 dark:bg-amber-950/35"
                    : "border-border bg-card",
            valueClass:
                lowStock > 0
                    ? "text-amber-700 dark:text-amber-300 text-2xl font-bold"
                    : "text-2xl font-semibold",
        },
        {
            title: "Lô sắp hết hạn",
            value: formatCount(nearExpiry),
            description: "Cảnh báo tồn kho hiện tại",
            cardClass:
                nearExpiry > 0
                    ? "border-orange-400 bg-orange-50/90 shadow-sm"
                    : "border-border bg-card",
            valueClass:
                nearExpiry > 0
                    ? "text-orange-700 dark:text-orange-300 text-3xl font-bold"
                    : "text-2xl font-semibold",
        },
        {
            title: "Lô đã hết hạn",
            value: formatCount(expired),
            description: "Cần thu hồi / hủy lô",
            cardClass:
                expired > 0 ? "border-rose-500 bg-rose-100/80 shadow-sm" : "border-border bg-card",
            valueClass:
                expired > 0
                    ? "text-rose-800 dark:text-rose-200 text-3xl font-extrabold"
                    : "text-2xl font-semibold",
        },
    ];
});
</script>

<template>
    <div class="flex flex-col gap-4">
        <!-- Hàng 1: Nhóm tài chính & tồn kho -->
        <div class="flex flex-row gap-4">
            <Card
                v-for="stat in financialStats"
                :key="stat.title"
                :class="['flex-1 transition-colors', stat.cardClass]"
            >
                <CardHeader class="pb-1">
                    <CardTitle :class="['text-sm font-semibold', stat.titleClass]">
                        {{ stat.title }}
                    </CardTitle>
                </CardHeader>
                <CardContent>
                    <div v-if="isLoading" class="h-8 w-36 animate-pulse rounded bg-muted" />
                    <div v-else :class="['text-2xl font-bold tracking-tight', stat.valueClass]">
                        {{ stat.value }}
                    </div>
                    <CardDescription class="mt-1 text-xs">
                        {{ stat.description }}
                    </CardDescription>
                </CardContent>
            </Card>
        </div>

        <!-- Hàng 2: Nhóm đơn hàng (trái) & Cụm cảnh báo hết hàng + hết hạn (phải) -->
        <div class="flex flex-col gap-4">
            <!-- Cụm đơn hàng -->
            <div class="flex flex-1 flex-row gap-4">
                <Card
                    v-for="stat in orderStats"
                    :key="stat.title"
                    :class="['flex-1', stat.cardClass]"
                >
                    <CardHeader class="pb-1">
                        <CardTitle class="text-sm font-medium text-muted-foreground">
                            {{ stat.title }}
                        </CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div v-if="isLoading" class="h-8 w-28 animate-pulse rounded bg-muted" />
                        <div
                            v-else
                            :class="['text-2xl font-semibold tracking-tight', stat.valueClass]"
                        >
                            {{ stat.value }}
                        </div>
                        <CardDescription class="mt-1 text-xs">
                            {{ stat.description }}
                        </CardDescription>
                    </CardContent>
                </Card>
            </div>

            <!-- Cụm cảnh báo nổi bật: Thuốc sắp hết hàng & Lô sắp hết hạn đi cùng nhau -->
            <div class="flex flex-1 flex-row gap-3 rounded-xl">
                <Card
                    v-for="alert in alertStats"
                    :key="alert.title"
                    :class="['flex-1', alert.cardClass]"
                >
                    <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
                        <div class="flex items-center gap-2">
                            <CardTitle class="text-sm font-semibold">
                                {{ alert.title }}
                            </CardTitle>
                        </div>
                    </CardHeader>
                    <CardContent>
                        <div v-if="isLoading" class="h-9 w-20 animate-pulse rounded bg-muted" />
                        <div v-else :class="['tracking-tight', alert.valueClass]">
                            {{ alert.value }}
                        </div>
                        <CardDescription class="mt-1 text-xs">
                            {{ alert.description }}
                        </CardDescription>
                    </CardContent>
                </Card>
            </div>
        </div>
    </div>
</template>
