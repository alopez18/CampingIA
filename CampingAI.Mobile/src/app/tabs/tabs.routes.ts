import { Routes } from '@angular/router';
import { TabsPage } from './tabs.page';
import { authGuard } from '../core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'tabs',
    component: TabsPage,
    canActivate: [authGuard],
    children: [
      {
        path: 'home',
        loadComponent: () => import('../pages/home/home.page').then(m => m.HomePage)
      },
      {
        path: 'campings',
        loadComponent: () => import('../pages/campings/list/campings-list.page').then(m => m.CampingsListPage)
      },
      {
        path: 'campings/:id',
        loadComponent: () => import('../pages/campings/detail/camping-detail.page').then(m => m.CampingDetailPage)
      },
      {
        path: 'map',
        loadComponent: () => import('../pages/map/map.page').then(m => m.MapPage)
      },
      {
        path: 'favorites',
        loadComponent: () => import('../pages/favorites/favorites.page').then(m => m.FavoritesPage)
      },
      {
        path: 'reservations',
        loadComponent: () => import('../pages/reservations/reservations.page').then(m => m.ReservationsPage)
      },
      {
        path: 'reservations/new',
        loadComponent: () => import('../pages/reservations/new/new-reservation.page').then(m => m.NewReservationPage)
      },
      {
        path: 'profile',
        loadComponent: () => import('../pages/profile/profile.page').then(m => m.ProfilePage)
      },
      {
        path: '',
        redirectTo: '/tabs/home',
        pathMatch: 'full'
      }
    ]
  },
  {
    path: '',
    redirectTo: '/tabs/home',
    pathMatch: 'full'
  }
];

