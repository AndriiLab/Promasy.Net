import { createRouter, createWebHashHistory } from "vue-router";
import { routes, publicPages } from "@/routes";
import { useSessionStore } from "@/store/session";

export const Router = createRouter({
  history: createWebHashHistory(),
  routes,
});

Router.beforeEach(async (to, from) => {
  const authRequired = !publicPages.includes(to.path);
  const sessionStore = useSessionStore();

  if (authRequired && !sessionStore.user) {
    sessionStore.lastUrl = to.fullPath;
    return "/login";
  }

  if (sessionStore.user && to.path === "/login") {
    return "/";
  }
  sessionStore.lastUrl = from.fullPath === "/logout" ? "/" : from.fullPath;
});
