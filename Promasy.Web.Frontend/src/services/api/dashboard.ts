import {ErrorApiResponse, Fetch, Response} from "@/utils/fetch-utils";
import {buildQueryParameters} from "@/utils/url-params-utils";
import {OrderTypeEnum} from "@/constants/OrderTypeEnum";

export default {
    getOrdersCount(
        year: number
    ): Promise<Response<DashboardCount, ErrorApiResponse>> {
        return Fetch.Get<DashboardCount, ErrorApiResponse>(
            `/api/dashboard/orders-count${buildQueryParameters([
                ["year", year.toString()]
            ])}`,
        );
    },
    getFundingLeft(
        year: number,
        type: OrderTypeEnum
    ): Promise<Response<DashboardCount, ErrorApiResponse>> {
        return Fetch.Get<DashboardCount, ErrorApiResponse>(
            `/api/dashboard/funding-left${ buildQueryParameters([
                [ "type", type.toString() ],
                [ "year", year.toString() ]
            ]) }`,
        );
    },
};

export interface DashboardCount{
    countTotal: number;
    countByPeriod: number;
}