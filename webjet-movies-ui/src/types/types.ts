export type MovieSummary = {
  id: string;
  title: string;
  year: number;
  type?: string | null;
  poster?: string | null;
};

export type MovieDetail = MovieSummary & {
  rated: string;
  released: string; 
  runtime: string;
  genre: string;
  director: string;
  writer: string;
  actors: string;
  plot: string;
  language: string;
  country: string;
  metascore: number;
  rating: number;
  votes: number;
  price: number;
};

export type ProviderStreamEvent = {
  provider: string;
  eventType: "movies" | "detail";
  isCached: boolean;
  movies?: MovieSummary[];
  detail?: MovieDetail;
};

export type MovieAggregate = {
  key: string; 
  title: string;
  year: number;
  providers: Record<string, number>; // provider -> price
  cheapest?: { provider: string; price: number; isCached: boolean };
  expectedProviders: Set<string>;
};
