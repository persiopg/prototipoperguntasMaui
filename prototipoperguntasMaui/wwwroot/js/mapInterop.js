window.mapInterop = {
  map: null,
  markersLayer: null,
  userMarker: null,
  userWatchId: null,
  // DotNet reference provided by Blazor so JS can callback
  dotNetRef: null,
  // optional user info to display in user popup
  userInfo: null,

  init(center, markers, zoom, includeUser, dotNetRef, userInfo) {
    this.dotNetRef = dotNetRef || null;
    this.userInfo = userInfo || null;

    if (!this.map) {
      this.map = L.map('map').setView(center, zoom || 13);
      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors'
      }).addTo(this.map);
      this.markersLayer = L.layerGroup().addTo(this.map);
    } else {
      this.markersLayer.clearLayers();
      this.map.setView(center, zoom || 13);
    }

    if (markers && markers.length) {
      markers.forEach(m => {
        // Create a popup with two actions: abrir questionário e abrir no Maps
        const popupHtml = `<div style="min-width:160px"><strong>${m.label}</strong><div style="margin-top:6px;display:flex;gap:6px;flex-wrap:wrap"><button class="leaflet-popup-button btn-q" data-id="${m.id}" style="padding:6px 8px;font-size:12px">Abrir questionário</button><button class="leaflet-popup-button btn-maps" data-lat="${m.lat}" data-lng="${m.lng}" data-label="${m.label}" style="padding:6px 8px;font-size:12px">Abrir no Maps</button></div></div>`;
        const marker = L.marker([m.lat, m.lng]).addTo(this.markersLayer).bindPopup(popupHtml);

        // Attach click handlers after popup opens
        marker.on('popupopen', () => {
          const popupEl = marker.getPopup().getElement();
          if (!popupEl) return;
          const btnQ = popupEl.querySelector('.btn-q');
          if (btnQ) btnQ.addEventListener('click', (e) => {
            e.preventDefault();
            // Diagnostic log for clicks and dotNetRef availability
            try { console.log('mapInterop: btnQ clicked, id=', m.id, 'dotNetRef:', !!this.dotNetRef); } catch (err) { }
            // Debug only: log click (native dialog will be shown by .NET)
            try { console.log('Abrir questionário (JS): ' + m.label); } catch (err) { }

            // call back into .NET to open questionnaire
            if (this.dotNetRef && this.dotNetRef.invokeMethodAsync) {
              try {
                this.dotNetRef.invokeMethodAsync('Pdv_OpenQuestionnaire', m.id).catch(err => console.warn('dotNet invoke failed (async)', err));
              } catch (err) { console.warn('dotNet invoke failed', err); }
            } else {
              // Fallback: navigate to the store route within the app (may work in Blazor routing as fallback)
              try {
                console.warn('mapInterop: dotNetRef missing, using fallback navigation to /store/' + m.id);
                window.location.href = '#/store/' + m.id;
              } catch (err) { console.warn('fallback nav failed', err); }
            }

            marker.closePopup();
          });

          const btnMaps = popupEl.querySelector('.btn-maps');
          if (btnMaps) btnMaps.addEventListener('click', (e) => {
            e.preventDefault();
            this.openExternalMaps(m.lat, m.lng, m.label);
            marker.closePopup();
          });
        });
      });
    }

    if (includeUser) {
      this.getUserPosition().then(u => {
        if (u) {
          this.setUserMarker(u.lat, u.lng, true);
        }
      });
    }

    // Ensure the map invalidates its size when the container becomes visible again
    try {
      if (this._renderCheckInterval) { clearInterval(this._renderCheckInterval); this._renderCheckInterval = null; }
      this._renderCheckInterval = setInterval(() => {
        if (!this.map) return;
        const container = document.getElementById('map');
        if (!container) return;
        if (container.offsetWidth > 0 && container.offsetHeight > 0) {
          try { this.map.invalidateSize(); } catch (e) { }
          clearInterval(this._renderCheckInterval);
          this._renderCheckInterval = null;
        }
      }, 200);
    } catch (e) { /* ignore */ }
  },

  getUserPosition() {
    return new Promise((resolve) => {
      if (!navigator.geolocation) { resolve(null); return; }
      navigator.geolocation.getCurrentPosition((p) => resolve({ lat: p.coords.latitude, lng: p.coords.longitude }), () => resolve(null));
    });
  },

  setUserMarker(lat, lng, pan) {
    if (!this.map) return;
    if (!this.markersLayer) this.markersLayer = L.layerGroup().addTo(this.map);

    const popupContent = this.userInfo ? `<div style="min-width:140px"><div style="font-weight:bold">${this.userInfo.uuid}</div><div style="font-size:12px">${this.userInfo.name}</div><div style="font-size:11px;opacity:0.85;margin-top:4px">Lat: ${lat.toFixed(6)} Lng: ${lng.toFixed(6)}</div></div>` : 'Você';

    if (!this.userMarker) {
      // Use a styled circle marker so we don't depend on image assets
      this.userMarker = L.circleMarker([lat, lng], { radius: 8, color: '#1976d2', fillColor: '#1976d2', fillOpacity: 0.95 }).addTo(this.markersLayer);
      this.userMarker.bindPopup(popupContent);
    } else {
      this.userMarker.setLatLng([lat, lng]);
      try { this.userMarker.setPopupContent(popupContent); } catch (e) { /* ignore */ }
    }

    if (pan) this.map.panTo([lat, lng]);
  },

  openExternalMaps(lat, lng, label) {
    try {
      // geo: scheme for native mapping apps (mobile devices)
      const geoUrl = `geo:${lat},${lng}?q=${encodeURIComponent(label)}`;
      window.location.href = geoUrl;
      // fallback: open Google Maps after a short delay
      setTimeout(() => {
        const google = `https://www.google.com/maps/search/?api=1&query=${lat},${lng}`;
        window.open(google, '_blank');
      }, 500);
    } catch (e) {
      const google = `https://www.google.com/maps/search/?api=1&query=${lat},${lng}`;
      window.open(google, '_blank');
    }
  },

  watchUserPosition(options) {
    if (!navigator.geolocation) return null;
    if (this.userWatchId != null) return this.userWatchId; // already watching

    this.userWatchId = navigator.geolocation.watchPosition((p) => {
      this.setUserMarker(p.coords.latitude, p.coords.longitude, false);
    }, (err) => {
      console.warn('geolocation watch error', err);
    }, options || { enableHighAccuracy: true, maximumAge: 10000, timeout: 5000 });

    return this.userWatchId;
  },

  stopWatchUserPosition() {
    if (this.userWatchId != null) {
      navigator.geolocation.clearWatch(this.userWatchId);
      this.userWatchId = null;
    }
  },

  centerOnUser() {
    if (!this.userMarker || !this.map) return;
    this.map.panTo(this.userMarker.getLatLng());
  },

  // Called when the containing view becomes visible again to ensure Leaflet renders
  refresh() {
    if (!this.map) return;
    // Delay slightly to allow container/layout to settle
    setTimeout(() => {
      try {
        this.map.invalidateSize();
        if (this.userMarker) this.map.panTo(this.userMarker.getLatLng());
      } catch (e) { console.warn('mapInterop.refresh error', e); }
    }, 100);
  }
};

