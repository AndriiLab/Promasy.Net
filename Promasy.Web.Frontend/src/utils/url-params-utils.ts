import { Nullable } from "primevue/ts-helpers";

export function buildQueryParameters(params: Nullable<string>[][]) {
  const filteredParams = params
    .filter((p) => p.every((s) => s))
    .map((p) => p.map((s) => s as string));
  return filteredParams.length
    ? `?${ new URLSearchParams(filteredParams).toString() }`
    : "";
}
