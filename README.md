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
  "sitename": string,
  "sitelogo": string,
  "defaultpass": string,
  "baseurl": string,
  "defaultadmins": [
    {
      "username": string,
      "email": string,
      "password":  string
    }
  ]
}
```

`dbprovider` is an enum value where 0 = PostgreSQL, 1 = Sqlite, and 2 = InMemory.

`dbstring` is the connection string to be used when connecting to an engine other than InMemory.

`sitename` is the name of your site.

`sitelogo` is the URL path to the image you want to use as the logo.

`defaultpass` is the default password for deleting Threads. Make sure that it is longer than 8 characters.

`baseurl` is the base url of the application.

`defaultadmins` is an IEnumerable of DefaultAdmin and is used to add a default list of admins when starting up. See below for details on the object.

#### DefaultAdmin

`Username` is the username for the Admin user.

`Email` is the email for the Admin user.

`Password` is the password for the Admin user. Make sure that it is longer than 8 characters.

## Cool stuff with CHANdler
[ChandlerSharpPlus](https://github.com/li223/ChandlerSharpPlus) by [Li223](https://github.com/li223)
