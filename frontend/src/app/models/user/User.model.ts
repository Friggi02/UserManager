export class UserModel {
  id: string;
  email: string;
  username: string;
  isDeleted: boolean;
  name: string | null;
  surname: string | null;
  profilePic: string | null = null;
  roles: string[];

  constructor(id: string, email: string, username: string, isDeleted: boolean, name: string, surname: string, profilePic: string, roles: string[]) {
      this.id = id;
      this.email = email;
      this.username = username;
      this.isDeleted = isDeleted;
      this.name = name;
      this.surname = surname;
      this.profilePic = profilePic;
      this.roles = roles;
  }
}