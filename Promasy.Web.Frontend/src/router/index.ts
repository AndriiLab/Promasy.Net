import { createRouter, createWebHashHistory } from "vue-router";
import { routes, publicPages, errorPages } from "@/routes";
import { useSessionStore } from "@/store/session";

export const Router = createRouter({
  history: createWebHashHistory(),
  routes,
});

Router.beforeEach((to, from) => {
  const authRequired = !publicPages.includes(to.path);
  const sessionStore = useSessionStore();

  if (authRequired && !sessionStore.user) {
    sessionStore.lastUrl = to.fullPath;
    return "/login";
  }

  if (sessionStore.user && to.path === "/login") {
    return "/";
  }
  if (!errorPages.includes(to.path)) {
    sessionStore.lastUrl = from.fullPath === "/logout" ? "/" : from.fullPath;
  }
});
