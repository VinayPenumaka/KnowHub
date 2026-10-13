import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { projects } from "../services/projectService";
import type { Project } from "../types";
const blank = { name: "", description: "" };
export default function Projects() {
  const [items, setItems] = useState<Project[]>([]),
    [form, setForm] = useState(blank),
    [editing, setEditing] = useState<Project | null>(null);
  const load = () => projects.list().then(setItems);
  useEffect(() => {
    load();
  }, []);
  const save = async (e: React.FormEvent) => {
    e.preventDefault();
    editing
      ? await projects.update(editing.id, form)
      : await projects.create(form);
    setForm(blank);
    setEditing(null);
    load();
  };
  return (
    <>
      <header>
        <div>
          <p className="eyebrow">PROJECTS</p>
          <h2>Organize what you’re learning.</h2>
        </div>
      </header>
      <div className="two-col">
        <form className="panel" onSubmit={save}>
          <h3>{editing ? "Edit project" : "New project"}</h3>
          <label>
            Name
            <input
              value={form.name}
              maxLength={100}
              onChange={(e) => setForm({ ...form, name: e.target.value })}
              required
            />
          </label>
          <label>
            Description
            <textarea
              value={form.description}
              maxLength={500}
              onChange={(e) =>
                setForm({ ...form, description: e.target.value })
              }
            />
          </label>
          <button>{editing ? "Save changes" : "Create project"}</button>
          {editing && (
            <button
              className="link"
              type="button"
              onClick={() => {
                setEditing(null);
                setForm(blank);
              }}
            >
              Cancel
            </button>
          )}
        </form>
        <div className="cards">
          {items.map((p) => (
            <article className="card" key={p.id}>
              <h3>{p.name}</h3>
              <p>{p.description}</p>
              <small>
                {p.noteCount} notes · {p.taskCount} tasks
              </small>
              <footer>
                <Link to={`/projects/${p.id}`}>Open</Link>
                <button
                  className="link"
                  onClick={() => {
                    setEditing(p);
                    setForm({ name: p.name, description: p.description || "" });
                  }}
                >
                  Edit
                </button>
                <button
                  className="danger"
                  onClick={async () => {
                    if (confirm(`Delete ${p.name}?`)) {
                      await projects.remove(p.id);
                      load();
                    }
                  }}
                >
                  Delete
                </button>
              </footer>
            </article>
          ))}
        </div>
      </div>
    </>
  );
}
