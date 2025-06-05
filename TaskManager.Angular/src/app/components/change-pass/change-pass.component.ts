import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Form, NgForm } from '@angular/forms';
import { AppComponent } from '../../app.component';
import { AuthService } from '../../auth/auth.service';

@Component({
    selector: 'app-change-pass',
    templateUrl: './change-pass.component.html',
    styleUrl: './change-pass.component.css',
    standalone: false
})
export class ChangePassComponent {
  constructor(private authService : AuthService) { }

 @Input() show : boolean = false;
 @Output() close = new EventEmitter<void>();

  showForm(){
    this.show = true;
  }
  hideForm(){
    this.close.emit();
  }

  onSubmit(form : NgForm){
    if(this.authService.changePassword(form.value.currentPassword, form.value.newPassword, form.value.confirmNewPassword)){
      this.hideForm();
      alert('Password changed successfully');
    }
  };
}
