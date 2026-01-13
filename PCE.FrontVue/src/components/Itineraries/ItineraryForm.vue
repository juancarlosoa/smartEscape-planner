<template>
  <div class="itinerary-form">
    <div class="form-header">
      <h2>{{ isEditing ? 'Editar Itinerario' : 'Crear Nuevo Itinerario' }}</h2>
      <button @click="goBack" class="btn btn-outline">
        <i class="fas fa-arrow-left"></i> Volver
      </button>
    </div>

    <form @submit.prevent="submitForm" class="form">
      <div class="form-group">
        <label for="name">Nombre del Itinerario *</label>
        <input
          id="name"
          v-model="form.name"
          type="text"
          required
          placeholder="Ej: Viaje a Madrid"
        />
      </div>

      <div class="form-group">
        <label for="description">Descripción</label>
        <textarea
          id="description"
          v-model="form.description"
          rows="4"
          placeholder="Describe tu itinerario..."
        ></textarea>
      </div>

      <div class="form-row">
        <div class="form-group">
          <label for="startDate">Fecha de Inicio *</label>
          <input
            id="startDate"
            v-model="form.startDate"
            type="date"
            required
          />
        </div>

        <div class="form-group">
          <label for="endDate">Fecha de Fin *</label>
          <input
            id="endDate"
            v-model="form.endDate"
            type="date"
            required
          />
        </div>
      </div>

      <div v-if="submitError" class="submit-error">
        <i class="fas fa-exclamation-triangle"></i> {{ submitError }}
      </div>

      <div class="form-actions">
        <button type="button" @click="goBack" class="btn btn-outline">
          Cancelar
        </button>
        <button type="submit" :disabled="loading" class="btn btn-primary">
          <i v-if="loading" class="fas fa-spinner fa-spin"></i>
          {{ isEditing ? 'Actualizar' : 'Crear' }} Itinerario
        </button>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { itineraryService } from '../../services/itineraryService';
// No imports needed

interface Props {
  slug?: string;
}

const props = defineProps<Props>();

const emit = defineEmits<{
  back: [];
  saved: [slug: string];
}>();

type ItineraryFormData = {
  name: string;
  description: string;
  startDate: string;
  endDate: string;
};

const form = ref<ItineraryFormData>({
  name: '',
  description: '',
  startDate: '',
  endDate: ''
});

const submitError = ref<string | null>(null);
const loading = ref(false);

const isEditing = computed(() => !!props.slug);

const loadItinerary = async () => {
  if (!props.slug) return;

  loading.value = true;
  try {
    const itinerary = await itineraryService.getItineraryBySlug(props.slug);
    form.value = {
      name: itinerary.name,
      description: itinerary.description,
      startDate: itinerary.startDate.split('T')[0],
      endDate: itinerary.endDate.split('T')[0]
    } as ItineraryFormData;
  } catch (err) {
    submitError.value = 'Error al cargar el itinerario';
    console.error(err);
  } finally {
    loading.value = false;
  }
};

const submitForm = async () => {
  loading.value = true;
  submitError.value = null;

  try {
    let result: string;

    if (isEditing.value && props.slug) {
      result = await itineraryService.updateItinerary(props.slug, form.value);
    } else {
      result = await itineraryService.createItinerary(form.value);
    }

    emit('saved', result);
  } catch (err: any) {
    submitError.value = err.response?.data?.error || 'Error al guardar el itinerario';
    console.error(err);
  } finally {
    loading.value = false;
  }
};

const goBack = () => {
  emit('back');
};

onMounted(() => {
  if (isEditing.value) {
    loadItinerary();
  }
});
</script>

<style scoped>
.itinerary-form {
  max-width: 600px;
  margin: 0 auto;
  padding: 20px;
}

.form-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
}

.form-header h2 {
  margin: 0;
  color: #333;
}

.form {
  background-color: white;
  padding: 30px;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.form-group {
  margin-bottom: 20px;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 5px;
  font-weight: 500;
  color: #333;
}

.form-group input,
.form-group textarea {
  width: 100%;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
  transition: border-color 0.3s ease;
}

.form-group input:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #007bff;
}

.form-group input.error,
.form-group textarea.error {
  border-color: #dc3545;
}

.error-message {
  color: #dc3545;
  font-size: 12px;
  margin-top: 5px;
  display: block;
}

.submit-error {
  background-color: #f8d7da;
  color: #721c24;
  padding: 10px;
  border-radius: 4px;
  margin-bottom: 20px;
  display: flex;
  align-items: center;
  gap: 8px;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  margin-top: 30px;
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

.btn-primary:hover:not(:disabled) {
  background-color: #0056b3;
}

.btn-primary:disabled {
  background-color: #6c757d;
  cursor: not-allowed;
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
</style>