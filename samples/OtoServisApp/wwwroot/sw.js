// OtoServisApp service worker — SADECE statik dosyaları (CSS/JS/ikon) önbelleğe alır.
// HTML sayfaları ve veri istekleri kasıtlı olarak önbelleğe alınmaz: bu çok kiracılı
// (multi-tenant) bir uygulama, bir kullanıcının oturum/işletme verisinin önbellekten
// başka bir kullanıcıya sızmasını önlemek için sayfalar her zaman ağdan taze gelmeli.

const CACHE_NAME = "otoservisapp-static-v1";
const STATIC_ASSETS = [
  "/css/site.css",
  "/js/site.js",
  "/lib/bootstrap/dist/css/bootstrap.min.css",
  "/lib/bootstrap/dist/js/bootstrap.bundle.min.js",
  "/lib/jquery/dist/jquery.min.js",
  "/icons/icon-192.png",
  "/icons/icon-512.png"
];

self.addEventListener("install", (event) => {
  event.waitUntil(
    caches.open(CACHE_NAME).then((cache) => cache.addAll(STATIC_ASSETS))
  );
  self.skipWaiting();
});

self.addEventListener("activate", (event) => {
  event.waitUntil(
    caches.keys().then((keys) =>
      Promise.all(keys.filter((key) => key !== CACHE_NAME).map((key) => caches.delete(key)))
    )
  );
  self.clients.claim();
});

self.addEventListener("fetch", (event) => {
  if (event.request.method !== "GET") return;

  const url = new URL(event.request.url);
  const isStaticAsset = STATIC_ASSETS.includes(url.pathname);
  if (!isStaticAsset) return; // HTML/veri istekleri: her zaman ağa git

  event.respondWith(
    caches.match(event.request).then((cached) => cached || fetch(event.request))
  );
});
