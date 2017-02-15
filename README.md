# Ada

## Deployment
### Microsoft Cognitive Services

The first step to deploy the web application is to subscribe to Microsoft Cognitive Services Face and Emotion apis and get keys.

On Microsoft Cognitive Face web console (https://goo.gl/afkZ6R), create a new persons group with a generated guid as group id.

### Web Application + SQL Database

The next step is to deploy WebApp + SQL services and make this configuration :

In web application → Application Settings → App settings, the value of this keys must be set :
* **OxfordFaceApiKey** : The key of the Microsoft Cognitive Face api
* **OxfordEmotionApiKey** : The key of the Microsoft Cognitive Emotion api
* **OxfordPersonGroupId** : The id of person group created on Microsoft Cognitive Face api
* **Host** : the full address of the website (ex : http://mywebsite.azurewebsites.net)

**Warning : The keys are case sensitive !**

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
