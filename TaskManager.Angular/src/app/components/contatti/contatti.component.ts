import { Component, OnInit } from '@angular/core';
import { ServizioProvaService } from '../../services/servizio-prova.service';
import { ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
    selector: 'app-contatti',
    templateUrl: './contatti.component.html',
    styleUrl: './contatti.component.css',
    standalone: false
})
export class ContattiComponent implements OnInit {

  persone : any
  isProfile : any
  constructor(private servizioProva : ServizioProvaService){}

  homeform : FormGroup

  
  ngOnInit(): void {
    this.persone = this.servizioProva.getPersone()
    this.homeform = new FormGroup({
          name : new FormControl('luca', Validators.required),
          email : new FormControl(null, [Validators.required, Validators.email]),
          color : new FormControl('red', Validators.required),
        })
  }

  onSubmit(){
    console.log(this.homeform)
  }
}
