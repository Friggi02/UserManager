import { Component } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { RegisterModel } from 'src/app/models/auth/Register.model';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent {
  constructor(
    private authService: AuthService,
    private alert: ToastrService,
    private router: Router
  ) {}

  registerForm: FormGroup = new FormGroup({});

  ngOnInit() {
    this.registerForm = new FormGroup({
      username: new FormControl('', Validators.required),
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [
        Validators.required,
        Validators.minLength(8),
      ]),
      passwordCheck: new FormControl('', [
        Validators.required,
        Validators.minLength(8),
      ]),
    });

    const passwordCheckControl = this.registerForm.get('passwordCheck');
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
      const passwordControl = this.registerForm.get('password');
      if (!passwordControl) {
        return null;
      }
      const password = passwordControl.value;
      const passwordCheck = control.value;
      return password !== passwordCheck ? { passwordMismatch: true } : null;
    };
  }

  onRegister() {
    
    let x = new RegisterModel(
      this.registerForm.get('username')?.value,
      this.registerForm.get('email')?.value,
      this.registerForm.get('password')?.value
    );

    this.authService.registerUser(x).subscribe({
      next:(response)=>{
        this.authService.user = response;
        this.router.navigate(['login']);
        this.alert.success('User successfully registered', 'Welcome!');
      },
      error:(e)=>{
        this.alert.error(e.error, 'Register failed');
      }
    })
  }

  // #region validation error
  getUsernameErrorMessage() {
    let field = this.registerForm.get('email');

    if (field?.touched) {
      if (field?.hasError('required')) {
        return 'You must enter a value';
      }
    }

    return '';
  }

  getEmailErrorMessage() {
    let field = this.registerForm.get('email');

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
    let field = this.registerForm.get('password');

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
    let field = this.registerForm.get('passwordCheck');

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
