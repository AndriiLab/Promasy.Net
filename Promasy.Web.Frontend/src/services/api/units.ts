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
  ): Promise<Response<PagedResponse<Unit>, ErrorApiResponse>> {
    return Fetch.Get<PagedResponse<Unit>, ErrorApiResponse>(
      `/api/units${buildQueryParameters([
        ["offset", offset.toString()],
        ["page", page.toString()],
        ["search", search],
        ["order", order],
        ["desc", descending ? "true" : undefined],
      ])}`
    );
  },
  getById(id: number) : Promise<Response<Unit, ErrorApiResponse>> {
    return Fetch.Get<Unit, ErrorApiResponse>(`/api/units/${id}`);
  },
  create(request: CreateUnitRequest) : Promise<Response<Unit, ErrorApiResponse>> {
    return Fetch.Post<Unit, ErrorApiResponse>("/api/units", { body: JSON.stringify(request) });
  },
  update(request: UpdateUnitRequest) : Promise<Response<string, ErrorApiResponse>> {
    return Fetch.Put<string, ErrorApiResponse>(`/api/units/${request.id}`, { body: JSON.stringify(request) });
  },
  delete(id: number) : Promise<Response<string, ErrorApiResponse>> {
    return Fetch.Delete<string, ErrorApiResponse>(`/api/units/${id}`);
  },
  merge(request: MergeUnitsRequest) : Promise<Response<string, ErrorApiResponse>> {
    return Fetch.Post<string, ErrorApiResponse>("/api/units/merge", { body: JSON.stringify(request) });
  },
};

export interface CreateUnitRequest {
    name: string;
}

export interface UpdateUnitRequest extends CreateUnitRequest {
    id: number;
}

export interface Unit {
  id: number;
  name: string;
  editorId: number;
  editor: string;
  editedDate: Date;
}

export interface MergeUnitsRequest {
  targetId: number,
  sourceIds: number[]
}
