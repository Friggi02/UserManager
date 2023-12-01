import { Injectable } from '@angular/core';
import { Config } from '../config';
import { HttpClient } from '@angular/common/http';
import { LoginModel } from '../models/auth/Login.model';
import { RegisterModel } from '../models/auth/Register.model';
import { UserRoles } from '../models/user/User.roles';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private ACCESS_TOKEN_NAME = 'accessJwtToken';
  private REFRESH_TOKEN_NAME = 'refreshJwtToken';
  private BASE_URL = Config.BASE_URL;

  constructor(private http: HttpClient) {}

  // #region http requests

  login(user: LoginModel) {
    return this.http.post(`${this.BASE_URL}/api/user/login`, user);
  }

  registerUser(user: RegisterModel) {
    return this.http.post(`${this.BASE_URL}/api/user/registeruser`, user);
  }

  registerAdmin(admin: RegisterModel) {
    return this.http.post(`${this.BASE_URL}/api/user/registeradmin`, admin);
  }

  refreshToken() {
    //let refreshTokenModel = this.
  }

  selfGet() {
    return this.http.get(`${this.BASE_URL}/api/user/selfget`);
  }

  selfDelete() {
    return this.http.delete(`${this.BASE_URL}/api/user/selfdelete`);
  }

  changeEmail(newEmail: string) {
    return this.http.put(
      `${this.BASE_URL}/api/user/changeemail?newEmail=${newEmail}`,
      {}
    );
  }

  changePassword(currentPassword: string, newPassword: string) {
    return this.http.put(`${this.BASE_URL}/api/user/changepassword`, {
      currentPassword: currentPassword,
      newPassword: newPassword,
    });
  }

  // #endregion

  // #region reading token's info
  getUserIdFromJwtToken(): string | null {
    let accessToken = this.getAccessToken();
    if (accessToken) {
      let decodedToken: any = jwtDecode(accessToken);
      return decodedToken[
        'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'
      ];
    }
    return null;
  }

  getRolesFromJwtToken(): string[] {
    if (this.getAccessToken()) {
      let decodedToken: any = jwtDecode(this.getAccessToken()!);
      return decodedToken[
        'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
      ];
    }
    return [];
  }

  isAdmin() {
    if (this.getRolesFromJwtToken().includes(UserRoles.Admin)) return true;
    return false;
  }

  isUser() {
    if (this.getRolesFromJwtToken().includes(UserRoles.User)) return true;
    return false;
  }

  // #endregion

  // #region managing AccessToken in LocalStorage

  getAccessToken(): string | null {
    return localStorage.getItem(this.ACCESS_TOKEN_NAME);
  }

  setAccessToken(token: string) {
    localStorage.setItem(this.ACCESS_TOKEN_NAME, JSON.stringify(token));
  }

  removeAccessToken() {
    localStorage.removeItem(this.ACCESS_TOKEN_NAME);
  }

  // #endregion

  // #region managing RefreshToken in LocalStorage

  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_NAME);
  }

  setRefreshToken(token: string, expires: string, id: string) {
    localStorage.setItem(this.REFRESH_TOKEN_NAME, JSON.stringify(token));
  }

  removeRefreshToken() {
    localStorage.removeItem(this.REFRESH_TOKEN_NAME);
  }

  // #endregion
}
