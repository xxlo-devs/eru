import './scss/common.scss'
import './scss/dashboard.scss'
import 'regenerator-runtime/runtime'
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
document.querySelector('#auto-refresh').checked = localStorage.getItem('auto-refresh') === 'true'
async function refreshData (force = false) {
    if (localStorage.getItem('auto-refresh') === 'true' || force) {
        await fetch('/admin/status')
            .then(res => {
                if (res.ok) {
                    res.json().then(json => {
                        const { days, hours, minutes, seconds } = json.uptime
                        document.querySelector('#uptime').textContent = `${days} Days ${hours} Hours ${minutes} Minutes ${seconds} Seconds`
                        document.querySelector('#subscribers').textContent = json.subscribers
                        const classesTable = document.querySelector('#classes-table>tbody')
                        json.classes.forEach(({ id, name, subscribersCount }) => {
                            if (Array.prototype.slice.call(classesTable.children).filter(x => x.innerText.startsWith(id)).length === 0) {
                                const row = classesTable.insertRow(0)
                                row.insertCell(0).textContent = id
                                row.insertCell(1).textContent = name
                                row.insertCell(2).textContent = subscribersCount
                                const removeButton = document.createElement('button')
                                removeButton.textContent = 'Remove'
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
setInterval(refreshData, 1000)
document.querySelector('#create-class-button').onclick = createClass
async function createClass () {
    const year = document.querySelector('#year-input').value
    const section = document.querySelector('#section-input').value
    if (year && section) {
        await fetch(`/admin/class?year=${year}&section=${section}`, {
            method: 'POST'
        }).then(res => {
            if (res.ok) {
                refreshData(true)
            } else {
                alert('Couldn\'t create new class with given name!')
            }
        })
    }
}
async function removeClass (id) {
    if (id) {
        await fetch(`/admin/class?id=${id}`, {
            method: 'DELETE'
        }).then(res => {
            if (res.ok) {
                const classesTable = document.querySelector('#classes-table>tbody')
                classesTable.removeChild(Array.from(classesTable.children).filter(x => x.innerText.startsWith(id))[0])
                refreshData(true)
            } else {
                alert('Couldn\'t remove class with given name!')
            }
        })
    }
}
refreshData(true)
