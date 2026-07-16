import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';

import { MapPage } from './map.page';
import { CampingsService } from '../../services/campings.service';

describe('MapPage', () => {
  let component: MapPage;
  let fixture: ComponentFixture<MapPage>;

  const campingsServiceStub = {
    getForMap: () => of([])
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MapPage],
      providers: [
        provideRouter([]),
        { provide: CampingsService, useValue: campingsServiceStub }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MapPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should start with loading false', () => {
    expect(component.loading()).toBeFalse();
  });
});
