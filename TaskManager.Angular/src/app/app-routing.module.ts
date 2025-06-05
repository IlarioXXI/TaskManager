import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { HomeComponent } from "./components/home/home.component";
import { ContattiComponent } from "./components/contatti/contatti.component";
import { ContattoComponent } from "./components/contatto/contatto.component";
import { NotfoundComponent } from "./components/notfound/notfound.component";
import { canActivate, canActivateChild } from "./auth/auth.guard";
import { SignupComponent } from "./components/signup/signup.component";
import { SigninComponent } from "./components/signin/signin.component";
import { TeamsComponent } from "./components/teams/teams.component";


const routes : Routes = [
    {
        path:'home',
        component:HomeComponent
    },
    {
        path:'',
        pathMatch:'full',
        redirectTo:'/homepage'
    },
    {
        path:'register',
        component:SignupComponent
    },
    {
        path:'login',
        component:SigninComponent
    },
    {

        path:'contatti',
        component:ContattiComponent,
        canActivate:[canActivate],
        canActivateChild:[canActivateChild],
        children:[
            {
                path:':id',
                component:ContattoComponent
            }
        ]
    },
    {
        path:'allTeams',
        component:TeamsComponent,
    },
    {
        path:'404',
        component:NotfoundComponent
    },
    {
        path:'**',
        redirectTo:'/404'
    }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule {}