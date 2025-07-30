## v0.5.0
- Добавлено меню для настроек оповещений о приближающихся мероприятиях.
- Реализован сервис рассылающий оповещения на основании индивидуальных настроек пользователей.
- В репозиторий добавлен скрипт для обновления БД с более ранних версий. Подробнее в файле [readme](README.md), в разделе `Update`
___
- Added a menu for setting notifications about upcoming events.
- A service has been implemented that sends alerts based on individual user settings.
- A script has been added to the repository to update the database from earlier versions. For more information, see the [readme](README.md ) file, in the `Update` section
## v0.4.1
- Исправлено: Не находились доступные для регистрации мероприятия без ограничений на количество участников.
- Удалена ссылка на ConsoleAppender в конфигурации логирования.
___
- Fixed: The events without participant limit restriction don't show on available to register events menu.
- Delete the ref to unused ConsoleAppender on the logging config.
___
## v0.4.0
- Добавлена команда `/report` для обратной связи
___
- The `/report` command for feedback added.
___
## v0.3.2
- Исправлен баг, когда пользователь мог зарегистрироваться на завершившееся мероприятие.
- Исправлен баг, когда пользователь многократно пытается регистрироваться на одно мероприятиею
- Исправлен баг, когда пользователь мог зарегистрироваться на отмененное мероприятие.
___
- Fixed a bug that let a user register for a completed event.
- The bug that caused users to try to register for the same event multiple times has been fixed.
- The bug that allowed users to register for canceled events has been fixed.
___
## v0.3.1
- Изменена стратегия ротирования логов. Теперь логи ротируются в зависимости от размера.
- Пользователи больше не будут получать уведомление об отмене мероприятий, если отменяется мероприятие, которое уже завершилось.
- Удалены из логов ошибочные номера chatID
- Добавлены в DEBUG-логи имена пользователей полученных сообщений и откликов.
- Изменен уровень логирования по умолчанию на WARN
- Изменен паттерн логирования для большего удобства.
___
- Change log's rollingStyle strategy to size-based
- If cancelled event has passed, participants will not receive notifications
- Incorrect chatId from log has been deleted
- Added Username of received message and callbackQuery to log
- Default logLevel changed to WARN
- Log's conversionPattern has been edited for more usability
___
## v0.3.0
- Изменено меню пользователя на более удобное и интуитивное.
___
- The user menu has been revamped to be more user-friendly and intuitive.
___
## v0.2.1
- Переписаны выводы ошибок с консоли на запись в лог
___
- Rewritten error outputs from console to logging
___
## v0.2.0
- Добавлено логирование
___
- Logging added
___
## v0.1.2
* Сообщение с основным меню остается (не удаляется) при возникновении всплывающего информационного сообщения.
___
* The main menu message will no longer be deleted when pop-up info messages are shown.
___
## v0.1.1
* Добавлен скрипт `start.sh` для легкого запуска приложения.
___
* Added script `start.sh` for simple start.
___
## v0.1.0
* Запуск альфа-версии.
___
* Alfa version release.
___
## v0.0.3
* В процессе разработки
___
* In the process of development
___