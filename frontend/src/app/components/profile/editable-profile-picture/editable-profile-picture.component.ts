import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ChangeProfilePictureDialogComponent } from '../change-profile-picture-dialog/change-profile-picture-dialog.component';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-editable-profile-picture',
  templateUrl: './editable-profile-picture.component.html',
  styleUrls: ['./editable-profile-picture.component.css']
})
export class EditableProfilePictureComponent {

  profilePic: boolean = false;

  constructor(
    public dialog: MatDialog,
    public authService: AuthService,
  ) {
    this.profilePic = this.authService.getProfilePic() !== null;
  }
  
  onEditPic(){
    this.dialog.open(ChangeProfilePictureDialogComponent);
  }

  loadFile(event: any){
    let image: HTMLElement = document.getElementById("output")!;
    console.log(URL.createObjectURL(event.target.files[0]));
  }
}