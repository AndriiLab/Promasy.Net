import { ErrorApiResponse, Fetch, Response, SelectItem } from "@/utils/fetch-utils";

export default {
  getById(id: number): Promise<Response<Organization, ErrorApiResponse>> {
    return Fetch.Get<Organization, ErrorApiResponse>(`/api/organizations/${ id }`);
  },
  getCityTypes(): Promise<Response<SelectItem<number>[], ErrorApiResponse>> {
    return Fetch.Get<SelectItem<number>[], ErrorApiResponse>("/api/organizations/city-types");
  },
  getStreetTypes(): Promise<Response<SelectItem<number>[], ErrorApiResponse>> {
    return Fetch.Get<SelectItem<number>[], ErrorApiResponse>("/api/organizations/street-types");
  },
  update(request: UpdateOrganizationRequest): Promise<Response<string, ErrorApiResponse>> {
    return Fetch.Put<string, ErrorApiResponse>(`/api/organizations/${ request.id }`, { body: JSON.stringify(request) });
  }
};

export interface Organization {
  id: number;
  name: string;
  editorId: number;
  editor: string;
  editedDate: Date;
  email: string;
  edrpou: string;
  phoneNumber: string;
  faxNumber: string | undefined;
  country: string;
  postalCode: string;
  region: string;
  city: string;
  cityType: number;
  street: string;
  streetType: number;
  buildingNumber: string;
  internalNumber: string | undefined;
}

export interface UpdateOrganizationRequest {
  id: number;
  name: string;
  email: string;
  edrpou: string;
  phoneNumber: string;
  faxNumber?: string;
  country: string;
  postalCode: string;
  region: string;
  city: string;
  cityType: number;
  street: string;
  streetType: number;
  buildingNumber: string;
  internalNumber?: string;
}