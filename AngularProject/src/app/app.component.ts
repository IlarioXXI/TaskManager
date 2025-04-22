import { Component,OnInit } from '@angular/core';
import { PostService } from './services/post.service';
import { Post } from './models/post.model';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit{
  title = 'Gianpy Rock';

  constructor(private httpClient : HttpClient, public postService:PostService){}

  ngOnInit(): void {
    this.httpClient.get<Post[]>('https://localhost:7109/api/history/getAll/5')
      .subscribe(result => {
        this.postService.allPosts = result;
        console.log(this.postService.allPosts);
      })
  }
}
