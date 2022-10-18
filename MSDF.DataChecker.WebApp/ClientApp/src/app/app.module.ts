
import { NgModule, CUSTOM_ELEMENTS_SCHEMA, ErrorHandler } from "@angular/core";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { BrowserModule } from '@angular/platform-browser';
/*import { AppRoutingModule } from './app-routing.module';*/
import { FormsModule } from "@angular/forms";
import { HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { RouterModule, Routes} from "@angular/router";
import { FontAwesomeModule, FaIconLibrary } from '@fortawesome/angular-fontawesome';
import { CodemirrorModule } from "@ctrl/ngx-codemirror";
import { faCoffee, fas } from '@fortawesome/free-solid-svg-icons';
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { NgxSpinnerModule } from "ngx-spinner";
import { ToastrModule } from "ngx-toastr";
import { HighlightModule } from "ngx-highlightjs";
import { TagInputModule } from 'ngx-chips';
import { AppComponent } from "./app.component";
import { NavMenuComponent } from "./nav-menu/nav-menu.component";
import { HomeComponent } from "./home/home.component";
import { NavCollectionComponent } from "./nav-collection/nav-collection.component";
import { NavCommunityCollectionComponent } from "./nav-community-collection/nav-community-collection.component";
import { UserProfileComponent } from "./user-profile/user-profile.component";
import { HttpConfigInterceptor } from "./interceptor/httpconfig.interceptor";
import { ServicesModule } from "./services/services.module";
import { GlobalErrorsService } from "./services/globalErrors.service"
import { DatabaseEnvironmentComponent } from './database-environment/database-environment.component';
import { RuleExecutionComponentComponent } from './rule-execution-component/rule-execution-component.component';
import { TagsComponent } from './tags/tags.component'
import { JobsComponent } from './jobs/jobs.component'
import { TagsSearchComponent } from './tags-search/tags-search.component'




const routes: Routes = [{ path: "", component: HomeComponent, pathMatch: "full" },
{ path: '*path', redirectTo: "" },
{ path: "**", redirectTo: "" }]

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    NavCollectionComponent,
    NavCommunityCollectionComponent,
    UserProfileComponent,
    DatabaseEnvironmentComponent,
    RuleExecutionComponentComponent,
    TagsComponent,
    TagsSearchComponent,
    JobsComponent
  ],
  exports: [FontAwesomeModule],
  imports: [
    BrowserModule.withServerTransition({ appId: "ng-cli-universal" }),
    HttpClientModule,
    FormsModule,
    ServicesModule,
    RouterModule.forRoot(routes),
    FontAwesomeModule ,

    NgxSpinnerModule,
    BrowserAnimationsModule, // required animations module
    ToastrModule.forRoot(),
    HighlightModule,
    NgbModule,
    CodemirrorModule,
    TagInputModule,

    NgMultiSelectDropDownModule.forRoot()
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: HttpConfigInterceptor, multi: true },
    { provide: ErrorHandler, useClass: GlobalErrorsService }
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  bootstrap: [AppComponent]
})
export class AppModule {
    constructor(library: FaIconLibrary) {
      library.addIconPacks(fas);
      library.addIcons(faCoffee);
    } }
