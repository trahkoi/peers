@description('Name of the App Service. Must be globally unique.')
param appName string

@description('Azure region for all resources.')
param location string = resourceGroup().location

@description('App Service Plan SKU.')
@allowed(['F1', 'B1', 'B2', 'B3'])
param appServicePlanSku string = 'B1'

var planName = '${appName}-plan'

resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: planName
  location: location
  sku: {
    name: appServicePlanSku
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}

resource appService 'Microsoft.Web/sites@2023-01-01' = {
  name: appName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|10.0'
      appSettings: [
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
        {
          name: 'AdminCredentials__Username'
          value: ''
        }
        {
          name: 'AdminCredentials__Password'
          value: ''
        }
        {
          name: 'ConnectionStrings__Sessions'
          value: 'Data Source=/home/data/peers.db'
        }
      ]
    }
    httpsOnly: true
  }
}

@description('Default hostname of the deployed App Service.')
output hostname string = appService.properties.defaultHostName
