import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { DeleteProfileDialogComponent } from '../delete-profile-dialog/delete-profile-dialog.component';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css'],
})
export class ProfileComponent {
  
  constructor(
    private alert: ToastrService,
    public dialog: MatDialog
  ) {}

  onDelete(){
    this.dialog.open(DeleteProfileDialogComponent);
  }
}
