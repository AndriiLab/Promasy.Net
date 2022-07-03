import {
  ErrorApiResponse,
  Fetch,
  Response,
  PagedResponse,
} from "@/utils/fetch-utils";
import { buildQueryParameters } from "@/utils/url-params-utils";

export default {
  getList(
    page: number,
    offset: number,
    search?: string,
    order?: string,
    descending?: boolean,
  ): Promise<Response<PagedResponse<Supplier>, ErrorApiResponse>> {
    return Fetch.Get<PagedResponse<Supplier>, ErrorApiResponse>(
      `/api/suppliers${ buildQueryParameters([
        [ "offset", offset.toString() ],
        [ "page", page.toString() ],
        [ "search", search ],
        [ "order", order ],
        [ "desc", descending ? "true" : undefined ],
      ]) }`,
    );
  },
  getById(id: number): Promise<Response<Supplier, ErrorApiResponse>> {
    return Fetch.Get<Supplier, ErrorApiResponse>(`/api/suppliers/${ id }`);
  },
  create(request: CreateSupplierRequest): Promise<Response<Supplier, ErrorApiResponse>> {
    return Fetch.Post<Supplier, ErrorApiResponse>("/api/suppliers", { body: JSON.stringify(request) });
  },
  update(request: UpdateSupplierRequest): Promise<Response<string, ErrorApiResponse>> {
    return Fetch.Put<string, ErrorApiResponse>(`/api/suppliers/${ request.id }`, { body: JSON.stringify(request) });
  },
  delete(id: number): Promise<Response<string, ErrorApiResponse>> {
    return Fetch.Delete<string, ErrorApiResponse>(`/api/suppliers/${ id }`);
  },
  merge(request: MergeSupplierRequest): Promise<Response<string, ErrorApiResponse>> {
    return Fetch.Post<string, ErrorApiResponse>("/api/suppliers/merge", { body: JSON.stringify(request) });
  },
};

export interface CreateSupplierRequest {
  name: string;
  comment: string | undefined;
  phone: string | undefined;
}

export interface UpdateSupplierRequest extends CreateSupplierRequest {
  id: number;
}

export interface Supplier {
  id: number;
  name: string;
  comment: string | undefined;
  phone: string | undefined;
  editorId: number;
  editor: string;
  editedDate: Date;
}

export interface MergeSupplierRequest {
  targetId: number,
  sourceIds: number[]
}
