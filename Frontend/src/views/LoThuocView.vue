<script setup lang="ts">
import { ref, computed } from "vue";
import type { LoThuoc } from "@/type";
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from "@/components/ui/card";
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
import { BoxesIcon, FlameIcon, SearchIcon } from "@lucide/vue";

interface LoThuocViewProps {
    lots: LoThuoc[];
    isLoading: boolean;
}

const props = defineProps<LoThuocViewProps>();

type LotStatus = "all" | "expired" | "expiring" | "soldOut" | "available";

const emit = defineEmits<{
    tieuHuy: [maLo: string, tenThuoc: string, soLuongCon: number];
}>();

const searchQuery = ref("");
const statusFilter = ref<LotStatus>("all");

function updateStatusFilter(value: string) {
  if (
    value === "all" ||
    value === "expired" ||
    value === "expiring" ||
    value === "soldOut" ||
    value === "available"
  ) {
    statusFilter.value = value;
  }
}

function isExpired(expired: Date | string): boolean {
    const DAYS_30 = 30 * 24 * 60 * 60 * 1000;
    const expired_date = new Date(expired);
    const now = new Date();
    return expired_date.getTime() < now.getTime() + DAYS_30;
}

function getLotStatus(lot: LoThuoc): Exclude<LotStatus, "all"> {
    if (lot.daHetHan && lot.soLuongCon > 0) return "expired";
    if (lot.soLuongCon === 0) return "soldOut";
    if (isExpired(lot.hanSuDung)) return "expiring";
    return "available";
}

const filteredLots = computed(() => {
    const q = searchQuery.value.trim().toLowerCase();
    return props.lots.filter((lot) => {
        const matchesSearch =
            !q ||
            lot.tenThuoc?.toLowerCase().includes(q) ||
            lot.maThuoc?.toLowerCase().includes(q) ||
            lot.maLo?.toLowerCase().includes(q) ||
            lot.maPhieuNhap?.toLowerCase().includes(q);
        const matchesStatus =
            statusFilter.value === "all" ||
            getLotStatus(lot) === statusFilter.value;

        return matchesSearch && matchesStatus;
    });
});

function formatCurrency(val: number): string {
    return new Intl.NumberFormat("vi-VN", {
        style: "currency",
        currency: "VND",
    }).format(val);
}

function formatDate(dateStr: string): string {
    try {
        const d = new Date(dateStr);
        return `${d.getDate().toString().padStart(2, "0")}/${(d.getMonth() + 1)
            .toString()
            .padStart(2, "0")}/${d.getFullYear()}`;
    } catch {
        return dateStr;
    }
}

function handleTieuHuy(lot: LoThuoc) {
    if (
        confirm(
            `CẢNH BÁO TIÊU HỦY:\nBạn có chắc muốn tiêu hủy toàn bộ ${lot.soLuongCon} sản phẩm của lô ${lot.maLo} (${lot.tenThuoc})?\nSố lượng này sẽ bị trừ trực tiếp khỏi tồn kho thuốc.`,
        )
    ) {
        emit("tieuHuy", lot.maLo, lot.tenThuoc, lot.soLuongCon);
    }
}
</script>

