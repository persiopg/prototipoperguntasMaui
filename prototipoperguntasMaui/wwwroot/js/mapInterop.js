window.mapInterop = {
  map: null,
  markersLayer: null,
  userMarker: null,
  userWatchId: null,

  init(center, markers, zoom, includeUser) {
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
        L.marker([m.lat, m.lng]).addTo(this.markersLayer).bindPopup(m.label || '');
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

    if (!this.userMarker) {
      // Use a styled circle marker so we don't depend on image assets
      this.userMarker = L.circleMarker([lat, lng], { radius: 8, color: '#1976d2', fillColor: '#1976d2', fillOpacity: 0.95 }).addTo(this.markersLayer);
      this.userMarker.bindPopup('Você');
    } else {
      this.userMarker.setLatLng([lat, lng]);
    }

    if (pan) this.map.panTo([lat, lng]);
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
