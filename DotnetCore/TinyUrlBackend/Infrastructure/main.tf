# 1. Define the Azure Provider
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
}

provider "azurerm" {
  features {}
  # This is the "Magic Line" for standard accounts to prevent the hang
  skip_provider_registration = true
}

# 2. Create a Resource Group 
resource "azurerm_resource_group" "rg" {
  name     = "rg-tinyurl-prod"
  location = "Central US"
}

# 3. Create a Random ID to keep names unique
resource "random_id" "id" {
  byte_length = 4
}

# 4. Create a Storage Account 
resource "azurerm_storage_account" "storage" {
  name                     = "sttinyurl${random_id.id.hex}"
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

# 5. Create Azure SQL Server & Database 
resource "azurerm_mssql_server" "sqlserver" {
  name                         = "sql-tinyurl-srv-${random_id.id.hex}"
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = azurerm_resource_group.rg.location
  version                      = "12.0"
  administrator_login          = "tinyadmin"
  administrator_login_password = "Password1234!" # Secure this later
}

resource "azurerm_mssql_database" "db" {
  name      = "db-tinyurl"
  server_id = azurerm_mssql_server.sqlserver.id
  sku_name  = "Basic" # Cheapest option for students
}

# 6. Create App Service Plan 
resource "azurerm_service_plan" "plan" {
  name                = "plan-tinyurl"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  os_type             = "Linux"
  sku_name            = "F1" # FREE TIER
}

# 7. Create the Web App
resource "azurerm_linux_web_app" "webapp" {
  name                = "app-tinyurl-${random_id.id.hex}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_service_plan.plan.location
  service_plan_id     = azurerm_service_plan.plan.id

  site_config {
    application_stack {
      dotnet_version = "8.0"
    }
    # ADD THIS LINE BELOW
    always_on = false 
  }

  app_settings = {
    "AppSettings__BaseDomain" = "https://app-tinyurl-${random_id.id.hex}.azurewebsites.net"
    "SecretToken"             = "MyCloudSecret123"
  }
}