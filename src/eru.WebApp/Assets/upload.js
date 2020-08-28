import './scss/common.scss'
import './scss/upload.scss'
import 'boxicons'
import 'regenerator-runtime/runtime'

document.addEventListener('DOMContentLoaded', () => {
    (document.querySelectorAll('.notification .delete') || []).forEach(($delete) => {
        const $notification = $delete.parentNode
        $delete.addEventListener('click', () => {
            $notification.parentNode.removeChild($notification)
        })
    })
})
