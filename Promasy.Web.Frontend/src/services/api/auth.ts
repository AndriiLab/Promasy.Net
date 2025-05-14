import { ErrorApiResponse, Response, Fetch } from "@/utils/fetch-utils";
import {PermissionAction} from "@/constants/PermissionActionEnum";
import {PermissionCondition} from "@/constants/PermissionConditionEnum";

export default {
  authUser(userData: AuthUserRequest): Promise<Response<AuthUserResponse, ErrorApiResponse>> {
    return Fetch.Post<AuthUserResponse, ErrorApiResponse>("/api/auth", { body: JSON.stringify(userData) });
  },
  refreshToken(): Promise<Response<AuthUserResponse, ErrorApiResponse>> {
    return Fetch.Get<AuthUserResponse, ErrorApiResponse>("/api/auth/refresh");
  },
  revokeToken(): Promise<Response<string, ErrorApiResponse>> {
    return Fetch.Post<string, ErrorApiResponse>("/api/auth/revoke");
  },
  getPermissions(): Promise<Response<PermissionsResponse, ErrorApiResponse>> {
    return Fetch.Get<PermissionsResponse, ErrorApiResponse>("/api/auth/permissions");
  },
};

interface AuthUserRequest {
  user: string;
  password: string;
}

interface AuthUserResponse {
  token: string;
}

interface PermissionsResponse {
  permissions: EndpointPermission[];
}

export interface EndpointPermission {
  key: string;
  action: PermissionAction,
  condition: PermissionCondition;
}
