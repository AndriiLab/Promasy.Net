import { LanguageSettings } from "./LanguageSettings";

const datetimeFormat = {
  short: {
    year: "numeric",
    month: "short",
    day: "numeric",
    hour12: true,
  },
  long: {
    year: "numeric",
    month: "short",
    day: "numeric",
    weekday: "short",
    hour: "numeric",
    minute: "numeric",
    hour12: true,
  },
};

const numberFormat = {
  currency: {
    style: "currency",
    currency: "USD",
    notation: "standard",
    currencyDisplay: "symbol",
  },
  decimal: {
    style: "decimal",
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  },
  percent: {
    style: "percent",
    useGrouping: false,
  },
};

export const en: LanguageSettings = {
  name: "English",
  key: "en",
  datetimeFormat: datetimeFormat,
  numberFormat: numberFormat,
};
