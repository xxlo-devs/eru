﻿'use strict'
document.addEventListener('DOMContentLoaded', () => {
    (document.querySelectorAll('.notification .delete') || []).forEach(($delete) => {
        const $notification = $delete.parentNode
        $delete.addEventListener('click', () => {
            $notification.parentNode.removeChild($notification)
        })
    })
})

function displayFileName() {
    document.getElementById('file-name').textContent = String(document.querySelector('#file-input').value).split('\\').reverse()[0];
}

function cancel() {
    document.getElementById('file-name').textContent = "";
    document.getElementById('file-input').value = null;
    document.getElementById('key-input').value = "";
}
if(!window.successMessage) {
    window.successMessage = undefined;
}
function submit() {
    const file = document.getElementById('file-input').files[0];
    if (file) {
        const reader = new FileReader();
        reader.readAsText(file, 'CP1250');
        reader.onload = async function(evt) {
            const data = evt.target.result.replace(/[\w\W]+?\n+?/, "");
            await fetch('substitutions', {
                method: 'POST',
                body: data,
                headers: {
                    'Content-Type': 'application/xml',
                    'Api-Key': document.getElementById('key-input').value
                }
            }).then(response => {
                cancel();
                if (response.ok) {
                    addNotification(window.successMessage, ['is-success']);
                } else {
                    response.json().then(json => {
                        let errors = [];
                        for (const property in json.errors) {
                            if (json.errors.hasOwnProperty(property)) {
                                errors = errors.concat(json.errors[property]);
                            }
                        }
                        addNotification(`${json.title} (${errors.join(' ')})`, ['is-danger'])
                    });
                }
            });
        }
    }
}

function addNotification(content, classes) {
    let notification = document.createElement('div');
    notification.classList.add('notification');
    classes.forEach(c => notification.classList.add(c));
    let closeButton = document.createElement('button');
    closeButton.classList.add('delete');
    closeButton.onclick = function() {
        notification.parentNode.removeChild(notification);
    };
    notification.append(closeButton);
    notification.append(content);
    document.getElementById('form-container').prepend(notification);
}