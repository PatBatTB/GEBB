## v0.3.2
- Исправлен баг, когда пользователь мог зарегистрироваться на завершившееся мероприятие.
- Исправлен баг, когда пользователь многократно пытается регистрироваться на одно мероприятиею
- Исправлен баг, когда пользователь мог зарегистрироваться на отмененное мероприятие.
___
- Fixed a bug that let a user register for a completed event.
- The bug that caused users to try to register for the same event multiple times has been fixed.
- The bug that allowed users to register for canceled events has been fixed.

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

## v0.3.0
- Изменено меню пользователя на более удобное и интуитивное.
___
- The user menu has been revamped to be more user-friendly and intuitive.

## v0.2.1
- Переписаны выводы ошибок с консоли на запись в лог
___
- Rewritten error outputs from console to logging

## v0.2.0
- Добавлено логирование
___
- Logging added

## v0.1.2
* Сообщение с основным меню остается (не удаляется) при возникновении всплывающего информационного сообщения.
___
* The main menu message will no longer be deleted when pop-up info messages are shown.

## v0.1.1
* Добавлен скрипт `start.sh` для легкого запуска приложения.
___
* Added script `start.sh` for simple start.

## v0.1.0
* Запуск альфа-версии.
___
* Alfa version release.

## v0.0.3
* В процессе разработки
___
* In the process of development
