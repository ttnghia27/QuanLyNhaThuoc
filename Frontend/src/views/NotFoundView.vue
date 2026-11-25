<script setup lang="ts">
import { useRouter } from "vue-router";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
  CardFooter,
} from "@/components/ui/card";
import { FileQuestionIcon, HomeIcon } from "@lucide/vue";

const router = useRouter();

function handleGoHome() {
  router.push("/");
}
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-neutral-50/60 dark:bg-neutral-950 p-4">
    <Card class="w-full max-w-md text-center shadow-lg border-border">
      <CardHeader class="space-y-3 pb-2">
        <div class="mx-auto w-14 h-14 rounded-2xl bg-destructive/10 text-destructive flex items-center justify-center border border-destructive/20">
          <FileQuestionIcon class="w-7 h-7" />
        </div>
        <div class="space-y-1">
          <span class="inline-block px-2.5 py-0.5 text-xs font-semibold uppercase tracking-wider text-destructive bg-destructive/10 rounded-full">
            Lỗi 404
          </span>
          <CardTitle class="text-2xl font-bold tracking-tight">
            Không tìm thấy trang
          </CardTitle>
        </div>
        <CardDescription class="text-sm text-muted-foreground">
          Đường dẫn bạn yêu cầu không tồn tại hoặc đã bị thay đổi trong hệ thống PharmaCare.
        </CardDescription>
      </CardHeader>

      <CardContent class="text-xs text-muted-foreground pt-2">
        Vui lòng kiểm tra lại địa chỉ URL hoặc quay về trang chủ.
      </CardContent>

      <CardFooter class="pt-4 flex justify-center">
        <Button class="w-full sm:w-auto" @click="handleGoHome">
          <HomeIcon class="w-4 h-4 mr-2" />
          Quay lại trang chủ
        </Button>
      </CardFooter>
    </Card>
  </div>
</template>
