import { ErrorApiResponse, Fetch, Response, SelectItem } from "@/utils/fetch-utils";

export default {
  getRolesList(): Promise<Response<SelectItem<number>[], ErrorApiResponse>> {
    return Fetch.Get<SelectItem<number>[], ErrorApiResponse>("/api/employees/roles");
  },
};