import {
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from '../auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler) {
    let accessToken = this.authService.getAccessToken();
    let authReq = req.clone();
    if (accessToken) {
      authReq = req.clone({
        headers: req.headers.set('Authorization', 'Bearer ' + accessToken),
      });
    }

    return next.handle(authReq);
  }
}
