import { createPinia } from "pinia";
import { useSessionStore } from "./session";

export const Store = createPinia();

export const initStores = () => {
  useSessionStore();
};
