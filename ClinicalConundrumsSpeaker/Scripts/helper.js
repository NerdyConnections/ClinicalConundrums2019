var settings = {
    appName: '',
    version:'1.2',
    loginPage: 'index.html',
    portalPage : 'portal.html'
};


var messages = {
    invalidEmail: 'The Email field is not a valid e-mail address.',
    invalidPassword: 'The Password field is required.',
    invalidString: 'Please enter valid name.',
    passwordNoMatch: 'Passwords do not match. Please try again.'
};

var programs = [
    {
        tID: 1,
        pName: 'CARDIOVASCULAR',
        pDescrition: '___',
        pImg: '__.jpg',
        pAction: [{aID:1, aName:'__'}, {aID:2, aName:'__'}, {aID:3, aName:'__'}],
        pMaterials: [{ mID: 1, mName: '__' }, { mID: 2, mName: '__' }, { mID: 3, mName: '__' }],
        colorCode: '#ff4b4b'
    },
     {
         tID: 2,
         pName: 'BONE HEALTH',
         pDescrition: '___',
         pImg: '__.jpg',
         pAction: [{ aID: 1, aName: '__' }, { aID: 2, aName: '__' }, { aID: 3, aName: '__' }],
         pMaterials: [{ mID: 1, mName: '__' }, { mID: 2, mName: '__' }, { mID: 3, mName: '__' }],
         colorCode: '#ce118b'
     },
      {
          tID: 3,
          pName: 'VACCINES',
          pDescrition: '___',
          pImg: '__.jpg',
          pAction: [{ aID: 1, aName: '__' }, { aID: 2, aName: '__' }, { aID: 3, aName: '__' }],
          pMaterials: [{ mID: 1, mName: '__' }, { mID: 2, mName: '__' }, { mID: 3, mName: '__' }],
          colorCode: '#ce7911'
      }
      ,
      {
          tID: 4,
          pName: 'ONCOLOGY',
          pDescrition: '___',
          pImg: '__.jpg',
          pAction: [{ aID: 1, aName: '__' }, { aID: 2, aName: '__' }, { aID: 3, aName: '__' }],
          pMaterials: [{ mID: 1, mName: '__' }, { mID: 2, mName: '__' }, { mID: 3, mName: '__' }],
          colorCode: '#b9bb56'
      }
      ,
      {
          tID: 5,
          pName: 'INFLAMMATION & IMMUNOLOGY',
          pDescrition: '___',
          pImg: '__.jpg',
          pAction: [{ aID: 1, aName: '__' }, { aID: 2, aName: '__' }, { aID: 3, aName: '__' }],
          pMaterials: [{ mID: 1, mName: '__' }, { mID: 2, mName: '__' }, { mID: 3, mName: '__' }],
          colorCode: '#8ab536'
      }
      ,
      {
          tID: 6,
          pName: 'RARE DISEASE',
          pDescrition: '___',
          pImg: '__.jpg',
          pAction: [{ aID: 1, aName: '__' }, { aID: 2, aName: '__' }, { aID: 3, aName: '__' }],
          pMaterials: [{ mID: 1, mName: '__' }, { mID: 2, mName: '__' }, { mID: 3, mName: '__' }],
          colorCode: '#4a78cb'
      }
];

var findProgramByID = function (arr, thID) {
    return $.grep(arr, function (elm, idx) {
        return elm.tID == thID
    });
};




var isValid = {
    email: function (arg) {
        var emailReg = /[\w\.=-]+@[\w\.-]+\.[a-z]{2,6}/;
        var isValidEmail = true;
        if (!emailReg.test(arg)) {
            isValidEmail = false;
        }
        return isValidEmail;
    },
    str: function (val) {
        var isValidStr = true;
        if (val.length < 1) {
            isValidStr = false;
        }
        return isValidStr;
    }
};

