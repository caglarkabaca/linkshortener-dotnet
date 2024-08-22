
function postVisitLog(from, to, id, name, tagname, raw) {
    const BASE_URL = "https://localhost:7031"
    fetch(BASE_URL + "/VisitLog", {
        method: "POST",
        body: JSON.stringify({
            from: from,
            to: to,
            htmlTagId: id,
            htmlTagName: name,
            htmlTag: tagname,
            htmlTagRaw: raw
        }),
        headers: {
            "Content-type": "application/json; charset=UTF-8"
        }
    });
}

// const aTags = document.querySelectorAll('a')
// aTags.forEach((tag) => {
//     tag.addEventListener("click", () => {
//         console.log(`clicked to id:${tag.id} name:${tag.name} tag:${tag.tagName} raw:${tag.outerHTML}`)
//         console.log(`from: ${window.location.href} to: ${tag.href}`)
//         postVisitLog(window.location.href, tag.href, tag.id, tag.name, tag.tagName, tag.outerHTML)
//     })
// })
//
// const inputTags = document.querySelectorAll('input')
// inputTags.forEach((tag) => {
//     tag.addEventListener("click", () => {
//         console.log(`clicked to id:${tag.id} name:${tag.name} tag:${tag.tagName} raw:${tag.outerHTML}`)
//         console.log(`from: ${window.location.href} to: ${tag.form.action}`)
//         postVisitLog(window.location.href, tag.form.action, tag.id, tag.name, tag.tagName, tag.outerHTML)
//     })
// })

window.addEventListener("click", (event) => {
    const tag = event.target
    // console.log(`clicked to id:${tag.id} name:${tag.name} tag:${tag.tagName} raw:${tag.outerHTML}`)
    // console.log(`from: ${window.location.href} to: ${tag.form.action}`)
    postVisitLog(window.location.href, null, tag.id, tag.name, tag.tagName, tag.outerHTML)
    })

// https://stackoverflow.com/questions/24081699/why-onbeforeunload-event-is-not-firing
window.addEventListener("beforeunload", () => {
    postVisitLog(window.location.href, null, null, null, "LEAVING", null)
})

window.addEventListener("load", () => {
    postVisitLog(document.referrer, window.location.href, null, null, "ENTERING", null)
});