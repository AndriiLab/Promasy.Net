import { useSessionStore } from "@/store/session";

export default {
  buildGetFileUrl(fileKey: string){
    const { user } = useSessionStore();
    return `/api/files/${fileKey}?token=${user?.token}`;
  }
}