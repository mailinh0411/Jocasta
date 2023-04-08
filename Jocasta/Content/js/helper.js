var userToken = GetCookie('UserToken');
var userAdminToken = GetCookie('UserAdminToken');
const Enum = {
    ResponseStatus: { SUCCESS: 'success', ERROR: 'error', UNAUTHORIZED: "unauthorized", UNAUTHENTICATED: "unauthenticated" },
    NumberConstant: { ADMIN_PAGE_SIZE: 20 },
    ApiUrl: '',
    OptionHeaderDefault: {
        'Content-Type': 'application/json',
        'Authorization': userToken
    },
    OptionAdminHeaderDefault: {
        'Content-Type': 'application/json',
        'Authorization': userAdminToken
    },
    SalaryType: { WITHDRAWAL: 'WITHDRAWAL', WITHDRAWAL_ROLL_BACK: 'WITHDRAWAL_ROLL_BACK', DISCOUNT_DIRECT_COMMISSION: 'DISCOUNT_DIRECT_COMMISSION', DISCOUNT_DOWNLINE_COMMISSION: 'DISCOUNT_DOWNLINE_COMMISSION', DISCOUNT_LEADER_COMMISSION: 'DISCOUNT_LEADER_COMMISSION' },
    Rank: ['', 'Khách hàng', 'Đại lý', 'Phó phòng', 'Trưởng phòng', 'Phó giám đốc', 'Giám đốc']
};
var Loading = {
    Show: function () { $('#preloading').css('display', 'flex'); },
    Hide: function () { $('#preloading').css('display', 'none'); }
}
function NumberFormat(num, joinChar = ',') { try { const numString = num + ''; var numberPart = numString.split('.'); var s = numberPart[0]; var arr = []; while (s.length > 0) { if (s.length > 3) { arr.push(s.slice(s.length - 3, s.length)); s = s.slice(0, s.length - 3); } else { arr.push(s); s = ''; } } return arr.reverse().join(joinChar) + (numberPart.length > 1 ? '.' + numberPart[1] : ''); } catch (ex) { return ''; } }
function GenerateRandomInt(min, max) { min = Math.ceil(min); max = Math.floor(max) + 1; return Math.floor(Math.random() * (max - min) + min); }
function GetObjectProperty(obj, prop, defaultValue = '') { try { if (obj === '' || obj === null || typeof obj === 'undefined') return defaultValue; if (obj[prop] === '' || obj[prop] === null || typeof obj[prop] === 'undefined') return defaultValue; return obj[prop]; } catch (err) { } return defaultValue; }
function DateStringFormat({ stringDate, currentFormat = 'yyyy/mm/dd', newFormat = 'dd/mm/yyyy' }) { if (stringDate === '' || stringDate === null || typeof stringDate === 'undefined') return ''; if (typeof stringDate === 'object') { newFormat = newFormat.replace('dd', (stringDate.getDate() > 9 ? stringDate.getDate() + '' : '0' + stringDate.getDate())); newFormat = newFormat.replace('mm', (stringDate.getMonth() + 1 > 9 ? (stringDate.getMonth() + 1) + '' : '0' + (stringDate.getMonth() + 1))); newFormat = newFormat.replace('yyyy', stringDate.getFullYear() + ''); newFormat = newFormat.replace('hh', stringDate.getHours() > 9 ? stringDate.getHours() + '' : '0' + stringDate.getHours()); newFormat = newFormat.replace('mi', stringDate.getMinutes() > 9 ? stringDate.getMinutes() + '' : '0' + stringDate.getMinutes()); newFormat = newFormat.replace('ss', stringDate.getSeconds() > 9 ? stringDate.getSeconds() + '' : '0' + stringDate.getSeconds()); return newFormat; } const stringDatePart = stringDate.split(/[-\/._,\\+=!@#$%ˆ&* :a-zA-Z]/g); const currentFormatPart = currentFormat.split(/[-\/._,\\+=!@#$%ˆ&* :]/g); for (var i = 0; i < stringDatePart.length; i++) { if (currentFormatPart[i] === 'dd') newFormat = newFormat.replace('dd', stringDatePart[i].length < 2 ? '0' + stringDatePart[i] : stringDatePart[i]); if (currentFormatPart[i] === 'mm') newFormat = newFormat.replace('mm', stringDatePart[i].length < 2 ? '0' + stringDatePart[i] : stringDatePart[i]); if (currentFormatPart[i] === 'yyyy') newFormat = newFormat.replace('yyyy', stringDatePart[i]); if (currentFormatPart[i] === 'hh') newFormat = newFormat.replace('hh', stringDatePart[i].length < 2 ? '0' + stringDatePart[i] : stringDatePart[i]); if (currentFormatPart[i] === 'mi') newFormat = newFormat.replace('mi', stringDatePart[i].length < 2 ? '0' + stringDatePart[i] : stringDatePart[i]); if (currentFormatPart[i] === 'ss') newFormat = newFormat.replace('ss', stringDatePart[i].length < 2 ? '0' + stringDatePart[i] : stringDatePart[i]); } return newFormat; }
function MD5(string) {
    function RotateLeft(lValue, iShiftBits) { return (lValue << iShiftBits) | (lValue >>> (32 - iShiftBits)); }
    function AddUnsigned(lX, lY) { var lX4, lY4, lX8, lY8, lResult; lX8 = (lX & 0x80000000); lY8 = (lY & 0x80000000); lX4 = (lX & 0x40000000); lY4 = (lY & 0x40000000); lResult = (lX & 0x3FFFFFFF) + (lY & 0x3FFFFFFF); if (lX4 & lY4) { return (lResult ^ 0x80000000 ^ lX8 ^ lY8); } if (lX4 | lY4) { if (lResult & 0x40000000) { return (lResult ^ 0xC0000000 ^ lX8 ^ lY8); } else { return (lResult ^ 0x40000000 ^ lX8 ^ lY8); } } else { return (lResult ^ lX8 ^ lY8); } }
    function F(x, y, z) { return (x & y) | ((~x) & z); }
    function G(x, y, z) { return (x & z) | (y & (~z)); }
    function H(x, y, z) { return (x ^ y ^ z); }
    function I(x, y, z) { return (y ^ (x | (~z))); }
    function FF(a, b, c, d, x, s, ac) { a = AddUnsigned(a, AddUnsigned(AddUnsigned(F(b, c, d), x), ac)); return AddUnsigned(RotateLeft(a, s), b); };
    function GG(a, b, c, d, x, s, ac) { a = AddUnsigned(a, AddUnsigned(AddUnsigned(G(b, c, d), x), ac)); return AddUnsigned(RotateLeft(a, s), b); };
    function HH(a, b, c, d, x, s, ac) { a = AddUnsigned(a, AddUnsigned(AddUnsigned(H(b, c, d), x), ac)); return AddUnsigned(RotateLeft(a, s), b); };
    function II(a, b, c, d, x, s, ac) { a = AddUnsigned(a, AddUnsigned(AddUnsigned(I(b, c, d), x), ac)); return AddUnsigned(RotateLeft(a, s), b); };
    function ConvertToWordArray(string) { var lWordCount; var lMessageLength = string.length; var lNumberOfWords_temp1 = lMessageLength + 8; var lNumberOfWords_temp2 = (lNumberOfWords_temp1 - (lNumberOfWords_temp1 % 64)) / 64; var lNumberOfWords = (lNumberOfWords_temp2 + 1) * 16; var lWordArray = Array(lNumberOfWords - 1); var lBytePosition = 0; var lByteCount = 0; while (lByteCount < lMessageLength) { lWordCount = (lByteCount - (lByteCount % 4)) / 4; lBytePosition = (lByteCount % 4) * 8; lWordArray[lWordCount] = (lWordArray[lWordCount] | (string.charCodeAt(lByteCount) << lBytePosition)); lByteCount++; } lWordCount = (lByteCount - (lByteCount % 4)) / 4; lBytePosition = (lByteCount % 4) * 8; lWordArray[lWordCount] = lWordArray[lWordCount] | (0x80 << lBytePosition); lWordArray[lNumberOfWords - 2] = lMessageLength << 3; lWordArray[lNumberOfWords - 1] = lMessageLength >>> 29; return lWordArray; };
    function WordToHex(lValue) { var WordToHexValue = "", WordToHexValue_temp = "", lByte, lCount; for (lCount = 0; lCount <= 3; lCount++) { lByte = (lValue >>> (lCount * 8)) & 255; WordToHexValue_temp = "0" + lByte.toString(16); WordToHexValue = WordToHexValue + WordToHexValue_temp.substr(WordToHexValue_temp.length - 2, 2); } return WordToHexValue; };
    function Utf8Encode(string) { string = string.replace(/\r\n/g, "\n"); var utftext = ""; for (var n = 0; n < string.length; n++) { var c = string.charCodeAt(n); if (c < 128) { utftext += String.fromCharCode(c); } else if ((c > 127) && (c < 2048)) { utftext += String.fromCharCode((c >> 6) | 192); utftext += String.fromCharCode((c & 63) | 128); } else { utftext += String.fromCharCode((c >> 12) | 224); utftext += String.fromCharCode(((c >> 6) & 63) | 128); utftext += String.fromCharCode((c & 63) | 128); } } return utftext; };

    var x = Array();
    var k, AA, BB, CC, DD, a, b, c, d;
    var S11 = 7, S12 = 12, S13 = 17, S14 = 22;
    var S21 = 5, S22 = 9, S23 = 14, S24 = 20;
    var S31 = 4, S32 = 11, S33 = 16, S34 = 23;
    var S41 = 6, S42 = 10, S43 = 15, S44 = 21;

    string = Utf8Encode(string);
    x = ConvertToWordArray(string);
    a = 0x67452301; b = 0xEFCDAB89; c = 0x98BADCFE; d = 0x10325476;

    for (k = 0; k < x.length; k += 16) {
        AA = a; BB = b; CC = c; DD = d;
        a = FF(a, b, c, d, x[k + 0], S11, 0xD76AA478);
        d = FF(d, a, b, c, x[k + 1], S12, 0xE8C7B756);
        c = FF(c, d, a, b, x[k + 2], S13, 0x242070DB);
        b = FF(b, c, d, a, x[k + 3], S14, 0xC1BDCEEE);
        a = FF(a, b, c, d, x[k + 4], S11, 0xF57C0FAF);
        d = FF(d, a, b, c, x[k + 5], S12, 0x4787C62A);
        c = FF(c, d, a, b, x[k + 6], S13, 0xA8304613);
        b = FF(b, c, d, a, x[k + 7], S14, 0xFD469501);
        a = FF(a, b, c, d, x[k + 8], S11, 0x698098D8);
        d = FF(d, a, b, c, x[k + 9], S12, 0x8B44F7AF);
        c = FF(c, d, a, b, x[k + 10], S13, 0xFFFF5BB1);
        b = FF(b, c, d, a, x[k + 11], S14, 0x895CD7BE);
        a = FF(a, b, c, d, x[k + 12], S11, 0x6B901122);
        d = FF(d, a, b, c, x[k + 13], S12, 0xFD987193);
        c = FF(c, d, a, b, x[k + 14], S13, 0xA679438E);
        b = FF(b, c, d, a, x[k + 15], S14, 0x49B40821);
        a = GG(a, b, c, d, x[k + 1], S21, 0xF61E2562);
        d = GG(d, a, b, c, x[k + 6], S22, 0xC040B340);
        c = GG(c, d, a, b, x[k + 11], S23, 0x265E5A51);
        b = GG(b, c, d, a, x[k + 0], S24, 0xE9B6C7AA);
        a = GG(a, b, c, d, x[k + 5], S21, 0xD62F105D);
        d = GG(d, a, b, c, x[k + 10], S22, 0x2441453);
        c = GG(c, d, a, b, x[k + 15], S23, 0xD8A1E681);
        b = GG(b, c, d, a, x[k + 4], S24, 0xE7D3FBC8);
        a = GG(a, b, c, d, x[k + 9], S21, 0x21E1CDE6);
        d = GG(d, a, b, c, x[k + 14], S22, 0xC33707D6);
        c = GG(c, d, a, b, x[k + 3], S23, 0xF4D50D87);
        b = GG(b, c, d, a, x[k + 8], S24, 0x455A14ED);
        a = GG(a, b, c, d, x[k + 13], S21, 0xA9E3E905);
        d = GG(d, a, b, c, x[k + 2], S22, 0xFCEFA3F8);
        c = GG(c, d, a, b, x[k + 7], S23, 0x676F02D9);
        b = GG(b, c, d, a, x[k + 12], S24, 0x8D2A4C8A);
        a = HH(a, b, c, d, x[k + 5], S31, 0xFFFA3942);
        d = HH(d, a, b, c, x[k + 8], S32, 0x8771F681);
        c = HH(c, d, a, b, x[k + 11], S33, 0x6D9D6122);
        b = HH(b, c, d, a, x[k + 14], S34, 0xFDE5380C);
        a = HH(a, b, c, d, x[k + 1], S31, 0xA4BEEA44);
        d = HH(d, a, b, c, x[k + 4], S32, 0x4BDECFA9);
        c = HH(c, d, a, b, x[k + 7], S33, 0xF6BB4B60);
        b = HH(b, c, d, a, x[k + 10], S34, 0xBEBFBC70);
        a = HH(a, b, c, d, x[k + 13], S31, 0x289B7EC6);
        d = HH(d, a, b, c, x[k + 0], S32, 0xEAA127FA);
        c = HH(c, d, a, b, x[k + 3], S33, 0xD4EF3085);
        b = HH(b, c, d, a, x[k + 6], S34, 0x4881D05);
        a = HH(a, b, c, d, x[k + 9], S31, 0xD9D4D039);
        d = HH(d, a, b, c, x[k + 12], S32, 0xE6DB99E5);
        c = HH(c, d, a, b, x[k + 15], S33, 0x1FA27CF8);
        b = HH(b, c, d, a, x[k + 2], S34, 0xC4AC5665);
        a = II(a, b, c, d, x[k + 0], S41, 0xF4292244);
        d = II(d, a, b, c, x[k + 7], S42, 0x432AFF97);
        c = II(c, d, a, b, x[k + 14], S43, 0xAB9423A7);
        b = II(b, c, d, a, x[k + 5], S44, 0xFC93A039);
        a = II(a, b, c, d, x[k + 12], S41, 0x655B59C3);
        d = II(d, a, b, c, x[k + 3], S42, 0x8F0CCC92);
        c = II(c, d, a, b, x[k + 10], S43, 0xFFEFF47D);
        b = II(b, c, d, a, x[k + 1], S44, 0x85845DD1);
        a = II(a, b, c, d, x[k + 8], S41, 0x6FA87E4F);
        d = II(d, a, b, c, x[k + 15], S42, 0xFE2CE6E0);
        c = II(c, d, a, b, x[k + 6], S43, 0xA3014314);
        b = II(b, c, d, a, x[k + 13], S44, 0x4E0811A1);
        a = II(a, b, c, d, x[k + 4], S41, 0xF7537E82);
        d = II(d, a, b, c, x[k + 11], S42, 0xBD3AF235);
        c = II(c, d, a, b, x[k + 2], S43, 0x2AD7D2BB);
        b = II(b, c, d, a, x[k + 9], S44, 0xEB86D391);
        a = AddUnsigned(a, AA);
        b = AddUnsigned(b, BB);
        c = AddUnsigned(c, CC);
        d = AddUnsigned(d, DD);
    }
    var temp = WordToHex(a) + WordToHex(b) + WordToHex(c) + WordToHex(d);
    return temp.toLowerCase();
}

function ValidateInputOnlyNumber(evt, el) {
    $(el).val($(el).val().replace(/[a-z,A-Z,!,@,#,$,%,^,&,*,(,),+,-,/,_,`,~,\\,[,\],;,:,',",<,>,?]/g, ''));
    let number = NumberFormat($(el).val().replace(/,/g, ''));
    $(el).val(number);
}

function ValidateInputOnlyPhoneNumber(evt, el) {
    let value = $(el).val().replace(/[a-z,A-Z,!,@,#,$,%,^,&,*,(,),+,-,/,_,`,~,\\,[,\],;,:,',",<,>,?]/g, '');
    $(el).val(value.substr(0, 10));
}

var NotificationShow = function (message = '', type = 'error', title = 'Thông báo') { //type : ['error','success']

    const notificationElement = $(`<div class="notification ${type}">
<div class="notification-header">
<div class="notification-title">${title}</div>
<a class="notification-close" onclick="(function (el) { $(el).closest('.notification').remove(); })(this);"><i class="fa fa-times" aria-hidden="true"></i></a>
</div><div class="notification-body">
<div class="notification-message">${message}</div></div></div>`);
    $('body').append(notificationElement);
    setTimeout(function () { $(notificationElement).remove(); }, 3000);
}

var CheckErrorResponse = function (model) {
    if (GetObjectProperty(model, 'status') === Enum.ResponseStatus.UNAUTHORIZED) {
        EraseCookie('UserToken');
        alert('Phiên làm việc của bạn đã hết hạn, vui lòng đăng nhập lại để tiếp tục sử dụng hệ thống.');
        window.location.href = '/dang-nhap';
        return false;
    } else if (GetObjectProperty(model, 'status') === Enum.ResponseStatus.ERROR) {
        if (GetObjectProperty(model, 'message') !== '') alert(GetObjectProperty(model, 'message'));
        return false;
    }
    return true;
}


var AdminCheckErrorResponse = function (model) {
    if (GetObjectProperty(model, 'status') === Enum.ResponseStatus.UNAUTHORIZED) {
        EraseCookie('UserAdminToken');
        alert('Phiên làm việc của bạn đã hết hạn, vui lòng đăng nhập lại để tiếp tục sử dụng hệ thống.');
        window.location.href = '/admin/system/login';
        return false;
    } else if (GetObjectProperty(model, 'status') === Enum.ResponseStatus.ERROR) {
        if (GetObjectProperty(model, 'message') !== '') alert(GetObjectProperty(model, 'message'));
        return false;
    }
    return true;
}

var RemoveVietnameseTones = function (str) {
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, "A");
    str = str.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, "E");
    str = str.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, "I");
    str = str.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, "O");
    str = str.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, "U");
    str = str.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, "Y");
    str = str.replace(/Đ/g, "D");
    // Some system encode vietnamese combining accent as individual utf-8 characters
    // Một vài bộ encode coi các dấu mũ, dấu chữ như một kí tự riêng biệt nên thêm hai dòng này
    str = str.replace(/\u0300|\u0301|\u0303|\u0309|\u0323/g, ""); // ̀ ́ ̃ ̉ ̣  huyền, sắc, ngã, hỏi, nặng
    str = str.replace(/\u02C6|\u0306|\u031B/g, ""); // ˆ ̆ ̛  Â, Ê, Ă, Ơ, Ư
    // Remove extra spaces
    // Bỏ các khoảng trắng liền nhau
    str = str.replace(/ + /g, " ");
    str = str.trim();
    // Remove punctuations
    // Bỏ dấu câu, kí tự đặc biệt
    str = str.replace(/!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'|\"|\&|\#|\[|\]|~|\$|_|`|-|{|}|\||\\/g, " ");
    return str;
}

var CountDownBetweenDate = function (fromDate, toDate, format = 'mm:ss') {
    if (fromDate === '' || fromDate === null || typeof fromDate === 'undefined') return '00:00';
    if (toDate === '' || toDate === null || typeof toDate === 'undefined') return '00:00';

    fromDate = fromDate.getTime();
    toDate = toDate.getTime();

    let msec = Math.abs(toDate - fromDate);
    var hh = Math.floor(msec / 1000 / 60 / 60);
    msec -= hh * 1000 * 60 * 60;
    var mm = Math.floor(msec / 1000 / 60);
    msec -= mm * 1000 * 60;
    var ss = Math.floor(msec / 1000);
    if (hh < 10) hh = '0' + hh; else hh = hh + '';
    if (mm < 10) mm = '0' + mm; else mm = mm + '';
    if (ss < 10) ss = '0' + ss; else ss = ss + '';
    return format.replace('hh', hh).replace('mm', mm).replace('ss', ss);
}

function SetCookie(cname, cvalue, exdays) {
    const d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    let expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}
function GetCookie(cname) {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') { c = c.substring(1); }
        if (c.indexOf(name) == 0) { return c.substring(name.length, c.length); }
    }
    return "";
}

function EraseCookie(cname) {
    document.cookie = cname + '=; Max-Age=0; path=/;';
}

const ConvertTime = function (date, hour = 0, minute = 0, second = 0, milisecond = 0) {
    if (date === '') return '';
    datePart = date.split('-');
    let month = parseInt(datePart[1]) - 1;
    let numberDate = new Date(datePart[2], month, datePart[0], hour, minute, second, milisecond).getTime();

    return numberDate;
}

var showDialogSuccess = function (message) {
    console.log(message)
    swal("Success", message, "success") 
}

const Pagination = function (searchModel, TotalPage, input, getDataFunction) {
    var html = '';
    var frist = '<li name="Frist" class="page-item page-indicator"><a class="page-link" style="padding:0 10px; border-radius: 10px !important; width: 100px;"  href= "javascript:void(0)">Trang Đầu</a></li>';
    var previous = '<li name="Previous" class="page-item page-indicator"><a class="page-link"  href= "javascript:void(0)"><i class="la la-angle-left"></i></a></li>';
    var next = '<li name="Next" class="page-item page-indicator"><a class="page-link"  href= "javascript:void(0)"><i class="la la-angle-right"></i></a></li>';
    var last = '<li name="Last" class="page-item page-indicator"><a class="page-link" style="padding:0 10px; border-radius: 10px !important; width: 100px;"  href= "javascript:void(0)">Trang Cuối</a></li>';

    if (searchModel.CurrentPage != 1 && searchModel.CurrentPage > 0) {
        html += frist + previous;
    }
    if (searchModel.CurrentPage - 4 > 1)
        html += '<li><a href= "#">...</a></li>';
    var start = searchModel.CurrentPage - 4;
    if (start < 1)
        start = 1;
    for (let i = start; i <= TotalPage; i++) {
        if (i == searchModel.CurrentPage + 5)
            break;
        if (i == searchModel.CurrentPage)
            html += '<li class="page-item active"><a class="page-link" href="javascript:void(0)">' + i + '</a></li>';
        else
            html += '<li name="PaginationItem" class="page-item "><a class="page-link" href="javascript:void(0)">' + i + '</a></li>';
    }

    if (searchModel.CurrentPage + 4 < TotalPage)
        html += '<li><a href= "#">...</a></li>';

    if (searchModel.CurrentPage != TotalPage && TotalPage > 0) {
        html += next + last;
    }
    if (input != undefined && input != '')
        input = $(input).find('[name=pagination]');
    else
        input = $('#pagination')

    $(input).html(html);

    $(input).find('[name=Frist]').on('click', function () {
        searchModel.CurrentPage = 1;
        if (getDataFunction !== undefined && getDataFunction !== '')
            getDataFunction(searchModel);
        else
            getdata(searchModel);
    })


    $(input).find('[name=Previous]').on('click', function () {
        searchModel.CurrentPage = searchModel.CurrentPage - 1;
        if (getDataFunction !== undefined && getDataFunction !== '')
            getDataFunction(searchModel);
        else
            getdata(searchModel);
    })


    $(input).find('[name=Next]').on('click', function () {
        searchModel.CurrentPage = searchModel.CurrentPage + 1;
        if (getDataFunction !== undefined && getDataFunction !== '')
            getDataFunction(searchModel);
        else
            getdata(searchModel);
    })


    $(input).find('[name=Last]').on('click', function () {
        searchModel.CurrentPage = TotalPage;
        if (getDataFunction !== undefined && getDataFunction !== '')
            getDataFunction(searchModel);
        else
            getdata(searchModel);
    })


    $(input).find('[name=PaginationItem]').on('click', function () {
        var page = $(this).find('a').html();
        searchModel.CurrentPage = parseInt(page);
        if (getDataFunction !== undefined && getDataFunction !== '')
            getDataFunction(searchModel);
        else
            getdata(searchModel);
    });

    /*var previous = '<li name="Previous" class="page-item page-indicator">< a class="page-link" href = "" ></a ></li > ';
    var next = '<li name="Next" class="page-item page-indicator"><a class="page-link" href=""></a></li>';*/

}


const ENUM_VIEW_ROOM = {
    "CITY_VIEW": "View nhìn ra thành phố",
    "SEA_VIEW": "View nhìn ra biển",
    "GARDEN_VIEW": "View nhìn ra vườn",
    "LAKE_VIEW": "View nhìn ra hồ bơi"
}

