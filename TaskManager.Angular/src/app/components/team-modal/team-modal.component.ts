import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Team } from '../../models/team.model';
import { User } from '../../models/user.model';
import { UserService } from '../../services/user.service';
import { TeamService } from '../../services/team.service';


@Component({
    selector: 'app-team-modal',
    templateUrl: './team-modal.component.html',
    styleUrl: './team-modal.component.css',
    standalone: false
})
export class TeamModalComponent implements OnInit {
  constructor(private userService : UserService, private teamService : TeamService) {}

  @Input() team: Team = { id: 0, name: '', users: [], taskItems: [], taskItemsIds: [], usersIds: [] }; 
  @Input() show: boolean = false;
  @Input() selectedUserIds: string[] = [];
  @Output() close = new EventEmitter<void>();


  users : User[] = [];

  ngOnInit() {
    this.selectedUserIds = [];
    this.userService.getAllUsers().subscribe((users: User[]) => {
      console.log('Utenti caricati dal servizio:', users);
      this.users = users;
      console.log('Utenti caricati:', this.users);
      if (this.team && this.team.users) {
        this.selectedUserIds = this.team.users.map(user => user.id);
      }
    });
  }

  onSubmit(form: any) {
    this.team.usersIds = this.selectedUserIds;  
    console.log('User IDs inviati:', this.team.usersIds);
    this.teamService.upsertTeam(
      this.team.id,
      form.value.name,
      this.selectedUserIds,
      this.team.taskItemsIds).subscribe({
      next: (team: Team) => {
        this.team = team;
        this.close.emit();
        window.location.reload();
      }
    });
  }
}
