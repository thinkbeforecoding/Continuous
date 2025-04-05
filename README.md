# Continuous

This repository has moved to Codeberg: https://codeberg.org/thinkbeforecoding/Continuous

This is a sample project to demo how to get continuous integration further with F#

By using SqlCommandProvider and deploying database locally, the compiler can check at compile time that
the sql generation/migration scripts, the Sql query strings and the code that use is are
in line by typechecking it all.

The whole thing can then be used easily on any build server for continuous integration and continuous deployment.


## Prerequisites

The sample use Sql Server Express for development and Sql Server local DB on the build server.

Editing the project require Visual Studio Data Tools.

That said, you can change slightly the project to use Sql scripts directly and integrate with
other databases like Postgre Sql.

## Build

Clone the repository and execute de build.cmd file.






