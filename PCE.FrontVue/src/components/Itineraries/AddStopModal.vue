<template>
  <div v-if="show" class="modal-overlay" @click="handleClose">
    <div class="modal-content" @click.stop>
      <div class="modal-header">
        <h3>Añadir Parada</h3>
        <button @click="handleClose" class="modal-close">
          <i class="fas fa-times"></i>
        </button>
      </div>
      <div class="modal-body">
        <div class="form-group">
          <label>Sala de Escape <span class="required">*</span></label>
          <div class="room-search-container">
            <input 
              v-model="searchQuery" 
              type="text" 
              placeholder="Buscar sala de escape..."
              class="form-control"
              @input="handleSearch"
              @focus="showDropdown = true"
            />
            <div v-if="showDropdown && filteredRooms.length > 0" class="room-dropdown">
              <div 
                v-for="room in filteredRooms" 
                :key="room.slug"
                class="room-option"
                @click="selectRoom(room)"
              >
                <strong>{{ room.name }}</strong>
                <small>{{ room.companySlug }}</small>
              </div>
            </div>
            <div v-if="selectedRoom" class="selected-room">
              <i class="fas fa-check-circle"></i>
              {{ selectedRoom.name }} ({{ selectedRoom.companySlug }})
              <button @click="clearSelection" class="clear-btn">
                <i class="fas fa-times"></i>
              </button>
            </div>
          </div>
        </div>
        
        <div class="form-group">
          <label>Fecha y Hora <span class="required">*</span></label>
          <input 
            v-model="form.scheduledTime" 
            type="datetime-local" 
            class="form-control"
          />
        </div>

        <div class="form-group">
          <label>Notas</label>
          <textarea 
            v-model="form.notes" 
            placeholder="Añade notas sobre esta parada..."
            class="form-control"
            rows="4"
          ></textarea>
        </div>

        <div v-if="error" class="error-message">
          {{ error }}
        </div>
      </div>
      <div class="modal-footer">
        <button @click="handleClose" class="btn btn-outline">Cancelar</button>
        <button @click="handleSubmit" class="btn btn-primary" :disabled="loading">
          <i v-if="loading" class="fas fa-spinner fa-spin"></i>
          <i v-else class="fas fa-plus"></i>
          {{ loading ? 'Añadiendo...' : 'Añadir Parada' }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue';
import { itineraryService } from '../../services/itineraryService';
import { escapeRoomService } from '../../services/escapeRoomService';
import type { EscapeRoomDto } from '../../types/models';

interface Props {
  show: boolean;
  itinerarySlug: string;
  prefilledDate?: Date;
}

const props = defineProps<Props>();

const emit = defineEmits<{
  close: [];
  stopAdded: [];
}>();

const form = ref({
  escapeRoomSlug: '',
  scheduledTime: '',
  notes: ''
});

const loading = ref(false);
const error = ref<string | null>(null);

// Room search functionality
const searchQuery = ref('');
const showDropdown = ref(false);
const selectedRoom = ref<EscapeRoomDto | null>(null);
const allRooms = ref<EscapeRoomDto[]>([]);

// Load rooms on component mount
const loadRooms = async () => {
  try {
    allRooms.value = await escapeRoomService.getAllRooms();
  } catch (err) {
    console.error('Error loading rooms:', err);
  }
};

// Watch for modal show to load rooms
watch(() => props.show, (isShowing) => {
  if (isShowing && allRooms.value.length === 0) {
    loadRooms();
  }
  if (!isShowing) {
    resetForm();
  }
});

// Filtered rooms based on search query
const filteredRooms = computed(() => {
  if (!searchQuery.value) return allRooms.value.slice(0, 10); // Show first 10 if no search
  return allRooms.value.filter(room => 
    room.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
    room.companySlug.toLowerCase().includes(searchQuery.value.toLowerCase())
  ).slice(0, 10); // Limit to 10 results
});

// Handle search input
const handleSearch = () => {
  showDropdown.value = true;
};

// Select a room from dropdown
const selectRoom = (room: EscapeRoomDto) => {
  selectedRoom.value = room;
  form.value.escapeRoomSlug = room.slug;
  searchQuery.value = room.name;
  showDropdown.value = false;
};

// Clear room selection
const clearSelection = () => {
  selectedRoom.value = null;
  form.value.escapeRoomSlug = '';
  searchQuery.value = '';
};

// Watch for date changes to prefill the datetime input
watch(() => props.prefilledDate, (newDate) => {
  if (newDate) {
    const year = newDate.getFullYear();
    const month = String(newDate.getMonth() + 1).padStart(2, '0');
    const day = String(newDate.getDate()).padStart(2, '0');
    form.value.scheduledTime = `${year}-${month}-${day}T10:00`;
  }
}, { immediate: true });

// Reset form when modal is closed
watch(() => props.show, (isShowing) => {
  if (!isShowing) {
    resetForm();
  }
});

const resetForm = () => {
  form.value = {
    escapeRoomSlug: '',
    scheduledTime: '',
    notes: ''
  };
  selectedRoom.value = null;
  searchQuery.value = '';
  error.value = null;
};

const handleClose = () => {
  emit('close');
};

const handleSubmit = async () => {
  error.value = null;
  
  // Validation
  if (!form.value.escapeRoomSlug || !form.value.scheduledTime) {
    error.value = 'Por favor, completa los campos obligatorios';
    return;
  }

  loading.value = true;
  try {
    const scheduledTime = new Date(form.value.scheduledTime).toISOString();
    
    await itineraryService.addStop(
      props.itinerarySlug,
      form.value.escapeRoomSlug,
      form.value.notes || '',
      scheduledTime
    );
    
    emit('stopAdded');
    emit('close');
  } catch (err) {
    console.error('Error adding stop:', err);
    error.value = 'Error al añadir la parada. Por favor, intenta de nuevo.';
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 8px;
  width: 90%;
  max-width: 500px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px;
  border-bottom: 1px solid #e9ecef;
}

.modal-header h3 {
  margin: 0;
  color: #333;
}

.modal-close {
  background: none;
  border: none;
  font-size: 24px;
  color: #666;
  cursor: pointer;
  padding: 0;
  width: 30px;
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.modal-close:hover {
  color: #333;
}

.modal-body {
  padding: 20px;
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  padding: 20px;
  border-top: 1px solid #e9ecef;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 8px;
  font-weight: 500;
  color: #333;
}

.required {
  color: #dc3545;
}

.form-control {
  width: 100%;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
  font-family: inherit;
}

.form-control:focus {
  outline: none;
  border-color: #007bff;
  box-shadow: 0 0 0 3px rgba(0, 123, 255, 0.1);
}

textarea.form-control {
  resize: vertical;
  min-height: 80px;
}

.error-message {
  padding: 10px;
  background-color: #f8d7da;
  color: #721c24;
  border: 1px solid #f5c6cb;
  border-radius: 4px;
  margin-top: 10px;
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
}

.btn-primary {
  background-color: #007bff;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background-color: #0056b3;
}

.btn-primary:disabled {
  opacity: 0.6;
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

/* Room Search Styles */
.room-search-container {
  position: relative;
}

.room-dropdown {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  background: white;
  border: 1px solid #ddd;
  border-radius: 4px;
  max-height: 200px;
  overflow-y: auto;
  z-index: 1000;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}

.room-option {
  padding: 10px 15px;
  cursor: pointer;
  border-bottom: 1px solid #f0f0f0;
  transition: background-color 0.2s;
}

.room-option:hover {
  background-color: #f8f9fa;
}

.room-option:last-child {
  border-bottom: none;
}

.room-option strong {
  display: block;
  color: #333;
}

.room-option small {
  color: #666;
  font-size: 0.9em;
}

.selected-room {
  margin-top: 10px;
  padding: 10px;
  background-color: #e8f5e8;
  border: 1px solid #c3e6c3;
  border-radius: 4px;
  color: #2d5a2d;
  display: flex;
  align-items: center;
  gap: 10px;
}

.clear-btn {
  background: none;
  border: none;
  color: #666;
  cursor: pointer;
  padding: 0;
  margin-left: auto;
}

.clear-btn:hover {
  color: #333;
}
</style>
