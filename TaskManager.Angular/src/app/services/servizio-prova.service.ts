import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ServizioProvaService {

  persone = [
    {name:'luca',surname:'rossi',isOnline:true},
    {name:'daivde',surname:'verdi',isOnline:true},
    {name:'gesu',surname:'bambino',isOnline:false}
  ]
  constructor() { }

  getPersone(){
    return this.persone
  }

  getPersona(index : number){
    return this.persone[index]
  }
}
