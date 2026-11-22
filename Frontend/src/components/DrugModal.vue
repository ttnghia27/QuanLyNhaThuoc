<script setup lang="ts">
import { ref, watch } from "vue";
import type { Thuoc, LoaiThuoc, CreateThuocRequest } from "@/type";
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
import {
    Select,
    SelectTrigger,
    SelectValue,
    SelectContent,
    SelectItem,
} from "@/components/ui/select";

interface DrugModalProps {
    open: boolean;
    categories: LoaiThuoc[];
    drugToEdit: Thuoc | null;
    isSaving: boolean;
}

const props = defineProps<DrugModalProps>();

const emit = defineEmits<{
    "update:open": [value: boolean];
    save: [payload: CreateThuocRequest, id?: string];
}>();

const tenThuoc = ref("");
const congDung = ref("");
const donGiaBan = ref<number | "">("");
const maLoai = ref("");
const errorMessage = ref("");

watch(
    () => props.open,
    (isOpen) => {
        if (isOpen) {
            errorMessage.value = "";
            if (props.drugToEdit) {
                tenThuoc.value = props.drugToEdit.tenThuoc;
                congDung.value = props.drugToEdit.congDung;
                donGiaBan.value = props.drugToEdit.donGiaBan;
                maLoai.value = props.drugToEdit.maLoai;
            } else {
                tenThuoc.value = "";
                congDung.value = "";
                donGiaBan.value = "";
                maLoai.value = props.categories.length > 0 ? props.categories[0].maLoai : "";
            }
        }
    },
);

function handleClose() {
    emit("update:open", false);
}

function handleSubmit() {
    errorMessage.value = "";
    if (!tenThuoc.value.trim()) {
        errorMessage.value = "Vui lòng nhập tên thuốc.";
        return;
    }
    if (!congDung.value.trim()) {
        errorMessage.value = "Vui lòng nhập công dụng của thuốc.";
        return;
    }
    if (donGiaBan.value === "" || Number(donGiaBan.value) < 0) {
        errorMessage.value = "Vui lòng nhập đơn giá bán hợp lệ (&ge; 0).";
        return;
    }
    if (!maLoai.value) {
        errorMessage.value = "Vui lòng chọn loại thuốc.";
        return;
    }

    const payload: CreateThuocRequest = {
        tenThuoc: tenThuoc.value.trim(),
        congDung: congDung.value.trim(),
        donGiaBan: Number(donGiaBan.value),
        maLoai: maLoai.value,
    };

    emit("save", payload, props.drugToEdit?.maThuoc);
}
</script>

<template>
    <Dialog :open="open" @update:open="emit('update:open', $event)">
        <DialogContent class="sm:max-w-120">
            <DialogHeader>
                <DialogTitle>
                    {{ drugToEdit ? "Chỉnh sửa thông tin thuốc" : "Thêm thuốc mới" }}
                </DialogTitle>
                <DialogDescription>
                    {{
                        drugToEdit
                            ? `Cập nhật thông tin cho thuốc ${drugToEdit.maThuoc}`
                            : "Điền các thông tin để thêm một mặt hàng thuốc mới vào hệ thống."
                    }}
                </DialogDescription>
            </DialogHeader>

            <form @submit.prevent="handleSubmit" class="space-y-4 py-2">
                <div
                    v-if="errorMessage"
                    class="p-3 text-xs text-destructive bg-destructive/10 rounded-md"
                >
                    {{ errorMessage }}
                </div>

                <div class="space-y-1.5">
                    <Label for="tenThuoc">Tên thuốc <span class="text-destructive">*</span></Label>
                    <Input
                        id="tenThuoc"
                        v-model="tenThuoc"
                        placeholder="Ví dụ: Paracetamol 500mg"
                        :disabled="isSaving"
                        required
                    />
                </div>

                <div class="space-y-1.5">
                    <Label for="maLoai">Loại thuốc <span class="text-destructive">*</span></Label>
                    <Select v-model="maLoai" :disabled="isSaving">
                        <SelectTrigger id="maLoai" class="w-full">
                            <SelectValue placeholder="Chọn phân loại thuốc" />
                        </SelectTrigger>
                        <SelectContent>
                            <SelectItem
                                v-for="cat in categories"
                                :key="cat.maLoai"
                                :value="cat.maLoai"
                            >
                                {{ cat.tenLoai }} ({{ cat.maLoai }})
                            </SelectItem>
                        </SelectContent>
                    </Select>
                </div>

                <div class="space-y-1.5">
                    <Label for="donGiaBan"
                        >Đơn giá bán (₫) <span class="text-destructive">*</span></Label
                    >
                    <Input
                        id="donGiaBan"
                        type="number"
                        min="0"
                        step="500"
                        v-model="donGiaBan"
                        placeholder="Ví dụ: 25000"
                        :disabled="isSaving"
                        required
                    />
                </div>

                <div class="space-y-1.5">
                    <Label for="congDung"
                        >Công dụng & chỉ định <span class="text-destructive">*</span></Label
                    >
                    <Input
                        id="congDung"
                        v-model="congDung"
                        placeholder="Ví dụ: Giảm đau, hạ sốt nhanh"
                        :disabled="isSaving"
                        required
                    />
                </div>

                <DialogFooter class="pt-4">
                    <Button
                        type="button"
                        variant="outline"
                        @click="handleClose"
                        :disabled="isSaving"
                    >
                        Hủy
                    </Button>
                    <Button type="submit" :disabled="isSaving">
                        <span
                            v-if="isSaving"
                            class="mr-2 inline-block w-3.5 h-3.5 border-2 border-background border-t-transparent rounded-full animate-spin"
                        />
                        {{ drugToEdit ? "Lưu thay đổi" : "Thêm thuốc" }}
                    </Button>
                </DialogFooter>
            </form>
        </DialogContent>
    </Dialog>
</template>
