import { acceptHMRUpdate, defineStore } from "pinia";
import jwtDecode from "jwt-decode";
import { en } from "@/i18n/settings/en";
import { Router } from "@/router";
import AuthApi from "@/services/api/auth";
import LocalStore, { keys } from "@/services/local-store";

export const useSessionStore = defineStore({
  id: "session",
  state: (): Session => ({
    locale: LocalStore.get(keys.language) ?? en.key,
    user: undefined,
    lastUrl: undefined,
  }),
  actions: {
    setLanguage(language: string) {
      this.locale = language;
      LocalStore.set(keys.language, this.locale);
    },
    async loginAsync(username: string, password: string, rememberMe: boolean) {
      const response = await AuthApi.authUser({
        user: username,
        password: password,
      });
      if (!response.success) {
        throw response.error;
      }
      rememberMe ? LocalStore.allow() : LocalStore.disable();

      this.loginWithToken(response.data?.token ?? "")
    },
    async refreshTokenAsync() {
        const response = await AuthApi.refreshToken();
        if(response.success) {
            this.loginWithToken(response.data!.token);
            return;
        }
        await this.logoutAsync();
    },
    loginWithToken(token: string) {
      const decodedToken = jwtDecode(token) as Object<string>;
      this.user = {
        token: token,
        roles: decodedToken.roles.split(","),
        id: parseInt(decodedToken.id),
        firstName: decodedToken.firstName,
        middleName: decodedToken.middleName,
        lastName: decodedToken.lastName,
        email: decodedToken.email,
        organization: decodedToken.organization,
        department: decodedToken.department,
        subDepartment: decodedToken.subDepartment,
        organizationId: parseInt(decodedToken.organizationId),
        departmentId: parseInt(decodedToken.departmentId),
        subDepartmentId: parseInt(decodedToken.subDepartmentId),
      };

      LocalStore.set(keys.token, token);
      LocalStore.set(keys.language, this.locale);

      Router.push(getUrl(this.lastUrl));
    },
    async logoutAsync() {
      this.user = undefined;
      LocalStore.remove(keys.token);
      await AuthApi.revokeToken();
      Router.push("/login");
    },
  },
  getters: {
    getLastUrl() : string {
      return getUrl(this.lastUrl);
    }
  }
});

function getUrl(url: string | undefined){
  return url ?? "/";
}

// Add HMR support
if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useSessionStore, import.meta.hot));
}

export interface Session {
  locale: string;
  user?: SessionUser;
  lastUrl?: string;
}

export interface SessionUser {
  id: number;
  firstName: string;
  middleName: string;
  lastName: string;
  email: string;

  roles: string[];

  organization: string;
  organizationId: number;
  department: string;
  departmentId: number;
  subDepartment: string;
  subDepartmentId: number;

  token: string;
}
