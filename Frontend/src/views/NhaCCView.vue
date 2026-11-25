<script setup lang="ts">
import { ref, computed } from "vue";
import type { NhaCC, CreateNhaCCRequest } from "@/type";
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
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { TruckIcon, PlusIcon, PencilIcon, Trash2Icon, SearchIcon } from "@lucide/vue";

interface NhaCCViewProps {
    suppliers: NhaCC[];
    isLoading: boolean;
}

const props = defineProps<NhaCCViewProps>();

const emit = defineEmits<{
    createSupplier: [payload: CreateNhaCCRequest];
    updateSupplier: [id: string, payload: CreateNhaCCRequest];
    deleteSupplier: [id: string];
}>();

const searchQuery = ref("");
const isModalOpen = ref(false);
const editingSupplier = ref<NhaCC | null>(null);
const tenNhaCC = ref("");
const sdtNhaCC = ref("");
const formError = ref("");

const filteredSuppliers = computed(() => {
    const q = searchQuery.value.trim().toLowerCase();
    if (!q) return props.suppliers;
    return props.suppliers.filter(
        (ncc) =>
            ncc.tenNhaCC.toLowerCase().includes(q) ||
            ncc.maNhaCC.toLowerCase().includes(q) ||
            (ncc.sdtNhaCC && ncc.sdtNhaCC.toLowerCase().includes(q)),
    );
});

function openCreateModal() {
    editingSupplier.value = null;
    tenNhaCC.value = "";
    sdtNhaCC.value = "";
    formError.value = "";
    isModalOpen.value = true;
}

function openEditModal(ncc: NhaCC) {
    editingSupplier.value = ncc;
    tenNhaCC.value = ncc.tenNhaCC;
    sdtNhaCC.value = ncc.sdtNhaCC || "";
    formError.value = "";
    isModalOpen.value = true;
}

function handleSave() {
    if (!tenNhaCC.value.trim()) {
        formError.value = "Vui lòng nhập tên nhà cung cấp.";
        return;
    }

    const payload: CreateNhaCCRequest = {
        tenNhaCC: tenNhaCC.value.trim(),
        sdtNhaCC: sdtNhaCC.value.trim() || null,
    };

    if (editingSupplier.value) {
        emit("updateSupplier", editingSupplier.value.maNhaCC, payload);
    } else {
        emit("createSupplier", payload);
    }
    isModalOpen.value = false;
}

function handleDelete(ncc: NhaCC) {
    if (confirm(`Bạn có chắc muốn xóa nhà cung cấp "${ncc.tenNhaCC}" (${ncc.maNhaCC})?`)) {
        emit("deleteSupplier", ncc.maNhaCC);
    }
}
</script>

