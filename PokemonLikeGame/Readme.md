Overview
ExerciceMonster is a Pokemon-Like combat game developed in C# using Windows Presentation Foundation (WPF). The game allows players to manage and engage in turn-based combat with a variety of monsters, utilizing different spells and techniques to overcome opponents.

Features
User Authentication: Login with username and password.
Monster Management: View and select monsters for combat, with details about each monster's health and abilities.
Spell Book: Access a list of available spells with details about their effects.
Combat System: Engage in turn-based battles with automated opponents.
Settings: Manage application settings, including database connection details, View and refresh database details directly from the UI.

Prerequisites
You need to have:
.NET Framework 4.7.2 or higher
SQL Server Express (LocalDB or regular SQL Server)

Set up the database:
Ensure your SQL Server is running.
Update the connection string in App.config to point to your SQL Server instance.
Run the database script provided in the DatabaseScript.sql file to set up your database schema and initial data.


How to Use
Login Screen: Enter your username and password.
Monster Management: Select a monster from the list to view its details or to select it for combat.
Spells Screen: Browse through available spells.
Combat Screen: Engage in combat by selecting spells and watching the turn-based battle unfold.
Settings Screen: Adjust application settings including the database connection string. Use the "Apply" button to save changes.