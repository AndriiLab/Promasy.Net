import { LanguageSettings } from "./LanguageSettings";

const datetimeFormat = {
  short: {
    year: "numeric",
    month: "short",
    day: "numeric",
    hour12: false,
  },
  long: {
    year: "numeric",
    month: "short",
    day: "numeric",
    weekday: "short",
    hour: "numeric",
    minute: "numeric",
    hour12: false,
  },
};

const numberFormat = {
  currency: {
    style: "currency",
    currency: "UAH",
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

export const uk: LanguageSettings = {
  name: "Українська",
  key: "uk",
  datetimeFormat: datetimeFormat,
  numberFormat: numberFormat,
};
