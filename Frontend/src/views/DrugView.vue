<script setup lang="ts">
import type { CreateThuocRequest, LoaiThuoc, Thuoc } from "@/type";
import DrugTable from "@/components/DrugTable.vue";
import DrugModal from "@/components/DrugModal.vue";

interface DrugViewProps {
  drugs: Thuoc[];
  categories: LoaiThuoc[];
  isLoading: boolean;
  searchQuery: string;
  selectedCategory: string;
  isModalOpen: boolean;
  drugToEdit: Thuoc | null;
  isSaving: boolean;
}

defineProps<DrugViewProps>();

const emit = defineEmits<{
  "update:searchQuery": [value: string];
  "update:selectedCategory": [value: string];
  search: [];
  createDrug: [];
  editDrug: [drug: Thuoc];
  deleteDrug: [drug: Thuoc];
  openCategoryManager: [];
  "update:modalOpen": [value: boolean];
  save: [payload: CreateThuocRequest, id?: string];
}>();
</script>

<template>
  <DrugTable
    :drugs="drugs"
    :categories="categories"
    :is-loading="isLoading"
    :search-query="searchQuery"
    :selected-category="selectedCategory"
    @update:search-query="emit('update:searchQuery', $event)"
    @update:selected-category="emit('update:selectedCategory', $event)"
    @search="emit('search')"
    @create-drug="emit('createDrug')"
    @edit-drug="emit('editDrug', $event)"
    @delete-drug="emit('deleteDrug', $event)"
    @open-category-manager="emit('openCategoryManager')"
  />

  <DrugModal
    :open="isModalOpen"
    :categories="categories"
    :drug-to-edit="drugToEdit"
    :is-saving="isSaving"
    @update:open="emit('update:modalOpen', $event)"
    @save="(payload, id) => emit('save', payload, id)"
  />
</template>
