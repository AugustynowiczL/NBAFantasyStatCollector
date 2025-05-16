
# NBAFantasyStatCollector

A C# project for collecting NBA fantasy statistics.

## About

This project aims to provide a tool for gathering and managing NBA fantasy statistics.

## Features

*   Collects NBA player statistics.
*   Provides an API for accessing the collected data.
*   Uses a SQLite database (`NBAFantasyDB.db`) for data storage.
*   Includes a Dockerfile for containerization.

## Prerequisites

Before you begin, ensure you have met the following requirements:

*   [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or later installed.
*   [Docker](https://www.docker.com/) (optional, for containerization).

## Installation

1.  Clone the repository:

    ```bash
    git clone https://github.com/AugustynowiczL/NBAFantasyStatCollector.git
    cd NBAFantasyStatCollector
    ```

2.  Restore dependencies:

    ```bash
    dotnet restore
    ```

3.  Build the project:

    ```bash
    dotnet build
    ```

## Running the Application

1.  Run the application:

    ```bash
    dotnet run
    ```

    Alternatively, you can use the Dockerfile to build and run the application in a container:

    ```bash
    docker build -t nbafantasystatcollector .
    docker run -p 8080:8080 -p 8081:8081 nbafantasystatcollector
    ```

## API Documentation

The application exposes the following endpoints:
*   **POST /api/Player/PopulatePlayers**: Populates SQLite database of all players in the league with their associated ESPN id
*   **GET /api/Player/GetPlayerByName{playerName}**: Retrieves ESPN Id of player e.x stephen-curry.
*   **GET /api/Player/GetPlayerIDsByTeam/{teamName}**: Retrieves list of ESPN player IDs of a team e.x Atlanta Hawks.

*   **GET /api/Stats/GetPlayerStatsByID{id}**: Retrieves stats of a player given their ESPN id e.x 6609.
*   **GET /api/Stats/GetAvgTeamStatsByTeam{teamName}**: Retrieves aggregate stats of a given team e.x Atlanta Hawks.

## Contributing

Contributions are welcome! Please follow these guidelines:

1.  Fork the repository.
2.  Create a new branch for your feature or bug fix.
3.  Make your changes and commit them with descriptive messages.
4.  Submit a pull request.

## License

This project is licensed under the terms of the `LICENSE.txt` file.
```
