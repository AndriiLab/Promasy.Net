import { ErrorApiResponse, Fetch, Response } from "@/utils/fetch-utils";
import { buildQueryParameters } from "@/utils/url-params-utils";

export default {
  getList(
    parentId?: number,
    searchText?: string,
  ): Promise<Response<Cpv[], ErrorApiResponse>> {
    return Fetch.Get<Cpv[], ErrorApiResponse>(
      `/api/cpv${ buildQueryParameters([
        [ "parentId", parentId?.toString() ],
        [ "term", searchText ],
      ]) }`,
    );
  },
};

export interface Cpv {
  id: number;
  code: string;
  descriptionEnglish: string;
  descriptionUkrainian: string;
  level: number;
  isTerminal: boolean;
  parentId?: number;
}
