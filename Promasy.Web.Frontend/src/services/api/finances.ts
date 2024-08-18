import { ErrorApiResponse, Fetch, PagedResponse, Response, SelectItem } from "@/utils/fetch-utils";
import { buildQueryParameters } from "@/utils/url-params-utils";

export default {
  getFundTypes(): Promise<Response<SelectItem<number>[], ErrorApiResponse>> {
    return Fetch.Get<SelectItem<number>[], ErrorApiResponse>("/api/finances/all-fund-types");
  },
  getList(
    year: number,
    extended: boolean,
    page: number,
    offset: number,
    search?: string,
    order?: string,
    descending?: boolean
  ): Promise<Response<PagedResponse<FinanceSource>, ErrorApiResponse>> {
    return Fetch.Get<PagedResponse<FinanceSource>, ErrorApiResponse>(
      `/api/finances${buildQueryParameters([
        ["year", year.toString()],
        ["extended", extended ? "true" : undefined],
        ["offset", offset.toString()],
        ["page", page.toString()],
        ["search", search],
        ["order", order],
        ["desc", descending ? "true" : undefined],
        ["desc", descending ? "true" : undefined],
      ])}`
    );
  },
  getById(id: number) : Promise<Response<FinanceSource, ErrorApiResponse>> {
    return Fetch.Get<FinanceSource, ErrorApiResponse>(`/api/finances/${id}`);
  },
  create(request: CreateFinanceSourceRequest) : Promise<Response<FinanceSource, ErrorApiResponse>> {
    return Fetch.Post<FinanceSource, ErrorApiResponse>("/api/finances", { body: JSON.stringify(request) });
  },
  update(request: UpdateFinanceSourceRequest) : Promise<Response<string, ErrorApiResponse>> {
    return Fetch.Put<string, ErrorApiResponse>(`/api/finances/${request.id}`, { body: JSON.stringify(request) });
  },
  delete(id: number) : Promise<Response<string, ErrorApiResponse>> {
    return Fetch.Delete<string, ErrorApiResponse>(`/api/finances/${id}`);
  },
};

export interface FinanceSource {
  id: number;
  number: string;
  name: string;
  fundType: number;
  start: Date;
  end: Date;
  kpkvk: string;
  totalEquipment: number;
  leftEquipment: number;
  unassignedEquipment: number;
  totalMaterials: number;
  leftMaterials: number;
  unassignedMaterials: number;
  totalServices: number;
  leftServices: number;
  unassignedServices: number;
  editorId: number;
  editor: string;
  editedDate: Date;
}

export interface CreateFinanceSourceRequest {
  number: string;
  name: string;
  fundType: number;
  start: string;
  end: string;
  kpkvk: string;
  totalEquipment: number;
  totalMaterials: number;
  totalServices: number;
}

export interface UpdateFinanceSourceRequest {
  id: number;
  number: string;
  name: string;
  fundType: number;
  start: string;
  end: string;
  kpkvk: string;
  totalEquipment: number;
  totalMaterials: number;
  totalServices: number;
}