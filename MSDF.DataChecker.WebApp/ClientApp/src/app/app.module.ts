import { BrowserModule } from "@angular/platform-browser";
import { NgModule, CUSTOM_ELEMENTS_SCHEMA, ErrorHandler } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { RouterModule } from "@angular/router";
import { AngularFontAwesomeModule } from "angular-font-awesome";
import { HighlightModule } from "ngx-highlightjs";
import { AppComponent } from "./app.component";
import { NavMenuComponent } from "./nav-menu/nav-menu.component";
import { HomeComponent } from "./home/home.component";

import { NgbModule } from "@ng-bootstrap/ng-bootstrap";

import { ToastrModule } from "ngx-toastr";
import { ServicesModule } from "./services/services.module";
import { NgxSpinnerModule } from "ngx-spinner";

import { NavCollectionComponent } from "./nav-collection/nav-collection.component";
import { NavCommunityCollectionComponent } from "./nav-community-collection/nav-community-collection.component";
import { UserProfileComponent } from "./user-profile/user-profile.component";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { HttpConfigInterceptor } from "./interceptor/httpconfig.interceptor";

import { GlobalErrorsService } from "./services/globalErrors.service"

import typescript from "highlight.js/lib/languages/typescript";
import scss from "highlight.js/lib/languages/scss";
import xml from "highlight.js/lib/languages/xml";
import sql from "highlight.js/lib/languages/sql";
import json from "highlight.js/lib/languages/json";
import { CodemirrorModule } from "@ctrl/ngx-codemirror";
import { DatabaseEnvironmentComponent } from './database-environment/database-environment.component';
import { RuleExecutionComponentComponent } from './rule-execution-component/rule-execution-component.component';
import { TagsComponent } from './tags/tags.component'
import { JobsComponent } from './jobs/jobs.component'
import { TagsSearchComponent } from './tags-search/tags-search.component'
import { TagInputModule } from 'ngx-chips';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';

var routes = [
  { path: "", component: HomeComponent, pathMatch: "full" }
  //{ path: "profile", component: UserProfileComponent }
];

export function hljsLanguages() {
  return [
    { name: "typescript", func: typescript },
    { name: "scss", func: scss },
    { name: "xml", func: xml },
    { name: "sql", func: sql },
    { name: "json", func: json }
  ];
}

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
  imports: [
    BrowserModule.withServerTransition({ appId: "ng-cli-universal" }),
    HttpClientModule,
    FormsModule,
    AngularFontAwesomeModule,
    RouterModule.forRoot(routes),
    ServicesModule,

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
export class AppModule {}
