# I want to help -> Contribution 101

You want to help? Great! Here are some ideas and tips how to get started.

## Add more message services.

Message service is a service that we use to communicate with users (eg. email, messenger, discord etc.).
In order to create new message service you need to design class inheriting from IMessageService that is able to on its own handle all incoming messages, send messages, handle subscriptions (preferably by saving their ids into database via EF Core).
You can't code but you want a new mean of communication to always stay in touch? Create an issue, we'll gladly take a look on it.