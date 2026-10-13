import api from "./api";
import type { ApiResponse, Project } from "../types";
export const projects = {
  list: () =>
    api.get<ApiResponse<Project[]>>("/projects").then((r) => r.data.data),
  get: (id: string) =>
    api.get<ApiResponse<Project>>(`/projects/${id}`).then((r) => r.data.data),
  create: (data: { name: string; description?: string }) =>
    api.post<ApiResponse<Project>>("/projects", data).then((r) => r.data.data),
  update: (id: string, data: { name: string; description?: string }) =>
    api
      .put<ApiResponse<Project>>(`/projects/${id}`, data)
      .then((r) => r.data.data),
  remove: (id: string) => api.delete(`/projects/${id}`),
};
