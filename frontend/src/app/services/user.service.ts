import { Injectable } from '@angular/core';
import { Config } from '../config';
import { HttpClient } from '@angular/common/http';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  
  private BASE_URL = Config.BASE_URL;

  constructor(private http: HttpClient, private authService: AuthService) {}

  getOData(query: string) {
    return this.http.get<any>(`${this.BASE_URL}/odata/user${query}`);
  }

  getSingleById(userId: string) {
    return this.http.get<any>(`${this.BASE_URL}/api/user/getsinglebyid?id=${userId}`);
  }

  restore(id: string) {
    return this.http.delete(`${this.BASE_URL}/api/user/restore?id=${id}`);
  }

  delete(id: string) {
    return this.http.delete(`${this.BASE_URL}/api/user/delete?id=${id}`);
  }

  userToAdmin(id: string) {
    return this.http.put(`${this.BASE_URL}/api/user/usertoadmin?id=${id}`, {});
  }

  adminToUser(id: string) {
    return this.http.put(`${this.BASE_URL}/api/user/admintouser?id=${id}`, {});
  }
}
