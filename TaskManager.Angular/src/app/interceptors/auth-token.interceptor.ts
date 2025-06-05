import { HttpEvent, HttpHandler, HttpInterceptor, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';


@Injectable({
  providedIn : 'root',
})
export class authTokenInterceptor implements HttpInterceptor  {

  constructor() {}
  intercept(req: HttpRequest<any>, next: HttpHandler) {
    const token = localStorage.getItem('token')

    req = req.clone({
      url : req.url,
      setHeaders:{
        Authorization : `Bearer ${token}`
      }
    });

    return next.handle(req);
  }
};
