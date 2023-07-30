# Infrastructure

## Overview
This project contains all sorts of configurations that are needed to make this application functional. Without this project, the application will be unable to work with its respective databases, authorize users properly, and so on.

## Navigation

### AuthConfig
Holds custom policy handlers and requirements, alongside a custom ``AppJwtBearerEvents`` class which, in addition to validating the user's JWT, also checks if it exists in the JWT blacklist.

### Data
Holds Entity Framework Core / Redis configurations and entities.

### Extensions
Holds custom extensions. Currently, there are extensions for queries/collections, normalization of strings, and for seeding via the built app.

### Filters
Holds custom filters

### Migrations
Holds migrations and snapshots for Entity Framework Core

### ModelBinders
Holds custom model binders.



In addition, the ``Infrastructure`` project directly provides a ``Repository`` class, which abstracts the ``DbContext`` away from the services. The repository is entity-agnostic and can be used for any entity through generics. The class is registered in the DI container under the ``IRepository`` interface.

