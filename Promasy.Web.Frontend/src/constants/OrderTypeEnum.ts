import {getLocalizedName} from "@/utils/enum-utils";
import {SelectItem} from "@/utils/fetch-utils";
import {useI18n} from "vue-i18n";

export enum OrderTypeEnum {
    Material = 1,
    Equipment = 2,
    Service = 3
}

export function getOrderTypesAsSelectItems() {
    const {t} = useI18n();
    return Object.values(OrderTypeEnum).filter(en => typeof en !== "string").map(en => {
        return {value: en, text: getOrderTypeName(en as OrderTypeEnum, t)} as SelectItem<OrderTypeEnum>
    })
}

export function getOrderTypeName(ot: OrderTypeEnum, localizer: any = null) {
    return getLocalizedName(ot, 'orderTypes', localizer);
}