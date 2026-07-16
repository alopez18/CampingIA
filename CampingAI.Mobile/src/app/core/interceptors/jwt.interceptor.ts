import { HttpErrorResponse, HttpInterceptorFn, HttpRequest, HttpHandlerFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { from, switchMap, catchError, throwError } from 'rxjs';
import { Preferences } from '@capacitor/preferences';

export const TOKEN_KEY = 'auth_token';

export const jwtInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn
) => {
  const router = inject(Router);

  return from(Preferences.get({ key: TOKEN_KEY })).pipe(
    switchMap(({ value }) => {
      const request = value
        ? req.clone({ setHeaders: { Authorization: `Bearer ${value}` } })
        : req;

      return next(request).pipe(
        catchError((error: HttpErrorResponse) => {
          if (error.status === 401) {
            // Token ausente/caducado/inválido: limpiamos la sesión y volvemos al login.
            return from(Preferences.remove({ key: TOKEN_KEY })).pipe(
              switchMap(() => from(router.navigate(['/auth/login']))),
              switchMap(() => throwError(() => error))
            );
          }
          return throwError(() => error);
        })
      );
    })
  );
};
