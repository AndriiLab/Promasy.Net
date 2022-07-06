export function capitalize(s: string | undefined) {
  return s && s[0].toUpperCase() + s.slice(1);
}

export function truncate(s: string | undefined, size: number) {
  if (!s) {
    return '';
  }
  if(s.length <= size) {
    return s;
  }
  
  return `${s.slice(0, size - 3)}...`;
}