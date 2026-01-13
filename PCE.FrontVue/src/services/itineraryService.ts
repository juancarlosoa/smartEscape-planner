import apiClient from './api';
import type {
  ItineraryDto,
  ItineraryListDto,
  CreateItineraryRequest,
  UpdateItineraryRequest,
} from '../types/models';

export const itineraryService = {
  /**
   * Obtiene todos los itinerarios del usuario
   */
  async getAllItineraries(): Promise<ItineraryListDto[]> {
    try {
      const response = await apiClient.get<ItineraryListDto[]>('/itineraries');
      return response.data;
    } catch (error) {
      console.error('Error fetching itineraries:', error);
      throw error;
    }
  },

  /**
   * Obtiene un itinerario específico por slug
   */
  async getItineraryBySlug(slug: string): Promise<ItineraryDto> {
    try {
      const response = await apiClient.get<ItineraryDto>(`/itineraries/${slug}`);
      return response.data;
    } catch (error) {
      console.error(`Error fetching itinerary ${slug}:`, error);
      throw error;
    }
  },

  /**
   * Crea un nuevo itinerario
   */
  async createItinerary(request: CreateItineraryRequest): Promise<string> {
    try {
      const response = await apiClient.post<string>('/itineraries', request);
      return response.data;
    } catch (error) {
      console.error('Error creating itinerary:', error);
      throw error;
    }
  },

  /**
   * Actualiza un itinerario existente
   */
  async updateItinerary(slug: string, request: UpdateItineraryRequest): Promise<string> {
    try {
      const response = await apiClient.put<string>(`/itineraries/${slug}`, request);
      return response.data;
    } catch (error) {
      console.error(`Error updating itinerary ${slug}:`, error);
      throw error;
    }
  },

  /**
   * Elimina un itinerario
   */
  async deleteItinerary(slug: string): Promise<void> {
    try {
      await apiClient.delete(`/itineraries/${slug}`);
    } catch (error) {
      console.error(`Error deleting itinerary ${slug}:`, error);
      throw error;
    }
  },

  async addStop(itinerarySlug: string, escapeRoomId: string, notes: string, scheduledTime: string): Promise<string> {
    try {
      const response = await apiClient.post<string>(`/itineraries/${itinerarySlug}/stops`, {
        escapeRoomId,
        notes,
        scheduledTime
      });
      return response.data;
    } catch (error) {
      console.error(`Error adding stop to itinerary ${itinerarySlug}:`, error);
      throw error;
    }
  },

  async updateStop(itinerarySlug: string, stopId: string, notes: string, scheduledTime: string): Promise<void> {
    try {
      await apiClient.put(`/itineraries/${itinerarySlug}/stops/${stopId}`, {
        notes,
        scheduledTime
      });
    } catch (error) {
      console.error(`Error updating stop ${stopId} in itinerary ${itinerarySlug}:`, error);
      throw error;
    }
  },

  async removeStop(itinerarySlug: string, stopId: string): Promise<void> {
    try {
      await apiClient.delete(`/itineraries/${itinerarySlug}/stops/${stopId}`);
    } catch (error) {
      console.error(`Error removing stop ${stopId} from itinerary ${itinerarySlug}:`, error);
      throw error;
    }
  }
};