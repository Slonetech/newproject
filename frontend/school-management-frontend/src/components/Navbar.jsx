import { Link } from "react-router-dom";

export default function Navbar() {
  return (
    <nav style={{ padding: "1rem", background: "#f4f4f4" }}>
      <div style={{ fontWeight: "bold", marginRight: "2rem", display: "inline-block" }}>
        <Link to="/" style={{ textDecoration: "none", color: "black" }}>
          School Management System
        </Link>
      </div>
      <div style={{ display: "inline-block" }}>
        <Link to="/dashboard" style={{ marginRight: "1rem", textDecoration: "none", color: "black" }}>
          Dashboard
        </Link>
        <Link to="/students" style={{ textDecoration: "none", color: "black" }}>
          Students
        </Link>
      </div>
    </nav>
  );
}
