export const keys = {
  allowStore: "allow",
  language: "lang",
  token: "token",
};

export default {
  allow() {
    localStorage.setItem(keys.allowStore, "true");
  },
  disable() {
    localStorage.clear();
  },
  get(key: string): string | null {
    if (!localStorage.getItem(keys.allowStore)) {
      return null;
    }
    return localStorage.getItem(key);
  },
  set(key: string, value: string) {
    if (localStorage.getItem(keys.allowStore)) {
      localStorage.setItem(key, value);
    }
  },
  remove(key: string) {
    localStorage.removeItem(key);
  },
};
