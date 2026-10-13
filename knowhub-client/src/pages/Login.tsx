import { useNavigate } from "react-router-dom";
export default function Login() {
  const go = useNavigate();
  return (
    <section className="center">
      <div className="panel">
        <h2>Welcome to KnowHub</h2>
        <p>
          V1 uses a local development user. Authentication is the next learning
          milestone.
        </p>
        <form
          onSubmit={(e) => {
            e.preventDefault();
            go("/");
          }}
        >
          <label>
            Email
            <input type="email" defaultValue="vinay@example.com" required />
          </label>
          <label>
            Password
            <input type="password" defaultValue="password" required />
          </label>
          <button>Enter workspace</button>
        </form>
      </div>
    </section>
  );
}
