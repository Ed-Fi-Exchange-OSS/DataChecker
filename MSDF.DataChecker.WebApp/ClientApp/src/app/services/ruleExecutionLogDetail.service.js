"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.RuleExecutionLogDetailService = void 0;
var operators_1 = require("rxjs/operators");
var util_service_1 = require("./util.service");
/*@Injectable()*/
var RuleExecutionLogDetailService = /** @class */ (function () {
    function RuleExecutionLogDetailService(http) {
        this.http = http;
        this.component = "RuleExecutionLogDetails";
        this.url = util_service_1.UtilService.apiUrl() + this.component;
    }
    RuleExecutionLogDetailService.prototype.getByRuleExecutionLogAsync = function (id) {
        return this.http
            .get(this.url + "/RuleExecutionLogAsync/" + id, { responseType: "json" })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    RuleExecutionLogDetailService.prototype.getLastRuleExecutionLogByEnvironmentAndRuleAsync = function (environmentId, ruleId) {
        return this.http
            .get(this.url + "/LastRuleExecutionLogByEnvironmentAndRule/" + environmentId + "/" + ruleId, { responseType: "json" })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    RuleExecutionLogDetailService.prototype.exportToCsvByRuleExecutionLogAsync = function (id) {
        return this.url + "/ExportToCsvAsync/" + id;
    };
    RuleExecutionLogDetailService.prototype.exportToTableByRuleExecutionLogAsync = function (id) {
        return this.http
            .get(this.url + "/ExportToTableAsync/" + id, { responseType: "json" })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    RuleExecutionLogDetailService.prototype.executeDiagnosticSqlFromLogIdAsync = function (id) {
        return this.http
            .get(this.url + "/ExecuteDiagnosticSqlFromLog/" + id, { responseType: "json" })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    return RuleExecutionLogDetailService;
}());
exports.RuleExecutionLogDetailService = RuleExecutionLogDetailService;
//# sourceMappingURL=ruleExecutionLogDetail.service.js.map