# lib-sql

## Description

This repo contains the source code for the `WordList.Data.Sql` shared library, which is used by the lambdas forming the Word List application's backend processing chain for updating word scores.

This library contains functions for accessing the Words database.

## Environment Variables

| Variable Name        | Description                                             |
|----------------------|---------------------------------------------------------|
| DB_CONNECTION_STRING | Connection string for connection to the words database. |

## Common Packages

This library is published on GitHub.  To be able to import it, you'll need to use the following command:

```
dotnet nuget add source --username <your-username> --password <github-PAT> --store-password-in-clear-text --name github "https://nuget.pkg.github.com/word-list/index.json"
```