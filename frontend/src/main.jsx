import React, { useEffect, useState } from "react";
import { createRoot } from "react-dom/client";
import "./styles.css";

function RemoteAdminLoader() {
  const [RemoteComp, setRemoteComp] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    let mounted = true;
      console.log('React(host):', React?.version);
    (async () => {
      try {
        const mod = await import("remoteApp/Admin");
        console.log("Remote module:", mod);
        const exported = mod && (mod.default ?? mod);

        console.log("Remote exported value:", exported);

        // Если экспорт — готовый React-элемент (объект с $$typeof)
        if (exported && typeof exported === "object" && exported.$$typeof) {
          if (mounted) setRemoteComp(() => () => exported);
          return;
        }

        // Если экспорт — функциональный или классовый компонент
        if (typeof exported === "function") {
          if (mounted) setRemoteComp(() => exported);
          return;
        }

        // Если экспорт — namespace (модуль) и содержит возможный компонент внутри
        if (exported && typeof exported === "object") {
          const candidate =
            exported.Admin || exported.default || exported.Component || null;
          if (candidate) {
            if (typeof candidate === "function") {
              if (mounted) setRemoteComp(() => candidate);
              return;
            }
            if (candidate && typeof candidate === "object" && candidate.$$typeof) {
              if (mounted) setRemoteComp(() => () => candidate);
              return;
            }
          }
        }

        throw new Error("Unsupported remote export shape");
      } catch (e) {
        console.error("Failed to load remote Admin:", e);
        if (mounted) setError(e);
      }
    })();

    return () => {
      mounted = false;
    };
  }, []);

  if (error) return <div>Ошибка загрузки админки: {String(error)}</div>;
  if (!RemoteComp) return <div>Загрузка админки...</div>;

  const C = RemoteComp;
  return <C />;
}

createRoot(document.getElementById("root")).render(
  <React.StrictMode>
    <RemoteAdminLoader />
  </React.StrictMode>
);