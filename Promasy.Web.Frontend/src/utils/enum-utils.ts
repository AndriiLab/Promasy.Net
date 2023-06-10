import {useI18n} from "vue-i18n";

export function getLocalizedName(en: any, enumName: string, localizer: any = null) {
    if (!localizer) {
        const {t} = useI18n();
        localizer = t;
    }
    return localizer(`enums.${enumName}.${en}`);
}