"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.UserParamService = void 0;
var operators_1 = require("rxjs/operators");
var util_service_1 = require("./util.service");
/*@Injectable()*/
var UserParamService = /** @class */ (function () {
    function UserParamService(http) {
        this.http = http;
        this.component = 'UserParam';
        this.url = util_service_1.UtilService.apiUrl() + this.component;
        this.userId = util_service_1.UtilService.currentUserId();
    }
    UserParamService.prototype.getDatabaseInfo = function (databaseEnvironmentId) {
        return this.http
            .get(this.url + "/GetDatabaseInfo/" + databaseEnvironmentId, { responseType: "json" })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    UserParamService.prototype.addUserParam = function (userParam) {
        userParam.createdByUserId = this.userId;
        return this.http
            .post(this.url, userParam)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    UserParamService.prototype.getUserParams = function (databaseEnvironmentId) {
        return this.http
            .get(this.url + "/" + databaseEnvironmentId, { responseType: "json" })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    UserParamService.prototype.updateUserParam = function (userParam) {
        return this.http
            .post(this.url + "/Update", userParam)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    UserParamService.prototype.updateUserParams = function (userParams, databaseEnvironmentId) {
        return this.http
            .post(this.url + "/UpdateUserParams/" + databaseEnvironmentId, userParams)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    return UserParamService;
}());
exports.UserParamService = UserParamService;
//# sourceMappingURL=userParam.service.js.map