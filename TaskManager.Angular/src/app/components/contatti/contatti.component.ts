import { Component, OnInit } from '@angular/core';
import { ServizioProvaService } from '../../services/servizio-prova.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-contatti',
  templateUrl: './contatti.component.html',
  styleUrl: './contatti.component.css'
})
export class ContattiComponent implements OnInit {

  persone : any
  isProfile : any
  constructor(private servizioProva : ServizioProvaService){}
  
  ngOnInit(): void {
    this.persone = this.servizioProva.getPersone()
    
  }
}
