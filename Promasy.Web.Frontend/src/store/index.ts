import { createPinia } from "pinia";
import { useSessionStore } from "./session";
import { useRolesStore } from "./roles";

export const Store = createPinia();

export const initStores = () => {
  useSessionStore();
  useRolesStore();
};
