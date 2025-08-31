import { useEffect, useMemo, useState } from "react";
import * as signalR from "@microsoft/signalr";
import type { MovieAggregate, ProviderStreamEvent } from "../types/types";

export function useSignalR(apiBase: string) {
  const [events, setEvents] = useState<ProviderStreamEvent[]>([]);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${apiBase}/hubs/movies`, {
        withCredentials: true,
      })
      .withAutomaticReconnect()
      .build();
      
    let unsub: { dispose: () => void } | null = null;
    let stopped = false;

    connection.onreconnected(() => {
    console.log("Reconnected to SignalR hub.");
    setError(null);
  });

  connection.onclose(() => {
    setError("Connection closed. Please try again later.");
  });

    (async () => {
      try {
        await connection.start();
        const stream = connection.stream<ProviderStreamEvent>("StreamMovies");

        unsub = stream.subscribe({
          next: (ev) => {
            setEvents((prev) => [...prev, ev]);
            setError(null);
          },
          error: (err) => {
            console.error("SignalR stream error:", err);
            setError("Having trouble getting data from the sources. Please try again later.");
          },
          complete: () => {
            if (!stopped) console.log("SignalR stream completed");
            setError(null);
          },
        });
      } catch (e) {
        console.error("SignalR connection failed:", e);
      }
    })();

    return () => {
      stopped = true;
      try { unsub?.dispose(); } catch {}
      connection.stop();
    };
  }, [apiBase]);


  const aggregates = useMemo<MovieAggregate[]>(() => {
    const map = new Map<string, MovieAggregate>();

    for (const ev of events) {
      if (ev.eventType === "movies" && ev.movies) {
        for (const m of ev.movies) {
          const key = `${m.title} (${m.year})`;
          if (!map.has(key)) {
            map.set(key, { key, title: m.title, year: m.year, providers: {}, expectedProviders: new Set([ev.provider]) });
          } else {
          const agg = map.get(key)!;
          agg.expectedProviders.add(ev.provider);
        }
        }
      }

      if (ev.eventType === "detail" && ev.detail) {
        const d = ev.detail;
        const key = `${d.title} (${d.year})`;
        if (!map.has(key)) {
          map.set(key, { key, title: d.title, year: d.year, providers: {}, expectedProviders: new Set([ev.provider])});
        } else {
          const agg = map.get(key)!;
          agg.expectedProviders.add(ev.provider);
        }
        const agg = map.get(key)!;
        agg.providers[ev.provider] = d.price;

        // compute cheapest
        let cheapestProv = "";
        let cheapestPrice = Number.POSITIVE_INFINITY;
        for (const [prov, price] of Object.entries(agg.providers)) {
          if (price < cheapestPrice) {
            cheapestPrice = price;
            cheapestProv = prov;
          }
        }
        if (isFinite(cheapestPrice)) {
          agg.cheapest = {
            provider: cheapestProv,
            price: cheapestPrice,
            isCached: !!ev.isCached,
          };
        }
      }
    }

    return Array.from(map.values()).sort((a, b) => a.title.localeCompare(b.title));
  }, [events]);

  const loading = !error && events.length === 0;
  return { aggregates, loading, error };
}

