import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class HomeServiceService {
  users : any
  constructor(private http : HttpClient) { }

  getAllURL = 'https://localhost:7109/api/getAllUsers'

  getAllUsers(){
    this.users = this.http.get<string[]>(this.getAllURL)
    return this.users
  }

  insertPerson(url : string, data : {}){
    return this.http.post(url,data, {responseType : 'text'})
  }
}
