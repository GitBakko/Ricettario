import { Component, ChangeDetectionStrategy, signal, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class RegisterComponent {
  registerForm: FormGroup;
  isLoading = signal(false);
  errorMessage = signal('');

  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  constructor() {
    this.registerForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit() {
    if (this.registerForm.valid) {
      this.isLoading.set(true);
      this.errorMessage.set('');
      const { email, password } = this.registerForm.value;

      this.authService.register(email, password).subscribe({
        next: () => {
          this.authService.login(email, password).subscribe(() => {
             this.router.navigate(['/']);
          });
        },
        error: (err: any) => {
          this.isLoading.set(false);
          console.error('Registration error:', err);
          
          let msg = 'Errore durante la registrazione.';
          
          if (err.error?.errors) {
              const errors = [];
              for (const key in err.error.errors) {
                  errors.push(...err.error.errors[key]);
              }
              if (errors.length) msg = errors.join(' ');
          } else if (err.error && typeof err.error === 'string') {
              msg = err.error; 
          }

          this.errorMessage.set(msg);
        }
      });
    }
  }
}
