import { acceptHMRUpdate, defineStore } from "pinia";
import { jwtDecode } from "jwt-decode";
import { en } from "@/i18n/settings/en";
import AuthApi from "@/services/api/auth";
import LocalStore, { keys } from "@/services/local-store";

export const useSessionStore = defineStore({
  id: "session",
  state: (): Session => ({
    locale: LocalStore.get(keys.language) ?? en.key,
    user: undefined,
    lastUrl: undefined,
    year: new Date().getFullYear(),
  }),
  actions: {
    setLanguage(language: string) {
      this.locale = language;
      LocalStore.set(keys.language, this.locale);
    },
    setYear(year: number) {
      this.year = year;
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

      if (response.data?.token) {
        this.loginWithToken(response.data.token);
      }
    },
    async refreshTokenAsync() {
      const response = await AuthApi.refreshToken();
      if (response.success) {
        this.loginWithToken(response.data!.token);
        return;
      }
      await this.logoutAsync();
    },
    loginWithToken(token: string) {
      const decodedToken = jwtDecode(token) as Object<string>;
      this.user = {
        token: token,
        roles: decodedToken[claims.roles].split(",").map(r => parseInt(r)),
        id: parseInt(decodedToken[claims.id]),
        firstName: decodedToken[claims.firstName],
        middleName: decodedToken[claims.middleName],
        lastName: decodedToken[claims.lastName],
        email: decodedToken[claims.email],
        organization: decodedToken[claims.organization],
        department: decodedToken[claims.department],
        subDepartment: decodedToken[claims.subDepartment],
        organizationId: parseInt(decodedToken[claims.organizationId]),
        departmentId: parseInt(decodedToken[claims.departmentId]),
        subDepartmentId: parseInt(decodedToken[claims.subDepartmentId]),
      };

      LocalStore.set(keys.token, token);
      LocalStore.set(keys.language, this.locale);
    },
    async logoutAsync() {
      LocalStore.remove(keys.token);
      await AuthApi.revokeToken();
      this.user = undefined;
    },
  },
  getters: {
    getLastUrl(): string {
      return getUrl(this.lastUrl);
    },
    isUserAdmin(): boolean {
      return this.user?.roles.some(r => r === 1) ?? false;
    }
  },
});

function getUrl(url: string | undefined) {
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
  year: number;
}

export interface SessionUser {
  id: number;
  firstName: string;
  middleName: string;
  lastName: string;
  email: string;

  roles: number[];

  organization: string;
  organizationId: number;
  department: string;
  departmentId: number;
  subDepartment: string;
  subDepartmentId: number;

  token: string;
}
const claims = {
  id: 'unique_name',
  firstName: 'given_name',
  middleName: 'middleName',
  lastName: 'family_name',
  email: 'email',
  organization: 'organization',
  department: 'department',
  subDepartment: 'subDepartment',
  organizationId: 'organizationId',
  departmentId: 'departmentId',
  subDepartmentId: 'subDepartmentId',
  roles: 'role'
}
