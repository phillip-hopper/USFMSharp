var timerInterval = 700;
var timeout = null;
var usfmContent = null;
var openingTag = '<span class="usfmTag">';
var closingTag = '</span>';
var knownTags_re = null;
var knownTags = [
    new RegExp(/(\\id|\\ide|\\h)(\s+)/), // book id, encoding and heading
    new RegExp(/(\\sts|\\rem)(\s+)/),    // status and remarks
    new RegExp(/(\\toc[0-9])(\s+)/),     // table of contents
    new RegExp(/(\\mt[0-9]?)(\s+)/),     // major titles
    new RegExp(/(\\p)(\s+)/),
    new RegExp(/(\\v [0-9-]+)(\s+)/),
    new RegExp(/(\\c [0-9]+)(\s+)/),

];
var unknownTags_re = /[^>](\\\w+)(\s+)/g;
var openingUnknownTag = '<span class="unknownTag">';

function initializePage() {

    // build the usfm tags regexp
    var temp_re = [];
    for (var i = 0; i < knownTags.length; i++) {
        tag = knownTags[i]
        temp_re.push('(?:' + tag.source + ')');
    }
    knownTags_re = new RegExp(temp_re.join('|'), 'g');

    // mark up any existing tags in the text
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

    // mark the known USFM tags in the text
    usfm = markTheseTags(usfm, knownTags_re, openingTag, 0);

    // mark the unknown USFM tags
    usfm = markTheseTags(usfm, unknownTags_re, openingUnknownTag, 1);

    // set the display HTML
    usfmContent.innerHTML = usfm;

    // try to put the cursor back where it was
    restoreRangePosition(rp);
}

function markTheseTags(usfm, tag_re, open_tag, offset) {

    // get the selected tags from the text
    var matches = [];
    while (true) {
        var match = tag_re.exec(usfm);
        if (!match) break;
        matches.push(match);
    }

    // surround the USFM tags with a formatting span
    for (var i = matches.length - 1; i > -1; i--) {
        var m = matches[i];
        var s = [];
        for (var j = 1; j < m.length; j++) {
            if (typeof m[j] !== 'undefined') s.push(m[j]);
        }
        usfm = usfm.substr(0, m.index + offset) + open_tag + s[0] + closingTag + s[1] + usfm.substr(m.index + m[0].length);
    }

    return usfm;
}

function keyPress() {
    if (timeout) window.clearTimeout(timeout);
    timeout = window.setTimeout(markUsfmTags, timerInterval);
}

// saveRangePosition() and restoreRangePosition() were borrowed from this StackOverflow answer:
// http://stackoverflow.com/questions/4576694/saving-and-restoring-caret-position-for-contenteditable-div/26477716#26477716
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
