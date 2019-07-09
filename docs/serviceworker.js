console.log("This is service worker talking");

const filesToCache = [
    'index.html',
    'css/site.css',
    'css/bootstrap/bootstrap.min.css',
    'css/open-iconic/font/css/open-iconic-bootstrap.min.css',
    'css/open-iconic/font/fonts/open-iconic.woff',

    '_framework/blazor.webassembly.js',
    '_framework/blazor.boot.json',

    'manifest.json',
    'serviceworker.js',
    'js/main.js',
    'images/kani192x192.png',
    'images/kani512x512.png',

    '_framework/wasm/mono.js',
    '_framework/wasm/mono.wasm',

    '_framework/_bin/dnSpy.Contracts.Logic.dll',
    '_framework/_bin/dnSpy.Contracts.Logic.resources.dll',
    '_framework/_bin/dnSpy.Decompiler.dll',
    '_framework/_bin/dnSpy.Decompiler.ILSpy.Core.dll',
    '_framework/_bin/ICSharpCode.Decompiler.dll',
    '_framework/_bin/ICSharpCode.NRefactory.CSharp.dll',
    '_framework/_bin/ICSharpCode.NRefactory.dll',
    '_framework/_bin/ICSharpCode.NRefactory.VB.dll',
    '_framework/_bin/Kani.dll',
    '_framework/_bin/Microsoft.AspNetCore.Authorization.dll',
    '_framework/_bin/Microsoft.AspNetCore.Blazor.dll',
    '_framework/_bin/Microsoft.AspNetCore.Components.Browser.dll',
    '_framework/_bin/Microsoft.AspNetCore.Components.dll',
    '_framework/_bin/Microsoft.AspNetCore.Metadata.dll',
    '_framework/_bin/Microsoft.Extensions.DependencyInjection.Abstractions.dll',
    '_framework/_bin/Microsoft.Extensions.DependencyInjection.dll',
    '_framework/_bin/Microsoft.Extensions.Logging.Abstractions.dll',
    '_framework/_bin/Microsoft.Extensions.Options.dll',
    '_framework/_bin/Microsoft.Extensions.Primitives.dll',
    '_framework/_bin/Microsoft.JSInterop.dll',
    '_framework/_bin/Microsoft.Win32.Registry.dll',
    '_framework/_bin/Mono.Security.dll',
    '_framework/_bin/Mono.WebAssembly.Interop.dll',
    '_framework/_bin/mscorlib.dll',
    '_framework/_bin/System.Buffers.dll',
    '_framework/_bin/System.ComponentModel.Annotations.dll',
    '_framework/_bin/System.Core.dll',
    '_framework/_bin/System.dll',
    '_framework/_bin/System.Memory.dll',
    '_framework/_bin/System.Net.Http.dll',
    '_framework/_bin/System.Numerics.dll',
    '_framework/_bin/System.Numerics.Vectors.dll',
    '_framework/_bin/System.Reflection.Emit.dll',
    '_framework/_bin/System.Reflection.Emit.ILGeneration.dll',
    '_framework/_bin/System.Reflection.Emit.Lightweight.dll',
    '_framework/_bin/System.Runtime.CompilerServices.Unsafe.dll',
    '_framework/_bin/System.Security.AccessControl.dll',
    '_framework/_bin/System.Security.Principal.Windows.dll',
    '_framework/_bin/System.Text.Json.dll',
    '_framework/_bin/System.Threading.Tasks.Extensions.dll',
    '_framework/_bin/System.Xml.dll',
    '_framework/_bin/System.Xml.Linq.dll',
];

const cacheName = 'Kani-pwa';
self.addEventListener('install', function (e) {
    console.log('[ServiceWorker] Install');
    e.waitUntil(
        caches.open(cacheName).then(function (cache) {
            console.log('[ServiceWorker] Caching app shell');
            return cache.addAll(filesToCache);
        })
    );
});
self.addEventListener('activate', event => {
    event.waitUntil(self.clients.claim());
});
self.addEventListener('fetch', event => {
    event.respondWith(
        caches.match(event.request, { ignoreSearch: true }).then(response => {
            return response || fetch(event.request);
        })
    );
});