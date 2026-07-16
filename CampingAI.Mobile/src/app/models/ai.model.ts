import { Camping, PagedCampingsResponse } from './camping.model';

export interface AiSearchRequest {
  query: string;
  page?: number;
  pageSize?: number;
}

export type AiSearchResponse = PagedCampingsResponse;

export interface AiRecommendationResponse {
  items: Camping[];
  reasoning: string;
}

export interface AiCompareRequest {
  campingIds: string[];
}

export interface AiCompareResponse {
  summary: string;
  bestForBudget?: string;
  bestOverall?: string;
}
