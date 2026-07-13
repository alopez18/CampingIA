import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Country, Province } from '../models/location.model';

@Injectable({ providedIn: 'root' })
export class LocationService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/location`;

  getCountries(): Observable<Country[]> {
    return this.http.get<Country[]>(`${this.base}/countries`);
  }

  getProvinces(): Observable<Province[]> {
    return this.http.get<Province[]>(`${this.base}/provinces`);
  }

  getProvincesByCountry(countryId: string): Observable<Province[]> {
    return this.http.get<Province[]>(`${this.base}/countries/${countryId}/provinces`);
  }
}
