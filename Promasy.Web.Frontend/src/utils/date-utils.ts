import { zeroPad } from "@/utils/string-utils";

export function formatAsDate(date: Date) {
  return `${ date.getFullYear() }-${ zeroPad(date.getMonth() + 1, 2) }-${ zeroPad(date.getDate(), 2) }`;
}