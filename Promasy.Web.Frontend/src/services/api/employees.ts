import { RoleEnum } from "@/constants/RoleEnum";
import { ErrorApiResponse, Fetch, PagedResponse, Response, SelectItem } from "@/utils/fetch-utils";
import { buildQueryParameters } from "@/utils/url-params-utils";

export default {
  getRolesList(): Promise<Response<SelectItem<number>[], ErrorApiResponse>> {
    return Fetch.Get<SelectItem<number>[], ErrorApiResponse>("/api/employees/all-roles");
  },
  getList(
    department: number,
    subDepartment: number,
    page: number,
    offset: number,
    search?: string,
    order?: string,
    descending?: boolean,
    roles?: RoleEnum[],
  ): Promise<Response<PagedResponse<EmployeeShort>, ErrorApiResponse>> {
    const rolesArray = roles ? roles.map(r => {
      return [ "roles", r.toString() ];
    }) : [ [ "roles", undefined ] ];
    return Fetch.Get<PagedResponse<EmployeeShort>, ErrorApiResponse>(
      `/api/employees${ buildQueryParameters([
        [ "offset", offset.toString() ],
        [ "page", page.toString() ],
        [ "search", search ],
        [ "order", order ],
        [ "desc", descending ? "true" : undefined ],
        [ "department", subDepartment > 0 ? undefined : department > 0 ? department.toString() : undefined ],
        [ "sub-department", subDepartment > 0 ? subDepartment.toString() : undefined ],
        ...rolesArray,
      ]) }`,
    );
  },
  getById(id: number): Promise<Response<Employee, ErrorApiResponse>> {
    return Fetch.Get<Employee, ErrorApiResponse>(`/api/employees/${ id }`);
  },
  create(request: CreateEmployeeRequest): Promise<Response<Employee, ErrorApiResponse>> {
    return Fetch.Post<Employee, ErrorApiResponse>("/api/employees", { body: JSON.stringify(request) });
  },
  changePassword(request: PasswordChangeRequest): Promise<Response<Employee, ErrorApiResponse>> {
    return Fetch.Post<Employee, ErrorApiResponse>(`/api/employees/${ request.id }/change-password`, { body: JSON.stringify(request) });
  },
  update(request: UpdateEmployeeRequest): Promise<Response<string, ErrorApiResponse>> {
    return Fetch.Put<string, ErrorApiResponse>(`/api/employees/${ request.id }`, { body: JSON.stringify(request) });
  },
  delete(id: number): Promise<Response<string, ErrorApiResponse>> {
    return Fetch.Delete<string, ErrorApiResponse>(`/api/employees/${ id }`);
  },
};

export interface EmployeeShort {
  id: number;
  name: string;
  department: string;
  subDepartment: string;
  roles: RoleEnum[];
  editorId: number;
  editor: string;
  editedDate: Date;
}

export interface Employee {
  id: number;
  firstName: string;
  middleName?: string;
  lastName?: string;
  email: string;
  primaryPhone: string;
  reservePhone?: string;
  departmentId: number;
  department: string;
  subDepartmentId: number;
  subDepartment: string;
  roles: RoleEnum[];
  editorId: number;
  editor: string;
  editedDate: Date;
}

export interface CreateEmployeeRequest {
  firstName: string;
  middleName?: string;
  lastName?: string;
  email: string;
  primaryPhone: string;
  reservePhone?: string;
  userName: string;
  password: string;
  subDepartmentId: number;
  roles: number[];
}

export interface UpdateEmployeeRequest {
  id: number;
  firstName: string;
  middleName?: string;
  lastName?: string;
  email: string;
  primaryPhone: string;
  reservePhone?: string;
  subDepartmentId: number;
  roles: number[];
}

export interface PasswordChangeRequest {
  id: number;
  password: string;
}