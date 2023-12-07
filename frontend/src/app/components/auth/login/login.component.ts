import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { LoginModel } from 'src/app/models/auth/Login.model';
import { LoginResponse } from 'src/app/models/auth/LoginResponse.model';
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
){}

  public loginForm: FormGroup = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [
      Validators.required,
      Validators.minLength(6),
    ]),
  });

  getEmailErrorMessage() {
    let emailControl = this.loginForm.get('email');

    if (emailControl?.touched) {
      if (emailControl?.hasError('required')) {
        return 'You must enter a value';
      }

      if (emailControl?.hasError('email')) {
        return 'Not a valid email';
      }
    }

    return '';
  }

  getPasswordErrorMessage() {
    let passwordControl = this.loginForm.get('password');

    if (passwordControl?.touched) {
      if (passwordControl?.hasError('required')) {
        return 'You must enter a value';
      }

      if (passwordControl?.hasError('minlength')) {
        return 'Too short';
      }
    }

    return '';
  }

  onLogin() {
    
    let x = new LoginModel(this.loginForm.get('email')?.value, this.loginForm.get('password')?.value);

    this.authService.login(x).subscribe({
      next:(response)=>{
        this.authService.setAccessToken(response.accessToken);
        this.authService.setRefreshToken(response.refreshToken);
        this.router.navigate(["/"]);
      },
      error:(e)=>{
        this.alert.error(
          e.error,
          'Login failed'
        );
      }
    });
  }
}
