# Ada

## Deployment
### Microsoft Cognitive Services

The first step to deploy the web application is to subscribe to Microsoft Cognitive Services Face and Emotion apis and get keys.(https://www.microsoft.com/cognitive-services/en-us/)

On Microsoft Cognitive Face web console (https://goo.gl/afkZ6R), create a new persons group with a generated guid as group id.

[![](/doc/assets/PersonGroupId.png)]()

### Web application + SQL Database

The next step is to deploy webapp+sql services and make this configuration :

In web application → Application Settings → App settings, the value of this keys must be set :

OxfordFaceApiKey : The key of the Microsoft Cognitive Face api
OxfordEmotionApiKey : The key of the Microsoft Cognitive Emotion api
OxfordPersonGroupId : The id of person group created on Microsoft Cognitive Face api
Host : the full address of website (ex : http://martine-o-bot.azurewebsites.net)
Warning : The keys are case sensitive !

[![](/doc/assets/KeyAzure.PNG)]()

In the Connection strings section, set the connection string to sql database with DefaultConnection as key.

A little more configuration

The default roles and users are created while the first migration of database. To edit them before publish the first time, go to Migrations → Configuration.cs → Seed method.

Warning : Don't forget to adapt credentials of client application if you edit the martineobot user on server

It's possible to personalize the destination of temporary uploaded files and person's picture location in the Global.cs file.

### ApplicationInsight

To use ApplicationInsight, right clic on project → ApplicationInsight → configure → follow the wizard.

Publication on azure

To deploy the application on azure :

Right clic on project → publish
In the wizard, select the azure resource, select database and check pre-compile and migration checkbox.
Publish.

### Application UWP

To use the UWP application you need to add a class named : AppConfig.cs directly in the UWP project.

The class is used for the URL and the login to the web application.

The class must be like : 

```
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaW10
{
    public static class AppConfig
    {
#if DEBUG
        public static readonly string WebUri = "";      // WebApp URL for Test
#else
        public static readonly string WebUri = "";      // WebApp URL fro Prod
#endif
        public static readonly string UserName = "";    // Use to get the API token
        public static readonly string Password = "";
    }
}
```
