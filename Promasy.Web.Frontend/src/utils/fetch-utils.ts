import { useSessionStore, SessionUser } from "@/store/session";

export const Fetch: FetchMethods = {
  Get: function <TOkResponse, TErrorResponse>(
    input: RequestInfo | URL,
    init?: RequestInit | undefined
  ): Promise<Response<TOkResponse, TErrorResponse>> {
    return typedFetchAsync<TOkResponse, TErrorResponse>("GET", input, init);
  },
  Post: function <TOkResponse, TErrorResponse>(
    input: RequestInfo | URL,
    init?: RequestInit | undefined
  ): Promise<Response<TOkResponse, TErrorResponse>> {
    return typedFetchAsync<TOkResponse, TErrorResponse>("POST", input, init);
  },
  Put: function <TOkResponse, TErrorResponse>(
    input: RequestInfo | URL,
    init?: RequestInit | undefined
  ): Promise<Response<TOkResponse, TErrorResponse>> {
    return typedFetchAsync<TOkResponse, TErrorResponse>("PUT", input, init);
  },
  Delete: function <TOkResponse, TErrorResponse>(
    input: RequestInfo | URL,
    init?: RequestInit | undefined
  ): Promise<Response<TOkResponse, TErrorResponse>> {
    return typedFetchAsync<TOkResponse, TErrorResponse>("DELETE", input, init);
  },
};
async function typedFetchAsync<TOkResponse, TErrorResponse>(
  method: string,
  input: RequestInfo | URL,
  init?: RequestInit
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
    await refreshTokenAsync();
  } else {
    response.error = body as TErrorResponse;
  }
  return response;
}

function buildHeaders(ri: RequestInit, user: SessionUser | undefined): Headers {
  const headers = (ri.headers as Headers) ?? (new Headers());
  const token = user?.token;
  if (token) {
    headers.append("Authorization", `Bearer ${token}`);
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

interface FetchMethods {
  Get<TOkResponse, TErrorResponse>(
    input: RequestInfo | URL,
    init?: RequestInit
  ): Promise<Response<TOkResponse, TErrorResponse>>;
  Post<TOkResponse, TErrorResponse>(
    input: RequestInfo | URL,
    init?: RequestInit
  ): Promise<Response<TOkResponse, TErrorResponse>>;
  Put<TOkResponse, TErrorResponse>(
    input: RequestInfo | URL,
    init?: RequestInit
  ): Promise<Response<TOkResponse, TErrorResponse>>;
  Delete<TOkResponse, TErrorResponse>(
    input: RequestInfo | URL,
    init?: RequestInit
  ): Promise<Response<TOkResponse, TErrorResponse>>;
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
