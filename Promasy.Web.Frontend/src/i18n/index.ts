import { createI18n, I18nOptions } from "vue-i18n";
import { usePrimeVue } from "primevue/config";
import { LanguageSettings } from "./settings/LanguageSettings";
import { en } from "./settings/en";
import { uk } from "./settings/uk";
import messages from "@intlify/unplugin-vue-i18n/messages";

const langs = [ en, uk ];
const options: I18nOptions = {
  legacy: false,
  globalInjection: true,
  fallbackRoot: true,
  locale: en.key,
  fallbackLocale: uk.key,
  messages: messages,
  datetimeFormats: getLocalesObject(langs, (l) => l.datetimeFormat),
  numberFormats: getLocalesObject(langs, (l) => l.numberFormat),
};

function getLocalesObject(
  langs: LanguageSettings[],
  fn: (s: LanguageSettings) => any,
) {
  const obj = {} as AnyObject;
  for (const lang of langs) {
    obj[lang.key] = fn(lang);
  }
  return obj;
}

export const i18n = createI18n(options);

export const availableLanguages: SelectObject<string>[] = [
  {
    name: en.name,
    value: en.key,
  },
  {
    name: uk.name,
    value: uk.key,
  },
];

type PrimeVue = ReturnType<typeof usePrimeVue>;
export function setPrimeVueLocale(primeVue: PrimeVue, locale: string) {
  let pvl;
  switch (locale) {
    case uk.key:
      pvl = uk.primeVue;
      break;
    default:
      pvl = en.primeVue;
      break;
  }
  primeVue.config.locale!.startsWith = pvl!.startsWith;
  primeVue.config.locale!.contains = pvl!.contains;
  primeVue.config.locale!.notContains = pvl!.notContains;
  primeVue.config.locale!.endsWith = pvl!.endsWith;
  primeVue.config.locale!.equals = pvl!.equals;
  primeVue.config.locale!.notEquals = pvl!.notEquals;
  primeVue.config.locale!.noFilter = pvl!.noFilter;
  primeVue.config.locale!.lt = pvl!.lt;
  primeVue.config.locale!.lte = pvl!.lte;
  primeVue.config.locale!.gt = pvl!.gt;
  primeVue.config.locale!.gte = pvl!.gte;
  primeVue.config.locale!.dateIs = pvl!.dateIs;
  primeVue.config.locale!.dateIsNot = pvl!.dateIsNot;
  primeVue.config.locale!.dateBefore = pvl!.dateBefore;
  primeVue.config.locale!.dateAfter = pvl!.dateAfter;
  primeVue.config.locale!.clear = pvl!.clear;
  primeVue.config.locale!.apply = pvl!.apply;
  primeVue.config.locale!.matchAll = pvl!.matchAll;
  primeVue.config.locale!.matchAny = pvl!.matchAny;
  primeVue.config.locale!.addRule = pvl!.addRule;
  primeVue.config.locale!.removeRule = pvl!.removeRule;
  primeVue.config.locale!.accept = pvl!.accept;
  primeVue.config.locale!.reject = pvl!.reject;
  primeVue.config.locale!.choose = pvl!.choose;
  primeVue.config.locale!.upload = pvl!.upload;
  primeVue.config.locale!.cancel = pvl!.cancel;
  primeVue.config.locale!.dayNames = pvl!.dayNames;
  primeVue.config.locale!.dayNamesShort = pvl!.dayNamesShort;
  primeVue.config.locale!.dayNamesMin = pvl!.dayNamesMin;
  primeVue.config.locale!.monthNames = pvl!.monthNames;
  primeVue.config.locale!.monthNamesShort = pvl!.monthNamesShort;
  primeVue.config.locale!.today = pvl!.today;
  primeVue.config.locale!.weekHeader = pvl!.weekHeader;
  primeVue.config.locale!.firstDayOfWeek = pvl!.firstDayOfWeek;
  primeVue.config.locale!.dateFormat = pvl!.dateFormat;
  primeVue.config.locale!.weak = pvl!.weak;
  primeVue.config.locale!.medium = pvl!.medium;
  primeVue.config.locale!.strong = pvl!.strong;
  primeVue.config.locale!.passwordPrompt = pvl!.passwordPrompt;
  primeVue.config.locale!.emptyFilterMessage = pvl!.emptyFilterMessage;
  primeVue.config.locale!.emptyMessage = pvl!.emptyMessage;
}


