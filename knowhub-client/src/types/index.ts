export type ApiResponse<T> = { success: boolean; data: T; message?: string };
export type Project = {
  id: string;
  name: string;
  description?: string;
  createdAt: string;
  updatedAt: string;
  noteCount: number;
  taskCount: number;
};
export type Note = {
  id: string;
  projectId: string;
  title: string;
  content: string;
  createdAt: string;
  updatedAt: string;
};
export type TaskItem = {
  id: string;
  projectId: string;
  title: string;
  description?: string;
  status: "Pending" | "InProgress" | "Completed";
  dueDate?: string;
  createdAt: string;
  updatedAt: string;
};
export type Document = {
  id: string;
  projectId: string;
  fileName: string;
  fileType: string;
  fileSize: number;
  uploadedAt: string;
};
export type Activity = {
  id: string;
  action: string;
  description: string;
  createdAt: string;
};
