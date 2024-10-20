#!/bin/sh

# Check if required environment variables are set
: "${YOUTUBE_API_KEY:?Environment variable YOUTUBE_API_KEY is required}"
: "${PLAYER_COLOR:?Environment variable PLAYER_COLOR is required}"
: "${CHAT_COLOR:?Environment variable CHAT_COLOR is required}"
: "${LOGO_URL:?Environment variable LOGO_URL is required}"
: "${COMPANY_NAME:?Environment variable COMPANY_NAME is required}"

# Start the application
exec dotnet TeamRadio.dll