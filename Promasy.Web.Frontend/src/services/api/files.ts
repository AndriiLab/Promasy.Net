export default {
  buildGetFileUrl(fileKey: string){
    return `${import.meta.env.VITE_PROMASY_API_URL}/api/files/${fileKey}`;
  }
}