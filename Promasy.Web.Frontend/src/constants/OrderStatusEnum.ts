import {getLocalizedName} from "@/utils/enum-utils";
import {SelectItem} from "@/utils/fetch-utils";
import {useI18n} from "vue-i18n";

export enum OrderStatus
{
    Created = 1,
    Submitted = 2,
    PostedToAuction = 3,
    Received = 4,
    NotReceived = 10,
    Declined = 20
}

export function getOrderStatusesAsSelectItems() {
    const {t} = useI18n();
    return Object.values(OrderStatus).filter(en => typeof en !== "string").map(en => {
        return {value: en, text: getOrderTypeName(en as OrderStatus, t)} as SelectItem<OrderStatus>
    })
}

export function getOrderTypeName(ot: OrderStatus, localizer: any = null) {
    return getLocalizedName(ot, 'orderStatuses', localizer);
}