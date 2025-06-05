import { Component, OnInit } from '@angular/core';
import { TeamService } from '../../services/team.service';
import { TaskItem } from '../../models/taskItem.model';

@Component({
    selector: 'app-teams',
    templateUrl: './teams.component.html',
    styleUrl: './teams.component.css',
    standalone: false
})
export class TeamsComponent implements OnInit {

  teams: any[] = [];
  selectedTask : TaskItem = null;
  showDetails: boolean = false;
  selectedTeamId: number | null = null;
  
  constructor(private teamService : TeamService) { }

  ngOnInit(): void {
    this.teamService.getAllTeams().subscribe({
      next: (data) => {
        console.log(data);
        this.teams = data;
      }
    })
  }

  openDetails(task?: TaskItem, teamId?: number) {
    this.selectedTask = task || null;
    this.selectedTeamId = teamId || null;
    this.showDetails = true;
  }

  closeDetails() {
    this.showDetails = false;
    this.selectedTask = null;
  }
}
