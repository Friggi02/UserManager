import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-change-profile-picture-dialog',
  templateUrl: './change-profile-picture-dialog.component.html',
  styleUrls: ['./change-profile-picture-dialog.component.css']
})
export class ChangeProfilePictureDialogComponent {
  constructor(
    public dialog: MatDialog,
    public authService: AuthService,
    public router: Router,
    public alert: ToastrService
  ) {}

  changeProfilePicForm: FormGroup = new FormGroup({
    link: new FormControl('', [
      Validators.required
    ])
  });

  onSave() {
    console.log(this.changeProfilePicForm.get('link')?.value);
  }

  // #region validation error

  getLinkErrorMessage() {
    let field = this.changeProfilePicForm.get('link');

    if (field?.touched) {
      if (field?.hasError('required')) {
        return 'You must enter a value';
      }
    }

    return '';
  }

  // #endregion
}
