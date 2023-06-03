# ProMaSy .NET Core

Rewrite of original [ProMaSy](https://github.com/AndriiLab/promasy) as web-application on .NET Core as backend and
Vue.js as frontend.

- Backend implemented
  as [minimal API](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-6.0) similar
  to my [Minimal API Demo](https://github.com/AndriiLab/Minimal_API_Demo).
- Vue.js web application is building and serving by [Vite](https://vitejs.dev)
- Web application theming is based on PrimeFaces [Sakai Vue theme](https://www.primefaces.org/sakai-vue)
  and [Prime Vue UI](https://www.primefaces.org/primevue)

### Warning: Current version is still under development and do not have required features from [ProMaSy](https://github.com/AndriiLab/promasy)!

## Requirements

- .NET Core 7
- PostgreSQL 14

## How to run

- Install .NET dependencies in whole solution
- Install NPM dependencies in project Promasy.Web.Frontend
- Configure database connection in `Promasy.Web.Api/appsettings.json`:`ConnectionStrings:DatabaseConnection`
- Create auth secret key in `Promasy.Web.Api/appsettings.json`:`AuthToken:Secret`,
- Configure defaults in `Promasy.Web.Api/appsettings.json`:`DefaultOrganizationSeed`,
- Execute API from `Promasy.Web.Api/Properties/launchSettings.json`
- Execute web-application `npm run dev` from `Promasy.Web.Frontend`
- Swagger is available on `https://localhost:5001/swagger`
- Web application is available on `https://localhost:5173`

## Implemented

- DB migration from previous version of ProMaSy (SQL script in folder `Migration`)
- Support of old password validation scheme from desktop version of ProMaSy (scheme updated automatically after user login)
- Authentication and authorization
- Most CRUDs

## TODO

- Reports generation as CSV/XLSX
- Dashboard
