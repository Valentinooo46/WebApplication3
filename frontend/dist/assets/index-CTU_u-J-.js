import { i as importShared } from './_virtual___federation_fn_import-DOzywRP-.js';
import { r as reactExports } from './index-Dm_EQZZA.js';
import { r as reactDomExports } from './index-COvqqES_.js';

true&&(function polyfill() {
  const relList = document.createElement("link").relList;
  if (relList && relList.supports && relList.supports("modulepreload")) {
    return;
  }
  for (const link of document.querySelectorAll('link[rel="modulepreload"]')) {
    processPreload(link);
  }
  new MutationObserver((mutations) => {
    for (const mutation of mutations) {
      if (mutation.type !== "childList") {
        continue;
      }
      for (const node of mutation.addedNodes) {
        if (node.tagName === "LINK" && node.rel === "modulepreload")
          processPreload(node);
      }
    }
  }).observe(document, { childList: true, subtree: true });
  function getFetchOpts(link) {
    const fetchOpts = {};
    if (link.integrity) fetchOpts.integrity = link.integrity;
    if (link.referrerPolicy) fetchOpts.referrerPolicy = link.referrerPolicy;
    if (link.crossOrigin === "use-credentials")
      fetchOpts.credentials = "include";
    else if (link.crossOrigin === "anonymous") fetchOpts.credentials = "omit";
    else fetchOpts.credentials = "same-origin";
    return fetchOpts;
  }
  function processPreload(link) {
    if (link.ep)
      return;
    link.ep = true;
    const fetchOpts = getFetchOpts(link);
    fetch(link.href, fetchOpts);
  }
}());

const remotesMap = {
'remoteApp':{url:'http://localhost:4173/remoteEntry.js',format:'esm',from:'vite'}
};
                const currentImports = {};

                function get(name, remoteFrom) {
                    return __federation_import(name).then(module => () => {
                        return module
                    })
                }
                
                function merge(obj1, obj2) {
                  const mergedObj = Object.assign(obj1, obj2);
                  for (const key of Object.keys(mergedObj)) {
                    if (typeof mergedObj[key] === 'object' && typeof obj2[key] === 'object') {
                      mergedObj[key] = merge(mergedObj[key], obj2[key]);
                    }
                  }
                  return mergedObj;
                }

                const wrapShareModule = remoteFrom => {
                  return merge({
                    'react':{'18.3.1':{get:()=>get(new URL('__federation_shared_react-BCcI129A.js', import.meta.url).href), loaded:1}},'react-dom':{'18.3.1':{get:()=>get(new URL('__federation_shared_react-dom-BN8Au471.js', import.meta.url).href), loaded:1}},'react-router-dom':{'6.30.2':{get:()=>get(new URL('__federation_shared_react-router-dom-h4onzRw0.js', import.meta.url).href), loaded:1}}
                  }, (globalThis.__federation_shared__ || {})['default'] || {});
                };

                async function __federation_import(name) {
                    currentImports[name] ??= import(name);
                    return currentImports[name]
                }

                async function __federation_method_ensure(remoteId) {
                    const remote = remotesMap[remoteId];
                    if (!remote.inited) {
                        if (['esm', 'systemjs'].includes(remote.format)) {
                            // loading js with import(...)
                            return new Promise((resolve, reject) => {
                                const getUrl = () => Promise.resolve(remote.url);
                                getUrl().then(url => {
                                    import(/* @vite-ignore */ url).then(lib => {
                                        if (!remote.inited) {
                                            const shareScope = wrapShareModule();
                                            lib.init(shareScope);
                                            remote.lib = lib;
                                            remote.lib.init(shareScope);
                                            remote.inited = true;
                                        }
                                        resolve(remote.lib);
                                    }).catch(reject);
                                });
                            })
                        }
                    } else {
                        return remote.lib;
                    }
                }

                function __federation_method_wrapDefault(module, need) {
                    if (!module?.default && need) {
                        let obj = Object.create(null);
                        obj.default = module;
                        obj.__esModule = true;
                        return obj;
                    }
                    return module;
                }

                function __federation_method_getRemote(remoteName, componentName) {
                    return __federation_method_ensure(remoteName).then((remote) => remote.get(componentName).then(factory => factory()));
                }

var jsxRuntime = {exports: {}};

var reactJsxRuntime_production_min = {};

/**
 * @license React
 * react-jsx-runtime.production.min.js
 *
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */
var f=reactExports,k=Symbol.for("react.element"),l=Symbol.for("react.fragment"),m$1=Object.prototype.hasOwnProperty,n=f.__SECRET_INTERNALS_DO_NOT_USE_OR_YOU_WILL_BE_FIRED.ReactCurrentOwner,p={key:true,ref:true,__self:true,__source:true};
function q(c,a,g){var b,d={},e=null,h=null;void 0!==g&&(e=""+g);void 0!==a.key&&(e=""+a.key);void 0!==a.ref&&(h=a.ref);for(b in a)m$1.call(a,b)&&!p.hasOwnProperty(b)&&(d[b]=a[b]);if(c&&c.defaultProps)for(b in a=c.defaultProps,a) void 0===d[b]&&(d[b]=a[b]);return {$$typeof:k,type:c,key:e,ref:h,props:d,_owner:n.current}}reactJsxRuntime_production_min.Fragment=l;reactJsxRuntime_production_min.jsx=q;reactJsxRuntime_production_min.jsxs=q;

{
  jsxRuntime.exports = reactJsxRuntime_production_min;
}

var jsxRuntimeExports = jsxRuntime.exports;

var createRoot;
var m = reactDomExports;
{
  createRoot = m.createRoot;
  m.hydrateRoot;
}

const React = await importShared('react');
const {useEffect,useState} = React;
function RemoteAdminLoader() {
  const [RemoteComp, setRemoteComp] = useState(null);
  const [error, setError] = useState(null);
  useEffect(() => {
    let mounted = true;
    console.log("React(host):", React?.version)(async () => {
      try {
        const mod = await __federation_method_getRemote("remoteApp" , "./Admin").then(module=>__federation_method_wrapDefault(module, true));
        console.log("Remote module:", mod);
        const exported = mod && (mod.default ?? mod);
        console.log("Remote exported value:", exported);
        if (exported && typeof exported === "object" && exported.$$typeof) {
          if (mounted) setRemoteComp(() => () => exported);
          return;
        }
        if (typeof exported === "function") {
          if (mounted) setRemoteComp(() => exported);
          return;
        }
        if (exported && typeof exported === "object") {
          const candidate = exported.Admin || exported.default || exported.Component || null;
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
  if (error) return /* @__PURE__ */ jsxRuntimeExports.jsxs("div", { children: [
    "Ошибка загрузки админки: ",
    String(error)
  ] });
  if (!RemoteComp) return /* @__PURE__ */ jsxRuntimeExports.jsx("div", { children: "Загрузка админки..." });
  const C = RemoteComp;
  return /* @__PURE__ */ jsxRuntimeExports.jsx(C, {});
}
createRoot(document.getElementById("root")).render(
  /* @__PURE__ */ jsxRuntimeExports.jsx(React.StrictMode, { children: /* @__PURE__ */ jsxRuntimeExports.jsx(RemoteAdminLoader, {}) })
);
