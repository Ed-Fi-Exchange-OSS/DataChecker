"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.CommunityService = void 0;
var http_1 = require("@angular/common/http");
var util_service_1 = require("./util.service");
var operators_1 = require("rxjs/operators");
//@Injectable({
//  providedIn: "root"
//})
var CommunityService = /** @class */ (function () {
    function CommunityService(http) {
        this.http = http;
        this.url = util_service_1.UtilService.communityUrl();
    }
    CommunityService.prototype.getCommunityCollections = function (tokenInfo) {
        this.url = util_service_1.UtilService.communityUrl();
        var headers = new http_1.HttpHeaders({
            'Content-Type': 'application/json',
            'Authorization': tokenInfo.type + ' ' + tokenInfo.token
        });
        return this.http
            .get(this.url + "container", { headers: headers })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    CommunityService.prototype.signInToCommunity = function (user) {
        this.url = util_service_1.UtilService.communityUrl();
        return this.http
            .post(this.url + "oauth/token", user)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    CommunityService.prototype.addCommunityUser = function (user) {
        this.url = util_service_1.UtilService.communityUrl();
        return this.http
            .post(this.url + "oauth/AddUser", user)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    CommunityService.prototype.getCollectionByName = function (collection, tokenInfo) {
        this.url = util_service_1.UtilService.communityUrl();
        var headers = new http_1.HttpHeaders({
            'Content-Type': 'application/json',
            'Authorization': tokenInfo.type + ' ' + tokenInfo.token
        });
        return this.http
            .post(this.url + "container/GetByName", collection, { headers: headers })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    CommunityService.prototype.uploadCollection = function (collection, tokenInfo) {
        this.url = util_service_1.UtilService.communityUrl();
        var headers = new http_1.HttpHeaders({
            'Content-Type': 'application/json',
            'Authorization': tokenInfo.type + ' ' + tokenInfo.token
        });
        return this.http
            .post(this.url + "container/Upload", collection, { headers: headers })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    CommunityService.prototype.isTokenValid = function (tokenInfo) {
        this.url = util_service_1.UtilService.communityUrl();
        var headers = new http_1.HttpHeaders({
            'Content-Type': 'application/json',
            'Authorization': tokenInfo.type + ' ' + tokenInfo.token
        });
        return this.http
            .post(this.url + "oauth/IsAuthenticated", { email: 'test1', password: 'test2' }, { headers: headers })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    CommunityService.prototype.validateDestinationTable = function (info, tokenInfo) {
        this.url = util_service_1.UtilService.communityUrl();
        var headers = new http_1.HttpHeaders({
            'Content-Type': 'application/json',
            'Authorization': tokenInfo.type + ' ' + tokenInfo.token
        });
        return this.http
            .post(this.url + "container/ValidateDestinationTable", info, { headers: headers })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    return CommunityService;
}());
exports.CommunityService = CommunityService;
//# sourceMappingURL=community.service.js.map