import api from "./api";
import type { ApiResponse, Note, TaskItem, Document, Activity } from "../types";
const read = <T>(p: Promise<{ data: ApiResponse<T> }>) =>
  p.then((r) => r.data.data);
export const notes = {
  forProject: (id: string) => read<Note[]>(api.get(`/projects/${id}/notes`)),
  search: (q: string) =>
    read<Note[]>(api.get(`/notes/search?q=${encodeURIComponent(q)}`)),
  create: (id: string, d: { title: string; content: string }) =>
    read<Note>(api.post(`/projects/${id}/notes`, d)),
  update: (id: string, d: { title: string; content: string }) =>
    read<Note>(api.put(`/notes/${id}`, d)),
  remove: (id: string) => api.delete(`/notes/${id}`),
};
export const tasks = {
  forProject: (id: string) =>
    read<TaskItem[]>(api.get(`/projects/${id}/tasks`)),
  create: (id: string, d: Partial<TaskItem>) =>
    read<TaskItem>(api.post(`/projects/${id}/tasks`, d)),
  update: (id: string, d: Partial<TaskItem>) =>
    read<TaskItem>(api.put(`/tasks/${id}`, d)),
  remove: (id: string) => api.delete(`/tasks/${id}`),
};
export const documents = {
  forProject: (id: string) =>
    read<Document[]>(api.get(`/projects/${id}/documents`)),
  upload: (id: string, file: File) => {
    const f = new FormData();
    f.append("file", file);
    return read<Document>(api.post(`/projects/${id}/documents`, f));
  },
  remove: (id: string) => api.delete(`/documents/${id}`),
};
export const activities = {
  forProject: (id: string) =>
    read<Activity[]>(api.get(`/projects/${id}/activities`)),
};
