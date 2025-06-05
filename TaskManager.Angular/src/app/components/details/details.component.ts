import { Component, EventEmitter, Input, OnChanges, OnInit, Output } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { TaskItem } from '../../models/taskItem.model';
import { TaskItemService } from '../../services/task-item.service';
import { NgForm } from '@angular/forms';
import Swal from 'sweetalert2';
import { Team } from '../../models/team.model';

@Component({
  selector: 'app-details',
  templateUrl: './details.component.html',
  styleUrl: './details.component.css'
})
export class DetailsComponent implements OnInit, OnChanges {

  id : number
  statusList : string[] = ['not started', 'in progress', 'completed'];
  priorityList : string[] = ['high', 'medium', 'low'];
  newComment : string = "";
  

  constructor(private route : ActivatedRoute, private taskItemService : TaskItemService ){}

  @Input() show : boolean = false;
  @Input() taskItem : TaskItem;
  @Input() teamId: number | null = null;
  @Output() close = new EventEmitter<void>();

  taskItemCopy: TaskItem;
  

  closeModal(){
    this.close.emit();
  }
  ngOnInit(): void {
    this.initTaskItemCopy();
  }

  ngOnChanges() {
    this.initTaskItemCopy();
  }

  private initTaskItemCopy() {
    if (this.taskItem) {
      this.taskItemCopy = JSON.parse(JSON.stringify(this.taskItem));
      if (this.taskItemCopy.dueDate) {
        const dateObj = new Date(this.taskItemCopy.dueDate);
        const year = dateObj.getFullYear();
        const month = String(dateObj.getMonth() + 1).padStart(2, '0');
        const day = String(dateObj.getDate()).padStart(2, '0');
        this.taskItemCopy.dueDateString = `${year}-${month}-${day}`;
      }
    } else {
      this.taskItemCopy = {
        id: 0,
        teamId: this.teamId ?? 0,
        teamName: '',
        statusId: 1,
        statusName: 'not started',
        priorityId: 1,
        priorityName: 'low',
        title: '',
        description: '',
        comments: [],
        dueDate: new Date(),
        appUserId: '',
        color: '',
        dueDateString: '',
      };
    }
  }

  onSubmit(form : NgForm){
    if(form.value.statusName == "not started"){
      this.taskItemCopy.statusId = 1;
    }else if(form.value.status == "in progress"){
      this.taskItemCopy.statusId = 2;
    }else if(form.value.status == "completed"){
      this.taskItemCopy.statusId = 3;
    }
    if(form.value.priority == "low"){
      this.taskItemCopy.priorityId = 1;
    }else if(form.value.priority == "medium"){
      this.taskItemCopy.priorityId = 2;
    }else if(form.value.priority == "high"){
      this.taskItemCopy.priorityId = 3;
    }
    const item = this.taskItemCopy;
    this.taskItemService.updateTaskItem(
    item.id,
    item.teamId,
    item.statusId,
    item.priorityId,
    form.value.title,
    form.value.description,
    form.value.dueDate,
    item.appUserId
  ).subscribe({
    next: () => {
      window.location.reload();
      this.closeModal();
    },
    error: () => {
      alert('Errore durante l\'aggiornamento');
    }
  });
 }

  addComment(form : NgForm){
    this.newComment = form.value.newComment;
    this.taskItemService.addComment(0, this.newComment, this.taskItem.id)
      .subscribe({
        next: () => {
        window.location.reload();
      }
    })
  }

  deleteComment(commentId: number) {
  Swal.fire({
    title: 'Sei sicuro?',
    text: 'Vuoi davvero eliminare questo commento?',
    icon: 'warning',
    showCancelButton: true,
    confirmButtonText: 'Sì, elimina',
    cancelButtonText: 'Annulla'
  }).then((result) => {
    if (result.isConfirmed) {
      this.taskItemService.deleteComment(commentId).subscribe({
        next: () => {
          window.location.reload();
        },
        error: () => {
          Swal.fire('Errore', 'Si è verificato un errore durante l\'eliminazione.', 'error');
        }
      });
    }
  });
}

  deleteTask() {
    Swal.fire({
    title: 'Sei sicuro?',
    text: 'Vuoi davvero eliminare questo task?',
    icon: 'warning',
    showCancelButton: true,
    confirmButtonText: 'Sì, elimina',
    cancelButtonText: 'Annulla'
  }).then((result) => {
    if (result.isConfirmed) {
      this.taskItemService.deleteTaskItem(this.taskItem.id).subscribe({
        next: () => {
          window.location.reload();
        },
        error: () => {
          Swal.fire('Errore', 'Si è verificato un errore durante l\'eliminazione.', 'error');
        }
      });
    }
  });
  }
}
