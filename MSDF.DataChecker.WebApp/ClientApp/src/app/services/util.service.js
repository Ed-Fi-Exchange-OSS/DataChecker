"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.UtilService = void 0;
var environment_1 = require("./../../environments/environment");
var UtilService = /** @class */ (function () {
    function UtilService() {
    }
    UtilService.tokenInformation = null;
    UtilService.communityUrlUser = null;
    UtilService.apiUrl = function () {
        var url = window.location.href + 'api/';
        return url;
    };
    UtilService.communityUrl = function () {
        if (UtilService.communityUrlUser == null || UtilService.communityUrlUser == environment_1.environment.communityUrl)
            return environment_1.environment.communityUrl;
        return UtilService.communityUrlUser;
    };
    UtilService.currentUserId = function () {
        return "661BFE54-ED59-4D59-B771-005F25F8356D";
    };
    UtilService.compareObj = function (obj1, obj2) {
        return diff(obj1, obj2);
    };
    return UtilService;
}());
exports.UtilService = UtilService;
var diff = function (obj1, obj2) {
    // Make sure an object to compare is provided
    if (!obj2 || Object.prototype.toString.call(obj2) !== '[object Object]') {
        return obj1;
    }
    //
    // Variables
    //
    var diffs = {};
    var key;
    //
    // Methods
    //
    /**
     * Check if two arrays are equal
     * @param  {Array}   arr1 The first array
     * @param  {Array}   arr2 The second array
     * @return {Boolean}      If true, both arrays are equal
     */
    var arraysMatch = function (arr1, arr2) {
        // Check if the arrays are the same length
        if (arr1.length !== arr2.length)
            return false;
        // Check if all items exist and are in the same order
        for (var i = 0; i < arr1.length; i++) {
            if (arr1[i] !== arr2[i])
                return false;
        }
        // Otherwise, return true
        return true;
    };
    /**
     * Compare two items and push non-matches to object
     * @param  {*}      item1 The first item
     * @param  {*}      item2 The second item
     * @param  {String} key   The key in our object
     */
    var compare = function (item1, item2, key) {
        // Get the object type
        var type1 = Object.prototype.toString.call(item1);
        var type2 = Object.prototype.toString.call(item2);
        // If type2 is undefined it has been removed
        if (type2 === '[object Undefined]') {
            diffs[key] = null;
            return;
        }
        // If items are different types
        if (type1 !== type2) {
            diffs[key] = item2;
            return;
        }
        // If an object, compare recursively
        if (type1 === '[object Object]') {
            var objDiff = diff(item1, item2);
            if (Object.keys(objDiff).length > 1) {
                diffs[key] = objDiff;
            }
            return;
        }
        // If an array, compare
        if (type1 === '[object Array]') {
            if (!arraysMatch(item1, item2)) {
                diffs[key] = item2;
            }
            return;
        }
        // Else if it's a function, convert to a string and compare
        // Otherwise, just compare
        if (type1 === '[object Function]') {
            if (item1.toString() !== item2.toString()) {
                diffs[key] = item2;
            }
        }
        else {
            if (item1 !== item2) {
                diffs[key] = item2;
            }
        }
    };
    //
    // Compare our objects
    //
    // Loop through the first object
    for (key in obj1) {
        if (obj1.hasOwnProperty(key)) {
            compare(obj1[key], obj2[key], key);
        }
    }
    // Loop through the second object and find missing items
    for (key in obj2) {
        if (obj2.hasOwnProperty(key)) {
            if (!obj1[key] && obj1[key] !== obj2[key]) {
                diffs[key] = obj2[key];
            }
        }
    }
    // Return the object of differences
    return diffs;
};
//# sourceMappingURL=util.service.js.map