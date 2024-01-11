import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { DeleteProfileDialogComponent } from '../delete-profile-dialog/delete-profile-dialog.component';
import { ChangePasswordDialogComponent } from '../change-password-dialog/change-password-dialog.component';
import { AuthService } from 'src/app/services/auth.service';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { UserModel } from 'src/app/models/user/User.model';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css'],
})
export class ProfileComponent {
  
  constructor(
    public dialog: MatDialog,
    public authService: AuthService,
    public router: Router,
    public alert: ToastrService
  ){}

  ngOnInit() {
    this.toggleFormFields();
    document.getElementById("cancel")!.setAttribute("hidden", "true");
    document.getElementById("save")!.setAttribute("hidden", "true");

    this.authService.selfGet().subscribe({
      next:(response)=>{
        this.authService.user = response;
        this.userDetailForm.get('email')?.setValue(this.authService.user!.email);
        this.userDetailForm.get('name')?.setValue(this.authService.user!.name);
        this.userDetailForm.get('surname')?.setValue(this.authService.user!.surname);
        this.userDetailForm.get('username')?.setValue(this.authService.user!.username);
      },
      error:(response)=>{
        this.alert.error(response.error, 'Error getting personal info');
      }
    });

    
  }

  onDelete() {
    this.dialog.open(DeleteProfileDialogComponent);
  }

  onChangePassword() {
    this.dialog.open(ChangePasswordDialogComponent);
  }

  userDetailForm: FormGroup = new FormGroup({
    name: new FormControl('', [
      Validators.required
    ]),
    surname: new FormControl('', [
      Validators.required
    ]),
    username: new FormControl('', [
      Validators.required
    ]),
    email: new FormControl('', [
      Validators.required,
      Validators.email
    ])
  });

  onSave() {
    console.log("save");
  }

  onEdit(){
    this.toggleFormFields();
    document.getElementById("cancel")!.removeAttribute("hidden");
    document.getElementById("save")!.removeAttribute("hidden");
    document.getElementById("edit")!.setAttribute("hidden", "true");
  }

  onCancel(){
    this.toggleFormFields();
    document.getElementById("cancel")!.setAttribute("hidden", "true");
    document.getElementById("save")!.setAttribute("hidden", "true");
    document.getElementById("edit")!.removeAttribute("hidden");
  }

  toggleFormFields() {
    Object.keys(this.userDetailForm.controls).forEach(key => {
      const control = this.userDetailForm.get(key)!;
      control.enabled ? control.disable() : control.enable();
    });
  }

  // #region validation error

  getNameErrorMessage() {
    let field = this.userDetailForm.get('name');

    if (field?.touched) {
      if (field?.hasError('required')) {
        return 'You must enter a value';
      }
    }

    return '';
  }

  getSurnameErrorMessage() {
    let field = this.userDetailForm.get('surname');

    if (field?.touched) {
      if (field?.hasError('required')) {
        return 'You must enter a value';
      }
    }

    return '';
  }

  getUsernameErrorMessage() {
    let field = this.userDetailForm.get('username');

    if (field?.touched) {
      if (field?.hasError('required')) {
        return 'You must enter a value';
      }
    }

    return '';
  }

  getEmailErrorMessage() {
    let field = this.userDetailForm.get('email');

    if (field?.touched) {
      if (field?.hasError('required')) {
        return 'You must enter a value';
      }

      if (field?.hasError('email')) {
        return 'Not a valid email';
      }
    }

    return '';
  }

  // #endregion
}