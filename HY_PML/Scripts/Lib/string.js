String.jsonToParams = function (obj) {
    if (typeof (obj) === "string")
        obj = JSON.parse(obj);
    return Object.keys(obj).map(function (key) {
        return encodeURIComponent(key) + '=' + encodeURIComponent(obj[key]);
    }).join('&');
};