import { NgModule, provideBrowserGlobalErrorListeners } from "@angular/core";
import { AppComponent } from "./app.component";
import { BrowserModule } from "@angular/platform-browser";
import { AppRoutingModule } from './app-routing.module';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import {MatCardModule} from '@angular/material/card'
import {MatSliderModule} from '@angular/material/slider'
import {MatButtonModule} from '@angular/material/button';
import {MatInputModule} from '@angular/material/input';
import { FormsModule, NgSelectOption, ReactiveFormsModule } from "@angular/forms";
import { ContattiComponent } from "./components/contatti/contatti.component";
import { HomeComponent } from "./components/home/home.component";
import { ContattoComponent } from "./components/contatto/contatto.component";
import { NotfoundComponent } from "./components/notfound/notfound.component";
import {MatFormFieldModule, MatLabel} from '@angular/material/form-field';
import {MatSelectModule} from '@angular/material/select';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from "@angular/common/http";
import { SignupComponent } from "./components/signup/signup.component";
import { SigninComponent } from "./components/signin/signin.component";
import { authTokenInterceptor } from "./interceptors/auth-token.interceptor";
import { ChangePassComponent } from "./components/change-pass/change-pass.component";
import { CardTaskItemComponent } from "./components/card-task-item/card-task-item.component";
import { DetailsComponent } from "./components/details/details.component";
import { CommentComponent } from "./components/comment/comment.component";
import { TeamsComponent } from "./components/teams/teams.component";
import { TeamModalComponent } from "./components/team-modal/team-modal.component";
import { MatCheckboxModule } from '@angular/material/checkbox';



@NgModule({ declarations: [
        AppComponent,
        ContattiComponent,
        HomeComponent,
        ContattoComponent,
        NotfoundComponent,
        SignupComponent,
        SigninComponent,
        ChangePassComponent,
        CardTaskItemComponent,
        DetailsComponent,
        CommentComponent,
        TeamsComponent,
        TeamModalComponent,
    ],
    bootstrap: [AppComponent], 
    imports: [
        AppRoutingModule,
        MatSliderModule,
        MatCardModule,
        MatButtonModule,
        MatInputModule,
        FormsModule,
        MatFormFieldModule,
        MatCheckboxModule,
        MatSelectModule,
        ReactiveFormsModule], providers: [
        provideAnimationsAsync(),
        provideHttpClient(withInterceptorsFromDi()),
        {
            provide: HTTP_INTERCEPTORS,
            useClass: authTokenInterceptor,
            multi: true
        },
        provideHttpClient(withInterceptorsFromDi()),
        provideBrowserGlobalErrorListeners(),
    ] })
export class AppModule{}
