<template>
  <div class="itinerary-list">
    <div class="header">
      <h2>Mis Itinerarios</h2>
      <button @click="showCreate" class="btn btn-primary">
        <i class="fas fa-plus"></i> Nuevo Itinerario
      </button>
    </div>

    <div v-if="loading" class="loading">
      <i class="fas fa-spinner fa-spin"></i> Cargando itinerarios...
    </div>

    <div v-else-if="error" class="error">
      <i class="fas fa-exclamation-triangle"></i> {{ error }}
    </div>

    <div v-else-if="itineraries.length === 0" class="empty-state">
      <i class="fas fa-map-marked-alt"></i>
      <h3>No tienes itinerarios aún</h3>
      <p>Crea tu primer itinerario para organizar tus visitas a salas de escape</p>
      <button @click="showCreate" class="btn btn-primary">
        <i class="fas fa-plus"></i> Crear mi primer itinerario
      </button>
    </div>

    <div v-else class="itinerary-grid">
      <div
        v-for="itinerary in itineraries"
        :key="itinerary.slug"
        class="itinerary-card"
        @click="viewItinerary(itinerary.slug)"
      >
        <div class="card-header">
          <h3>{{ itinerary.name }}</h3>
          <div class="card-actions">
            <button @click.stop="editItinerary(itinerary.slug)" class="btn btn-sm btn-outline">
              <i class="fas fa-edit"></i>
            </button>
            <button @click.stop="deleteItinerary(itinerary.slug)" class="btn btn-sm btn-danger">
              <i class="fas fa-trash"></i>
            </button>
          </div>
        </div>

        <p class="description">{{ itinerary.description }}</p>

        <div class="date-range">
          <i class="fas fa-calendar-alt"></i>
          <span>{{ formatDate(itinerary.startDate) }} - {{ formatDate(itinerary.endDate) }}</span>
        </div>

        <div class="card-footer">
          <span class="slug">{{ itinerary.slug }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { itineraryService } from '../../services/itineraryService';
import type { ItineraryListDto } from '../../types/models';

const itineraries = ref<ItineraryListDto[]>([]);
const loading = ref(true);
const error = ref<string | null>(null);

const emit = defineEmits<{
  create: [];
  edit: [slug: string];
  view: [slug: string];
}>();

const loadItineraries = async () => {
  loading.value = true;
  error.value = null;
  try {
    itineraries.value = await itineraryService.getAllItineraries();
  } catch (err) {
    error.value = 'Error al cargar los itinerarios';
    console.error(err);
  } finally {
    loading.value = false;
  }
};

const showCreate = () => {
  emit('create');
};

const viewItinerary = (slug: string) => {
  emit('view', slug);
};

const editItinerary = (slug: string) => {
  emit('edit', slug);
};

const deleteItinerary = async (slug: string) => {
  if (!confirm('¿Estás seguro de que quieres eliminar este itinerario?')) {
    return;
  }

  try {
    await itineraryService.deleteItinerary(slug);
    await loadItineraries(); // Recargar la lista
  } catch (err) {
    error.value = 'Error al eliminar el itinerario';
    console.error(err);
  }
};

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('es-ES', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  });
};

onMounted(() => {
  loadItineraries();
});
</script>

<style scoped>
.itinerary-list {
  padding: 20px;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
}

.header h2 {
  margin: 0;
  color: #333;
}

.btn {
  padding: 10px 20px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-size: 14px;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  transition: all 0.3s ease;
}

.btn-primary {
  background-color: #007bff;
  color: white;
}

.btn-primary:hover {
  background-color: #0056b3;
}

.btn-sm {
  padding: 5px 10px;
  font-size: 12px;
}

.btn-outline {
  background-color: transparent;
  border: 1px solid #007bff;
  color: #007bff;
}

.btn-outline:hover {
  background-color: #007bff;
  color: white;
}

.btn-danger {
  background-color: #dc3545;
  color: white;
}

.btn-danger:hover {
  background-color: #c82333;
}

.loading, .error, .empty-state {
  text-align: center;
  padding: 50px 20px;
  color: #666;
}

.error {
  color: #dc3545;
}

.empty-state i {
  font-size: 64px;
  color: #ccc;
  margin-bottom: 20px;
}

.empty-state h3 {
  margin: 0 0 10px 0;
  color: #333;
}

.empty-state p {
  margin: 0 0 20px 0;
}

.itinerary-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 20px;
}

.itinerary-card {
  border: 1px solid #ddd;
  border-radius: 8px;
  padding: 20px;
  cursor: pointer;
  transition: all 0.3s ease;
  background-color: white;
}

.itinerary-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  border-color: #007bff;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 15px;
}

.card-header h3 {
  margin: 0;
  color: #333;
  flex: 1;
}

.card-actions {
  display: flex;
  gap: 5px;
}

.description {
  color: #666;
  margin-bottom: 15px;
  line-height: 1.4;
}

.date-range {
  display: flex;
  align-items: center;
  gap: 8px;
  color: #666;
  font-size: 14px;
  margin-bottom: 15px;
}

.card-footer {
  border-top: 1px solid #eee;
  padding-top: 10px;
}

.slug {
  font-family: monospace;
  font-size: 12px;
  color: #999;
}
</style>