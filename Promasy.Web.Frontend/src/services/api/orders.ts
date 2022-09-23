﻿import { ErrorApiResponse, Fetch, PagedResponse, Response, SelectItem } from "@/utils/fetch-utils";
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
  spentAmount: number;
  leftAmount?: number;
}

export interface OrderShort {
  id: number;
  description: string;
  total: number;
  status: number;
  financeId: number;
  subDepartmentId: number;
  editorId: number;
  editor: string;
  editedDate: Date;
}

export interface Order {
  id: number;
  description: string;
  catNum?: string;
  onePrice: number;
  amount: number;
  type: number;
  kekv?: string;
  procurementStartDate?: Date;
  unit: Unit;
  cpv: Cpv;
  financeSubDepartment: FinanceSubDepartment;
  subDepartment: SubDepartment;
  department: Department;
  manufacturer?: Manufacturer;
  supplier?: Supplier;
  reason?: ReasonForSupplierChoice;
  editorId: number;
  editor: string;
  editedDate: string;
}

export interface Unit {
  id: number;
  name: string;
}

export interface Cpv {
  id: number;
  code: string;
  descriptionEnglish: string;
  descriptionUkrainian: string;
  level: number;
  isTerminal: boolean;
  parentId: number | null;
}

export interface FinanceSubDepartment {
  id: number;
  financeSourceId: number;
  financeSource: string;
  financeSourceNumber: string;
}

export interface Department {
  id: number;
  name: string;
}

export interface SubDepartment {
  id: number;
  name: string;
}

export interface Manufacturer {
  id: number;
  name: string;
}

export interface Supplier {
  id: number;
  name: string;
}

export interface ReasonForSupplierChoice {
  id: number;
  name: string;
}

export interface CreateOrderRequest {
  description: string;
  catNum?: string;
  onePrice: number;
  amount: number;
  type: number;
  kekv?: string;
  procurementStartDate?: string;
  unitId: number;
  cpvId: number;
  financeSubDepartmentId: number;
  manufacturerId?: number;
  supplierId?: number;
  reasonId?: number;
}

export interface UpdateOrderRequest extends CreateOrderRequest {
  id: number;
}

export enum OrderType {
  Material = 1,
  Equipment = 2,
  Service = 3
}