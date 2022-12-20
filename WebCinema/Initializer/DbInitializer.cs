using System.Collections.Specialized;
using CinemaCore;
using CinemaCore.Models;
using Microsoft.EntityFrameworkCore;

namespace WebCinema
{
    public class DbInitializer
    {
        static Random randObj = new Random(1);

        public static async Task InitializeAsync(CinemaContext db)
        {
            db.Database.EnsureCreated();

            ///Имена, отчества и фамилии
            string FemaleSuf = "на";
            string MaleSuf = "ич";
            string[] MaleName = { "Даниил", "Данил", "Андрей", "Олег", "Иван", "Евгений", "Артем", "Владислав", "Владимир", "Александр", "Кирилл", "Павел", "Артемий", "Василий", "Максим", "Дмитрий", "Алексей" };
            string[] FemaleName = { "Анна", "Дарья", "Мария", "Юлия", "Наталия", "Наталья", "Ксения", "Ирина", "Евгения", "Екатерина", "Руфь", "Полина", "Сара", "Мария", "Александра", "Евгения", "София" };
            string[] SurName = { "Пашкевич", "Мороз", "Занко", "Свибович", "Асенчик", "Муха", "Бут-Гусаим", "Шух", "Морозько", "Верхогляд", "Каханчик", "Федоренко", "Лещун", "Гапоненко", "Новичук", "Ялченко" };

            string[] MiddleName = { "Андреев", "Даниилов", "Данилов", "Алексеев", "Олегов", "Иванов", "Кириллов", "Владимиров", "Васильев", "Александров", "Максимов", "Дмитриев", "Валерьев", "Артемов" };

            //Страны
            string[] сountries = { "Армения", "Казахстан", "Беларусь", "Россия", "Турция", "США", "Франция", "Финляндия", "Бразилия", "Индия", "Конго", "ЮАР", "Северная Корея", "Китай", "Канада", "Германия", "Египет", "Польша", "Украина", "Литва", "Латвия", "Нидерланды", "Дания", "Сирия", "Монголия", "Мексика", "Англия", "Португалия", "Италия", "Австрия", "Япония" };

            //Жанры фильмов
            string[] genres = { "Комедия", "Ужасы", "Боевик", "Приключение", "Триллер", "Мистика", "Детектив", "Биография", "Драма", "Мелодрама", "Фантастика", "Исторический", "Военный", "Семейный", "Вестерн", "Трагедия", "Фэнтэзи" };

            int actorsCount = 500;
            await InitTableActors(db, actorsCount, MaleName, FemaleName, MiddleName, SurName, FemaleSuf, MaleSuf);

            int сountriesCount = await InitTableCountryProductions(db, сountries);
            int genresCount = await InitGenresTable(db, genres);

            int filmProductionsCount = 500;
            await InitFilmProductions(db, filmProductionsCount);

            int filmsCount = 1000;
            await InitTableFilms(db, filmProductionsCount, genresCount, сountriesCount, filmsCount);

            int actorsCasts = 500;
            await InitTableActorCasts(db, actorsCount, filmsCount, actorsCasts);

            int staffsCount = 250;
            await InitTableStaffs(db, staffsCount, MaleName, FemaleName, MiddleName, SurName, FemaleSuf, MaleSuf);

            int cinemaHallsCount = 3;
            int[] hallsPlaces = await InitTableCinemaHalls(db, cinemaHallsCount);

            int eventsCount = 150;
            await InitTableListEvents(db, eventsCount, filmsCount, cinemaHallsCount);

            int staffCastsCount = 500;
            await InitTableStaffCasts(db, staffsCount, eventsCount, staffCastsCount);

            int placesCount = 250;
            await InitTablePlaces(db, placesCount, hallsPlaces, eventsCount, cinemaHallsCount);
        }

