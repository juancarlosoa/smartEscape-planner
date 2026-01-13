<template>
  <div class="itinerary-detail">
    <div class="detail-header">
      <button @click="goBack" class="btn btn-outline back-btn">
        <i class="fas fa-arrow-left"></i> Volver
      </button>
      <div class="header-actions">
        <button @click="editItinerary" class="btn btn-outline">
          <i class="fas fa-edit"></i> Editar
        </button>
        <button @click="deleteItinerary" class="btn btn-danger">
          <i class="fas fa-trash"></i> Eliminar
        </button>
      </div>
    </div>

    <div v-if="loading" class="loading">
      <i class="fas fa-spinner fa-spin"></i> Cargando itinerario...
    </div>

    <div v-else-if="error" class="error">
      <i class="fas fa-exclamation-triangle"></i> {{ error }}
    </div>

    <div v-else-if="itinerary" class="itinerary-content">
      <div class="itinerary-header">
        <h1>{{ itinerary.name }}</h1>
        <p class="description">{{ itinerary.description }}</p>
        <div class="date-range">
          <i class="fas fa-calendar-alt"></i>
          <span>{{ formatDate(itinerary.startDate) }} - {{ formatDate(itinerary.endDate) }}</span>
        </div>
      </div>

      <div class="itinerary-days">
        <h2>Días del Itinerario</h2>

        <div class="days-list">
          <div
            v-for="day in computedDays"
            :key="day.dayNumber"
            class="day-card"
          >
            <div class="day-header">
              <h3>Día {{ day.dayNumber }}</h3>
              <span class="day-date">{{ formatDate(day.date) }}</span>
            </div>

            <div class="day-stops">
              <div v-if="day.stops.length === 0" class="empty-stops">
                <i class="fas fa-map-marker-alt"></i>
                <p>No hay paradas planificadas para este día</p>
                <button class="btn btn-sm btn-outline-primary" @click="console.log('Add stop to', day.date)">
                    <i class="fas fa-plus"></i> Añadir Parada
                </button>
              </div>

              <div v-else class="stops-list">
                <div
                  v-for="stop in day.stops"
                  :key="stop.id"
                  class="stop-item"
                >
                  <div class="stop-time">
                    {{ formatTime(stop.scheduledTime) }}
                  </div>
                  <div class="stop-icon">
                    <i class="fas fa-map-marker-alt"></i>
                  </div>
                  <div class="stop-content">
                    <div class="stop-notes">{{ stop.notes || 'Sin notas' }}</div>
                    <div class="stop-room">Sala ID: {{ stop.escapeRoomId }}</div>
                  </div>
                </div>
                 <div class="add-more-stops">
                    <button class="btn btn-sm btn-outline-primary" @click="console.log('Add another stop to', day.date)">
                        <i class="fas fa-plus"></i> Añadir Parada
                    </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { itineraryService } from '../../services/itineraryService';
import type { ItineraryDto, ItineraryStopDto } from '../../types/models';

interface Props {
  slug: string;
}

interface DayWithStops {
  dayNumber: number;
  date: Date;
  stops: ItineraryStopDto[];
}

const props = defineProps<Props>();

const emit = defineEmits<{
  back: [];
  edit: [slug: string];
}>();

const itinerary = ref<ItineraryDto | null>(null);
const loading = ref(true);
const error = ref<string | null>(null);

const computedDays = computed<DayWithStops[]>(() => {
  if (!itinerary.value) return [];

  const days: DayWithStops[] = [];
  const start = new Date(itinerary.value.startDate);
  const end = new Date(itinerary.value.endDate);
  
  // Normalize dates to start of day for accurate comparison
  start.setHours(0, 0, 0, 0);
  end.setHours(0, 0, 0, 0);

  let current = new Date(start);
  let dayCount = 1;

  while (current <= end) {
    // Filter stops for this specific day
    const stopsForDay = itinerary.value.stops.filter(stop => {
      const stopDate = new Date(stop.scheduledTime);
      stopDate.setHours(0, 0, 0, 0);
      return stopDate.getTime() === current.getTime();
    }).sort((a, b) => new Date(a.scheduledTime).getTime() - new Date(b.scheduledTime).getTime());

    days.push({
      dayNumber: dayCount,
      date: new Date(current),
      stops: stopsForDay
    });

    // Advance to next day
    current.setDate(current.getDate() + 1);
    dayCount++;
  }

  return days;
});

