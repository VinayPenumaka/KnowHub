import { NavLink, Route, Routes } from "react-router-dom";
import Dashboard from "./pages/Dashboard";
import Projects from "./pages/Projects";
import ProjectDetails from "./pages/ProjectDetails";
import Login from "./pages/Login";
export default function App() {
  return (
    <div className="shell">
      <aside>
        <h1>
          Know<span>Hub</span>
        </h1>
        <p className="tag">Your learning workspace</p>
        <nav>
          <NavLink to="/">Dashboard</NavLink>
          <NavLink to="/projects">Projects</NavLink>
        </nav>
        <div className="future">
          <b>AI-ready foundation</b>
          <br />
          AI is intentionally off in V1.
        </div>
      </aside>
      <main>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/" element={<Dashboard />} />
          <Route path="/projects" element={<Projects />} />
          <Route path="/projects/:id" element={<ProjectDetails />} />
        </Routes>
      </main>
    </div>
  );
}
