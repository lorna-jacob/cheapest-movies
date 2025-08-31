# 🎬 Cheapest Movies App

A **.NET 8 + React (Vite) application** that compares movie prices from multiple providers (Cinemaworld, Filmworld) and shows the result to the UI.

## 🚀 Features
- **SignalR streaming:** backend pushes updates in real time to the UI for the movies from multiple providers
- **Resilience:** simple retry, timeout, and circuit breaker policies with Microsoft.Extensions.Http.Resilience
- **Caching:** fallback to last known data when provider API is unavailable.
---

## ✅ Assumptions & Design Choices

- **Single instance (dev/small deploys):**  
  The Web API is assumed to run as a **single container/instance** initially. Therefore I used **`IMemoryCache`** for simplicity and speed.

- **Future scale‑out:**  
  If running multiple replicas (Kubernetes, ECS), switch to a **distributed cache** (e.g., Redis) so cache is shared across instances and consistent for fallback.

- **Secret injection at deploy time:**  
  The CI/CD pipeline is assumed to fetch the API key from a **secure store** (e.g., **AWS Systems Manager Parameter Store** or **AWS Secrets Manager**) and inject it as the environment variable `WebjetSettings__ApiKey` during deployment. The key is **not** stored in images or configs.

- **SignalR for streaming:**  
  I used **SignalR streaming** (Hub method returning `IAsyncEnumerable<T>`) for robust server→client updates and auto‑reconnect. 
  
- **Agile delivery:**  
  The current state is just an MVP (Minimum Viable Product). The assumption is that the app will be enhanced in multiple iterations until deemed ready (i.e, add features to show details about the movie, add search bar, monitoring, alerting, etc).
---

## 🛠️ Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/en/download/) (with `npm`)
- An API key for the [Webjet API Test](https://webjetapitest.azurewebsites.net/)

---

## ⚙️ Backend (ASP.NET Core)

### 1. Clone and restore
```bash
git clone <github-repo-url>
cd webjet-movies-api
dotnet restore
```

### 2. Configure your API key

The app expects `WebjetSettings:ApiKey` to be provided via configuration.

#### Option A: Use environment variable

On **PowerShell**:
```powershell
$env:WebjetSettings__ApiKey="<your-api-key-here>"
```
#### Option B: Add to appsettings.json
Open appsettings.json in your editor, and add the key for the ApiKey under WebjetSettings

### 3. Run the API
Inside the webjet-movies-api csproj directory, run

```bash
dotnet run
```

The API will start on:
- `https://localhost:7070`
- `http://localhost:5070`

You can test health:
```
curl https://localhost:7070/api/health
```

If you need to trust the certificate, run the command `dotnet dev-certs https --trust`

---

## 🎨 Frontend (React + Vite)

In another terminal:

### 1. Setup
```bash
cd webjet-movies-ui
npm install
```

### 2. Run the dev server
```bash
npm run dev
```

Browser this url in your browser:
```
https://localhost:5173
```
---