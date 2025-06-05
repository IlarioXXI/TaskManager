import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from '../models/user.model';


@Injectable({
  providedIn: 'root'
})
export class AuthService {

  isAdmin = true;

  user : User

  singInURL = 'https://localhost:7109/api/auth'
  signUpURL = 'https://localhost:7109/api/register'
  getEmailURL = 'https://localhost:7109/api/getEmail'
  changePasswordURL = 'https://localhost:7109/api/changePassword'

  constructor(private http : HttpClient) { }

  isRoleAdmin(){
    if(this.user.role == 'Admin'){
      return true;
    }else{
      return false;
    }
  }

  signIn(email : string, password:string){
    return this.http.post(this.singInURL, {email:email, password:password}, {responseType:'text'})
  }

  singUp(email : string, password : string, confirmPassword : string, role : string){
    return this.http.post(this.signUpURL,{email : email, password : password, confirmPassword : confirmPassword, role : role}, {responseType : 'text'})
  }

  isTokenExpired(token: string) {
    if (token == null) {
      return null;
    }
    const expiry = (JSON.parse(atob(token.split('.')[1]))).exp;
    return (Math.floor((new Date).getTime() / 1000)) >= expiry;
  }
  

  createUser(token : string) {
    if (token == null) {
      return null;
    }else{
      const userFromToken = JSON.parse(atob(token.split('.')[1]));
      this.user = new User('', '', token, userFromToken.role);
      return this.user; 
    }
  }

  getEmail(){
    return this.http.get(this.getEmailURL, {responseType:'text'})
  }

  changePassword(currentPassword : string, newPassword : string, confirmNewPassword : string){
    return this.http.post(this.changePasswordURL,{currentPassword : currentPassword, newPassword : newPassword, confirmNewPassword : confirmNewPassword}, {responseType : 'text'} )
  }

}
