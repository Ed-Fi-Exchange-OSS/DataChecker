"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.JobService = void 0;
var operators_1 = require("rxjs/operators");
var util_service_1 = require("./util.service");
//@Injectable()
var JobService = /** @class */ (function () {
    function JobService(http) {
        this.http = http;
        this.component = "jobs";
        this.url = util_service_1.UtilService.apiUrl() + this.component;
    }
    JobService.prototype.addJob = function (job) {
        return this.http
            .post(this.url, job)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    JobService.prototype.modifyJob = function (job) {
        return this.http
            .post(this.url + "/Update", job)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    JobService.prototype.runAndForget = function (job) {
        return this.http
            .post(this.url + "/RunAndForget", job)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    JobService.prototype.deleteJob = function (jobId) {
        return this.http
            .get(this.url + "/Delete/" + jobId, {})
            .pipe(operators_1.map(function (result) { return result; }));
    };
    JobService.prototype.getJobs = function () {
        return this.http
            .get(this.url + "/", { responseType: "json" })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    JobService.prototype.enqueueJob = function (jobId) {
        return this.http
            .get(this.url + "/Enqueue/" + jobId, { responseType: "json" })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    return JobService;
}());
exports.JobService = JobService;
//# sourceMappingURL=job.service.js.map