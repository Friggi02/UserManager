import { UserModel } from "../user/User.model";

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  user: UserModel;
}