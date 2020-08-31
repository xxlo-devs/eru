document.addEventListener('DOMContentLoaded', () => {
    // Get all "navbar-burger" elements
    const $navbarBurgers = Array.prototype.slice.call(document.querySelectorAll('.navbar-burger'), 0)

    // Check if there are any navbar burgers
    if ($navbarBurgers.length > 0) {
        // Add a click event on each of them
        $navbarBurgers.forEach(el => {
            el.addEventListener('click', () => {
                // Get the target from the "data-target" attribute
                const target = el.dataset.target
                const $target = document.getElementById(target)

                // Toggle the "is-active" class on both the "navbar-burger" and the "navbar-menu"
                el.classList.toggle('is-active')
                $target.classList.toggle('is-active')
            })
        })
    }
})
if(!window.remove_class_button) {
    window.remove_class_button = undefined;
}
async function refreshData (force = false) {
    if (window.localStorage.getItem('auto-refresh') === 'true' || force) {
        await window.fetch('/admin/status')
            .then(res => {
                if (res.ok) {
                    res.json().then(json => {
                        const { days, hours, minutes, seconds } = json.uptime
                        document.querySelector('#uptime').textContent = `${days} Days ${hours} Hours ${minutes} Minutes ${seconds} Seconds`
                        document.querySelector('#subscribers').textContent = json.subscribers
                        const classesTable = document.querySelector('#classes-table>tbody')
                        json.classes.forEach((e) => {
                            const { id, name, subscribersCount } = e;
                            if (Array.prototype.slice.call(classesTable.children).filter(x => x.innerText.startsWith(id)).length === 0) {
                                const row = classesTable.insertRow(0)
                                row.insertCell(0).textContent = id
                                row.insertCell(1).textContent = name
                                row.insertCell(2).textContent = subscribersCount
                                const removeButton = document.createElement('button')
                                removeButton.textContent = window.remove_class_button;
                                removeButton.addEventListener('click', async function () {
                                    await removeClass(id)
                                })
                                row.insertCell(3).appendChild(removeButton)
                            }
                        })
                    })
                } else {
                    console.error(`Couldn't load data from server. ${res.statusText} ${res.status}`)
                }
            })
    }
}
async function createClass () {
    const year = document.querySelector('#year-input').value
    const section = document.querySelector('#section-input').value
    if (year && section) {
        await window.fetch(`/admin/class?year=${year}&section=${section}`, {
            method: 'POST'
        }).then(res => {
            if (res.ok) {
                refreshData(true)
            } else {
                window.alert('Couldn\'t create new class with given name!')
            }
        })
    }
}
async function removeClass (id) {
    if (id) {
        await window.fetch(`/admin/class?id=${id}`, {
            method: 'DELETE'
        }).then(res => {
            if (res.ok) {
                const classesTable = document.querySelector('#classes-table>tbody')
                classesTable.removeChild(Array.from(classesTable.children).filter(x => x.innerText.startsWith(id))[0])
                refreshData(true)
            } else {
                window.alert('Couldn\'t remove class with given name!')
            }
        })
    }
}

function toggleRefresh() {
    localStorage.setItem('auto-refresh', document.querySelector('#auto-refresh').checked);
}

document.querySelector('#auto-refresh').checked = window.localStorage.getItem('auto-refresh') === 'true';
setInterval(refreshData, 1000);
refreshData(true).then();