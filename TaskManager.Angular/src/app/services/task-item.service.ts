import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { TaskItem } from '../models/taskItem.model';

@Injectable({
  providedIn: 'root'
})
export class TaskItemService {

  constructor(private http : HttpClient) { }

  taskItems : TaskItem[];
  
  getAllURL = "https://localhost:7109/api/taskItem/getall"
  getByIdURL = "https://localhost:7109/api/taskItem/getbyid/"
  upsertURL = "https://localhost:7109/api/taskItem/upsert"
  addCommentURL = "https://localhost:7109/api/comment/upsert"
  deleteCommentURL = "https://localhost:7109/api/comment/delete/"
  deleteTaskURL = "https://localhost:7109/api/taskItem/delete/"

  
  getAllTaskItems() {
    return this.http.get<TaskItem[]>(this.getAllURL)
  }
  getTaskItemById(id : number) {
    return this.http.get<TaskItem>(this.getByIdURL + id)
  }
  updateTaskItem(id : number, teamId : number, statusId : number,priorityId : number, title : string, description : string, dueDate : Date, appUserId : string){
    return this.http.post(this.upsertURL,{id : id, teamId : teamId, statusId : statusId,priorityId : priorityId, title : title,description : description,dueDate : dueDate, appUserId : appUserId }, {responseType : 'text'} )
  }
  addComment(id : number, description : string, taskItemId : number){
    return this.http.post(this.addCommentURL, {id : id, description : description, taskItemId : taskItemId}, {responseType : 'text'})
  }
  deleteComment(id : number){
    return this.http.delete(this.deleteCommentURL + id, {responseType : 'text'})
  }
  deleteTaskItem(id : number){
    return this.http.delete(this.deleteTaskURL + id, {responseType : 'text'})
  }
}
