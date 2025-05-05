import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { ServizioProvaService } from '../../services/servizio-prova.service';

@Component({
  selector: 'app-contatto',
  standalone: false,
  templateUrl: './contatto.component.html',
  styleUrl: './contatto.component.css'
})
export class ContattoComponent implements OnInit {
  id : number 
  persona : any
  

  constructor(private route : ActivatedRoute, private servizioProva : ServizioProvaService) {}

  ngOnInit(): void {
    this.id = parseInt(this.route.snapshot.paramMap.get('id'));
    this.route.paramMap.subscribe((params : ParamMap)=>{
      this.id = parseInt(params.get('id'))
      this.persona = this.servizioProva.getPersona(this.id)
    })
  }


}
