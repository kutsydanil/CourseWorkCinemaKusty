USE master

IF DB_ID('Cinema') IS NOT NULL
DROP DATABASE Cinema

USE master
CREATE DATABASE Cinema
Go

USE Cinema

/*Таблица Кинокомпания-производитель*/
CREATE TABLE FilmProductions(
	Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Name NVARCHAR(100) NOT NULL
);

/*ТАБЛИЦА ЖАНРЫ*/
CREATE TABLE Genres(
	Id INT IDENTITY(1 , 1) NOT NULL PRIMARY KEY,
	Name NVARCHAR(100) NOT NULL,
);

/*Таблица АКТЕРЫ*/
CREATE TABLE Actors(
	Id INT IDENTITY(1 , 1) NOT NULL PRIMARY KEY,
	Name NVARCHAR(60) NOT NULL,
	Surname NVARCHAR(60) NOT NULL,
	MiddleName NVARCHAR(60) NOT NULL,
);

/*Таблица страна-производитель*/
CREATE TABLE CountryProductions(
	Id INT IDENTITY(1 , 1) NOT NULL PRIMARY KEY,
	Name NVARCHAR(150)
);

/*Таблица ФИЛЬМЫ*/
CREATE TABLE Films(
	Id INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
	Name NVARCHAR(100) NOT NULL,
	GenreId INT NOT NULL,
	Duration INT NOT NULL,
	FilmProductionId INT NOT NULL,
	CountryProductionId INT NOT NULL,
	AgeLimit INT NOT NULL,
	Description NVARCHAR(350) NOT NULL

	FOREIGN KEY(CountryProductionId) REFERENCES CountryProductions(Id) ON DELETE CASCADE,
	FOREIGN KEY(FilmProductionId) REFERENCES FilmProductions(Id) ON DELETE CASCADE,
	FOREIGN KEY(GenreId) REFERENCES Genres(Id) ON DELETE CASCADE, 
);

/*Таблица АктерскийСостав*/
CREATE TABLE ActorCasts(
	Id INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
	ActorId INT NOT NULL,
	FilmId INT NOT NULL,

	FOREIGN KEY(ActorId) REFERENCES Actors(Id) ON DELETE CASCADE,
	FOREIGN KEY(FilmId) REFERENCES Films(Id) ON DELETE CASCADE
);

/*Таблица Сотрудники*/
CREATE TABLE Staffs(
	Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Name NVARCHAR(60) NOT NULL,
	Surname NVARCHAR(60) NOT NULL,
	MiddleName NVARCHAR(60) NOT NULL,
	Post NVARCHAR(150) NOT NULL,
	WorkExperience INT NOT NULL
)

/*Таблица Зал*/
CREATE TABLE CinemaHalls(
	 Id INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
	 HallNumber INT NOT NULL,
	 MaxPlaceNumber INT NOT NULL,
	 Rows int,
	 Columns int
);

/*Таблица Проведение мероприятий*/
CREATE TABLE ListEvents(
	Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Name NVARCHAR(150) NOT NULL,
	Date DATE NOT NULL,
	StartTime TIME NOT NULL,
	EndTime TIME NOT NULL,
	TicketPrice MONEY NOT NULL,
	FilmId INT NOT NULL,
	CinemaHallId INT NOT NULL,

	FOREIGN KEY(FilmId) REFERENCES Films(Id) ON DELETE CASCADE,
	FOREIGN KEY(CinemaHallId) REFERENCES CinemaHalls(Id) 
);

/*Таблица Группа сотрудников*/
CREATE TABLE StaffCasts
(
	Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	StaffId INT NOT NULL,
	ListEventId INT NOT NUll,

	FOREIGN KEY(StaffId) REFERENCES Staffs(Id) ON DELETE CASCADE,
	FOREIGN KEY(ListEventId) REFERENCES ListEvents(Id) ON DELETE CASCADE,
);

/*Таблица Места*/
CREATE TABLE Places(
	Id INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
	ListEventId INT NOT NULL,

	CinemaHallId INT NOT NULL,
	PlaceNumber INT NOT NULL,
	TakenSeat BIT NOT NULL,

	FOREIGN KEY(CinemaHallId) REFERENCES CinemaHalls(Id) ON DELETE CASCADE,
	FOREIGN KEY(ListEventId) REFERENCES ListEvents(Id) ON DELETE CASCADE,

);







