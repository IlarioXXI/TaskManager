import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Team } from '../models/team.model';
import { User } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class TeamService {

  constructor(private http : HttpClient) { }

  getAllTeamsURL = "https://localhost:7109/api/team/getall";
  upsertTeamURL = "https://localhost:7109/api/Team/upsert";

  getAllTeams() {
    return this.http.get<Team[]>(this.getAllTeamsURL);
  }
  upsertTeam(id: number, name: string,usersIds?: string[], taskItemsIds?: number[]) {
    return this.http.post<Team>(this.upsertTeamURL, { id: id, name: name, usersIds: usersIds, taskItemsIds: taskItemsIds });
  }
}
