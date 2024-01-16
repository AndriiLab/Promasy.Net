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
  ): Promise<Response<PagedResponse<Manufacturer>, ErrorApiResponse>> {
    return Fetch.Get<PagedResponse<Manufacturer>, ErrorApiResponse>(
      `/api/manufacturers${buildQueryParameters([
        ["offset", offset.toString()],
        ["page", page.toString()],
        ["search", search],
        ["order", order],
        ["desc", descending ? "true" : undefined],
      ])}`
    );
  },
  getById(id: number) : Promise<Response<Manufacturer, ErrorApiResponse>> {
    return Fetch.Get<Manufacturer, ErrorApiResponse>(`/api/manufacturers/${id}`);
  },
  create(request: CreateManufacturerRequest) : Promise<Response<Manufacturer, ErrorApiResponse>> {
    return Fetch.Post<Manufacturer, ErrorApiResponse>("/api/manufacturers", { body: JSON.stringify(request) });
  },
  update(request: UpdateManufacturerRequest) : Promise<Response<string, ErrorApiResponse>> {
    return Fetch.Put<string, ErrorApiResponse>(`/api/manufacturers/${request.id}`, { body: JSON.stringify(request) });
  },
  delete(id: number) : Promise<Response<string, ErrorApiResponse>> {
    return Fetch.Delete<string, ErrorApiResponse>(`/api/manufacturers/${id}`);
  },  
  merge(request: MergeManufacturersRequest) : Promise<Response<string, ErrorApiResponse>> {
    return Fetch.Post<string, ErrorApiResponse>("/api/manufacturers/merge", { body: JSON.stringify(request) });
  },
};

export interface CreateManufacturerRequest {
    name: string;
}

export interface UpdateManufacturerRequest extends CreateManufacturerRequest {
    id: number;
}

export interface Manufacturer {
  id: number;
  name: string;
  editorId: number;
  editor: string;
  editedDate: Date;
}

export interface MergeManufacturersRequest {
  targetId: number,
  sourceIds: number[]
}
