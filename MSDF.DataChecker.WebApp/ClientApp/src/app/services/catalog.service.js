"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.CatalogService = void 0;
var util_service_1 = require("./util.service");
var operators_1 = require("rxjs/operators");
/*@Injectable()*/
var CatalogService = /** @class */ (function () {
    function CatalogService(http) {
        this.http = http;
        this.component = "catalogs";
        this.url = util_service_1.UtilService.apiUrl() + this.component;
    }
    CatalogService.prototype.get = function () {
        return this.http
            .get(this.url + "/", { responseType: "json" })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    CatalogService.prototype.getByType = function (catalogType) {
        return this.http
            .get(this.url + "/GetByType?type=" + catalogType, { responseType: "json" })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    CatalogService.prototype.getById = function (id) {
        return this.http
            .get(this.url + "/" + id, { responseType: "json" })
            .pipe(operators_1.map(function (result) { return result; }));
    };
    return CatalogService;
}());
exports.CatalogService = CatalogService;
//# sourceMappingURL=catalog.service.js.map