import { ErrorApiResponse } from "@/utils/fetch-utils";

export default function processError(error: ErrorApiResponse | undefined, callback: (errs: Object<string[]>) => void) {
  if (!error) {
    return;
  }
  let errs = error.errors;
  if (!errs) {
    errs = {} as Object<string[]>;
    errs[""] = [ error.detail ];
  }
  callback(errs);
}