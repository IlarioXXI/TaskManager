import { Component, EventEmitter, Input, OnChanges, OnInit, Output } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import Swal from 'sweetalert2';
import { TaskItem } from '../../models/taskItem.model';
import { TaskItemService } from '../../services/task-item.service';
import { CommentComponent } from '../comment/comment.component';

@Component({
  selector: 'app-details',
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.css'],
  standalone: true,
  imports: [ReactiveFormsModule,CommentComponent]
})
export class DetailsComponent implements OnInit, OnChanges {

  id: number;
  statusList: string[] = ['not started', 'in progress', 'completed'];
  priorityList: string[] = ['high', 'medium', 'low'];

 taskItemCopy: TaskItem;

  

  @Input() show: boolean = false;
  @Input() taskItem: TaskItem;
  @Input() teamId: number | null = null;
  @Output() close = new EventEmitter<void>();

  

  constructor(
    private route: ActivatedRoute,
    private taskItemService: TaskItemService
  ) {}

  ngOnInit(): void {
    this.initTaskItemCopy();
  }

  ngOnChanges() {
    this.initTaskItemCopy();
  }

  closeModal() {
    this.close.emit();
  }

  taskForm = new FormGroup({
    title: new FormControl(''),
    description: new FormControl(''),
    status: new FormControl('not started'),
    priority: new FormControl('low'),
    dueDate: new FormControl<string>(null),
    newComment: new FormControl<string>('')
  });


  private formatDateInput(date  : Date | string): string{
    const d = new Date(date);
    const year = d.getFullYear();
    const month = String(d.getMonth()+1).padStart(2,'0');
    const day = String(d.getDate()).padStart(2,'0');
    return `${year}-${month}-${day}`
  }

 private initTaskItemCopy() {
  if (this.taskItem) {
    this.taskItemCopy = JSON.parse(JSON.stringify(this.taskItem));

    if (this.taskItemCopy.dueDate) {
      this.formatDateInput(this.taskItemCopy.dueDate)
    }

    this.taskForm.patchValue({
      title: this.taskItemCopy.title || '',
      description: this.taskItemCopy.description || '',
      status: this.taskItemCopy.statusName || 'not started',
      priority: this.taskItemCopy.priorityName || 'low',
      dueDate: this.taskItemCopy.dueDate ? this.formatDateInput(this.taskItemCopy.dueDate) : null,
      newComment: ''
    });

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

    this.taskForm.reset({
      title: '',
      description: '',
      status: 'not started',
      priority: 'low',
      dueDate: null,
      newComment: ''
    });
  }
}


  onSubmit() {
    const form = this.taskForm;
    const dueDateString = this.taskForm.value.dueDate;
    const dueDateObj = new Date(dueDateString) ;
    const statusValue = form.value.status;
    const priorityValue = form.value.priority;

    if (statusValue === 'not started') this.taskItemCopy.statusId = 1;
    else if (statusValue === 'in progress') this.taskItemCopy.statusId = 2;
    else if (statusValue === 'completed') this.taskItemCopy.statusId = 3;

    if (priorityValue === 'low') this.taskItemCopy.priorityId = 1;
    else if (priorityValue === 'medium') this.taskItemCopy.priorityId = 2;
    else if (priorityValue === 'high') this.taskItemCopy.priorityId = 3;

    this.taskItemService.updateTaskItem(
      this.taskItemCopy.id,
      this.taskItemCopy.teamId,
      this.taskItemCopy.statusId,
      this.taskItemCopy.priorityId,
      form.value.title,
      form.value.description,
      dueDateObj,
      this.taskItemCopy.appUserId
    ).subscribe({
      next: () => {
        window.location.reload();
        this.closeModal();
      },
      error: () => {
        alert("Errore durante l'aggiornamento");
      }
    });
  }

  addComment() {
    const comment = this.taskForm.value.newComment;
    if (!comment || comment.trim() === '') return;

    this.taskItemService.addComment(0, comment, this.taskItem.id)
      .subscribe({
        next: () => {
          window.location.reload();
        },
        error: () => {
          alert("Errore nell'aggiunta del commento");
        }
      });
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
            Swal.fire('Errore', 'Errore durante l\'eliminazione.', 'error');
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
            Swal.fire('Errore', 'Errore durante l\'eliminazione.', 'error');
          }
        });
      }
    });
  }
}