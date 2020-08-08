# I want to help -> Contribution 101

You want to help? Great! Here are some ideas and tips how to get started.

## Add more message services.

Message service is a service that we use to communicate with users (eg. email, messenger, discord etc.).
In order to create new message service you need to design class inheriting from IMessageService that is able to on its own handle all incoming messages, send messages, handle subscriptions (preferably by saving their ids into database via EF Core).
You can't code but you want a new mean of communication to always stay in touch? Create an issue, we'll gladly take a look on it.

## Create better upload page.

We're not a great frontend developers so we'll always gladly accept help in this matter. Here is how (from technical perspective and from point of view of person whose javascript skills aren't that great but at least it works) upload should look like.

index.html
```html
<html>
    <head>
        <title>Simple uploader</title>
    </head>
    <body>
      <input type="file" id="file-input">
      <button id="upload">Upload</button>

      <script src="main.js"></script>
    </body>
</html>
```

main.js
```js
document.getElementById('upload').onclick = upload;

function upload() {
  const file = document.getElementById('file-input').files[0];
  if (file) {
    var reader = new FileReader();
    //AscSubstitution files are encoded with CP1250 so in order not to lose polish letters reading file with this encoding is required.
    reader.readAsText(file, "CP1250");
    reader.onload = async function (evt) {
      //Remove first line with hard-coded CP-1250 encoding (VERY IMPORTANT!!!)
      let data = evt.target.result.replace(/[\w\W]+?\n+?/,"");
      await fetch('https://localhost:5001/substitutions', {
        method: 'POST',
        body: data,
        mode: 'cors',
        headers: {
          'Content-Type': 'application/xml',
          'Api-Key': document.getElementById('key').value
        }
      }).then(() => alert('SENT!'));
    }
    reader.onerror = function (evt) {
      document.getElementById("fileContents").innerHTML = "error reading file";
    }
  }
}

```