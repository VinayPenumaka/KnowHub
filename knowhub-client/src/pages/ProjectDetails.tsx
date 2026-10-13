import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { projects } from "../services/projectService";
import {
  activities,
  documents,
  notes,
  tasks,
} from "../services/contentServices";
import type { Activity, Document, Note, Project, TaskItem } from "../types";
type Tab = "Overview" | "Notes" | "Tasks" | "Documents" | "Activity";
export default function ProjectDetails() {
  const { id = "" } = useParams();
  const [project, setProject] = useState<Project>(),
    [tab, setTab] = useState<Tab>("Overview"),
    [ns, setNs] = useState<Note[]>([]),
    [ts, setTs] = useState<TaskItem[]>([]),
    [ds, setDs] = useState<Document[]>([]),
    [as, setAs] = useState<Activity[]>([]);
  const load = () =>
    Promise.all([
      projects.get(id).then(setProject),
      notes.forProject(id).then(setNs),
      tasks.forProject(id).then(setTs),
      documents.forProject(id).then(setDs),
      activities.forProject(id).then(setAs),
    ]);
  useEffect(() => {
    load().catch(console.error);
  }, [id]);
  if (!project) return <p>Loading project…</p>;
  return (
    <>
      <header>
        <div>
          <p className="eyebrow">PROJECT</p>
          <h2>{project.name}</h2>
          <p>{project.description}</p>
        </div>
      </header>
      <div className="tabs">
        {(["Overview", "Notes", "Tasks", "Documents", "Activity"] as Tab[]).map(
          (x) => (
            <button
              className={tab === x ? "active" : ""}
              onClick={() => setTab(x)}
              key={x}
            >
              {x}
            </button>
          ),
        )}
      </div>
      {tab === "Overview" && (
        <div className="stats">
          <Stat l="Notes" v={ns.length} />
          <Stat l="Tasks" v={ts.length} />
          <Stat l="Documents" v={ds.length} />
        </div>
      )}
      {tab === "Notes" && <Notes id={id} items={ns} reload={load} />}{" "}
      {tab === "Tasks" && <Tasks id={id} items={ts} reload={load} />}{" "}
      {tab === "Documents" && <Documents id={id} items={ds} reload={load} />}{" "}
      {tab === "Activity" && (
        <section className="panel">
          {as.map((a) => (
            <p className="activity" key={a.id}>
              <b>{a.action}</b> {a.description}
              <small>{new Date(a.createdAt).toLocaleString()}</small>
            </p>
          ))}
        </section>
      )}
    </>
  );
}
function Stat({ l, v }: { l: string; v: number }) {
  return (
    <div className="stat">
      <span>{l}</span>
      <strong>{v}</strong>
    </div>
  );
}
function Notes({
  id,
  items,
  reload,
}: {
  id: string;
  items: Note[];
  reload: () => Promise<unknown>;
}) {
  const [title, setTitle] = useState(""),
    [content, setContent] = useState("");
  return (
    <section className="split">
      <form
        className="panel"
        onSubmit={async (e) => {
          e.preventDefault();
          await notes.create(id, { title, content });
          setTitle("");
          setContent("");
          reload();
        }}
      >
        <h3>New note</h3>
        <label>
          Title
          <input
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            required
          />
        </label>
        <label>
          Content
          <textarea
            value={content}
            onChange={(e) => setContent(e.target.value)}
            required
          />
        </label>
        <button>Save note</button>
      </form>
      <div>
        {items.map((n) => (
          <article className="panel item" key={n.id}>
            <h3>{n.title}</h3>
            <p>{n.content}</p>
            <button
              className="danger"
              onClick={async () => {
                await notes.remove(n.id);
                reload();
              }}
            >
              Delete
            </button>
          </article>
        ))}
      </div>
    </section>
  );
}
function Tasks({
  id,
  items,
  reload,
}: {
  id: string;
  items: TaskItem[];
  reload: () => Promise<unknown>;
}) {
  const [title, setTitle] = useState("");
  return (
    <section className="split">
      <form
        className="panel"
        onSubmit={async (e) => {
          e.preventDefault();
          await tasks.create(id, { title, status: "Pending" });
          setTitle("");
          reload();
        }}
      >
        <h3>New task</h3>
        <label>
          Title
          <input
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            required
          />
        </label>
        <button>Add task</button>
      </form>
      <div>
        {items.map((t) => (
          <article className="panel item" key={t.id}>
            <h3>{t.title}</h3>
            <select
              value={t.status}
              onChange={async (e) => {
                await tasks.update(t.id, {
                  ...t,
                  status: e.target.value as TaskItem["status"],
                });
                reload();
              }}
            >
              <option>Pending</option>
              <option>InProgress</option>
              <option>Completed</option>
            </select>
            <button
              className="danger"
              onClick={async () => {
                await tasks.remove(t.id);
                reload();
              }}
            >
              Delete
            </button>
          </article>
        ))}
      </div>
    </section>
  );
}
function Documents({
  id,
  items,
  reload,
}: {
  id: string;
  items: Document[];
  reload: () => Promise<unknown>;
}) {
  return (
    <section className="split">
      <form
        className="panel"
        onSubmit={async (e) => {
          e.preventDefault();
          const file = (
            e.currentTarget.elements.namedItem("file") as HTMLInputElement
          ).files?.[0];
          if (file) {
            await documents.upload(id, file);
            e.currentTarget.reset();
            reload();
          }
        }}
      >
        <h3>Upload document</h3>
        <input name="file" type="file" accept=".pdf,.txt,.docx" required />
        <button>Upload</button>
        <p>
          <small>PDF, TXT, or DOCX only.</small>
        </p>
      </form>
      <div>
        {items.map((d) => (
          <article className="panel item" key={d.id}>
            <h3>{d.fileName}</h3>
            <p>{(d.fileSize / 1024).toFixed(1)} KB</p>
            <button
              className="danger"
              onClick={async () => {
                await documents.remove(d.id);
                reload();
              }}
            >
              Delete
            </button>
          </article>
        ))}
      </div>
    </section>
  );
}
