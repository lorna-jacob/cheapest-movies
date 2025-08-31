import { useSignalR } from "./hooks/useSignalR";
import './App.css';

export default function App() {
  const apiBase = import.meta.env.VITE_API_BASE;
  const { aggregates, loading, error } = useSignalR(apiBase);

  return (
    <div className="main-page">
      <h1>View Movies</h1>

      {error && (
        <div className="error-panel">
          <p>{error}</p>
        </div>        
      )} 
      
      {loading && (
        <div className="loading">
          <div className="spinner" />
          <span>Searching…</span>
        </div>
      )}
      
      <ul className="movie-list">
        {aggregates.map((m) => (
          <li key={m.key} className="movie-item">
            <div className="movie-title">
              {m.title} ({m.year})
            </div>
          <div className="movie-price">
            {m.cheapest ? (
              <>
                Cheapest: ${m.cheapest.price.toFixed(2)}{" "}
                <small>
                  via {m.cheapest.provider}
                  {m.cheapest.isCached ? " (Note: cached price)" : ""}
                </small>
                {(() => {
                  const expected = m.expectedProviders?.size ?? 0;
                  const actual = Object.keys(m.providers).length;
                  const pending = Math.max(0, expected - actual);
                  return pending > 0 ? (
                    <span className="inline-loading">
                      <div className="spinner small" /> waiting for other sources...
                    </span>
                  ) : null;
                })()}
              </>
            ) : (
              <span className="inline-loading">
                <div className="spinner small" />
                <em>Getting prices…</em>
              </span>
            )}
          </div>
            <div className="movie-sources">
              Sources:{" "}
              {Object.entries(m.providers).length === 0
                ? "—"
                : Object.entries(m.providers)
                    .map(([p, price]) => `${p}: $${price.toFixed(2)}`)
                    .join(" · ")}
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
}
