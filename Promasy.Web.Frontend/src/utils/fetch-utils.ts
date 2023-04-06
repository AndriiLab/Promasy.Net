import { getCultureName } from "@/i18n";
import { useSessionStore, SessionUser } from "@/store/session";
import { Router } from "@/router";

export const Fetch = {
  Get: function <TOkResponse, TErrorResponse>(
    input: RequestInfo | URL,
    init?: RequestInit | undefined,
  ): Promise<Response<TOkResponse, TErrorResponse>> {
    return typedFetchAsync<TOkResponse, TErrorResponse>("GET", input, init, true);
  },
  Post: function <TOkResponse, TErrorResponse>(
    input: RequestInfo | URL,
    init?: RequestInit | undefined,
  ): Promise<Response<TOkResponse, TErrorResponse>> {
    return typedFetchAsync<TOkResponse, TErrorResponse>("POST", input, init, true);
  },
  Put: function <TOkResponse, TErrorResponse>(
    input: RequestInfo | URL,
    init?: RequestInit | undefined,
  ): Promise<Response<TOkResponse, TErrorResponse>> {
    return typedFetchAsync<TOkResponse, TErrorResponse>("PUT", input, init, true);
  },
  Delete: function <TOkResponse, TErrorResponse>(
    input: RequestInfo | URL,
    init?: RequestInit | undefined,
  ): Promise<Response<TOkResponse, TErrorResponse>> {
    return typedFetchAsync<TOkResponse, TErrorResponse>("DELETE", input, init, true);
  },
};

const nonRetryUrls = [ "/api/auth/refresh", "/api/auth/revoke" ];

async function typedFetchAsync<TOkResponse, TErrorResponse>(
  method: string,
  input: RequestInfo | URL,
  init?: RequestInit,
  retry?: boolean,
): Promise<Response<TOkResponse, TErrorResponse>> {
  const ri = init ?? ({} as RequestInit);
  ri.method = method;

  const { user, refreshTokenAsync } = useSessionStore();
  ri.headers = buildHeaders(ri, user);

  const fetchResponse = await fetch(input, ri);
  const response: Response<TOkResponse, TErrorResponse> = {
    success: fetchResponse.ok,
    status: fetchResponse.status,
    statusText: fetchResponse.statusText,
  };

  const body = await safeReadJsonBodyAsync(fetchResponse);
  if (fetchResponse.ok) {
    response.data = body as TOkResponse;
  } else if (fetchResponse.status === 401) {
    if (retry && nonRetryUrls.every(u => u !== input)) {
      await refreshTokenAsync();
      if (user) {
        return typedFetchAsync<TOkResponse, TErrorResponse>(method, input, init, false);
      }
    }
    await Router.push({ name: "Logout" });
  } else {
    response.error = body as TErrorResponse;
  }
  return response;
}

function buildHeaders(ri: RequestInit, user: SessionUser | undefined): Headers {
  const headers = (ri.headers as Headers) ?? (new Headers());
  const { locale } = useSessionStore();
  headers.append("Accept-Language", getCultureName(locale));
  headers.append("Time-Zone", Intl.DateTimeFormat().resolvedOptions().timeZone);

  const token = user?.token;
  if (token) {
    headers.append("Authorization", `Bearer ${ token }`);
  }
  const contentNotDefined = !headers.has("Content-Type");
  if (contentNotDefined && ri.body) {
    headers.append("Content-Type", "application/json");
  }
  return headers;
}

async function safeReadJsonBodyAsync(response: globalThis.Response) {
  try {
    return await response.json();
  } catch {
    return undefined;
  }
}

export interface Response<TOkResponse, TErrorResponse> {
  success: boolean;
  status: number;
  statusText: string;
  data?: TOkResponse;
  error?: TErrorResponse;
}

export interface ErrorApiResponse extends Error {
  type: string,
  title: string,
  status: number,
  detail: string,
  errors?: Object<string[]>;
}

export interface PagedResponse<T> {
  page: number;
  total: number;
  collection: T[];
}

export interface SelectItem<T> {
  value: T;
  text: string;
}