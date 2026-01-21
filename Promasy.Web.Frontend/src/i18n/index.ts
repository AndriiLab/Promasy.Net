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

export function getCultureName(key: string){
  switch (key) {
    case uk.key:
      return uk.cultureName;
    case en.key:
      return en.cultureName;
    default:
      return en.cultureName;
  }
}
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
  primeVue.config.locale = {
    ...primeVue.config.locale,
    ...pvl
  };
}


