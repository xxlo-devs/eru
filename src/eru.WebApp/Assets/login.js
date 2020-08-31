import './scss/common.scss'
import './scss/login.scss'

document.querySelector('#cancel-button').onclick = function () {
    document.querySelector('#Model_Username').value = '';
    document.querySelector('#Model_Password').value = '';
}