var timerInterval = 700;
var re_p = /(\\p)(\s+)/g;
var timeout = null;
var usfmContent = null;
var openingTag = '<span class="usfmTag">';
var closingTag = '</span>';

var knownTags = [
    /(\\p)(\s+)/g,
    /(\\v [0-9-])(\s+)/g
];

function initializePage() {
    usfmContent = document.getElementById('usfm-content');
    usfmContent.focus();
    markUsfmTags();
}

function markUsfmTags() {

    // clear the timer handle
    timeout = null;

    // remember the current cursor location
    var rp = saveRangePosition();

    // get the USFM, without HTML tags
    var usfm = usfmContent.innerText;

    // get the USFM tags from the text
    var matches = [];

    // todo: need to sort the matches returned from knownTags by index
    while (true) {
        var match = re_p.exec(usfm);
        if (!match) break;
        matches.push(match);
    }

    // surround the USFM tags with a formatting span
    for (var i = matches.length - 1; i > -1; i--) {
        var m = matches[i];
        usfm = usfm.substr(0, m.index) + openingTag + m[1] + closingTag + m[2] + usfm.substr(m.index + m[0].length);
    }

    // set the display HTML
    usfmContent.innerHTML = usfm;

    // try to put the cursor back where it was
    restoreRangePosition(rp);
}

function keyPress() {
    if (timeout) window.clearTimeout(timeout);
    timeout = window.setTimeout(markUsfmTags, timerInterval);
}

function saveRangePosition() {
    var range=window.getSelection().getRangeAt(0);
    var sC=range.startContainer,eC=range.endContainer;

    var a = []; while(sC!==usfmContent){a.push(getNodeIndex(sC)); sC=sC.parentNode}
    var b = []; while(eC!==usfmContent){b.push(getNodeIndex(eC)); eC=eC.parentNode}

    return {"sC":a,"sO":range.startOffset,"eC":b,"eO":range.endOffset};
}

function restoreRangePosition(rp) {
    usfmContent.focus();
    var sel = window.getSelection(), range=sel.getRangeAt(0);
    var x, c, sC=usfmContent,eC=usfmContent;

    c=rp.sC; x=c.length; while(x--)sC=sC.childNodes[c[x]];
    c=rp.eC; x=c.length; while(x--)eC=eC.childNodes[c[x]];

    range.setStart(sC, rp.sO);
    range.setEnd(eC, rp.eO);
    sel.removeAllRanges();
    sel.addRange(range);
}

function getNodeIndex(n) {
    var i=0;
    // ReSharper disable once AssignmentInConditionExpression
    while(n=n.previousSibling) i++;
    return i;
}