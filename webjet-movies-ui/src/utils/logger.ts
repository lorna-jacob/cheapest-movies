type LogLevel = "debug" | "info" | "warn" | "error";

function log(level: LogLevel, message: string, ...args: any[]) {
  if (import.meta.env.MODE === "development") {
    switch (level) {
      case "debug":
        console.debug(`[DEBUG] ${message}`, ...args);
        break;
      case "info":
        console.info(`[INFO] ${message}`, ...args);
        break;
      case "warn":
        console.warn(`[WARN] ${message}`, ...args);
        break;
      case "error":
        console.error(`[ERROR] ${message}`, ...args);
        break;
    }
  } else {
    // In production: forward to remote logging service, i.e Sentry
  }
}

export const logger = {
  debug: (msg: string, ...args: any[]) => log("debug", msg, ...args),
  info: (msg: string, ...args: any[]) => log("info", msg, ...args),
  warn: (msg: string, ...args: any[]) => log("warn", msg, ...args),
  error: (msg: string, ...args: any[]) => log("error", msg, ...args),
};
