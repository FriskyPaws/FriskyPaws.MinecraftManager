# FriskyPaws Minecraft Manager

## Configuring

- Copy `appsettings.example.json` to `appsettings.local.json`
- Edit `appsettings.local.json` and put in your details

## FAQ

### Why?

Because a lot of the scripts I found for automatically doing minecraft updates were just "drop kick the server" this one will notify users with a 5 and 1 minute warning via rcon

### No backups?

Not yet

### No Windows?

I will if there is interest, as far as I can think there is a only one item that needs to be changed, the way we start/stop minecraft, currently that depends on bash and expects bash in a specific place.

### Why C#? The freaking thing is 20mb!

If I want to mature it into a web-based inferface or something, it's trivial, yeah it's heafty but it's a single file app. I could probably trim it further... but eh.

### Will you add [x]?

Feel free to open an issue

### Why aren't there any tests?

Because I'm awful and wrote this in like 2 hours.

## Possible future ideas?

### Command-line argument support

Why use appsettings.local.json when we can feed it all in via command line args? Allows users to run mutliple servers with one manager

### Web interface

Further server management tools/expose RCON/Add auth?