import { createRouter, createWebHistory, type RouteRecordRaw } from "vue-router";
import { isAuthenticated } from "@/services/api";

const routes: RouteRecordRaw[] = [
  {
    path: "/login",
    name: "Login",
    component: () => import("@/views/LoginView.vue"),
    meta: { guestOnly: true },
  },
  {
    path: "/",
    name: "Home",
    component: () => import("@/views/MainLayoutView.vue"),
    meta: { requiresAuth: true },
  },
  {
    path: "/forbidden",
    name: "Forbidden",
    component: () => import("@/views/ForbiddenView.vue"),
  },
  {
    path: "/404",
    name: "NotFound",
    component: () => import("@/views/NotFoundView.vue"),
  },
  {
    path: "/:pathMatch(.*)*",
    redirect: "/404",
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach((to, _from, next) => {
  const authed = isAuthenticated();

  // Yêu cầu: Routing mặc định luôn là trang loginview.vue chỉ khi đăng nhập thành công mới redirect sang root "/"
  // Thêm trang 403 forbidden khi người dùng chưa đăng nhập cố truy cập trang yêu cầu đăng nhập.
  if (to.meta.requiresAuth) {
    if (!authed) {
      next({ name: "Forbidden" });
      return;
    }
  }

  // Nếu người dùng đã đăng nhập mà vào lại /login thì redirect về "/"
  if (to.meta.guestOnly && authed) {
    next({ name: "Home" });
    return;
  }

  next();
});

export default router;
