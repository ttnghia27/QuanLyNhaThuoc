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
import { ShieldAlertIcon, LogInIcon } from "@lucide/vue";
import { clearAuthSession } from "@/services/api";

const router = useRouter();

function handleGoLogin() {
  clearAuthSession();
  router.push("/login");
}
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-neutral-50/60 dark:bg-neutral-950 p-4">
    <Card class="w-full max-w-md text-center shadow-lg border-border">
      <CardHeader class="space-y-3 pb-2">
        <div class="mx-auto w-14 h-14 rounded-2xl bg-amber-500/10 text-amber-600 dark:text-amber-400 flex items-center justify-center border border-amber-500/20">
          <ShieldAlertIcon class="w-7 h-7" />
        </div>
        <div class="space-y-1">
          <span class="inline-block px-2.5 py-0.5 text-xs font-semibold uppercase tracking-wider text-amber-700 dark:text-amber-400 bg-amber-100 dark:bg-amber-950/60 rounded-full">
            Lỗi 403
          </span>
          <CardTitle class="text-2xl font-bold tracking-tight">
            Truy cập bị từ chối
          </CardTitle>
        </div>
        <CardDescription class="text-sm text-muted-foreground">
          Bạn chưa đăng nhập hoặc phiên làm việc đã hết hạn. Vui lòng đăng nhập để tiếp tục truy cập hệ thống.
        </CardDescription>
      </CardHeader>

      <CardContent class="text-xs text-muted-foreground pt-2">
        Mã lỗi: <code class="font-mono bg-muted px-1.5 py-0.5 rounded">403_FORBIDDEN_UNAUTHENTICATED</code>
      </CardContent>

      <CardFooter class="pt-4 flex justify-center">
        <Button class="w-full sm:w-auto" @click="handleGoLogin">
          <LogInIcon class="w-4 h-4 mr-2" />
          Đăng nhập ngay
        </Button>
      </CardFooter>
    </Card>
  </div>
</template>
