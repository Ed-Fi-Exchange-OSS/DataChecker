"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ApiService = void 0;
var rule_service_1 = require("./rule.service");
var container_service_1 = require("./container.service");
var community_service_1 = require("./community.service");
var localUser_service_1 = require("./localUser.service");
var databaseEnvironment_service_1 = require("./databaseEnvironment.service");
var userParam_service_1 = require("./userParam.service");
var catalog_service_1 = require("./catalog.service");
var tag_service_1 = require("./tag.service");
var job_service_1 = require("./job.service");
var ruleExecutionLogDetail_service_1 = require("./ruleExecutionLogDetail.service");
/*@Injectable()*/
var ApiService = /** @class */ (function () {
    function ApiService(injector) {
        this.injector = injector;
        this.rule = injector.get(rule_service_1.RuleService);
        this.container = injector.get(container_service_1.ContainerService);
        this.community = injector.get(community_service_1.CommunityService);
        this.localUser = injector.get(localUser_service_1.LocalUserService);
        this.databaseEnvironment = injector.get(databaseEnvironment_service_1.DatabaseEnvironmentService);
        this.userParam = injector.get(userParam_service_1.UserParamService);
        this.catalog = injector.get(catalog_service_1.CatalogService);
        this.tag = injector.get(tag_service_1.TagService);
        this.job = injector.get(job_service_1.JobService);
        this.ruleExecutionLogDetail = injector.get(ruleExecutionLogDetail_service_1.RuleExecutionLogDetailService);
    }
    return ApiService;
}());
exports.ApiService = ApiService;
//# sourceMappingURL=api.service.js.map