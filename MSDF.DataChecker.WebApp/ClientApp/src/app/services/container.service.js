"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ContainerService = void 0;
var operators_1 = require("rxjs/operators");
var util_service_1 = require("./util.service");
/*@Injectable()*/
var ContainerService = /** @class */ (function () {
    function ContainerService(http) {
        this.http = http;
        this.component = "Containers";
        this.url = util_service_1.UtilService.apiUrl() + this.component;
        this.userId = util_service_1.UtilService.currentUserId();
    }
    ContainerService.prototype.getContainerByContainerIdAndDatabaseEnvironmentId = function (containerId, databaseEnvironmentId) {
        if (containerId === void 0) { containerId = ""; }
        if (databaseEnvironmentId === void 0) { databaseEnvironmentId = ""; }
        return this.http
            .get(this.url + "/" + containerId + "/details/" + databaseEnvironmentId, {
            responseType: "json"
        })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    ContainerService.prototype.getAllCollections = function () {
        return this.http
            .get(this.url, { responseType: "json" })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    ContainerService.prototype.getChildContainers = function () {
        return this.http
            .get(this.url + '/ChildContainers', { responseType: "json" })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    ContainerService.prototype.getParentContainers = function () {
        return this.http
            .get(this.url + '/ParentContainers', { responseType: "json" })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    ContainerService.prototype.addContainerFromCommunity = function (collection) {
        return this.http
            .post(this.url + "/AddContainerFromCommunity", collection)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    ContainerService.prototype.addCollectionContainer = function (collection) {
        collection.createdByUserId = this.userId;
        collection.containerTypeId = 1;
        return this.http
            .post(this.url, collection)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    ContainerService.prototype.addContainer = function (collection) {
        collection.createdByUserId = this.userId;
        collection.containerTypeId = 2;
        return this.http
            .post(this.url, collection)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    ContainerService.prototype.deleteContainer = function (containerId) {
        return this.http
            .get(this.url + "/Delete/" + containerId, {})
            .pipe(operators_1.map(function (result) { return result; }));
    };
    ContainerService.prototype.updateContainer = function (container) {
        return this.http
            .post(this.url + "/Update", container)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    ContainerService.prototype.setDefaultContainer = function (container) {
        return this.http
            .post(this.url + "/SetDefaultAsync", container)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    ContainerService.prototype.getContainerToCommunity = function (containerId) {
        return this.http
            .get(this.url + "/GetToCommunity/" + containerId, {
            responseType: "json"
        })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    ContainerService.prototype.getContainerByName = function (collection) {
        return this.http
            .post(this.url + "/GetByName", collection)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    ContainerService.prototype.validateDestinationTable = function (info) {
        return this.http
            .post(this.url + "/ValidateDestinationTable", info)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    ContainerService.prototype.downloadContainerJson = function (collection) {
        return this.http
            .post(this.url + "/DownloadCollection", collection)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    ContainerService.prototype.uploadContainerJson = function (collection) {
        return this.http
            .post(this.url + "/UploadCollection", collection)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    return ContainerService;
}());
exports.ContainerService = ContainerService;
//# sourceMappingURL=container.service.js.map