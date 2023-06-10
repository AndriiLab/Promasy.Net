import {RoleEnum} from "@/constants/RoleEnum";
import {ErrorApiResponse, Fetch, PagedResponse, Response, SelectItem} from "@/utils/fetch-utils";
import {buildQueryParameters} from "@/utils/url-params-utils";

export default {
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
  getSuggestionsList(
    page: number,
    offset: number,
    search?: string,
    order?: string,
    descending?: boolean,
    catNum?: string,
    excludeId?: number,
  ): Promise<Response<PagedResponse<OrderSuggestion>, ErrorApiResponse>> {
    return Fetch.Get<PagedResponse<OrderSuggestion>, ErrorApiResponse>(
      `/api/orders/suggestions${ buildQueryParameters([
        [ "offset", offset.toString() ],
        [ "page", page.toString() ],
        [ "exclude", excludeId?.toString() ],
        [ "search", search ],
        [ "cat", catNum ],
        [ "order", order ],
        [ "desc", descending ? "true" : undefined ],
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
  exportAsPdf(request: ExportToPdfRequest): Promise<Response<ExportResponse, ErrorApiResponse>> {
    return Fetch.Post<ExportResponse, ErrorApiResponse>("/api/orders/export/pdf", { body: JSON.stringify(request) });
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

export interface OrderSuggestion {
  id: number;
  description: string;
  catNum?: string;
  onePrice: number;
  type: number;
  kekv?: string;
  unit: Unit;
  cpv: Cpv;
  manufacturer?: Manufacturer;
  supplier?: Supplier;
  reason?: ReasonForSupplierChoice;
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

export interface ExportToPdfRequest{
  orderIds: number[],
  signEmployees: Object<RoleEnum>
}

export interface ExportResponse {
  fileKey: string;
}