import { NgModule } from "@angular/core";
import { AppComponent } from "./app.component";
import { BrowserModule } from "@angular/platform-browser";
import { AppRoutingModule } from './app-routing.module';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import {MatCardModule} from '@angular/material/card'
import {MatSliderModule} from '@angular/material/slider'
import {MatButtonModule} from '@angular/material/button';
import {MatInputModule} from '@angular/material/input';
import { FormsModule } from "@angular/forms";
import { ContattiComponent } from "./components/contatti/contatti.component";
import { HomeComponent } from "./components/home/home.component";
import { ContattoComponent } from "./components/contatto/contatto.component";
import { NotfoundComponent } from "./components/notfound/notfound.component";


@NgModule({
    declarations:[
        AppComponent,
        ContattiComponent,
        HomeComponent,
        ContattoComponent,
        NotfoundComponent
    ],
    imports: [
    BrowserModule,
    AppRoutingModule,
    MatSliderModule,
    MatCardModule,
    MatButtonModule,
    MatInputModule,
    FormsModule,
],
    providers:[
    provideAnimationsAsync()
  ],
    bootstrap:[AppComponent]
})
export class AppModule {}