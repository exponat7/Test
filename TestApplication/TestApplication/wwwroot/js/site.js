// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
//Notify = {
//    TYPE_INFO: 0,
//    TYPE_SUCCESS: 1,
//    TYPE_WARNING: 2,
//    TYPE_DANGER: 3,

//    generate: function (aText, aOptHeader, aOptType_int, timeout = null) {
//        var lTypeIndexes = [this.TYPE_INFO, this.TYPE_SUCCESS, this.TYPE_WARNING, this.TYPE_DANGER];
//        var ltypes = ['alert-info', 'alert-success', 'alert-warning', 'alert-danger'];
//        var ltype = ltypes[this.TYPE_INFO];

//        if (aOptType_int !== undefined && lTypeIndexes.indexOf(aOptType_int) !== -1) {
//            ltype = ltypes[aOptType_int];
//        }

//        var lText = '';
//        if (aOptHeader) {
//            lText += "<h4>" + aOptHeader + "</h4>";
//        }
//        lText += "<p class='notifyText'>" + aText + "</p>";
//        var lNotify_e = $("<div class='alert " + ltype + "'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button>" + lText + "</div>");

//        if (!timeout) timeout = 3000;

//        setTimeout(function () {
//            lNotify_e.alert('close');
//        }, timeout);
//        lNotify_e.appendTo($("#notifies"));
//    }
//};