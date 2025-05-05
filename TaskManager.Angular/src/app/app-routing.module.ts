import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { HomeComponent } from "./components/home/home.component";
import { ContattiComponent } from "./components/contatti/contatti.component";
import { ContattoComponent } from "./components/contatto/contatto.component";
import { NotfoundComponent } from "./components/notfound/notfound.component";
import { canActivate, canActivateChild } from "./auth/auth.guard";


const routes : Routes = [
    {
        path:'homepage',
        component:HomeComponent
    },
    {
        path:'',
        pathMatch:'full',
        redirectTo:'/homepage'
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