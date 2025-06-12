import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormControl, FormGroup,ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../auth/auth.service';

@Component({
    selector: 'app-change-pass',
    templateUrl: './change-pass.component.html',
    styleUrl: './change-pass.component.css',
    standalone: true,
    imports:[ReactiveFormsModule]
})
export class ChangePassComponent {
  constructor(private authService : AuthService) { }

 changePassform = new FormGroup({
    currentPassword: new FormControl(''),
    newPassword: new FormControl(''),
    confirmNewPassword: new FormControl('')
  });

 @Input() show : boolean = false;
 @Output() close = new EventEmitter<void>();

  showForm(){
    this.show = true;
  }
  hideForm(){
    this.close.emit();
  }

  onSubmit() {
  this.authService.changePassword(
    this.changePassform.value.currentPassword,
    this.changePassform.value.newPassword,
    this.changePassform.value.confirmNewPassword
  ).subscribe({
    next: (response) => {
      console.log('Password changed successfully:', response);
      this.hideForm();
      alert('Password changed successfully');
    },
    error: (err) => {
      console.error('Error changing password:', err);
    }
  });
}
}
