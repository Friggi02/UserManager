import { Component } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-change-password-dialog',
  templateUrl: './change-password-dialog.component.html',
  styleUrls: ['./change-password-dialog.component.css'],
})
export class ChangePasswordDialogComponent {
  constructor(
    public dialog: MatDialog,
    public authService: AuthService,
    public router: Router,
    public alert: ToastrService
  ) {}

  changePasswordForm: FormGroup = new FormGroup({});

  ngOnInit() {
    this.changePasswordForm = new FormGroup({
      currentPassword: new FormControl('', [
        Validators.required,
        Validators.minLength(8),
      ]),
      newPassword: new FormControl('', [
        Validators.required,
        Validators.minLength(8),
      ]),
      passwordCheck: new FormControl('', [
        Validators.required,
        Validators.minLength(8),
      ]),
    });

    const passwordCheckControl = this.changePasswordForm.get('passwordCheck');

    if (passwordCheckControl) {
      passwordCheckControl.setValidators([
        Validators.required,
        Validators.minLength(8),
        this.passwordMatchValidator(),
      ]);
    }
  }

  passwordMatchValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
      const passwordControl = this.changePasswordForm.get('newPassword');
      if (!passwordControl) {
        return null;
      }
      const password = passwordControl.value;
      const passwordCheck = control.value;
      return password !== passwordCheck ? { passwordMismatch: true } : null;
    };
  }

  onSave() {
    console.log("aaaaaa");
    this.authService.changePassword(
      this.changePasswordForm.get('currentPassword')?.value,
      this.changePasswordForm.get('newPassword')?.value
    ).subscribe({
      next:()=>{
        this.dialog.closeAll();
        this.alert.success('Your password has been successfully changed', 'Success');
      },
      error:(response)=>{
        this.alert.error(response.error, 'Password change failed');
      }
    });
  }

  // #region validation error

  getCurrentPasswordErrorMessage() {
    let field = this.changePasswordForm.get('currentPassword');

    if (field?.touched) {
      if (field?.hasError('required')) {
        return 'You must enter a value';
      }

      if (field?.hasError('minlength')) {
        return 'Too short';
      }
    }

    return '';
  }

  getNewPasswordErrorMessage() {
    let field = this.changePasswordForm.get('newPassword');

    if (field?.touched) {
      if (field?.hasError('required')) {
        return 'You must enter a value';
      }

      if (field?.hasError('minlength')) {
        return 'Too short';
      }
    }

    return '';
  }

  getPasswordCheckErrorMessage() {
    let field = this.changePasswordForm.get('passwordCheck');

    if (field?.touched) {
      if (field?.hasError('required')) {
        return 'You must enter a value';
      }

      if (field?.hasError('minlength')) {
        return 'Too short';
      }

      if (field?.hasError('passwordMismatch')) {
        return 'Passwords do not match';
      }
    }

    return '';
  }

  // #endregion
}
