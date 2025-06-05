import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { AuthService } from '../../auth/auth.service';
import { Router } from '@angular/router';
import { AppComponent } from '../../app.component';

@Component({
  selector: 'app-signin',
  templateUrl: './signin.component.html',
  styleUrl: './signin.component.css'
})
export class SigninComponent {

  constructor(private authService : AuthService, private router : Router, private appComponent : AppComponent){}
  
    ngOnInit(): void {
      
    }
  
    onSubmit(form : NgForm){
      this.authService.signIn(form.value.email,form.value.password)
      .subscribe(token => {
        console.log(token);
        localStorage.setItem('token',token);
        this.router.navigate(['/home']);
        this.appComponent.ngOnInit();
      })
      
      
    }
}
