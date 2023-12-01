export class UserModel {
  constructor(
    public userId: string,
    public email: string,

    public personalImage: string,

    public name: string | null,
    public surname: string | null,
    public username: string | null,

    public birthDate: string | null,
    public cf: string | null,
    public fidelityCardNumber: string | null,

    public phoneNumberConfirmed: boolean,
    public accessFailedCount: any,
    public binaryFidelityNumber: any,
    public lockoutEnabled: any,
    public lockoutEnd: any,

    public isDeleted: boolean,
    public roles: string
  ) {}
}