import { NgFor } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, NgForm } from '@angular/forms';
import { interval, Observable, Subscription } from 'rxjs';
import { HomeServiceService } from '../../services/home-service.service';
import { AuthService } from '../../auth/auth.service';
import { Route, Router } from '@angular/router';
import { TaskItem } from '../../models/taskItem.model';
import { TaskItemService } from '../../services/task-item.service';


@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrl: './home.component.css',
    standalone: false
})
export class HomeComponent implements OnInit{

  constructor(private router : Router, private taskItemService : TaskItemService){}

  taskItems : TaskItem[] = [];
  selectedTask : TaskItem;
  taskItemCopy: any;
  show : boolean = false;

  
  ngOnInit(): void {
    this.taskItemService.getAllTaskItems().subscribe(items => {
    this.taskItems = items;
    for (let i = 0; i < this.taskItems.length; i++) {
      if(this.taskItems[i].priorityName == "high"){
        this.taskItems[i].color = "danger";
      }
      if(this.taskItems[i].priorityName == "medium"){
        this.taskItems[i].color = "warning";
      }
      if(this.taskItems[i].priorityName == "low"){
        this.taskItems[i].color = "success";
      }
    }
    console.log(this.taskItems);
  });

  }

  showModal(item : any){
    this.selectedTask =  JSON.parse(JSON.stringify(item));
    this.show = true;
  }
  
  closeModal(){
    this.show = false;
  }

}
