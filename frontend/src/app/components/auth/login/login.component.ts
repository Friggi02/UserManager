import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { LoginModel } from 'src/app/models/auth/Login.model';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {

  constructor(
    private authService: AuthService,
    private alert: ToastrService,
    private router: Router
  ){
    this.loginForm.get('email')?.setValue(this.authService.user);
  }

  public loginForm: FormGroup = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(8)])
  });

  // #region validation error
  getEmailErrorMessage() {
    let field = this.loginForm.get('email');

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

  getPasswordErrorMessage() {
    let field = this.loginForm.get('password');

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

  // #endregion

  onLogin() {
    
    let x = new LoginModel(
      this.loginForm.get('email')?.value,
      this.loginForm.get('password')?.value
    );

    this.authService.login(x).subscribe({
      next:(response)=>{
        this.authService.setAccessToken(response.accessToken);
        this.authService.setRefreshToken(response.refreshToken);
        this.router.navigate(["/"]);
      },
      error:(e)=>{
        this.alert.error(e.error, 'Login failed');
      }
    });
  }
}
