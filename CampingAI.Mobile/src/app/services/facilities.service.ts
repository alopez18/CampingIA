import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Facility } from '../models/facility.model';

@Injectable({ providedIn: 'root' })
export class FacilitiesService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/facilities`;

  getFacilities(): Observable<Facility[]> {
    return this.http.get<Facility[]>(this.base);
  }
}
