import { Component, OnInit } from '@angular/core';
import { TeamService } from '../../services/team.service';
import { TaskItem } from '../../models/taskItem.model';
import { DetailsComponent } from '../details/details.component';
import { CommentComponent } from '../comment/comment.component';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-teams',
    templateUrl: './teams.component.html',
    styleUrl: './teams.component.css',
    standalone: true,
    imports: [DetailsComponent,CommentComponent]
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

  deleteTeam(id : number){
    Swal.fire({
          title: 'Sei sicuro?',
          text: 'Vuoi davvero eliminare questo team?',
          icon: 'warning',
          showCancelButton: true,
          confirmButtonText: 'Sì, elimina',
          cancelButtonText: 'Annulla'
        }).then((result) => {
          if (result.isConfirmed) {
            this.teamService.deleteTeam(id).subscribe({
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
