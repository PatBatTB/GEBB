## Overview
Сервис GEBB предназначен для планирования и организации посещения мероприятий, автоматизации процесса поиска спутников, 
регистрации пользователей в группы и обмена контактами между зарегистрированными на мероприятие участниками.
Сервис не предполагает выбор конкретных участников события или отправку приглашений выбранным пользователям. 
Рассылка приглашений отправляется всем участникам программы, а регистрация производится в “естественном порядке” 
(кто раньше подтвердил участие). 
Таким образом сервис предназначен для использования внутри групп знакомых между собой пользователей, 
либо связанных между собой какими-либо другими факторами 
(например внутри одной организации, как средство повышения корпоративной культуры).
Сервис не имеет пользовательского интерфейса, 
взаимодействие производится через обмен сообщениями с ботом в приложении Telegram.
___
The GEBB service is designed to plan and organise participation in events, 
automate the process of searching for satellites, 
register users in groups and exchange contacts between participants registered for the event. 
The service does not include selection of specific event participants or sending invitations to selected users. 
Invitations are sent to all participants in the programme and registration takes place in "natural order" 
(those who have previously confirmed their participation). 
Thus, the service is intended for use within groups of users who know each other or are connected by some other factors 
(e.g. within the same organisation as a means of enhancing corporate culture). 
The service does not have a user interface, 
the interaction takes place through messaging with a bot in the Telegram application.

## Building

Для сборки проекта необходимо иметь установленную `.NET SDK 9.0`
В скрипте ниже измените `path_to_output_dir` на путь к существующей директории.
Данный скрипт выполняет полную сборку со всеми зависимостями под операционную систему `linux-x64`
___
To build the project, you must have the `.NET SDK 9.0` installed.
In the script below, replace `path_to_output_dir` with the path to an existing directory.
This script performs a complete build with all dependencies for the Linux x64 operating system.

```bash
dotnet publish -c Release -r linux-x64 -v d -p:PublishDir=path_to_output_dir,PublishSingleFile=true
```

Для дальнейшего запуска сборки не нужен dotnet SDK или Runtime
___
You do not need the .NET SDK or Runtime to running the build.


## Troubleshooting
...

## Release Notes
Can be found in [RELEASE_NOTES](RELEASE_NOTES.md).

## Authors
* Patrick Bates - [PatBatTB](https://github.com/PatBatTB)

## Code of Conduct
Please, follow [Code of Conduct](CODE_OF_CONDUCT.md) page.

## License
This project is Apache License 2.0 - see the [LICENSE](LICENSE) file for details
