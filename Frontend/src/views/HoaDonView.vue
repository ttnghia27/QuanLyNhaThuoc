<script setup lang="ts">
import { ref } from "vue";
import type { HoaDon, KhachHang, Thuoc, CreateHoaDonRequest, CreateHoaDonDetailItem } from "@/type";
import { pharmacyApi } from "@/services/api";
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from "@/components/ui/card";
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
import {
    Select,
    SelectTrigger,
    SelectValue,
    SelectContent,
    SelectItem,
} from "@/components/ui/select";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { ReceiptIcon, PlusIcon, EyeIcon, Trash2Icon, InfoIcon } from "@lucide/vue";

interface HoaDonViewProps {
    invoices: HoaDon[];
    customers: KhachHang[];
    drugs: Thuoc[];
    isLoading: boolean;
}

const props = defineProps<HoaDonViewProps>();

const emit = defineEmits<{
    createInvoice: [payload: CreateHoaDonRequest];
}>();

// Detail Modal
const isDetailOpen = ref(false);
const selectedInvoice = ref<HoaDon | null>(null);
const isLoadingDetail = ref(false);

async function openDetailModal(inv: HoaDon) {
    isDetailOpen.value = true;
    isLoadingDetail.value = true;
    selectedInvoice.value = inv;
    try {
        const full = await pharmacyApi.getHoaDonById(inv.maHD);
        selectedInvoice.value = full;
    } catch (err) {
        console.error("Lỗi khi tải chi tiết hóa đơn:", err);
    } finally {
        isLoadingDetail.value = false;
    }
}

// Create Modal
const isCreateOpen = ref(false);
const selectedCustomer = ref("");
const invoiceItems = ref<CreateHoaDonDetailItem[]>([]);
const formError = ref("");

function openCreateModal() {
    selectedCustomer.value = props.customers.length > 0 ? props.customers[0].maKH : "";
    invoiceItems.value = [
        {
            maThuoc: props.drugs.length > 0 ? props.drugs[0].maThuoc : "",
            soLuongBan: 1,
        },
    ];
    formError.value = "";
    isCreateOpen.value = true;
}

function addItemRow() {
    invoiceItems.value.push({
        maThuoc: props.drugs.length > 0 ? props.drugs[0].maThuoc : "",
        soLuongBan: 1,
    });
}

function removeItemRow(index: number) {
    if (invoiceItems.value.length > 1) {
        invoiceItems.value.splice(index, 1);
    }
}

function handleSaveInvoice() {
    formError.value = "";
    if (!selectedCustomer.value) {
        formError.value = "Vui lòng chọn khách hàng mua thuốc.";
        return;
    }

    for (const item of invoiceItems.value) {
        if (!item.maThuoc) {
            formError.value = "Vui lòng chọn thuốc cho tất cả các dòng.";
            return;
        }
        if (!item.soLuongBan || item.soLuongBan <= 0) {
            formError.value = "Số lượng bán phải lớn hơn 0.";
            return;
        }
    }

    const payload: CreateHoaDonRequest = {
        maKH: selectedCustomer.value,
        chiTiet: invoiceItems.value.map((i) => ({
            maThuoc: i.maThuoc,
            soLuongBan: Number(i.soLuongBan),
        })),
    };

    emit("createInvoice", payload);
    isCreateOpen.value = false;
}