var fadeMessage = function (arg) {
    $(arg).stop(true, true).delay(1800).fadeTo(900, 0, 'easeInOutSine', function () {       
        $(arg).html('&nbsp;').fadeTo(1, 1, 'easeInOutSine', function () { });
    });
};

var getQuerystring = function (arg) {
    var loc = location.search.substring(1, location.search.length);
    var paramValue = false;
    var params = loc.split('&');
    for (i = 0; i < params.length; i++) {
        paramName = params[i].substring(0, params[i].indexOf('='));
        if (paramName == arg) {
            paramValue = params[i].substring(params[i].indexOf('=') + 1)
        }
    }
    if (paramValue) {
        return paramValue;
    }
    else {
        return false;
    }
};

var setCookie = function (cookoieName, value, expire) {
    if (expire) {
        var date = new Date();
        date.setTime(date.getTime() + (expire * 24 * 60 * 60 * 1000));
        var expires = '; expires=' + date.toGMTString();
    } else var expires = '';
    document.cookie = cookoieName + '=' + escape(value) + expires + '; path=/';
};

var getCookie = function (cookoieName) {
    var i, x, y, arr = document.cookie.split(';');
    for (i = 0; i < arr.length; i++) {
        x = arr[i].substr(0, arr[i].indexOf('='));
        y = arr[i].substr(arr[i].indexOf('=') + 1);
        x = x.replace(/^\s+|\s+$/g, '');
        if (x == cookoieName) {
            return unescape(y);
        }
    }
};

var delCookie = function (name) {
    setCookie(name, '', -1);
};

var isIE = function (userAgent) {
    var ua = window.navigator.userAgent;
    var ie = ua.search(/(MSIE|Trident|Edge)/);
    return ie > -1;
};

var hideModal  = function(modalID) {
    if (modalID != null) {
        $(modalID).modal('hide');
    }
}

var showModal = function(modalID) {
    if (modalID.length <= 0) {
        $(modalID).modal('show');
    }
}

var addModalHtml = function (modalHtml) {
    if ($('#cb-modal-container').length) {
        $('#cb-modal-container').append(modalHtml);
    }
    else {
        $('body').append('<div id="cb-modal-container"></div>');
        $('#cb-modal-container').append(modalHtml);
    }
};

var textInputFocus = function(element) {
    var input = $(element).find('input:text:visible:first');

    if (input.length) {
        input.focusTextToEnd();
    }

};

var loadModal = function (modalID, modalTitle, htmlContent, modalSize) {
    if (modalSize == null || modalSize.length == 0) {
        modalSize = "modal-lg";
    }

    var modalContent = modalID + '-ct';
    var modalSizeClass = 'modal-dialog ' + modalSize;

    var modalHtml = '<div id="' + modalID.replace('#', '') + '" tabindex="-1" class="modal fade in"><div class="' + modalSizeClass + '"><div class="modal-content"><div id="' + modalContent.replace('#', '') + '"></div></div></div></div>';

    addModalHtml(modalHtml);
    //$(modalContent).html(htmlContent);
    $(modalContent).html('<div class="modal-header">' +
      '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button><h4 class="modal-title">' + modalTitle + '</h4>' +
      '</div>' +
      '<div class="modal-body">' + htmlContent + '</div>' +
      '<div class="modal-footer"><button class="btn btn-default btn-sm" data-dismiss="modal">Close</button></div>');


    $(modalID).on('shown.bs.modal', function () {
        textInputFocus(this);
    });

    $(modalID).on('hidden.bs.modal', function () {
        $(this).remove();
    });

    $(modalID).modal({
        keyboard: false,
        backdrop: 'static'
    }, 'show');

};

//            loadModal('#abc', 'Add Session Contact', '> Modal Content', 'modal-lg');


var redirectToLogin = function () {
    window.location.href = settings.loginPage;
};

var logOut = function () {
    delCookie('chrc_sessionID');
    delCookie('chrc_username');
    delCookie('chrc_name');
    redirectToLogin();
};