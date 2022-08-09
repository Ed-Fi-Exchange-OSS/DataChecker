"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.RuleService = void 0;
var http_1 = require("@angular/common/http");
var operators_1 = require("rxjs/operators");
var util_service_1 = require("./util.service");
/*@Injectable()*/
var RuleService = /** @class */ (function () {
    function RuleService(http) {
        this.http = http;
        this.component = "rules";
        this.url = util_service_1.UtilService.apiUrl() + this.component;
        this.userId = util_service_1.UtilService.currentUserId();
    }
    RuleService.prototype.getRuleDetail = function (id, databaseEnvironmentId) {
        var params = new http_1.HttpParams();
        return this.http
            .get(this.url + "/Results/" + id + "/" + databaseEnvironmentId, { headers: new http_1.HttpHeaders(), params: params })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    RuleService.prototype.addRule = function (rule) {
        rule.createdByUserId = this.userId;
        return this.http
            .post(this.url, rule)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    RuleService.prototype.modifyRule = function (rule) {
        return this.http
            .post(this.url + "/Update", rule)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    RuleService.prototype.deleteRule = function (ruleId) {
        return this.http
            .get(this.url + "/Delete/" + ruleId, {})
            .pipe(operators_1.map(function (result) { return result; }));
    };
    RuleService.prototype.executeRule = function (parameters) {
        return this.http
            .post(this.url + '/Run', parameters, { responseType: "json" })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    RuleService.prototype.executeDiagnostic = function (idDiagnostic, databaseEnvironmentId) {
        return this.http
            .post(this.url + '/RunDiagnosticAndReturnTable', { ruleExecutionLogId: idDiagnostic, databaseEnvironmentId: databaseEnvironmentId }, { responseType: "json" })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    RuleService.prototype.executeRuleTest = function (rule, databaseEnvironmentId) {
        return this.http
            .post(this.url + '/TestRun', { databaseEnvironmentId: databaseEnvironmentId, ruleToTest: rule }, { responseType: "json" })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    RuleService.prototype.searchRules = function (model) {
        return this.http
            .post(this.url + "/SearchRules", model)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    RuleService.prototype.copyRuleTo = function (model) {
        return this.http
            .post(this.url + "/CopyRuleTo", model)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    RuleService.prototype.deleteRules = function (model) {
        return this.http
            .post(this.url + "/DeleteByIds", model)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    RuleService.prototype.assignTagsToRules = function (model) {
        return this.http
            .post(this.url + "/AssignTagsByIds", model)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    RuleService.prototype.copyRulesToContainers = function (model) {
        return this.http
            .post(this.url + "/CopyToByIds", model)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    RuleService.prototype.moveRuleToContainer = function (rules, container) {
        return this.http
            .post(this.url + "/MoveRulesToContainer", { rules: rules, containerTo: container })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    return RuleService;
}());
exports.RuleService = RuleService;
//# sourceMappingURL=rule.service.js.map