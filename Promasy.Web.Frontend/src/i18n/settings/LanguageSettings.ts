import { usePrimeVue } from "primevue/config";

type PrimeVueLocaleOptions = ReturnType<typeof usePrimeVue>['config']['locale'];
export interface LanguageSettings {
  name: string;
  key: string;
  datetimeFormat: AnyObject;
  numberFormat: AnyObject;
  primeVue: PrimeVueLocaleOptions;
}
