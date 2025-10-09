#!/usr/bin/env bash
set -euo pipefail

ROOT="VendingMachine"
APP="src/VendingMachine.App"

echo "Creating solution and project structure..."

# Create solution and app project (dotnet 9)
mkdir -p "$APP"
dotnet new sln -n $ROOT
dotnet new console -f net9.0 -o "$APP" --name VendingMachine.App
dotnet sln add "$APP/VendingMachine.App.csproj"

# Create folders
declare -a dirs=(
  "$APP/Domain/Models"
  "$APP/Domain/Enums"
  "$APP/Domain/Interfaces"
  "$APP/Services"
  "$APP/Infrastructure"
  "$APP/Infrastructure/Repositories"
  "$APP/Infrastructure/Hardware"
  "$APP/DTOs"
  "$APP/Exceptions"
)

for d in "${dirs[@]}"; do
  mkdir -p "$d"
done

# Create files (placeholders) - full contents added in the design doc
touch "$APP/Program.cs"
touch "$APP/Domain/Models/Product.cs"
touch "$APP/Domain/Models/Slot.cs"
touch "$APP/Domain/Enums/Coin.cs"
touch "$APP/Domain/Interfaces/ICoinAcceptor.cs"
touch "$APP/Domain/Interfaces/IInventoryRepository.cs"
touch "$APP/Domain/Interfaces/IDispenser.cs"
touch "$APP/Domain/Interfaces/IDisplay.cs"
touch "$APP/Services/VendingMachine.cs"
touch "$APP/Infrastructure/Repositories/InMemoryInventoryRepository.cs"
touch "$APP/Infrastructure/Hardware/SimpleCoinAcceptor.cs"
touch "$APP/Infrastructure/Hardware/SimpleDispenser.cs"
touch "$APP/Infrastructure/Hardware/ConsoleDisplay.cs"
touch "$APP/DTOs/TransactionResult.cs"
touch "$APP/Exceptions/VendingException.cs"

echo "Scaffold created. Now add the provided code into these files."
echo "You can build the project with: dotnet build $APP"
