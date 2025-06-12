import { Component, DoCheck, numberAttribute, OnChanges, OnInit, SimpleChanges} from "@angular/core";
import { AuthService } from "./auth/auth.service";
import { User } from "./models/user.model";
import { Team } from "./models/team.model";
import { ChangePassComponent } from "./components/change-pass/change-pass.component";
import { TeamModalComponent } from "./components/team-modal/team-modal.component";
import { Router, RouterModule } from "@angular/router";

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css'],
    standalone: true,
    imports: [ChangePassComponent,TeamModalComponent,RouterModule]
})
export class AppComponent implements OnInit {
  constructor(private authService : AuthService, private router : Router){}
  title = 'TaskManager';
  user : User
  isOpen : boolean = false;
  isOpenTeam : boolean = false;
  
    selectedTeam : Team = {
    id: 0, name: '', 
    users: [],
    taskItems: [],
    taskItemsIds: [],
    usersIds: []
  };


  ngOnInit(): void {
    if (localStorage.getItem('token') == null) {
          this.router.navigate(['/login']);
        }
    if (this.authService.isTokenExpired(localStorage.getItem('token'))) {
          console.log('token expired');
          localStorage.removeItem('token');
          this.router.navigate(['/login']);
        }
    else {
          this.user = this.authService.createUser(localStorage.getItem('token'));
          this.authService.getEmail().subscribe((res : string)=>{
            this.user.email = res;
          });
          console.log(this.user);
        }    
  }

  logout(){
    localStorage.removeItem('token');
    window.location.reload();
  }
  

  showModal(){
    this.isOpen = true;
  }
  
  closeModal(){
    this.isOpen = false;
  }

  showModalTeam(){
    this.selectedTeam = {
    id: 0,
    name: '',
    users: [],
    taskItems: [],
    taskItemsIds: [],
    usersIds: []
  };
  this.isOpenTeam = true;
  }
  closeModalTeam(){
    this.isOpenTeam = false;
  }
}

