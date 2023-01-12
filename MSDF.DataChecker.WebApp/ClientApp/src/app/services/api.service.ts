import { Injectable, Injector } from "@angular/core";
import { RuleService } from "./rule.service";
import { ContainerService } from "./container.service";
import { CommunityService } from "./community.service";
import { LocalUserService } from "./localUser.service";
import { DatabaseEnvironmentService } from "./databaseEnvironment.service";
import { ValidationRunService } from "./validationRun.service";
import { UserParamService } from "./userParam.service";
import { CatalogService } from "./catalog.service"
import { TagService } from "./tag.service"
import { JobService } from "./job.service"
import { RuleExecutionLogDetailService } from "./ruleExecutionLogDetail.service"

@Injectable()
export class ApiService {

  public rule: RuleService;
  public container: ContainerService;
  public community: CommunityService;
  public localUser: LocalUserService;
  public databaseEnvironment: DatabaseEnvironmentService;
  public validationRun: ValidationRunService;
  public userParam: UserParamService;
  public catalog: CatalogService;
  public tag: TagService;
  public job: JobService;
  public ruleExecutionLogDetail: RuleExecutionLogDetailService;

  constructor(private injector: Injector) {
    this.rule = injector.get(RuleService);
    this.container = injector.get(ContainerService);
    this.community = injector.get(CommunityService);
    this.localUser = injector.get(LocalUserService);
    this.databaseEnvironment = injector.get(DatabaseEnvironmentService);
    this.validationRun = injector.get(ValidationRunService);
    this.userParam = injector.get(UserParamService);
    this.catalog = injector.get(CatalogService);
    this.tag = injector.get(TagService);
    this.job = injector.get(JobService);
    this.ruleExecutionLogDetail = injector.get(RuleExecutionLogDetailService);
  }
}
