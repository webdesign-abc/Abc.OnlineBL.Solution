============================================================================================
INTERFACE GUIDELINES
============================================================================================

- WCF doesn't allow overloading functions so use 1 function and try to accommondate all parameters
	for use custom class to hold all parameters.
- If there is a need for List<EntityRelations> loadOptions parameter then it should be the last parameter.

============================================================================================
DATASTORE GUIDELINES
============================================================================================

- A T4 template is broken up to 3 templates.
	1) First part is to generate the data context only in the Abc.OnlineBL.DataStore
	2) Second part is to generate the entities class files in Abc.OnlineBL.Entities
	3) Third part is to generate the entity relations.
	
- In Abc.OnlineBL.DataStore the template is modified to use a connection string from a custom config.
- In Abc.OnlineBL.Entities the template is modified to change the UpdateCheck attribute of a column
	to UpdateCheck.Never if the column is not the timestamp type otherwise we have problem save
	databack using LINQEntityBase features.
- Regenerate the DBML classes from both Abc.OnlineBL.DataStore and Abc.OnlineBL.Entities project if you update
  the DBML file. In Abc.OnlineBL.Entities project there is two T4 file needs to be touched to generate 
  the entities and it relations
- The easiest way to update an Entity is to drop it from DBML and refresh and put it back in from
  Server Explorer. Don't forget to regenerate the classes from both project as states above.

	
============================================================================================
WCF GUIDELINES
============================================================================================
	
- Interface name must start with I
- Service name must be the same as the Interface name with out the I ending with .svc
- Service Must be implemented in Abc.OnlineBL.Service.Implementation Project.
- For hosting use Abc.OnlineBL.Service.Host project

============================================================================================
WCF PROXY GUIDELINES
============================================================================================
	
- Everytime you update/modify Abc.OnlineBL.Service Interfaces you must regenerate the Service Proxy
- The service proxy project makes use of the Abc.OnlineBL.Utility library for Configuration of the
  endpoint address of OnlineBL service. Make sure you have added the Abc.OnlineBL.Utility reference and
  the following config in your App.config of the client application:
  
  <abcSettings activeProfile="ABC_BETA">
	<profile name="ABC_BETA">
		<add key="OnlineBL_ROOT_URL" value="http://localhost/Abc.OnlineBL.Service.Host/"/>
	</profile>
  </abcSettings>