<template>
    <Card class="border-border/60 shadow-xs">
        <CardHeader class="pb-3">
            <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
                <div>
                    <CardTitle class="text-base font-semibold flex items-center gap-2">
                        <BoxesIcon class="w-4 h-4 text-primary" />
                        Bảng Dữ liệu: Lô thuốc trong kho (LoThuoc)
                    </CardTitle>
                    <CardDescription class="text-xs">
                        Theo dõi từng lô thuốc nhập về, hạn sử dụng và thực hiện thao tác Tiêu hủy
                        lô hết hạn
                    </CardDescription>
                </div>

                <div class="w-48 shrink-0">
                    <Select
                        :model-value="statusFilter"
                        @update:model-value="updateStatusFilter(String($event))"
                    >
                        <SelectTrigger class="h-9 text-xs">
                            <SelectValue placeholder="Lọc trạng thái" />
                        </SelectTrigger>
                        <SelectContent>
                            <SelectItem value="all">Tất cả lô thuốc</SelectItem>
                            <SelectItem value="expired">Đã hết hạn</SelectItem>
                            <SelectItem value="expiring">Sắp hết hạn</SelectItem>
                            <SelectItem value="soldOut">Đã xuất hết</SelectItem>
                            <SelectItem value="available">Khả dụng</SelectItem>
                        </SelectContent>
                    </Select>
                </div>
            </div>

            <!-- Ô tìm kiếm lô thuốc theo tên thuốc / mã thuốc -->
            <div class="pt-3 max-w-md relative">
                <SearchIcon
                    class="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground"
                />
                <Input
                    v-model="searchQuery"
                    placeholder="Tìm theo tên thuốc (vd: Paracetamol...), mã thuốc hoặc mã lô..."
                    class="pl-9 text-sm h-9"
                />
            </div>
        </CardHeader>

        <CardContent>
            <div class="border rounded-md overflow-x-auto">
                <Table>
                    <TableHeader>
                        <TableRow>
                            <TableHead class="w-24">Mã lô</TableHead>
                            <TableHead class="w-24">Mã thuốc</TableHead>
                            <TableHead class="min-w-[160px]">Tên thuốc</TableHead>
                            <TableHead class="w-28">Phiếu nhập</TableHead>
                            <TableHead class="w-28 text-center">Hạn sử dụng</TableHead>
                            <TableHead class="w-24 text-right">Giá nhập</TableHead>
                            <TableHead class="w-24 text-center">SL Nhập</TableHead>
                            <TableHead class="w-24 text-center">SL Còn</TableHead>
                            <TableHead class="w-28 text-center">Trạng thái</TableHead>
                            <TableHead class="w-24 text-right">Tiêu hủy</TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        <TableRow v-if="isLoading">
                            <TableCell
                                colspan="10"
                                class="text-center py-10 text-muted-foreground text-sm"
                            >
                                Đang tải dữ liệu lô thuốc...
                            </TableCell>
                        </TableRow>
                        <TableRow v-else-if="lots.length === 0">
                            <TableCell
                                colspan="10"
                                class="text-center py-10 text-muted-foreground text-sm"
                            >
                                Chưa có lô thuốc nào trong kho.
                            </TableCell>
                        </TableRow>
                        <TableRow v-else-if="filteredLots.length === 0">
                            <TableCell
                                colspan="10"
                                class="text-center py-10 text-muted-foreground text-sm"
                            >
                                Không tìm thấy lô thuốc nào phù hợp với tiêu chí đã chọn.
                            </TableCell>
                        </TableRow>
                        <TableRow
                            v-else
                            v-for="lot in filteredLots"
                            :key="lot.maLo"
                            class="hover:bg-muted/40 transition-colors"
                        >
                            <TableCell class="font-mono text-xs font-semibold text-primary">
                                {{ lot.maLo }}
                            </TableCell>
                            <TableCell class="font-mono text-xs text-muted-foreground">
                                {{ lot.maThuoc }}
                            </TableCell>
                            <TableCell class="font-medium text-xs">
                                {{ lot.tenThuoc }}
                            </TableCell>
                            <TableCell class="font-mono text-xs text-muted-foreground">
                                {{ lot.maPhieuNhap }}
                            </TableCell>
                            <TableCell class="text-center text-xs font-medium">
                                {{ formatDate(lot.hanSuDung) }}
                            </TableCell>
                            <TableCell class="text-right text-xs">
                                {{ formatCurrency(lot.donGiaNhap) }}
                            </TableCell>
                            <TableCell class="text-center text-xs">
                                {{ lot.soLuongNhap }}
                            </TableCell>
                            <TableCell class="text-center text-xs font-semibold">
                                {{ lot.soLuongCon }}
                            </TableCell>
                            <TableCell class="text-center">
                                <Badge
                                    v-if="getLotStatus(lot) === 'expired'"
                                    variant="destructive"
                                    class="text-[10px] px-1.5 py-0.5"
                                >
                                    Đã hết hạn
                                </Badge>
                                <Badge
                                    v-else-if="getLotStatus(lot) === 'soldOut'"
                                    variant="secondary"
                                    class="text-[10px] px-1.5 py-0.5"
                                >
                                    Đã xuất hết
                                </Badge>
                                <Badge
                                    v-else-if="getLotStatus(lot) === 'expiring'"
                                    class="text-[10px] px-1.5 py-0.5 bg-amber-100 text-amber-800 border-amber-300"
                                    >Sắp hết hạn</Badge
                                >
                                <Badge
                                    v-else
                                    variant="outline"
                                    class="text-[10px] px-1.5 py-0.5 border-emerald-500 text-emerald-600"
                                >
                                    Khả dụng
                                </Badge>
                            </TableCell>
                            <TableCell class="text-right">
                                <Button
                                    v-if="lot.soLuongCon > 0"
                                    variant="ghost"
                                    size="icon-xs"
                                    class="text-destructive hover:text-destructive hover:bg-destructive/10"
                                    title="Tiêu hủy toàn bộ số lượng còn lại của lô này"
                                    @click="handleTieuHuy(lot)"
                                >
                                    <FlameIcon class="w-3.5 h-3.5" />
                                </Button>
                                <span v-else class="text-xs text-muted-foreground">—</span>
                            </TableCell>
                        </TableRow>
                    </TableBody>
                </Table>
            </div>

            <!-- Count footer -->
            <div class="flex items-center justify-between text-xs text-muted-foreground pt-3 px-1">
                <span>Hiển thị {{ filteredLots.length }} / {{ lots.length }} lô thuốc</span>
                <span v-if="searchQuery"
                    >Đang lọc theo tên thuốc / từ khóa: "{{ searchQuery }}"</span
                >
                <span v-else-if="statusFilter !== 'all'">
                    Đang lọc theo trạng thái
                </span>
            </div>
        </CardContent>
    </Card>
</template>
