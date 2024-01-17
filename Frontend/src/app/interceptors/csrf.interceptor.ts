import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';


export const CsrfInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService)

  console.log(authService.XSRF_Token)
  const csrfToken = authService.XSRF_Token;

  if(csrfToken != undefined) {
    const csrfReq = req.clone({
      headers: req.headers.append('XSRF-Token', csrfToken)
    });
    console.log(csrfReq)
    return next(csrfReq)
  }

  return next(req);
};
