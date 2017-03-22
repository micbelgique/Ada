# Ada

[![](/doc/assets/AdaHello.jpg)]()

## Deployment
### Microsoft Cognitive Services

The first step to deploy the web application is to subscribe to Microsoft Cognitive Services Face and Emotion apis and get keys.(https://www.microsoft.com/cognitive-services/en-us/)

On Microsoft Cognitive Face web console (https://goo.gl/afkZ6R), create a new persons group with a generated guid as group id.

[![](/doc/assets/PersonGroupId.png)]()

### Web application + SQL Database

The next step is to deploy WebApp + SQL services and make this configuration :

In web application → Application Settings → App settings, the value of this keys must be set :
* **OxfordFaceApiKey** : The key of the Microsoft Cognitive Face api
* **OxfordEmotionApiKey** : The key of the Microsoft Cognitive Emotion api
* **OxfordPersonGroupId** : The id of person group created on Microsoft Cognitive Face api
* **Host** : the full address of the website (ex : http://mywebsite.azurewebsites.net)

**Warning : The keys are case sensitive !**

[![](/doc/assets/KeyAzure.PNG)]()

In the Connection strings section, set the connection string to sql database with DefaultConnection as key.

### A little more configuration

The default roles and users are created while the first migration of database. To edit them before publish the first time, go to Migrations → Configuration.cs → Seed method.

Warning : Don't forget to adapt credentials of client application if you edit the martineobot user on server

It's possible to personalize the destination of temporary uploaded files and person's picture location in the Global.cs file.

### ApplicationInsight

To use ApplicationInsight, right clic on project → ApplicationInsight → configure → follow the wizard.

### Publication on azure

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

### Bot

# Step 1

The first step to deploy the bot is to import the 2 different Luis in your Luis account (they are in the folder Luis).

[![](/doc/assets/ImportLuis.PNG)]()


And deploy the 2 Luis in azure : 

[![](/doc/assets/LuisAddKey.PNG)]()
[![](/doc/assets/LuisBuyAzure.PNG)]()


# Step 2

Now you need to deploy the bot on azur. For that you need to create a new web app and publish the bot on this web app.

# Step 3

You need to register your bot on the bot framework website : https://dev.botframework.com.

[![](/doc/assets/RegisterBot.PNG)]()

# Step 4

After that you need to add a file a file AppSettings.config 

```
<?xml version="1.0"?>

<appSettings>
  
	<!-- Keys for BotFramework -->
  
	<add key="BotId" value="" />
  
	<add key="MicrosoftAppId" value="" />
  
	<add key="MicrosoftAppPassword" value="" />

 

	<!-- Keys for LUIS Ada-->
  
	<add key="ModelId" value=""/>
  
	<add key="SubscriptionKey" value=""/>

  

	<!-- Keys for LUIS trivial-->
  
	<add key="ModelIdTrivial" value=""/>
  
	<add key="SubscriptionKeyTrivial" value=""/>

  

	<!-- URI for bot -->
  
	<add key="WebAppUrl" value="" /> <!--Use for the picture-->
 
  
	<add key="FaceBookMIC" value="" /> <!--Link to the Facebook page-->
  
	<add key="YoutubeMIC" value="" /> <!--Link to the youtube page-->
  
	<add key="MeetupMIC" value="" /> <!--Link to the meetup page-->
  
	<add key="SiteMIC" value="" /> <!--Link to the website-->
  
	<add key="IMGYoutube" value="" /> <!--Link to an image for youtube-->
  
	<add key="IMGMIC" value="" /> <!--Link to an image for the society-->
  
	<add key="IMGFacebook" value="" /> <!--Link to an image for Facebook-->
  
	<add key="IMGMeetup" value="" /> <!--Link to an image for the meetup-->
  
	<add key="IMGMore" value=""/> <!--Link to an image for the button more-->

</appSettings>
```


[![](/doc/assets/AddFileConfig.PNG)]()
[![](/doc/assets/FileConfig.PNG)]()
[![](/doc/assets/IdLuis.PNG)]()
[![](/doc/assets/MicrosoftAppId.PNG)]()

# Step 5

If you want to use the bot on facebook messenger or an other application allow by the bot framework. You just need to login to
the bot framework's website and follow the different step to add the bot in the application.
[![](/doc/assets/Application.PNG)]()
