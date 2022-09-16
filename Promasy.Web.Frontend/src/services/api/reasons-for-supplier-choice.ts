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
    descending?: boolean
  ): Promise<Response<PagedResponse<ReasonForSupplierChoice>, ErrorApiResponse>> {
    return Fetch.Get<PagedResponse<ReasonForSupplierChoice>, ErrorApiResponse>(
      `/api/orders/reasons-for-supplier-choice${buildQueryParameters([
        ["offset", offset.toString()],
        ["page", page.toString()],
        ["search", search],
        ["order", order],
        ["desc", descending ? "true" : undefined],
      ])}`
    );
  },
  getById(id: number) : Promise<Response<ReasonForSupplierChoice, ErrorApiResponse>> {
    return Fetch.Get<ReasonForSupplierChoice, ErrorApiResponse>(`/api/orders/reasons-for-supplier-choice/${id}`);
  },
  create(request: CreateReasonForSupplierChoiceRequest) : Promise<Response<ReasonForSupplierChoice, ErrorApiResponse>> {
    return Fetch.Post<ReasonForSupplierChoice, ErrorApiResponse>("/api/orders/reasons-for-supplier-choice", { body: JSON.stringify(request) });
  },
  update(request: UpdateReasonForSupplierChoiceRequest) : Promise<Response<string, ErrorApiResponse>> {
    return Fetch.Put<string, ErrorApiResponse>(`/api/orders/reasons-for-supplier-choice/${request.id}`, { body: JSON.stringify(request) });
  },
  delete(id: number) : Promise<Response<string, ErrorApiResponse>> {
    return Fetch.Delete<string, ErrorApiResponse>(`/api/orders/reasons-for-supplier-choice/${id}`);
  },
  merge(request: MergeReasonForSupplierChoiceRequest) : Promise<Response<string, ErrorApiResponse>> {
    return Fetch.Post<string, ErrorApiResponse>("/api/orders/reasons-for-supplier-choice/merge", { body: JSON.stringify(request) });
  },
};

export interface CreateReasonForSupplierChoiceRequest {
    name: string;
}

export interface UpdateReasonForSupplierChoiceRequest extends CreateReasonForSupplierChoiceRequest {
    id: number;
}

export interface ReasonForSupplierChoice {
  id: number;
  name: string;
  editorId: number;
  editor: string;
  editedDate: Date;
}

export interface MergeReasonForSupplierChoiceRequest {
  targetId: number,
  sourceIds: number[]
}
