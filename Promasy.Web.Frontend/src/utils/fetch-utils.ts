import { Router } from "@/router";
import { useSessionStore, SessionUser } from "@/store/session";

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

async function typedFetchAsync<TOkResponse, TErrorResponse>(
  method: string,
  input: RequestInfo | URL,
  init?: RequestInit,
  retry?: boolean,
): Promise<Response<TOkResponse, TErrorResponse>> {
  const ri = init ?? ({} as RequestInit);
  ri.method = method;

  const sessionStore = useSessionStore();
  ri.headers = buildHeaders(ri, sessionStore.user);

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
    if (retry) {
      await sessionStore.refreshTokenAsync();
      if (sessionStore.user) {
        return typedFetchAsync<TOkResponse, TErrorResponse>(method, input, init, false);
      }
    }
    Router.push("/");
  } else {
    response.error = body as TErrorResponse;
  }
  return response;
}

function buildHeaders(ri: RequestInit, user: SessionUser | undefined): Headers {
  const headers = (ri.headers as Headers) ?? (new Headers());
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
  errors: Object<string[]>;
}
