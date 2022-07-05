import { SelectItem } from "@/utils/fetch-utils";
import EmployeesApi from "@/services/api/employees";
import { acceptHMRUpdate, defineStore } from "pinia";

export const useRolesStore = defineStore({
  id: "roles",
  state: (): Roles => ({
    roles: [],
  }),
  actions: {
    async setRolesAsync() {
      const response = await EmployeesApi.getRolesList();
      if (response.success) {
        this.roles = response.data!;
      }
    },
  }
});

// Add HMR support
if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useRolesStore, import.meta.hot));
}

export interface Roles {
  roles: SelectItem<number>[];
}
