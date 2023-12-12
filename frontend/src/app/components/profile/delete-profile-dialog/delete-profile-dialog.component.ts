import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-delete-profile-dialog',
  templateUrl: './delete-profile-dialog.component.html',
  styleUrls: ['./delete-profile-dialog.component.css'],
})
export class DeleteProfileDialogComponent {
  
  constructor(
    public dialog: MatDialog,
    public authService: AuthService,
    public router: Router,
    public alert: ToastrService
  ) {}

  onDelete() {
    this.dialog.closeAll();
    this.authService.selfDelete().subscribe({
      next:(response)=>{
        this.authService.removeAccessToken();
        this.authService.removeRefreshToken();
        this.router.navigate(['/']);
      },
      error:(e)=>{
        this.alert.error(e.error, 'Delete account failed');
      }
    })
  }
}