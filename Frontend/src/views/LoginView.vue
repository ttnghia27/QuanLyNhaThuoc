<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import type { LoginRequest, LoginResponse } from "@/type";
import { pharmacyApi, setAuthSession } from "@/services/api";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
  CardFooter,
} from "@/components/ui/card";
import { PillIcon, Loader2Icon, AlertCircleIcon } from "@lucide/vue";

const router = useRouter();

const emit = defineEmits<{
  success: [response: LoginResponse];
}>();

const tenDangNhap = ref("");
const matKhau = ref("");
const isLoading = ref(false);
const errorMessage = ref("");

async function handleSubmit() {
  errorMessage.value = "";

  const trimmedUsername = tenDangNhap.value.trim();
  if (!trimmedUsername || !matKhau.value) {
    errorMessage.value = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
    return;
  }

  const payload: LoginRequest = {
    tenDangNhap: trimmedUsername,
    matKhau: matKhau.value,
  };

  isLoading.value = true;
  try {
    const response = await pharmacyApi.login(payload);
    // Lưu token và thông tin người dùng vào localStorage
    setAuthSession(response.token, response.hoTen);
    emit("success", response);
    // Điều hướng sang root "/"
    await router.push("/");
  } catch (error: unknown) {
    errorMessage.value =
      error instanceof Error ? error.message : "Đăng nhập thất bại. Vui lòng thử lại.";
  } finally {
    isLoading.value = false;
  }
}
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-neutral-50/60 dark:bg-neutral-950 p-4">
    <Card class="w-full max-w-md shadow-lg border-border">
      <CardHeader class="space-y-2 text-center pb-4">
        <div class="mx-auto w-12 h-12 rounded-xl bg-primary text-primary-foreground flex items-center justify-center shadow-md mb-2">
          <PillIcon class="w-6 h-6" />
        </div>
        <CardTitle class="text-2xl font-bold tracking-tight">
          Hệ thống PharmaCare
        </CardTitle>
        <CardDescription class="text-sm">
          Đăng nhập vào tài khoản để quản lý quầy thuốc
        </CardDescription>
      </CardHeader>

      <form @submit.prevent="handleSubmit">
        <CardContent class="space-y-4 pt-2">
          <!-- Error alert -->
          <div
            v-if="errorMessage"
            role="alert"
            class="flex items-center gap-2 p-3 text-sm rounded-lg border border-destructive/40 bg-destructive/10 text-destructive"
          >
            <AlertCircleIcon class="w-4 h-4 shrink-0" />
            <span>{{ errorMessage }}</span>
          </div>

          <!-- Tên đăng nhập -->
          <div class="space-y-2">
            <Label for="tenDangNhap">Tên đăng nhập</Label>
            <Input
              id="tenDangNhap"
              v-model="tenDangNhap"
              name="username"
              type="text"
              autocomplete="username"
              placeholder="Nhập tên đăng nhập..."
              :disabled="isLoading"
              required
            />
          </div>

          <!-- Mật khẩu -->
          <div class="space-y-2">
            <Label for="matKhau">Mật khẩu</Label>
            <Input
              id="matKhau"
              v-model="matKhau"
              name="password"
              type="password"
              autocomplete="current-password"
              placeholder="••••••••"
              :disabled="isLoading"
              required
            />
          </div>
        </CardContent>

        <CardFooter class="flex flex-col gap-3 pt-2">
          <Button
            type="submit"
            class="w-full"
            :disabled="isLoading"
          >
            <Loader2Icon v-if="isLoading" class="w-4 h-4 mr-2 animate-spin" />
            {{ isLoading ? "Đang xác thực..." : "Đăng nhập" }}
          </Button>

          <p class="text-xs text-center text-muted-foreground">
            Tài khoản mẫu: <span class="font-medium text-foreground">admin</span> / <span class="font-medium text-foreground">admin</span>
          </p>
        </CardFooter>
      </form>
    </Card>
  </div>
</template>
