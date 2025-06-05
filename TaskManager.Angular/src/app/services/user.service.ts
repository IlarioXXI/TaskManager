import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private http:HttpClient) { }


  getAllUsersURL  = "https://localhost:7109/api/getAllUsers";

  getAllUsers() {
    return this.http.get<User[]>(this.getAllUsersURL);
  }
}
