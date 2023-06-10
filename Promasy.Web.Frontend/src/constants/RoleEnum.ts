import {useI18n} from "vue-i18n";
import {SelectItem} from "@/utils/fetch-utils";
import {getLocalizedName} from "@/utils/enum-utils";

export enum RoleEnum {
    Administrator = 1,
    Director = 2,
    DeputyDirector = 3,
    HeadOfTenderCommittee = 4,
    SecretaryOfTenderCommittee = 5,
    ChiefAccountant = 6,
    ChiefEconomist = 7,
    HeadOfDepartment = 8,
    PersonallyLiableEmployee = 9,
    User = 10
}

export function getRolesAsSelectItems() {
    const {t} = useI18n();
    return Object.values(RoleEnum).filter(en => typeof en !== "string").map(en => {
        return {value: en, text: getRoleName(en as RoleEnum, t)} as SelectItem<RoleEnum>
    })
}

export function getRoleName(role: RoleEnum, localizer: any = null) {
    return getLocalizedName(role, 'roles', localizer);
}