const loadItinerary = async () => {
  loading.value = true;
  error.value = null;
  try {
    itinerary.value = await itineraryService.getItineraryBySlug(props.slug);
  } catch (err) {
    error.value = 'Error al cargar el itinerario';
    console.error(err);
  } finally {
    loading.value = false;
  }
};

const goBack = () => {
  emit('back');
};

const editItinerary = () => {
  emit('edit', props.slug);
};

const deleteItinerary = async () => {
  if (!confirm('¿Estás seguro de que quieres eliminar este itinerario? Esta acción no se puede deshacer.')) {
    return;
  }

  try {
    await itineraryService.deleteItinerary(props.slug);
    emit('back');
  } catch (err) {
    error.value = 'Error al eliminar el itinerario';
    console.error(err);
  }
};

const formatDate = (date: Date | string) => {
  return new Date(date).toLocaleDateString('es-ES', {
    weekday: 'long',
    day: '2-digit',
    month: 'long',
    year: 'numeric'
  });
};

const formatTime = (dateString: string) => {
    return new Date(dateString).toLocaleTimeString('es-ES', {
        hour: '2-digit',
        minute: '2-digit'
    });
};

onMounted(() => {
  loadItinerary();
});
</script>

<style scoped>
.itinerary-detail {
  padding: 20px;
  max-width: 800px;
  margin: 0 auto;
}

.detail-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
}

.header-actions {
  display: flex;
  gap: 10px;
}

.back-btn {
  margin-right: auto;
}

.loading, .error {
  text-align: center;
  padding: 50px 20px;
  color: #666;
}

.error {
  color: #dc3545;
}

.itinerary-content {
  background-color: white;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.itinerary-header {
  padding: 30px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.itinerary-header h1 {
  margin: 0 0 10px 0;
  font-size: 2.5em;
}

.description {
  font-size: 1.1em;
  margin-bottom: 15px;
  opacity: 0.9;
}

.date-range {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 1.1em;
}

.itinerary-days {
  padding: 30px;
}

.itinerary-days h2 {
  margin: 0 0 20px 0;
  color: #333;
  border-bottom: 2px solid #007bff;
  padding-bottom: 10px;
}

.empty-days, .empty-stops {
  text-align: center;
  padding: 40px 20px;
  color: #666;
}

.empty-days i, .empty-stops i {
  font-size: 48px;
  color: #ccc;
  margin-bottom: 15px;
}

.days-list {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.day-card {
  border: 1px solid #e9ecef;
  border-radius: 8px;
  overflow: hidden;
}

.day-header {
  background-color: #f8f9fa;
  padding: 15px 20px;
  border-bottom: 1px solid #e9ecef;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.day-header h3 {
  margin: 0;
  color: #333;
}

.day-date {
  color: #666;
  font-weight: 500;
}

.day-stops {
  padding: 20px;
}

.stops-list {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.stop-item {
  display: flex;
  align-items: flex-start;
  gap: 15px;
  padding: 15px;
  background-color: #f8f9fa;
  border-radius: 6px;
  border-left: 4px solid #007bff;
}

.stop-icon {
  color: #007bff;
  font-size: 18px;
  margin-top: 2px;
}

.stop-content {
  flex: 1;
}

.stop-notes {
  font-weight: 500;
  color: #333;
  margin-bottom: 5px;
}

.stop-room {
  font-size: 14px;
  color: #666;
}

.btn {
  padding: 8px 16px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-size: 14px;
  display: inline-flex;
  align-items: center;
  gap: 6px;
  transition: all 0.3s ease;
  text-decoration: none;
}

.btn-outline {
  background-color: transparent;
  border: 1px solid #6c757d;
  color: #6c757d;
}

.btn-outline:hover {
  background-color: #6c757d;
  color: white;
}

.btn-danger {
  background-color: #dc3545;
  color: white;
}

.btn-danger:hover {
  background-color: #c82333;
}
</style>