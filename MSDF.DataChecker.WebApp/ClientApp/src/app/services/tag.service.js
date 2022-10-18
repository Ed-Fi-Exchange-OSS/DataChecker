"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.TagService = void 0;
var operators_1 = require("rxjs/operators");
var util_service_1 = require("./util.service");
/*@Injectable()*/
var TagService = /** @class */ (function () {
    function TagService(http) {
        this.http = http;
        this.component = "tags";
        this.url = util_service_1.UtilService.apiUrl() + this.component;
    }
    TagService.prototype.addTag = function (tag) {
        return this.http
            .post(this.url, tag)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    TagService.prototype.modifyTag = function (tag) {
        return this.http
            .post(this.url + "/Update", tag)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    TagService.prototype.deleteTag = function (tagId) {
        return this.http
            .get(this.url + "/Delete/" + tagId, {})
            .pipe(operators_1.map(function (result) { return result; }));
    };
    TagService.prototype.getByContainer = function (containerId) {
        return this.http
            .get(this.url + "/GetByContainer/" + containerId, { responseType: "json" })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    TagService.prototype.getByRule = function (ruleId) {
        return this.http
            .get(this.url + "/GetByRule/" + ruleId, { responseType: "json" })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    TagService.prototype.getById = function (id) {
        return this.http
            .get(this.url + "/" + id, { responseType: "json" })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    TagService.prototype.getTags = function () {
        return this.http
            .get(this.url + "/", { responseType: "json" })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    TagService.prototype.searchByTags = function (tags) {
        return this.http
            .post(this.url + "/SearchByTags", tags)
            .pipe(operators_1.map(function (result) { return result; }));
    };
    return TagService;
}());
exports.TagService = TagService;
//# sourceMappingURL=tag.service.js.map