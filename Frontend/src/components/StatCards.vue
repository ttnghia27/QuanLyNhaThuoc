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
            cardClass: "border-emerald-500/25 bg-card",
            titleClass: "text-emerald-700 dark:text-emerald-400",
            valueClass: "text-foreground",
        },
        {
            title: "Lợi nhuận ước tính",
            value: formatCurrency(s?.loiNhuanUocTinh),
            description: "Trong kỳ đã chọn",
            cardClass: "border-sky-500/25 bg-card",
            titleClass: "text-sky-700 dark:text-sky-400",
            valueClass: "text-foreground",
        },
        {
            title: "Giá trị tồn kho",
            value: formatCurrency(s?.tongGiaTriTonKho),
            description: "Giá trị hàng tồn hiện tại",
            cardClass: "border-violet-500/25 bg-card",
            titleClass: "text-violet-700 dark:text-violet-400",
            valueClass: "text-foreground",
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
            cardClass: "border-border bg-card",
            valueClass: "text-foreground",
        },
        {
            title: "Giá trị trung bình đơn",
            value: formatCurrency(s?.giaTriTrungBinhDon),
            description: "Trung bình mỗi hóa đơn",
            cardClass: "border-border bg-card",
            valueClass: "text-foreground",
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
                    ? "border-amber-500/40 bg-amber-50/50 dark:bg-amber-950/20"
                    : "border-border bg-card",
            valueClass:
                lowStock > 0
                    ? "text-2xl font-semibold tabular-nums tracking-tight text-amber-700 dark:text-amber-400"
                    : "text-2xl font-semibold tabular-nums tracking-tight text-foreground",
        },
        {
            title: "Lô sắp hết hạn",
            value: formatCount(nearExpiry),
            description: "Cảnh báo tồn kho hiện tại",
            cardClass:
                nearExpiry > 0
                    ? "border-amber-500/40 bg-amber-50/50 dark:bg-amber-950/20"
                    : "border-border bg-card",
            valueClass:
                nearExpiry > 0
                    ? "text-2xl font-semibold tabular-nums tracking-tight text-amber-700 dark:text-amber-400"
                    : "text-2xl font-semibold tabular-nums tracking-tight text-foreground",
        },
        {
            title: "Lô đã hết hạn",
            value: formatCount(expired),
            description: "Cần thu hồi / hủy lô",
            cardClass:
                expired > 0
                    ? "border-rose-500/40 bg-rose-50/50 dark:bg-rose-950/20"
                    : "border-border bg-card",
            valueClass:
                expired > 0
                    ? "text-2xl font-semibold tabular-nums tracking-tight text-rose-700 dark:text-rose-400"
                    : "text-2xl font-semibold tabular-nums tracking-tight text-foreground",
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
                :class="['flex-1 transition-colors gap-1.5', stat.cardClass]"
            >
                <CardHeader class="pb-1">
                    <CardTitle :class="['text-xs font-medium', stat.titleClass]">
                        {{ stat.title }}
                    </CardTitle>
                </CardHeader>
                <CardContent>
                    <div v-if="isLoading" class="h-8 w-36 animate-pulse rounded bg-muted" />
                    <div
                        v-else
                        :class="[
                            'text-2xl font-semibold tabular-nums tracking-tight',
                            stat.valueClass,
                        ]"
                    >
                        {{ stat.value }}
                    </div>
                    <CardDescription class="mt-0.5 text-xs">
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
                    :class="['flex-1 gap-1.5', stat.cardClass]"
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
        </div>
        <!-- Cụm cảnh báo nổi bật: Thuốc sắp hết hàng & Lô sắp hết hạn đi cùng nhau -->
        <div class="flex flex-1 flex-row gap-3 rounded-xl">
            <Card
                v-for="alert in alertStats"
                :key="alert.title"
                :class="['flex-1 gap-1.5', alert.cardClass]"
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
</template>
