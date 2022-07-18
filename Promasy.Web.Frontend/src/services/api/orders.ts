import { ErrorApiResponse, Fetch, PagedResponse, Response, SelectItem } from "@/utils/fetch-utils";
import { buildQueryParameters } from "@/utils/url-params-utils";

export default {
  getExistingOrderTypes(): Promise<Response<SelectItem<number>[], ErrorApiResponse>> {
    return Fetch.Get<SelectItem<number>[], ErrorApiResponse>("/api/orders/all-types");
  },
  getExistingOrderStatuses(): Promise<Response<SelectItem<number>[], ErrorApiResponse>> {
    return Fetch.Get<SelectItem<number>[], ErrorApiResponse>("/api/orders/all-statuses");
  },
  getList(
    type: number,
    year: number,
    page: number,
    offset: number,
    search?: string,
    order?: string,
    descending?: boolean,
    departmentId?: number,
    subDepartmentId?: number,
    financeSourceId?: number,
  ): Promise<Response<OrderPagedResponse, ErrorApiResponse>> {
    return Fetch.Get<OrderPagedResponse, ErrorApiResponse>(
      `/api/orders${ buildQueryParameters([
        [ "offset", offset.toString() ],
        [ "page", page.toString() ],
        [ "search", search ],
        [ "order", order ],
        [ "desc", descending ? "true" : undefined ],
        [ "type", type.toString() ],
        [ "department", subDepartmentId ? undefined : departmentId ? departmentId?.toString() : undefined ],
        [ "subDepartment", subDepartmentId ? subDepartmentId.toString() : undefined ],
        [ "finance", financeSourceId ? financeSourceId.toString() : undefined ],
        [ "year", !financeSourceId ? year.toString() : undefined ],
      ]) }`,
    );
  },
  getById(id: number): Promise<Response<Order, ErrorApiResponse>> {
    return Fetch.Get<Order, ErrorApiResponse>(`/api/orders/${ id }`);
  },
  create(request: CreateOrderRequest): Promise<Response<Order, ErrorApiResponse>> {
    return Fetch.Post<Order, ErrorApiResponse>("/api/orders", { body: JSON.stringify(request) });
  },
  update(request: UpdateOrderRequest): Promise<Response<string, ErrorApiResponse>> {
    return Fetch.Put<string, ErrorApiResponse>(`/api/orders/${ request.id }`, { body: JSON.stringify(request) });
  },
  delete(id: number): Promise<Response<string, ErrorApiResponse>> {
    return Fetch.Delete<string, ErrorApiResponse>(`/api/orders/${ id }`);
  },
};

export interface OrderPagedResponse extends PagedResponse<OrderShort> {
  spentAmount: string;
  leftAmount?: string;
}

export interface OrderShort {
  id: number;
  description: string;
  total: string;
  status: number;
  editorId: number;
  editor: string;
  editedDate: Date;
}

export interface Order {
  id: number;
  description: string;
  catNum?: string;
  onePrice: string;
  amount: string;
  type: number;
  kekv?: string;
  procurementStartDate?: Date;
  unitId: number;
  unit: string;
  cpvId: number;
  cpv: string;
  financeSubDepartmentId: number;
  financeSourceNumber: string;
  manufacturerId: number;
  manufacturer: string;
  supplierId: number;
  supplier: string;
  reasonId: number;
  reason: string;
  status: number;
  editorId: number;
  editor: string;
  editedDate: Date;
}

export interface CreateOrderRequest {
  description: string;
  catNum?: string;
  onePrice: string;
  amount: string;
  type: number;
  kekv?: string;
  procurementStartDate?: Date;
  unitId: number;
  cpvId: number;
  financeSubDepartmentId: number;
  manufacturerId: number;
  supplierId: number;
  reasonId: number;
}

export interface UpdateOrderRequest {
  id: number;
  description: string;
  catNum?: string;
  onePrice: string;
  amount: string;
  type: number;
  kekv?: string;
  procurementStartDate?: Date;
  unitId: number;
  cpvId: number;
  financeSubDepartmentId: number;
  manufacturerId: number;
  supplierId: number;
  reasonId: number;
}