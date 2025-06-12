#!/bin/bash
### Write your bot token:
BOT_TOKEN=""
### Write the desired min logging level:
# Available values: ALL / DEBUG / INFO / WARN / ERROR / FATAL / OFF
### You need the "xmlstarlet" utility to replace the LOG_LEVEL. If it is not installed, the logging level "WARN" is used by default.
LOG_LEVEL="WARN"
###

if [ "$BOT_TOKEN" = "" ]; then
	echo " Write your bot token in the start.sh file"
	exit 1
fi

WORK_DIR="$(dirname "$0")"
APPSETTINGS="$WORK_DIR/appsettings.json"
EXEC_FILE_PATH="$WORK_DIR/GEBB"
LOG_CONFIG_PATH="$WORK_DIR/log4net.config"
LOG_FILE_PATH="$WORK_DIR/gebb.log"

if [ ! -f "$LOG_CONFIG_PATH" ]; then
	echo "<log4net>
    <appender name=\"RollingFileAppender\" type=\"log4net.Appender.RollingFileAppender\">
        <file value=\""$LOG_FILE_PATH"\" />
        <appendToFile value=\"true\" />
        <rollingStyle value=\"Size\" />
        <maximumFileSize value=\"3MB\" />
        <maxSizeRollBackups value=\"5\" />
        <layout type=\"log4net.Layout.PatternLayout\">
            <conversionPattern value=\"%date [%2thread] %-5level %.40logger - %message%newline\" />
        </layout>
    </appender>
    
    <root>
        <level value=\"WARN\" />
        <appender-ref ref=\"RollingFileAppender\" />
    </root>
</log4net>" > "$LOG_CONFIG_PATH"
fi

if command -v xmlstarlet > /dev/null 2>&1; then
  xmlstarlet edit -L -O -u "//log4net/root/level/@value" -v "$LOG_LEVEL" "$LOG_CONFIG_PATH"
else
  echo "You need to install \"xmlstarlet\" utility if you want to autoupdate logging level from variable LOG_LEVEL"
fi
if [ ! -f "$APPSETTINGS" ]; then
	echo "{
    \"ConnectionStrings\": {
        \"tgbot\": \"Data Source=tgbot.sqlite\" 
	    }
	}" > "$APPSETTINGS"
fi

if [ ! -x "$EXEC_FILE_PATH" ]; then
	chmod +x "$EXEC_FILE_PATH"
fi
nohup env bot.token="$BOT_TOKEN" "$EXEC_FILE_PATH" >> "$LOG_FILE_PATH" 2>&1 &