import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { from, map } from 'rxjs';
import { Preferences } from '@capacitor/preferences';
import { TOKEN_KEY } from '../interceptors/jwt.interceptor';

export const authGuard: CanActivateFn = () => {
  const router = inject(Router);
  return from(Preferences.get({ key: TOKEN_KEY })).pipe(
    map(({ value }) => {
      if (value) return true;
      router.navigate(['/auth/login']);
      return false;
    })
  );
};
