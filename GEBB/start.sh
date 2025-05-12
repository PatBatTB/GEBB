#!/bin/bash
### Write your bot token below
BOT_TOKEN=""
###
if [ "$BOT_TOKEN" = "" ]; then
	echo " Write your bot token in the start.sh file"
	exit 1
fi
WORK_DIR="$(dirname "$0")"
APPSETTINGS="$WORK_DIR/appsettings.json"
EXECFILE="$WORK_DIR/GEBB"
LOGFILE="$WORK_DIR/gebb.log"
if [ ! -f "$APPSETTINGS" ]; then
	echo "{
    \"ConnectionStrings\": {
        \"tgbot\": \"Data Source=tgbot.sqlite\" 
	    }
	}" > "$APPSETTINGS"
fi

if [ ! -x "$EXECFILE" ]; then
	chmod +x "$EXECFILE"
fi
nohup env bot.token="$BOT_TOKEN" "$EXECFILE" > "$LOGFILE" 2>&1 &