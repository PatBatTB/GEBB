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

## Getting started

### Prerequisites

- Для сборки проекта необходимо иметь установленную `.NET SDK 9.0`
___
- To build the project, you must have the `.NET SDK 9.0` installed.

[Download SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

- Установите утилиту `xmlstarlet`
___
- Install the `xmlstarlet` utility

Debian/Ubuntu
```bash
$ apt install xmlstarlet
```

### Installation

- Клонируйте репозиторий.
___
- Clone the repo.

```bash
git clone https://gitlab.com/PatBatTB/GEBB.git
```

- В скрипте ниже измените `path_to_output_dir` на путь к существующей директории.
  Данный скрипт выполняет полную сборку со всеми зависимостями под операционную систему `linux-x64`
___
- In the script below, replace `path_to_output_dir` with the path to an existing directory.
  This script performs a complete build with all dependencies for the Linux x64 operating system.

```bash
dotnet publish -c Release -r linux-x64 -v d -p:PublishDir=path_to_output_dir,PublishSingleFile=true
```

Для дальнейшего запуска сборки dotnet SDK или Runtime <ins>не нужен</ins>
___
You <ins>do not need</ins> the .NET SDK or Runtime to running the build.

## Usage

### Настройка файла запуска start.sh / Configuring the runnable script start.sh

- Укажите токен своего бота в переменной `BOT_TOKEN`
- Укажите желаемый уровень логирования в переменной `LOG_LEVEL`
___
- Enter the token for your bot in the `BOT_TOKEN` variable.
- Enter the desired minimum logging level in the `LOG_LEVEL` variable.

```text
BOT_TOKEN="<your_bot_token>"
LOG_LEVEL="<logging level>"
```

- Сделайте файл `start.sh` исполняемым.
___
- Make the `start.sh` file executable.

```bash
chmod +x ./start.sh
```
### Запуск / Launch

Запустите файл `start.sh`
___
Launch the `start.sh` file

## Troubleshooting
...

## Changelog
Can be found in [CHANGELOG](CHANGELOG.md).

## Authors
* Patrick Bates - [PatBatTB](https://github.com/PatBatTB)

## Code of Conduct
Please, follow [Code of Conduct](CODE_OF_CONDUCT.md) page.

## License
This project is Apache License 2.0 - see the [LICENSE](LICENSE) file for details
