"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.DatabaseEnvironmentService = void 0;
var operators_1 = require("rxjs/operators");
var util_service_1 = require("./util.service");
/*@Injectable()*/
var DatabaseEnvironmentService = /** @class */ (function () {
    function DatabaseEnvironmentService(http) {
        this.http = http;
        this.component = 'DatabaseEnvironments';
        this.url = util_service_1.UtilService.apiUrl() + this.component;
        this.userId = util_service_1.UtilService.currentUserId();
    }
    DatabaseEnvironmentService.prototype.getDatabaseInfo = function (databaseEnvironment) {
        return this.http
            .get(this.url + "/" + databaseEnvironment.id, { responseType: "json" })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    DatabaseEnvironmentService.prototype.addDatabaseEnvironment = function (databaseEnvironment) {
        databaseEnvironment.createdByUserId = this.userId;
        return this.http
            .post(this.url, databaseEnvironment)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    DatabaseEnvironmentService.prototype.getDatabaseEnvironments = function () {
        return this.http
            .get(this.url, { responseType: "json" })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    DatabaseEnvironmentService.prototype.updateDatabaseEnvironment = function (databaseEnvironment) {
        return this.http
            .post(this.url + "/Update", databaseEnvironment)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    DatabaseEnvironmentService.prototype.deleteDatabaseEnvironment = function (databaseEnvironment) {
        return this.http
            .get(this.url + "/Delete/" + databaseEnvironment.id, {})
            .pipe(operators_1.map(function (result) { return result; }));
    };
    DatabaseEnvironmentService.prototype.duplicateDatabaseEnvironment = function (databaseEnvironment) {
        return this.http
            .post(this.url + "/Duplicate/" + databaseEnvironment.id, {})
            .pipe(operators_1.map(function (result) { return result; }));
    };
    DatabaseEnvironmentService.prototype.testDatabaseEnvironment = function (databaseEnvironment) {
        return this.http
            .post(this.url + "/TestConnection", databaseEnvironment)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    DatabaseEnvironmentService.prototype.testDatabaseEnvironmentById = function (databaseEnvironment) {
        return this.http
            .post(this.url + "/TestConnectionById", databaseEnvironment)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    DatabaseEnvironmentService.prototype.getMaxNumberResults = function () {
        return this.http
            .get(this.url + "/GetMaxNumberResults/", {})
            .pipe(operators_1.map(function (result) { return result; }));
    };
    return DatabaseEnvironmentService;
}());
exports.DatabaseEnvironmentService = DatabaseEnvironmentService;
//# sourceMappingURL=databaseEnvironment.service.js.map