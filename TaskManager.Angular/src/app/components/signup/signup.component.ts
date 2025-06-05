import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { AuthService } from '../../auth/auth.service';
import { Router } from '@angular/router';

@Component({
    selector: 'app-signup',
    templateUrl: './signup.component.html',
    styleUrl: './signup.component.css',
    standalone: false
})
export class SignupComponent {

  constructor(private authService : AuthService, private router : Router){}

  onSubmit(form : NgForm){
    console.log(form);
    this.authService.singUp( 
        form.value.email,
        form.value.password,
        form.value.confirmPassword,
        form.value.role
    ).subscribe(token => {
      console.log(token)
      localStorage.setItem('token',token)
    })
    this.router.navigate(['/home']);
  }
}
