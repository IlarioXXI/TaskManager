import { Component, Input } from '@angular/core';
import { Comment } from '../../models/comment.model';

@Component({
    selector: 'app-comment',
    templateUrl: './comment.component.html',
    styleUrl: './comment.component.css',
    standalone: true
})
export class CommentComponent {
  @Input() comment: any;
}
