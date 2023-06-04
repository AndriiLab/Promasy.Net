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
    departmentId: number,
    page: number,
    offset: number,
    search?: string,
    order?: string,
    descending?: boolean,
  ): Promise<Response<PagedResponse<SubDepartment>, ErrorApiResponse>> {
    return Fetch.Get<PagedResponse<SubDepartment>, ErrorApiResponse>(
      `/api/organizations/${ organizationId }/departments/${ departmentId }/sub-departments${ buildQueryParameters([
        [ "offset", offset.toString() ],
        [ "page", page.toString() ],
        [ "search", search ],
        [ "order", order ],
        [ "desc", descending ? "true" : undefined ],
      ]) }`,
    );
  },
  getById(id: number, organizationId: number, departmentId: number): Promise<Response<SubDepartment, ErrorApiResponse>> {
    return Fetch.Get<SubDepartment, ErrorApiResponse>(`/api/organizations/${ organizationId }/departments/${ departmentId }/sub-departments/${ id }`);
  },
  create(request: CreateSubDepartmentRequest): Promise<Response<SubDepartment, ErrorApiResponse>> {
    return Fetch.Post<SubDepartment, ErrorApiResponse>(`/api/organizations/${ request.organizationId }/departments/${ request.departmentId }/sub-departments`, { body: JSON.stringify(request) });
  },
  update(request: UpdateSubDepartmentRequest): Promise<Response<string, ErrorApiResponse>> {
    return Fetch.Put<string, ErrorApiResponse>(`/api/organizations/${ request.organizationId }/departments/${ request.departmentId }/sub-departments/${ request.id }`, { body: JSON.stringify(request) });
  },
  delete(id: number, organizationId: number, departmentId: number): Promise<Response<string, ErrorApiResponse>> {
    return Fetch.Delete<string, ErrorApiResponse>(`/api/organizations/${ organizationId }/departments/${ departmentId }/sub-departments/${ id }`);
  },
};

export interface CreateSubDepartmentRequest {
  name: string;
  organizationId: number;
  departmentId: number;
}

export interface UpdateSubDepartmentRequest extends CreateSubDepartmentRequest {
  id: number;
}

export interface SubDepartment {
  id: number;
  name: string;
  departmentId: number;
  editorId: number;
  editor: string;
  editedDate: Date;
}

export const EmptySubDepartmentName = "<відсутній>";
