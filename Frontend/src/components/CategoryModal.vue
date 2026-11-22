<script setup lang="ts">
import { ref } from "vue";
import type { LoaiThuoc } from "@/type";
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogDescription,
    DialogFooter,
} from "@/components/ui/dialog";
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
import { PlusIcon, Trash2Icon } from "@lucide/vue";

interface CategoryModalProps {
    open: boolean;
    categories: LoaiThuoc[];
    isLoading: boolean;
}

const props = defineProps<CategoryModalProps>();

const emit = defineEmits<{
    "update:open": [value: boolean];
    createCategory: [tenLoai: string];
    deleteCategory: [maLoai: string];
}>();

const newCategoryName = ref("");

function handleAddCategory() {
    if (!newCategoryName.value.trim()) return;
    emit("createCategory", newCategoryName.value.trim());
    newCategoryName.value = "";
}

function handleDeleteCategory(maLoai: string) {
    if (confirm(`Bạn có chắc chắn muốn xóa loại thuốc mã ${maLoai}?`)) {
        emit("deleteCategory", maLoai);
    }
}
</script>

<template>
    <Dialog :open="open" @update:open="emit('update:open', $event)">
        <DialogContent class="sm:max-w-140">
            <DialogHeader>
                <DialogTitle>Quản lý Loại thuốc</DialogTitle>
                <DialogDescription>
                    Thêm hoặc quản lý các danh mục phân loại thuốc trong nhà thuốc.
                </DialogDescription>
            </DialogHeader>

            <!-- Form thêm loại mới -->
            <form @submit.prevent="handleAddCategory" class="flex gap-2 items-center pt-2">
                <Input
                    v-model="newCategoryName"
                    placeholder="Nhập tên loại thuốc mới (vd: Thuốc kháng sinh)..."
                    class="flex-1"
                />
                <Button type="submit" size="sm" :disabled="!newCategoryName.trim()">
                    <PlusIcon class="w-4 h-4 mr-1" />
                    Thêm loại
                </Button>
            </form>

            <!-- Danh sách loại -->
            <div class="border rounded-md mt-3 max-h-64 overflow-y-auto">
                <Table>
                    <TableHeader>
                        <TableRow>
                            <TableHead class="w-24">Mã loại</TableHead>
                            <TableHead>Tên phân loại</TableHead>
                            <TableHead class="w-28 text-center">Số lượng thuốc</TableHead>
                            <TableHead class="w-20 text-right">Xóa</TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        <TableRow v-if="categories.length === 0">
                            <TableCell
                                colspan="4"
                                class="text-center py-6 text-muted-foreground text-sm"
                            >
                                Chưa có loại thuốc nào.
                            </TableCell>
                        </TableRow>
                        <TableRow v-for="cat in categories" :key="cat.maLoai">
                            <TableCell class="font-mono text-xs font-semibold">{{
                                cat.maLoai
                            }}</TableCell>
                            <TableCell class="font-medium text-sm">{{ cat.tenLoai }}</TableCell>
                            <TableCell class="text-center">
                                <Badge variant="secondary" class="text-xs">
                                    {{ cat.soLuongThuoc ?? 0 }} thuốc
                                </Badge>
                            </TableCell>
                            <TableCell class="text-right">
                                <Button
                                    variant="ghost"
                                    size="icon-xs"
                                    class="text-destructive hover:text-destructive hover:bg-destructive/10"
                                    title="Xóa loại thuốc"
                                    @click="handleDeleteCategory(cat.maLoai)"
                                >
                                    <Trash2Icon class="w-3.5 h-3.5" />
                                </Button>
                            </TableCell>
                        </TableRow>
                    </TableBody>
                </Table>
            </div>

            <DialogFooter class="pt-2">
                <Button variant="outline" @click="emit('update:open', false)"> Đóng </Button>
            </DialogFooter>
        </DialogContent>
    </Dialog>
</template>
