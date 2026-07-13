import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { CreateReservationRequest, Reservation } from '../models/reservation.model';

@Injectable({ providedIn: 'root' })
export class ReservationsService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/reservations`;

  getAll(): Observable<Reservation[]> {
    return this.http.get<Reservation[]>(this.base);
  }

  create(request: CreateReservationRequest): Observable<Reservation> {
    return this.http.post<Reservation>(this.base, request);
  }

  cancel(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
