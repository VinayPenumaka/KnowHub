import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { projects } from "../services/projectService";
import type { Project } from "../types";
export default function Dashboard() {
  const [items, setItems] = useState<Project[]>([]);
  useEffect(() => {
    projects.list().then(setItems).catch(console.error);
  }, []);
  const notes = items.reduce((n, p) => n + p.noteCount, 0),
    tasks = items.reduce((n, p) => n + p.taskCount, 0);
  return (
    <>
      <header>
        <div>
          <p className="eyebrow">OVERVIEW</p>
          <h2>Good to see you, Vinay.</h2>
          <p>Keep ideas, learning, and work in one calm place.</p>
        </div>
        <Link className="button" to="/projects">
          Manage projects
        </Link>
      </header>
      <div className="stats">
        <Stat label="Total projects" value={items.length} />
        <Stat label="Total notes" value={notes} />
        <Stat label="Open tasks" value={tasks} />
        <Stat label="AI features" value="Later" />
      </div>
      <section>
        <h3>Recent projects</h3>
        <div className="cards">
          {items.slice(0, 3).map((p) => (
            <Link className="card" to={`/projects/${p.id}`} key={p.id}>
              <h3>{p.name}</h3>
              <p>{p.description || "No description yet."}</p>
              <small>
                {p.noteCount} notes · {p.taskCount} tasks
              </small>
            </Link>
          ))}
        </div>
      </section>
    </>
  );
}
function Stat({ label, value }: { label: string; value: string | number }) {
  return (
    <div className="stat">
      <span>{label}</span>
      <strong>{value}</strong>
    </div>
  );
}