        /// <summary>
        /// Генератор для таблицы Актеры
        /// </summary>
        public static async Task InitTableActors(CinemaContext db, int actorsCount, string[] MaleName, string[] FemaleName, string[] MiddleName, string[] SurName, string FemaleSuf, string MaleSuf)
        {
            if (!db.Actors.Any())
            {

                for (int i = 0; i < actorsCount; i += 2)
                {
                    string femaleMiddleName = MiddleName[randObj.Next(MiddleName.Length)] + FemaleSuf;
                    string maleMiddleName = MiddleName[randObj.Next(MiddleName.Length)] + MaleSuf;

                    string maleName = MaleName[randObj.Next(MaleName.Length)];
                    string femaleName = FemaleName[randObj.Next(FemaleName.Length)];

                    string surName = SurName[randObj.Next(SurName.Length)];

                    await db.AddAsync(new Actors { Name = maleName, Surname = surName, MiddleName = maleMiddleName });
                    await db.AddAsync(new Actors { Name = femaleName, Surname = surName, MiddleName = femaleMiddleName });
                }
                await db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Генератор для таблицы Страна-производитель
        /// </summary>
        public static async Task<int> InitTableCountryProductions(CinemaContext db, string[] сountries)
        {
            int countryCount = 0;
            if (!await db.CountryProductions.AnyAsync())
            {
                countryCount = сountries.Length;
                for (int i = 0; i < countryCount; i++)
                {
                    await db.AddAsync(new CountryProductions { Name = сountries[i] });
                }
                await db.SaveChangesAsync();
            }
            else
            {
                return await db.CountryProductions.CountAsync();
            }
            return countryCount;
        }

        /// <summary>
        /// Генератор для таблицы Жанры
        /// </summary>
        public static async Task<int> InitGenresTable(CinemaContext db, string[] genres)
        {
            int genresCount = 0;
            if (!await db.Genres.AnyAsync())
            {
                genresCount = genres.Length;
                for (int i = 0; i < genresCount; i++)
                {
                    await db.AddAsync(new Genres { Name = genres[i] });
                }
                await db.SaveChangesAsync();
            }
            else
            {
                return await db.Genres.CountAsync();
            }
            return genresCount;
        }

        /// <summary>
        /// Генератор для таблицы Компания-производитель
        /// </summary>
        public static async Task InitFilmProductions(CinemaContext db, int filmProductionCount)
        {
            if (!await db.FilmProductions.AnyAsync())
            {

                for (int i = 0; i < filmProductionCount; i++)
                {
                    string filmProduction = "Компания_" + i.ToString();
                    await db.AddAsync(new FilmProductions { Name = filmProduction });
                }
                await db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Генератор для таблицы Фильмы
        /// </summary>
        public static async Task InitTableFilms(CinemaContext db, int filmProductionCount, int genresCount, int сountriesCount, int filmsCount)
        {
            if (!await db.Films.AnyAsync())
            {
                for (int i = 0; i < filmsCount; i++)
                {
                    string filmName = "Название_" + i.ToString();
                    int genreId = randObj.Next(1, genresCount);
                    int duration = randObj.Next(5, 230);
                    int filmProductionId = randObj.Next(1, filmProductionCount);
                    int countryProductionId = randObj.Next(1, сountriesCount);
                    int ageLimit = randObj.Next(0, 18);
                    string description = "Описание_" + i.ToString();
                    await db.AddAsync(new Films
                    {
                        Name = filmName,
                        GenreId = genreId,
                        Duration = duration,
                        FilmProductionId = filmProductionId,
                        CountryProductionId = countryProductionId,
                        AgeLimit = ageLimit,
                        Description = description
                    });
                }
                await db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Генератор для таблицы Актерские группы
        /// </summary>
        public static async Task InitTableActorCasts(CinemaContext db, int actorCount, int filmCount, int actorCastCount)
        {
            if (!await db.ActorCasts.AnyAsync())
            {
                for (int i = 0; i < actorCastCount; i++)
                {
                    int actorId = randObj.Next(1, actorCount);
                    int filmId = randObj.Next(1, filmCount);
                    await db.AddAsync(new ActorCasts { ActorId = actorId, FilmId = filmId });
                }
                await db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Генератор для таблицы Сотрудники
        /// </summary>
        public static async Task InitTableStaffs(CinemaContext db, int staffsCount, string[] MaleName, string[] FemaleName, string[] MiddleName, string[] SurName, string FemaleSuf, string MaleSuf)
        {
            if (!await db.Staffs.AnyAsync())
            {
                string[] posts = { "Старший кассир", "Младший кассир", "Стажер", "Бухгалтер", "Уборщик", "Директор", "Менеджер", "Рекламщик", "Переводчик" };
                for (int i = 0; i < staffsCount; i++)
                {
                    string femaleMiddleName = MiddleName[randObj.Next(MiddleName.Length)] + FemaleSuf;
                    string maleMiddleName = MiddleName[randObj.Next(MiddleName.Length)] + MaleSuf;

                    string maleName = MaleName[randObj.Next(MaleName.Length)];
                    string femaleName = FemaleName[randObj.Next(FemaleName.Length)];

                    string surName = SurName[randObj.Next(SurName.Length)];

                    int workExperience = randObj.Next(0, 450);
                    string post = posts[randObj.Next(posts.Length)];

                    await db.AddAsync(new Staffs { Name = maleName, Surname = surName, MiddleName = maleMiddleName, Post = post, WorkExperience = workExperience });
                    await db.AddAsync(new Staffs { Name = femaleName, Surname = surName, MiddleName = femaleMiddleName, Post = post, WorkExperience = workExperience });
                }
                await db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Генератор для таблицы Залы
        /// </summary>
        public static async Task<int[]> InitTableCinemaHalls(CinemaContext db, int cinemaHallsCount)
        {
            int[] hallsPlaces = new int[cinemaHallsCount];
            if (!await db.CinemaHalls.AnyAsync())
            {
                for (int i = 0; i < cinemaHallsCount; i++)
                {
                    int hallNumber = i;
                    int rowsCount = randObj.Next(4, 10);
                    int columnsCount = randObj.Next(6, 9);
                    hallsPlaces[i] = rowsCount * columnsCount;
                    await db.AddAsync(new CinemaHalls { HallNumber = hallNumber + 1, MaxPlaceNumber = columnsCount * rowsCount,
                                                        Rows = rowsCount, Columns = columnsCount});
                }
                await db.SaveChangesAsync();
            }
            else
            {
                var count = await db.CinemaHalls.CountAsync();
                hallsPlaces = new int[count];
                for(var i = 0; i < count; i++)
                {
                    hallsPlaces[i] = db.CinemaHalls.ToList().ElementAt(i).MaxPlaceNumber;
                }
            }
            return hallsPlaces;
        }

        /// <summary>
        /// Генератор для таблицы События
        /// </summary>
        public static async Task InitTableListEvents(CinemaContext db, int eventsCount, int filmsCount, int cinemaHallsCount)
        {
            if (!await db.ListEvents.AnyAsync())
            {
                for (int i = 0; i < eventsCount; i++)
                {
                    string eventName = "Событие_" + i.ToString();
                    int filmId = randObj.Next(1, filmsCount);
                    decimal ticketPrice = randObj.Next(25, 125);

                    int randomMonth = randObj.Next(1, 12);
                    int randomDay = randObj.Next(1, DateTime.DaysInMonth(2022, randomMonth));
                    DateTime date = new DateTime(2022, randomMonth, randomDay);

                    int randomStartHour = randObj.Next(8, 20);
                    int randomStartMinute = randObj.Next(1, 59);
                    TimeSpan startTime = new TimeSpan(randomStartHour, randomStartMinute, 0);

                    int randomEndHour = randomStartHour + randObj.Next(0, 3);
                    int randomEndMinute = randomStartMinute + randObj.Next(1, 59);
                    TimeSpan endTime = new TimeSpan(randomEndHour, randomEndMinute, 0);

                    int randomCinemaHallId = randObj.Next(1, cinemaHallsCount);

                    await db.AddAsync(new ListEvents {/* Name = eventName, */Date = date, StartTime = startTime, EndTime = endTime, TicketPrice = ticketPrice, FilmId = filmId, CinemaHallId = randomCinemaHallId });
                }
                await db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Генератор для таблицы Места
        /// </summary>
        public static async Task InitTablePlaces(CinemaContext db, int placesCount, int[] hallsPlaces, int eventsCount, int cinemaHallCount)
        {
            if (!await db.Places.AnyAsync())
            {
                for (int i = 0; i < placesCount; i++)
                {
                    int listEventId = randObj.Next(1, eventsCount);
                    int cinemaHallId = randObj.Next(1, cinemaHallCount);

                    int placeNumber = randObj.Next(1, hallsPlaces[cinemaHallId]);

                    await db.AddAsync(new Places { ListEventId = listEventId, CinemaHallId = cinemaHallId, PlaceNumber = placeNumber, TakenSeat = true });
                }
                await db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Генератор для таблицы группы Сотрудников
        /// </summary>
        public static async Task InitTableStaffCasts(CinemaContext db, int staffsCount, int eventsCount, int staffCastsCount)
        {
            if (!await db.StaffCasts.AnyAsync())

                for (int i = 0; i < staffCastsCount; i++)
                {
                    int staffNumber = randObj.Next(1, staffsCount);
                    int eventNumber = randObj.Next(1, eventsCount);
                    await db.AddAsync(new StaffCasts { StaffId = staffNumber, ListEventId = eventNumber });
                }
            await db.SaveChangesAsync();
        }
    }
}