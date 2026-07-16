export interface Camping {
  id: string;
  name: string;
  description: string;
  latitude: number;
  longitude: number;
  pricePerNight: number;
  ownerId: string;
  categoryId: number;
  provinciaId?: string;
  facilityIds: string[];
  createdOn: string;
  updatedOn: string;
}

export interface PagedCampingsResponse {
  items: Camping[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface CampingSearchParams {
  name?: string;
  provinciaId?: string;
  provinciaCode?: string;
  categoryId?: number;
  minPrice?: number;
  maxPrice?: number;
  facilityIds?: string[];
  minLat?: number;
  maxLat?: number;
  minLng?: number;
  maxLng?: number;
  page?: number;
  pageSize?: number;
}
