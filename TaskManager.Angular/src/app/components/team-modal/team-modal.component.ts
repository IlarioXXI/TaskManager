import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Team } from '../../models/team.model';
import { User } from '../../models/user.model';
import { UserService } from '../../services/user.service';
import { TeamService } from '../../services/team.service';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';


@Component({
    selector: 'app-team-modal',
    templateUrl: './team-modal.component.html',
    styleUrl: './team-modal.component.css',
    standalone: true,
    imports: [ReactiveFormsModule]
})
export class TeamModalComponent implements OnInit {
  constructor(private userService : UserService, private teamService : TeamService) {}

  @Input() team: Team = { id: 0, name: '', users: [], taskItems: [], taskItemsIds: [], usersIds: [] }; 
  @Input() show: boolean = false;
  @Input() selectedUserIds: string[] = [];
  @Output() close = new EventEmitter<void>();

  teamForm = new FormGroup({
    name : new FormControl(''),
    usersIds : new FormControl<string[]> ([])
  });

  users : User[] = [];

  ngOnInit() {
    this.userService.getAllUsers().subscribe((users: User[]) => {
      this.users = users;
      if (this.team && this.team.users) {
        this.selectedUserIds = this.team.users.map(user => user.id);
      }
    });
    this.teamForm.patchValue({
      name : this.team.name,
      usersIds : this.selectedUserIds
    })
  }

  onSubmit() {
    this.selectedUserIds = this.teamForm.value.usersIds ;  
    this.teamService.upsertTeam(
      this.team.id,
      this.teamForm.value.name,
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