<template>
    <Card class="border-border/60 shadow-xs">
        <CardHeader class="pb-3">
            <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
                <div>
                    <CardTitle class="text-base font-semibold flex items-center gap-2">
                        <TruckIcon class="w-4 h-4 text-primary" />
                        Bảng Dữ liệu: Nhà cung cấp (NhaCC)
                    </CardTitle>
                    <CardDescription class="text-xs">
                        Quản lý các công ty dược và đối tác cung ứng thuốc cho nhà thuốc
                    </CardDescription>
                </div>

                <Button size="sm" @click="openCreateModal">
                    <PlusIcon class="w-3.5 h-3.5 mr-1" />
                    Thêm nhà cung cấp
                </Button>
            </div>

            <!-- Ô tìm kiếm nhà cung cấp -->
            <div class="relative flex-1">
                <SearchIcon
                    class="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground"
                />
                <Input
                    v-model="searchQuery"
                    placeholder="Tìm theo tên nhà cung cấp, SĐT hoặc mã NCC..."
                    class="pl-9 text-sm h-9"
                />
            </div>
        </CardHeader>

        <CardContent>
            <div class="border rounded-md overflow-x-auto">
                <Table>
                    <TableHeader>
                        <TableRow>
                            <TableHead class="w-28">Mã NCC</TableHead>
                            <TableHead>Tên nhà cung cấp</TableHead>
                            <TableHead class="w-48">Số điện thoại</TableHead>
                            <TableHead class="w-28 text-right">Thao tác</TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        <TableRow v-if="isLoading">
                            <TableCell
                                colspan="4"
                                class="text-center py-10 text-muted-foreground text-sm"
                            >
                                Đang tải danh sách nhà cung cấp...
                            </TableCell>
                        </TableRow>
                        <TableRow v-else-if="suppliers.length === 0">
                            <TableCell
                                colspan="4"
                                class="text-center py-10 text-muted-foreground text-sm"
                            >
                                Chưa có nhà cung cấp nào.
                            </TableCell>
                        </TableRow>
                        <TableRow v-else-if="filteredSuppliers.length === 0">
                            <TableCell
                                colspan="4"
                                class="text-center py-10 text-muted-foreground text-sm"
                            >
                                Không tìm thấy nhà cung cấp nào phù hợp với từ khóa "{{
                                    searchQuery
                                }}".
                            </TableCell>
                        </TableRow>
                        <TableRow
                            v-else
                            v-for="ncc in filteredSuppliers"
                            :key="ncc.maNhaCC"
                            class="hover:bg-muted/40 transition-colors"
                        >
                            <TableCell class="font-mono text-xs font-semibold text-primary">
                                {{ ncc.maNhaCC }}
                            </TableCell>
                            <TableCell class="font-medium text-sm">
                                {{ ncc.tenNhaCC }}
                            </TableCell>
                            <TableCell class="text-xs font-mono">
                                {{ ncc.sdtNhaCC || "—" }}
                            </TableCell>
                            <TableCell class="text-right">
                                <div class="inline-flex items-center justify-end gap-1">
                                    <Button
                                        variant="ghost"
                                        size="icon-xs"
                                        class="hover:text-primary"
                                        title="Chỉnh sửa thông tin"
                                        @click="openEditModal(ncc)"
                                    >
                                        <PencilIcon class="w-3.5 h-3.5" />
                                    </Button>
                                    <Button
                                        variant="ghost"
                                        size="icon-xs"
                                        class="text-destructive hover:text-destructive hover:bg-destructive/10"
                                        title="Xóa nhà cung cấp"
                                        @click="handleDelete(ncc)"
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
                <span
                    >Hiển thị {{ filteredSuppliers.length }} / {{ suppliers.length }} nhà cung
                    cấp</span
                >
                <span v-if="searchQuery">Đang lọc theo: "{{ searchQuery }}"</span>
            </div>
        </CardContent>

        <!-- Dialog Add/Edit -->
        <Dialog :open="isModalOpen" @update:open="isModalOpen = $event">
            <DialogContent class="sm:max-w-[460px]">
                <DialogHeader>
                    <DialogTitle>{{
                        editingSupplier ? "Chỉnh sửa Nhà cung cấp" : "Thêm Nhà cung cấp mới"
                    }}</DialogTitle>
                    <DialogDescription>
                        Điền tên và số điện thoại liên hệ của đối tác cung ứng.
                    </DialogDescription>
                </DialogHeader>

                <form @submit.prevent="handleSave" class="space-y-4 py-2">
                    <div
                        v-if="formError"
                        class="p-2 text-xs text-destructive bg-destructive/10 rounded"
                    >
                        {{ formError }}
                    </div>

                    <div class="space-y-1.5">
                        <Label for="tenNhaCC"
                            >Tên nhà cung cấp <span class="text-destructive">*</span></Label
                        >
                        <Input
                            id="tenNhaCC"
                            v-model="tenNhaCC"
                            placeholder="Ví dụ: Công ty Dược TW 1"
                            required
                        />
                    </div>

                    <div class="space-y-1.5">
                        <Label for="sdtNhaCC">Số điện thoại</Label>
                        <Input id="sdtNhaCC" v-model="sdtNhaCC" placeholder="Ví dụ: 02438250000" />
                    </div>

                    <DialogFooter class="pt-2">
                        <Button type="button" variant="outline" @click="isModalOpen = false">
                            Hủy
                        </Button>
                        <Button type="submit">
                            {{ editingSupplier ? "Lưu thay đổi" : "Thêm mới" }}
                        </Button>
                    </DialogFooter>
                </form>
            </DialogContent>
        </Dialog>
    </Card>
</template>
