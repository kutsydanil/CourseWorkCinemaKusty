// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

async function sendStaff(eventId) {
    let ul = document.getElementById("staff-list")
    let ids = Array.from(ul.getElementsByTagName("select")).map(s => s.value)

    let url = location.href.split('/')

    console.log(url)

    url = '/' + url[3] + '/' + url[4] + '/' + url[5].split('?')[0]

    let params = ''

    for (let i = 0; i < ids.length; i++) {
        params += `StaffsId=` + ids[i]
        if (i + 1 != ids.length) {
            params += '&'
        }
    }

    document.location.href = url + '?' + params
}
