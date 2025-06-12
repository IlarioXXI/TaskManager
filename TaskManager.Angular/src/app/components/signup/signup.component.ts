import { AuthService } from '../../auth/auth.service';
import { Router } from '@angular/router';
import {MatSelectModule} from '@angular/material/select';
import {MatInputModule} from '@angular/material/input';
import {MatFormFieldModule} from '@angular/material/form-field';
import { Component } from '@angular/core';
import { FormControl, FormGroup, FormsModule, NgForm, ReactiveFormsModule } from '@angular/forms';

@Component({
    selector: 'app-signup',
    templateUrl: './signup.component.html',
    styleUrl: './signup.component.css',
    standalone: true,
    imports: [MatFormFieldModule, MatInputModule, MatSelectModule,ReactiveFormsModule,FormsModule]
})
export class SignupComponent {

  constructor(private authService : AuthService, private router : Router){}

  registerForm = new FormGroup({
    email: new FormControl(''),
    password:new FormControl(''),
    confirmPassword: new FormControl(''),
    role: new FormControl('')
  });

  onSubmit(){
    this.authService.singUp( 
        this.registerForm.value.email,
        this.registerForm.value.password,
        this.registerForm.value.confirmPassword,
        this.registerForm.value.role
    ).subscribe(token => {
      console.log(token)
      localStorage.setItem('token',token)
    })
    this.router.navigate(['/home']);
  }
}
