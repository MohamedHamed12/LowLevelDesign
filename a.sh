#!/bin/bash
set -e

PROJECT_NAME="TicTacToe"
mkdir $PROJECT_NAME && cd $PROJECT_NAME

# Create solution
dotnet new sln -n $PROJECT_NAME

# Core layer
dotnet new classlib -n $PROJECT_NAME.Core
mkdir -p $PROJECT_NAME.Core/Entities
mkdir -p $PROJECT_NAME.Core/Rules
mkdir -p $PROJECT_NAME.Core/Services
mkdir -p $PROJECT_NAME.Core/Interfaces

# Application layer
dotnet new classlib -n $PROJECT_NAME.Application
mkdir -p $PROJECT_NAME.Application/Game
mkdir -p $PROJECT_NAME.Application/Players
mkdir -p $PROJECT_NAME.Application/AI

# Infrastructure layer
dotnet new classlib -n $PROJECT_NAME.Infrastructure
mkdir -p $PROJECT_NAME.Infrastructure/Logging
mkdir -p $PROJECT_NAME.Infrastructure/Persistence

# Console UI
dotnet new console -n $PROJECT_NAME.ConsoleUI

# Tests
dotnet new xunit -n $PROJECT_NAME.Tests
mkdir -p $PROJECT_NAME.Tests/Core
mkdir -p $PROJECT_NAME.Tests/Application

# Add to solution
dotnet sln add $PROJECT_NAME.Core/$PROJECT_NAME.Core.csproj
dotnet sln add $PROJECT_NAME.Application/$PROJECT_NAME.Application.csproj
dotnet sln add $PROJECT_NAME.Infrastructure/$PROJECT_NAME.Infrastructure.csproj
dotnet sln add $PROJECT_NAME.ConsoleUI/$PROJECT_NAME.ConsoleUI.csproj
dotnet sln add $PROJECT_NAME.Tests/$PROJECT_NAME.Tests.csproj

# Add references
dotnet add $PROJECT_NAME.Application/$PROJECT_NAME.Application.csproj reference $PROJECT_NAME.Core/$PROJECT_NAME.Core.csproj
dotnet add $PROJECT_NAME.Infrastructure/$PROJECT_NAME.Infrastructure.csproj reference $PROJECT_NAME.Core/$PROJECT_NAME.Core.csproj
dotnet add $PROJECT_NAME.ConsoleUI/$PROJECT_NAME.ConsoleUI.csproj reference $PROJECT_NAME.Core/$PROJECT_NAME.Core.csproj
dotnet add $PROJECT_NAME.ConsoleUI/$PROJECT_NAME.ConsoleUI.csproj reference $PROJECT_NAME.Application/$PROJECT_NAME.Application.csproj
dotnet add $PROJECT_NAME.Tests/$PROJECT_NAME.Tests.csproj reference $PROJECT_NAME.Core/$PROJECT_NAME.Core.csproj
dotnet add $PROJECT_NAME.Tests/$PROJECT_NAME.Tests.csproj reference $PROJECT_NAME.Application/$PROJECT_NAME.Application.csproj

echo "✅ Project structure created (targeting your installed SDK: $(dotnet --version))"
