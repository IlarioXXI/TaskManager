import { Component } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../auth/auth.service';
import { Router } from '@angular/router';
import { AppComponent } from '../../app.component';


@Component({
    selector: 'app-signin',
    templateUrl: './signin.component.html',
    styleUrl: './signin.component.css',
    standalone: true,
    imports: [ReactiveFormsModule,FormsModule]
})
export class SigninComponent {

 loginForm = new FormGroup({
    email: new FormControl(''),
    password: new FormControl('')
})

  constructor(private authService : AuthService, private router : Router, private appComponent : AppComponent){}

    ngOnInit(): void {
      
    }
  
    onSubmit(){
      this.authService.signIn(this.loginForm.value.email,this.loginForm.value.password)
      .subscribe(token => {
        console.log(token);
        localStorage.setItem('token',token);
        this.router.navigate(['/home']);
        this.appComponent.ngOnInit();
      })
      
      
    }
}
