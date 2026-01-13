<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import LeafletMap from '../components/Maps/LeafletMap.vue';
import EscapeRoomList from '../components/EscapeRooms/EscapeRoomList.vue';
import EscapeRoomForm from '../components/EscapeRooms/EscapeRoomForm.vue';
import ItineraryList from '../components/Itineraries/ItineraryList.vue';
import ItineraryForm from '../components/Itineraries/ItineraryForm.vue';
import ItineraryDetail from '../components/Itineraries/ItineraryDetail.vue';
import { escapeRoomService } from '../services/escapeRoomService';
import type { EscapeRoomDto } from '../types/models';

const router = useRouter();

const rooms = ref<EscapeRoomDto[]>([]);
const loading = ref(true);
const error = ref<string | null>(null);
const currentView = ref<'map' | 'list' | 'create' | 'edit' | 'itineraries' | 'create-itinerary' | 'edit-itinerary' | 'view-itinerary'>('map');
const editingSlug = ref<string | undefined>(undefined);
const debugInfo = ref('');

const loadData = async () => {
  loading.value = true;
  try {
    rooms.value = await escapeRoomService.getAllRooms();
    debugInfo.value = `Cargadas ${rooms.value.length} salas`;
  } catch (err) {
    error.value = 'Error al cargar las salas del backend';
    console.error(err);
  } finally {
    loading.value = false;
  }
};

const showCreate = () => {
  editingSlug.value = undefined;
  currentView.value = 'create';
};

const showEdit = (slug: string) => {
  editingSlug.value = slug;
  currentView.value = 'edit';
};

const onSaved = async () => {
  await loadData();
  currentView.value = 'list';
};

const onCancel = () => {
  currentView.value = 'list';
};

const showItineraries = () => {
  currentView.value = 'itineraries';
};

const showCreateItinerary = () => {
  editingSlug.value = undefined;
  currentView.value = 'create-itinerary';
};

const showEditItinerary = (slug: string) => {
  editingSlug.value = slug;
  currentView.value = 'edit-itinerary';
};


const onItinerarySaved = () => {
  currentView.value = 'itineraries';
};

const onItineraryCancel = () => {
  currentView.value = 'itineraries';
};

const showViewItinerary = (slug: string) => {
  editingSlug.value = slug;
  currentView.value = 'view-itinerary';
};

const onViewItineraryBack = () => {
    currentView.value = 'itineraries';
};

const logout = () => {
  localStorage.removeItem('google_id_token');
  router.push('/login');
};

onMounted(async () => {
  await loadData();
});
</script>

<template>
  <div id="dashboard-container">
    <header>
      <h1>Planea con Enanos</h1>
      <nav>
        <button @click="currentView = 'map'" :class="{ active: currentView === 'map' }">Mapa</button>
        <button @click="currentView = 'list'" :class="{ active: currentView === 'list' }">Salas</button>
        <button @click="showCreate" :class="{ active: currentView === 'create' }">Nueva Sala</button>
        <div class="separator"></div>
        <button @click="showItineraries" :class="{ active: currentView === 'itineraries' || currentView === 'create-itinerary' || currentView === 'edit-itinerary' }">📅 Itinerarios</button>
        <button @click="showCreateItinerary" :class="{ active: currentView === 'create-itinerary' }">Nuevo Itinerario</button>
        <div class="separator"></div>
        <button @click="logout" class="logout-btn">Salir</button>
      </nav>
    </header>
    
    <div v-if="error" class="error-banner">
      {{ error }}
    </div>
    
    <main>
      <div v-if="currentView === 'map'" class="map-view">
        <LeafletMap :rooms="rooms"></LeafletMap>
      </div>
      
      <div v-else-if="currentView === 'list'" class="list-view">
        <EscapeRoomList @edit="showEdit" ref="listComponent" />
      </div>
      
      <div v-else-if="currentView === 'create' || currentView === 'edit'" class="form-view">
        <EscapeRoomForm 
          :slug="editingSlug" 
          @saved="onSaved" 
          @cancel="onCancel" 
        />
      </div>

      <div v-else-if="currentView === 'itineraries'" class="list-view">
        <ItineraryList @edit="showEditItinerary" @view="showViewItinerary" />
      </div>

      <div v-else-if="currentView === 'create-itinerary' || currentView === 'edit-itinerary'" class="form-view">
        <ItineraryForm 
          :slug="editingSlug" 
          @saved="onItinerarySaved" 
          @cancel="onItineraryCancel" 
        />
      </div>

      <div v-else-if="currentView === 'view-itinerary'" class="detail-view">
          <ItineraryDetail 
            :slug="editingSlug!" 
            @back="onViewItineraryBack" 
            @edit="showEditItinerary" 
          />
      </div>
    </main>
    
    <footer>
      <p>{{ debugInfo }}</p>
    </footer>
  </div>
</template>

<style scoped>
#dashboard-container {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  background-color: #f5f5f5;
}

header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 1rem 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

header h1 {
  margin: 0;
  font-size: 1.5rem;
}

nav {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.separator {
  width: 1px;
  height: 24px;
  background-color: rgba(255,255,255,0.3);
  margin: 0 0.5rem;
}

nav button {
  background: rgba(255, 255, 255, 0.2);
  border: none;
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  transition: background 0.2s;
  font-size: 0.9rem;
}

nav button:hover, nav button.active {
  background: rgba(255, 255, 255, 0.4);
}

.logout-btn {
  background-color: rgba(255, 0, 0, 0.2) !important;
}

.logout-btn:hover {
  background-color: rgba(255, 0, 0, 0.4) !important;
}

.error-banner {
  background-color: #fff3cd;
  color: #856404;
  padding: 1rem;
  text-align: center;
}

main {
  flex: 1;
  padding: 2rem;
  display: flex;
  flex-direction: column;
}

.map-view {
  flex: 1;
  min-height: 500px;
  display: flex;
  flex-direction: column;
}

.detail-view {
  flex: 1;
  display: flex;
  justify-content: center;
}

footer {
  text-align: center;
  padding: 1rem;
  color: #666;
  font-size: 0.8rem;
}
</style>
