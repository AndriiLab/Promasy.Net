import { ErrorApiResponse, Fetch, PagedResponse, Response, SelectItem } from "@/utils/fetch-utils";
import { buildQueryParameters } from "@/utils/url-params-utils";

export default {
  getFundTypes(): Promise<Response<SelectItem<number>[], ErrorApiResponse>> {
    return Fetch.Get<SelectItem<number>[], ErrorApiResponse>("/api/finances/all-fund-types");
  },
  getList(
    financeSourceId: number,
    extended: boolean,
    page: number,
    offset: number,
    search?: string,
    order?: string,
    descending?: boolean
  ): Promise<Response<PagedResponse<FinanceSubDepartment>, ErrorApiResponse>> {
    return Fetch.Get<PagedResponse<FinanceSubDepartment>, ErrorApiResponse>(
      `/api/finances/${financeSourceId}/sub-departments${buildQueryParameters([
        ["offset", offset.toString()],
        ["extended", extended ? "true" : undefined],
        ["page", page.toString()],
        ["search", search],
        ["order", order],
        ["desc", descending ? "true" : undefined],
      ])}`
    );
  },
  get(subDepartmentId: number, financeSourceId: number) : Promise<Response<FinanceSubDepartment, ErrorApiResponse>> {
    return Fetch.Get<FinanceSubDepartment, ErrorApiResponse>(`/api/finances/${ financeSourceId }/sub-departments/${subDepartmentId}`);
  },
  create(request: CreateFinanceSubDepartmentRequest) : Promise<Response<FinanceSubDepartment, ErrorApiResponse>> {
    return Fetch.Post<FinanceSubDepartment, ErrorApiResponse>(`/api/finances/${ request.financeSourceId }/sub-departments/`, { body: JSON.stringify(request) });
  },
  update(request: UpdateFinanceSubDepartmentRequest) : Promise<Response<string, ErrorApiResponse>> {
    return Fetch.Put<string, ErrorApiResponse>(`/api/finances/${ request.financeSourceId }/sub-departments/${request.subDepartmentId}`, { body: JSON.stringify(request) });
  },
  delete(subDepartmentId: number, financeSourceId: number) : Promise<Response<string, ErrorApiResponse>> {
    return Fetch.Delete<string, ErrorApiResponse>(`/api/finances/${ financeSourceId }/sub-departments/${subDepartmentId}`);
  },
};

export interface FinanceSubDepartment {
  id: number;
  financeSourceId: number;
  subDepartmentId: number;
  subDepartment: string;
  departmentId: number;
  department: string;
  totalEquipment: string;
  leftEquipment: string;
  totalMaterials: string;
  leftMaterials: string;
  totalServices: string;
  leftServices: string;
  editorId: number;
  editor: string;
  editedDate: Date;
}

export interface CreateFinanceSubDepartmentRequest {
  financeSourceId: number;
  subDepartmentId: number;
  totalEquipment: string;
  totalMaterials: string;
  totalServices: string;
}

export interface UpdateFinanceSubDepartmentRequest {
  id: number;
  financeSourceId: number;
  subDepartmentId: number;
  totalEquipment: string;
  totalMaterials: string;
  totalServices: string;
}