// Simple signature helper: initializes a canvas for pointer drawing, returns dataURL and can clear the canvas.
window.signature = {
  _canvases: {},
  init: function (canvasId) {
    try {
      const canvas = document.getElementById(canvasId);
      if (!canvas) return;
      const ctx = canvas.getContext('2d');
      const ratio = Math.max(window.devicePixelRatio || 1, 1);
      canvas.width = canvas.offsetWidth * ratio;
      canvas.height = canvas.offsetHeight * ratio;
      ctx.scale(ratio, ratio);
      ctx.strokeStyle = '#1e293b';
      ctx.lineWidth = 2;
      ctx.lineCap = 'round';
      let drawing = false;
      function getPos(e) {
        const rect = canvas.getBoundingClientRect();
        const clientX = e.touches ? e.touches[0].clientX : e.clientX;
        const clientY = e.touches ? e.touches[0].clientY : e.clientY;
        return { x: clientX - rect.left, y: clientY - rect.top };
      }
      canvas.addEventListener('pointerdown', function (e) { drawing = true; const p = getPos(e); ctx.beginPath(); ctx.moveTo(p.x, p.y); e.preventDefault(); });
      canvas.addEventListener('pointermove', function (e) { if (!drawing) return; const p = getPos(e); ctx.lineTo(p.x, p.y); ctx.stroke(); e.preventDefault(); });
      canvas.addEventListener('pointerup', function (e) { drawing = false; });
      canvas.addEventListener('pointercancel', function () { drawing = false; });
      this._canvases[canvasId] = { canvas: canvas, ctx: ctx, ratio: ratio };
    } catch (e) { console.warn('signature.init error', e); }
  },
  getDataUrl: function (canvasId) {
    try {
      const c = document.getElementById(canvasId);
      if (!c) return null;
      return c.toDataURL('image/png');
    } catch (e) { console.warn('signature.getDataUrl error', e); return null; }
  },
  clear: function (canvasId) {
    try {
      const entry = this._canvases[canvasId];
      if (entry) { const c = entry.canvas; const ctx = entry.ctx; ctx.clearRect(0, 0, c.width, c.height); }
      else { const c = document.getElementById(canvasId); if (c) c.getContext('2d').clearRect(0, 0, c.width, c.height); }
    } catch (e) { console.warn('signature.clear error', e); }
  }
};
