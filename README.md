# Password Search PowerToys Command Palette Extension

An extension for the PowerToys Command Palette that lets you quickly search, generate, and open KeePass entries.

## Features

* **Configure KeePass DB**: set your database path and master password via a settings page.
* **Search KeePass**: perform a case-insensitive title search in your KeePass vault and copy passwords or usernames to the clipboard.
* **Generate Password**: choose a custom length at runtime (e.g., type `16` for a 16-character password or pick from default suggestions) to generate a cryptographically secure password and copy it to the clipboard.

## Requirements

* Windows 10 or higher
* PowerToys installed (v0.90+)

## Installation
1. Clone this repository:
2. Open `PasswordSearch.sln` in Visual Studio 2022 or later.
3. Restore NuGet packages and build the solution.
4. Package the extension as AppX or MSIX, then install it. (dont forget cert)

## Usage

* **Open the Command Palette**
* **Configure KeePass DB**: type `Configure KeePass DB` to set your vault path and master password.
* **Search KeePass**: type `Search KeePass` followed by your query to find entries.

  * Press **Enter** to copy the password.
  * Press **Ctrl+Enter** (or right-click) for additional options: copy username, open URL, or open entry in KeePass.
* **Generate Password**: type `Generate Password`, then either select a suggested length or type a custom number to generate and copy a random password of that length.

## Configuration

* **DatabasePath**: path to your `.kdbx` file.
* **MasterPassword**: optional master password.

These settings are available under **PowerToys > Commands > Password Search**.

## Contributing

Feel free to open issues or submit pull requests. Fork the repository, make your changes, and submit a PR.

