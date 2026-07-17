import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { CampingsService } from './campings.service';
import { Camping, PagedCampingsResponse } from '../models/camping.model';
import { environment } from '../../environments/environment';

function makeCamping(id: string, latitude: number, longitude: number): Camping {
  return {
    id,
    name: `Camping ${id}`,
    description: '',
    latitude,
    longitude,
    pricePerNight: 20,
    ownerId: 'owner',
    categoryId: '00000000-0000-0000-0000-000000000000',
    facilityIds: [],
    additionalCategoryIds: [],
    createdOn: '',
    updatedOn: ''
  };
}

describe('CampingsService.getForMap', () => {
  let service: CampingsService;
  let httpMock: HttpTestingController;
  const base = `${environment.apiUrl}/campings`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(CampingsService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should discard campings without valid coordinates', () => {
    const items: Camping[] = [
      makeCamping('valid', 40.4, -3.7),
      makeCamping('zero', 0, 0),
      makeCamping('outOfRange', 120, 200)
    ];

    let result: Camping[] = [];
    service.getForMap().subscribe(campings => (result = campings));

    const req = httpMock.expectOne(r => r.url === base);
    const response: PagedCampingsResponse = { items, totalCount: items.length, page: 1, pageSize: 100 };
    req.flush(response);

    expect(result.length).toBe(1);
    expect(result[0].id).toBe('valid');
  });

  it('should stop paginating when all pages are loaded', () => {
    let result: Camping[] = [];
    service.getForMap().subscribe(campings => (result = campings));

    const req = httpMock.expectOne(r => r.url === base);
    req.flush({ items: [makeCamping('a', 41, -3)], totalCount: 1, page: 1, pageSize: 100 } as PagedCampingsResponse);

    expect(result.length).toBe(1);
    httpMock.expectNone(r => r.url === base);
  });

  it('should query the search endpoint with the bounding box when bounds are provided', () => {
    let result: Camping[] = [];
    service.getForMap({ minLat: 40, maxLat: 41, minLng: -4, maxLng: -3 }).subscribe(campings => (result = campings));

    const req = httpMock.expectOne(r => r.url === `${base}/search`);
    expect(req.request.params.get('minLat')).toBe('40');
    expect(req.request.params.get('maxLat')).toBe('41');
    expect(req.request.params.get('minLng')).toBe('-4');
    expect(req.request.params.get('maxLng')).toBe('-3');
    req.flush({ items: [makeCamping('a', 40.5, -3.5)], totalCount: 1, page: 1, pageSize: 100 } as PagedCampingsResponse);

    expect(result.length).toBe(1);
  });
});
