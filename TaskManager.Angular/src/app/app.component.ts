import { Component, numberAttribute, OnInit} from "@angular/core";
import { ServizioProvaService } from "./services/servizio-prova.service";
import { interval, Observable } from "rxjs";

@Component({
  selector: 'app-root',
  templateUrl : './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  constructor(private servizioProva : ServizioProvaService){}
  title = 'TaskManager';

  ngOnInit(): void {
    // interval(1000).subscribe(numero => {
    //   console.log(numero)
    // })

    // new Observable(observer => {
    //   let count = 0;
    //   setInterval(() => {
    //     observer.next(count)
    //     count++
    //   }, 1000);
    // }).subscribe((numero) => {
    //   console.log(numero)
    // })
  }
  

}

