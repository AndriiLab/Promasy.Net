import { ErrorApiResponse, Response, Fetch } from "@/utils/fetch-utils";

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
};

interface AuthUserRequest {
  user: string;
  password: string;
}

interface AuthUserResponse {
  token: string;
}
