import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RuleService } from "./rule.service";
import { ContainerService } from "./container.service";
import { ApiService } from "./api.service";
import { DatabaseEnvironmentService } from "./databaseEnvironment.service";
import { UserParamService } from "./userParam.service";
import { LogService } from "./log.service";
import { CatalogService } from "./catalog.service";
import { TagService } from "./tag.service";
import { JobService } from "./job.service";
import { RuleExecutionLogDetailService } from "./ruleExecutionLogDetail.service"

@NgModule({
  imports: [CommonModule],
  providers: [
    RuleService,
    ContainerService,
    DatabaseEnvironmentService,
    UserParamService,
    LogService,
    CatalogService,
    TagService,
    JobService,
    RuleExecutionLogDetailService,
    ApiService]
})
export class ServicesModule { }
