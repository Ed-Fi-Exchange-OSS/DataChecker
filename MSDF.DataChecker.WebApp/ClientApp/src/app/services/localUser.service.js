"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.LocalUserService = void 0;
var operators_1 = require("rxjs/operators");
var util_service_1 = require("./util.service");
//@Injectable({
//  providedIn: "root"
//})
var LocalUserService = /** @class */ (function () {
    function LocalUserService(http) {
        this.http = http;
        this.component = "communityuser";
        this.url = util_service_1.UtilService.apiUrl() + this.component;
        this.userId = util_service_1.UtilService.currentUserId();
    }
    LocalUserService.prototype.getLocalUser = function (id) {
        if (id === void 0) { id = ""; }
        return this.http
            .get(this.url + "/" + this.userId, { responseType: "json" })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    LocalUserService.prototype.updateUserInformation = function (localUser) {
        return this.http
            .post(this.url + "/Update", localUser)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    return LocalUserService;
}());
exports.LocalUserService = LocalUserService;
//# sourceMappingURL=localUser.service.js.map