function formatCurrency(val?: number): string {
    if (val === undefined || val === null) return "0 ₫";
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
            .padStart(2, "0")}/${d.getFullYear()} ${d.getHours().toString().padStart(2, "0")}:${d
            .getMinutes()
            .toString()
            .padStart(2, "0")}`;
    } catch {
        return dateStr;
    }
}
</script>

<template>
    <Card class="border-border/60 shadow-xs">
        <CardHeader class="pb-3">
            <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
                <div>
                    <CardTitle class="text-base font-semibold flex items-center gap-2">
                        <ReceiptIcon class="w-4 h-4 text-primary" />
                        Bảng Dữ liệu: Hóa đơn bán lẻ (HoaDon & ChiTietHoaDon)
                    </CardTitle>
                    <CardDescription class="text-xs">
                        Lập hóa đơn và tự động trừ số lượng sản phẩm trên kho theo FEFO (Pure SQL /
                        Business Logic)
                    </CardDescription>
                </div>

                <Button size="sm" @click="openCreateModal" class="text-xs">
                    <PlusIcon class="w-3.5 h-3.5 mr-1" />
                    Lập hóa đơn bán
                </Button>
            </div>
        </CardHeader>

        <CardContent>
            <div class="border rounded-md overflow-x-auto">
                <Table>
                    <TableHeader>
                        <TableRow>
                            <TableHead class="w-28">Mã HĐ</TableHead>
                            <TableHead class="w-44">Ngày lập</TableHead>
                            <TableHead>Khách hàng</TableHead>
                            <TableHead class="w-36 text-right">Tổng tiền</TableHead>
                            <TableHead class="w-24 text-right">Chi tiết</TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        <TableRow v-if="isLoading">
                            <TableCell
                                colspan="5"
                                class="text-center py-10 text-muted-foreground text-sm"
                            >
                                Đang tải danh sách hóa đơn...
                            </TableCell>
                        </TableRow>
                        <TableRow v-else-if="invoices.length === 0">
                            <TableCell
                                colspan="5"
                                class="text-center py-10 text-muted-foreground text-sm"
                            >
                                Chưa có hóa đơn bán nào được lập.
                            </TableCell>
                        </TableRow>
                        <TableRow
                            v-else
                            v-for="inv in invoices"
                            :key="inv.maHD"
                            class="hover:bg-muted/40 transition-colors"
                        >
                            <TableCell class="font-mono text-xs font-semibold text-primary">
                                {{ inv.maHD }}
                            </TableCell>
                            <TableCell class="text-xs">
                                {{ formatDate(inv.ngayLapHD) }}
                            </TableCell>
                            <TableCell class="font-medium text-xs">
                                {{ inv.tenKH }}
                                <span class="font-mono text-muted-foreground"
                                    >({{ inv.maKH }})</span
                                >
                            </TableCell>
                            <TableCell
                                class="text-right font-semibold text-xs text-emerald-600 dark:text-emerald-400"
                            >
                                {{ formatCurrency(inv.tongTien) }}
                            </TableCell>
                            <TableCell class="text-right">
                                <Button
                                    variant="ghost"
                                    size="icon-xs"
                                    class="hover:text-primary"
                                    title="Xem chi tiết các mặt hàng xuất từ lô"
                                    @click="openDetailModal(inv)"
                                >
                                    <EyeIcon class="w-3.5 h-3.5" />
                                </Button>
                            </TableCell>
                        </TableRow>
                    </TableBody>
                </Table>
            </div>
        </CardContent>

        <!-- Dialog: Xem chi tiết hóa đơn -->
        <Dialog :open="isDetailOpen" @update:open="isDetailOpen = $event">
            <DialogContent class="sm:max-w-[620px]">
                <DialogHeader>
                    <DialogTitle>Chi tiết Hóa đơn: {{ selectedInvoice?.maHD }}</DialogTitle>
                    <DialogDescription>
                        Khách hàng: {{ selectedInvoice?.tenKH }} &bull; Ngày lập:
                        {{
                            selectedInvoice?.ngayLapHD ? formatDate(selectedInvoice.ngayLapHD) : ""
                        }}
                    </DialogDescription>
                </DialogHeader>

                <div class="py-2">
                    <div class="border rounded-md overflow-x-auto max-h-72">
                        <Table>
                            <TableHeader>
                                <TableRow>
                                    <TableHead class="w-20">Mã lô</TableHead>
                                    <TableHead>Tên thuốc</TableHead>
                                    <TableHead class="w-24 text-center">Hạn dùng</TableHead>
                                    <TableHead class="w-20 text-center">SL</TableHead>
                                    <TableHead class="w-24 text-right">Đơn giá</TableHead>
                                    <TableHead class="w-28 text-right">Thành tiền</TableHead>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                <TableRow v-if="isLoadingDetail">
                                    <TableCell
                                        colspan="6"
                                        class="text-center py-6 text-sm text-muted-foreground"
                                    >
                                        Đang tải chi tiết hóa đơn...
                                    </TableCell>
                                </TableRow>
                                <TableRow
                                    v-else
                                    v-for="(item, idx) in selectedInvoice?.chiTiet || []"
                                    :key="idx"
                                >
                                    <TableCell
                                        class="font-mono text-xs font-semibold text-primary"
                                        >{{ item.maLo }}</TableCell
                                    >
                                    <TableCell class="font-medium text-xs">{{
                                        item.tenThuoc
                                    }}</TableCell>
                                    <TableCell class="text-center text-xs font-mono">{{
                                        formatDate(item.hanSuDung)
                                    }}</TableCell>
                                    <TableCell class="text-center text-xs font-semibold">{{
                                        item.soLuongBan
                                    }}</TableCell>
                                    <TableCell class="text-right text-xs">{{
                                        formatCurrency(item.giaBan)
                                    }}</TableCell>
                                    <TableCell
                                        class="text-right text-xs font-semibold text-emerald-600"
                                        >{{ formatCurrency(item.thanhTien) }}</TableCell
                                    >
                                </TableRow>
                            </TableBody>
                        </Table>
                    </div>

                    <div class="flex justify-between items-center pt-3 text-sm font-semibold">
                        <span>Tổng tiền thanh toán:</span>
                        <span class="text-base text-emerald-600 dark:text-emerald-400">
                            {{ formatCurrency(selectedInvoice?.tongTien) }}
                        </span>
                    </div>
                </div>

                <DialogFooter>
                    <Button variant="outline" @click="isDetailOpen = false">Đóng</Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>

        <!-- Dialog: Lập hóa đơn mới (Trừ kho tự động theo FEFO) -->
        <Dialog :open="isCreateOpen" @update:open="isCreateOpen = $event">
            <DialogContent class="sm:max-w-145">
                <DialogHeader>
                    <DialogTitle>Lập hóa đơn bán hàng</DialogTitle>
                    <DialogDescription>
                        Chọn khách hàng và thêm các mặt hàng cần bán. Kho hàng sẽ tự động được kiểm
                        tra và trừ sản phẩm trực tiếp theo lô hạn gần nhất (FEFO).
                    </DialogDescription>
                </DialogHeader>

                <form @submit.prevent="handleSaveInvoice" class="space-y-4 py-2">
                    <div
                        v-if="formError"
                        class="p-2.5 text-xs text-destructive bg-destructive/10 rounded"
                    >
                        {{ formError }}
                    </div>

                    <div class="space-y-1.5">
                        <Label for="maKH">Khách hàng <span class="text-destructive">*</span></Label>
                        <Select v-model="selectedCustomer">
                            <SelectTrigger id="maKH" class="w-full">
                                <SelectValue placeholder="Chọn khách hàng" />
                            </SelectTrigger>
                            <SelectContent>
                                <SelectItem v-for="kh in customers" :key="kh.maKH" :value="kh.maKH">
                                    {{ kh.tenKH }} ({{ kh.maKH }})
                                    {{ kh.sdtKH ? `- ${kh.sdtKH}` : "" }}
                                </SelectItem>
                            </SelectContent>
                        </Select>
                    </div>

                    <!-- Items list -->
                    <div class="space-y-2">
                        <div class="flex items-center justify-between">
                            <Label>Danh sách thuốc cần bán</Label>
                            <Button type="button" variant="outline" size="xs" @click="addItemRow">
                                <PlusIcon class="w-3 h-3 mr-1" />
                                Thêm dòng
                            </Button>
                        </div>

                        <div class="space-y-2 max-h-52 overflow-y-auto pr-1">
                            <div
                                v-for="(row, idx) in invoiceItems"
                                :key="idx"
                                class="flex items-center gap-2 border p-2 rounded-md bg-muted/20"
                            >
                                <!-- Drug Select -->
                                <div class="flex-1">
                                    <Select v-model="row.maThuoc">
                                        <SelectTrigger class="h-8 text-xs">
                                            <SelectValue placeholder="Chọn thuốc" />
                                        </SelectTrigger>
                                        <SelectContent>
                                            <SelectItem
                                                v-for="d in drugs"
                                                :key="d.maThuoc"
                                                :value="d.maThuoc"
                                            >
                                                {{ d.tenThuoc }} (Tồn: {{ d.soLuongTonKho }}) -
                                                {{ formatCurrency(d.donGiaBan) }}
                                            </SelectItem>
                                        </SelectContent>
                                    </Select>
                                </div>

                                <!-- Quantity -->
                                <div class="w-24">
                                    <Input
                                        type="number"
                                        min="1"
                                        v-model="row.soLuongBan"
                                        placeholder="SL"
                                        class="h-8 text-xs text-center"
                                        required
                                    />
                                </div>

                                <!-- Remove button -->
                                <Button
                                    type="button"
                                    variant="ghost"
                                    size="icon-xs"
                                    class="text-destructive hover:bg-destructive/10"
                                    :disabled="invoiceItems.length <= 1"
                                    @click="removeItemRow(idx)"
                                >
                                    <Trash2Icon class="w-3.5 h-3.5" />
                                </Button>
                            </div>
                        </div>
                    </div>

                    <div
                        class="p-2.5 rounded-md bg-blue-50 dark:bg-blue-950/40 text-blue-700 dark:text-blue-300 text-xs flex gap-2 items-start"
                    >
                        <InfoIcon class="w-4 h-4 shrink-0 mt-0.5" />
                        <span>
                            Hệ thống sẽ lấy các lô thuốc còn hạn và ưu tiên xuất lô có hạn dùng gần
                            nhất (FEFO), sau đó cập nhật trực tiếp tồn kho mà không cần
                            trigger/procedure.
                        </span>
                    </div>

                    <DialogFooter class="pt-2">
                        <Button type="button" variant="outline" @click="isCreateOpen = false">
                            Hủy
                        </Button>
                        <Button type="submit"> Lập hóa đơn & Trừ kho </Button>
                    </DialogFooter>
                </form>
            </DialogContent>
        </Dialog>
    </Card>
</template>
