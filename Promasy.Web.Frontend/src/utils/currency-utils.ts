import Currency from "currency.js";

export default function currency(v: string | number) {
  return Currency(v, { symbol: "₴ ", separator: " ", decimal: "," });
};