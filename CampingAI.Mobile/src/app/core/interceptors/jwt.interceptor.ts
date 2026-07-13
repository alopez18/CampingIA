import { HttpInterceptorFn, HttpRequest, HttpHandlerFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { from, switchMap } from 'rxjs';
import { Preferences } from '@capacitor/preferences';

export const TOKEN_KEY = 'auth_token';

export const jwtInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn
) => {
  return from(Preferences.get({ key: TOKEN_KEY })).pipe(
    switchMap(({ value }) => {
      if (value) {
        const cloned = req.clone({
          setHeaders: { Authorization: `Bearer ${value}` }
        });
        return next(cloned);
      }
      return next(req);
    })
  );
};
