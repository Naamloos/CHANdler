# CHANdler
CHANdler is image board software using ASP.NET Core MVC

![](https://i.kym-cdn.com/photos/images/newsfeed/000/779/388/d33.jpg)

## API Documentation
API Documentation is now avaliable at `/docs`.

A live version of the docs can be found at: https://chandler.li223.dev/docs

Alternate link for live docs is: https://chandler.naamloos.dev/docs

## Setup
### Images
Images can be self hosted and placed in `/res`

### Configuration
The configuration file is set out like so:

```
{
  "dbprovider": int,
  "dbstring": string,
  "site": {
    "sitename": string,
    "sitefav": string,
    "sitelogo": string,
    "adminusername": string,
    "adminemail": string,
    "defaultpass": string,
    "baseurl": string
  },
  "discordoauth": {
    "clientid": string,
    "clientsecret": string,
    "redirecturi": string
  }
}
```

`dbprovider` is an enum value where 0 = PostgreSQL, 1 = Sqlite, and 2 = InMemory.

`dbstring` is the connection string to be used when connecting to an engine other than InMemory.

#### Site

`sitefav` is the path to your site's Favicon.

`sitelogo` is the path to your site's logo.

`adminusername` is the Username of the initial Admin User.
 
`adminemail` is the Email of the initial Admin User.

`adminpassword` is the Password of the initial Admin User.

`sitename` is the name of your site.

`sitelogo` is the URL path to the image you want to use as the logo.

`defaultpass` is the default password for deleting Threads. Make sure that it is longer than 8 characters.

`baseurl` is the base url of the application.

#### DiscordOAuth

`clientid` is the Application Id.

`clientsecret` is the Application Secret.

`redirecturi` is the URI to redirect to after signning in.

## Cool stuff with CHANdler
[ChandlerSharpPlus](https://github.com/li223/ChandlerSharpPlus) by [Li223](https://github.com/li223)
