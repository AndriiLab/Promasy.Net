import {
  ErrorApiResponse,
  Fetch,
  Response,
  PagedResponse,
} from "@/utils/fetch-utils";
import { buildQueryParameters } from "@/utils/url-params-utils";

export default {
  getList(
    organizationId: number,
    page: number,
    offset: number,
    search?: string,
    order?: string,
    descending?: boolean,
  ): Promise<Response<PagedResponse<Department>, ErrorApiResponse>> {
    return Fetch.Get<PagedResponse<Department>, ErrorApiResponse>(
      `/api/organizations/${ organizationId }/departments${ buildQueryParameters([
        [ "offset", offset.toString() ],
        [ "page", page.toString() ],
        [ "search", search ],
        [ "order", order ],
        [ "desc", descending ? "true" : undefined ],
      ]) }`,
    );
  },
  getById(id: number, organizationId: number): Promise<Response<Department, ErrorApiResponse>> {
    return Fetch.Get<Department, ErrorApiResponse>(`/api/organizations/${ organizationId }/departments/${ id }`);
  },
  create(request: CreateDepartmentRequest): Promise<Response<Department, ErrorApiResponse>> {
    return Fetch.Post<Department, ErrorApiResponse>(`/api/organizations/${ request.organizationId }/departments`, { body: JSON.stringify(request) });
  },
  update(request: UpdateDepartmentRequest): Promise<Response<string, ErrorApiResponse>> {
    return Fetch.Put<string, ErrorApiResponse>(`/api/organizations/${ request.organizationId }/departments/${ request.id }`, { body: JSON.stringify(request) });
  },
  delete(id: number, organizationId: number): Promise<Response<string, ErrorApiResponse>> {
    return Fetch.Delete<string, ErrorApiResponse>(`/api/organizations/${ organizationId }/departments/${ id }`);
  },
};

export interface CreateDepartmentRequest {
  name: string;
  organizationId: number;
}

export interface UpdateDepartmentRequest extends CreateDepartmentRequest {
  id: number;
}

export interface Department {
  id: number;
  name: string;
  organizationId: number;
  editorId: number;
  editor: string;
  editedDate: Date;
}
