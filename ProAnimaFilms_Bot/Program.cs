using System;
using System.Net.Http;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using ProAnimaFilms_Bot.Models;
using System.Text.Json;
using System.Collections.Generic;
using System.Data;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Requests;
using System.Text;
using System.Globalization;
using System.Diagnostics.Contracts;

namespace ProAnimaFilms_Bot
{
    public class Program
    {
        public static Dictionary<int, string> status = new Dictionary<int, string>();
        public static Dictionary<int, string> moviesbygenre = new Dictionary<int, string>();
        public static Dictionary<string, string> genre_status = new Dictionary<string, string>();
        public static string image_path = "https://image.tmdb.org/t/p/original";
        public static string basic_movie_path = "https://www.themoviedb.org/movie/";
        public static string basic_tvshows_path = "https://www.themoviedb.org/tv/";
        public static string points = ". . .";
        public static int int_genre;
        public static string string_genre;

        public static HttpClient client = new HttpClient();
        public static TelegramBotClient Bot;
        static void Main(string[] args)
        {
            Bot = new TelegramBotClient("877770429:AAGXmOKe_rxxbOa5aNwkz3GfKixjE8qVwnk");

            Bot.OnMessage += Bot_OnMessageReceived;

            var me = Bot.GetMeAsync().Result;
            Console.WriteLine(me.FirstName);

            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
        }

        public static async void Bot_OnMessageReceived(object sender, MessageEventArgs e)
        {
            var message = e.Message;

            if (message == null || message.Type != MessageType.Text)
                return;

            string name = $"{message.From.FirstName} {message.From.LastName}";
            int user_id = message.From.Id;
            Console.WriteLine($"id:{user_id} Name:{user_id} отправил '{message.Text}'");

            string instruction =
                @"Самое время разобраться с функциями бота!

Поиск фильма - поиск фильма по названию.
Поиск сериала - поиск сериала по названию.
Поиск фильма по жанру - поиск фильма по выбранному жанру.
Популярное - получить топ фильмов за сегодня. Обновляется каждый день.
Избранное - просмотреть список избранных фильмов/сериалов. 
Добавить в избранное - добавить фильм/сериал в избранное.
Удалить из избранного - удалить фильм/сериал из избранного.

Бот предназначен для русскогоязычного комьюнити, но запросы можно задавать как на английском, так и на русском языках!

Почему не находит фильм/сериал?
   1. В введенном Вами названии содержится ошибка. 
   2. Фильма/сериала с запрашиваемым названием не существует.
   3. Введенное Вами сообщение не является названием фильма.
      
Как очистить весь список избранного?
Введите в чат 'Clear'

Чтобы получить инструкицию по использованию бота, введите 'help'.

Удачи!
                            ";

            if (status.ContainsKey(user_id))
            {
                if (message.Text == "/start" || message.Text.ToLower() == "help")
                {
                    await Bot.SendTextMessageAsync(message.From.Id, instruction);
                    status[user_id] = "Основное меню";
                }
                if (message.Text == "/movie_search" || message.Text == "Поиск фильма")
                    status[user_id] = "/movie_search";
                if (message.Text == "/tv_series_search" || message.Text == "Поиск сериала")
                    status[user_id] = "/tv_series_search";
                if (message.Text == "/search_movies_by_genre" || message.Text == "Поиск фильма по жанру")
                    status[user_id] = "/search_movies_by_genre";
                if (message.Text == "Популярное")
                    status[user_id] = "Популярное";
                if (message.Text == "/favorites" || message.Text == "Избранное")
                    status[user_id] = "/favorites";
                if (message.Text == "Добавить в избранное")
                    status[user_id] = "Добавить в избранное";
                if (message.Text == "Удалить из избранного")
                    status[user_id] = "Удалить из избранного";
                else if (message.Text == "Основное меню")
                    status[user_id] = "Основное меню";
            }
            else
            {
                if (message.Text != null)
                {
                    if (message.Text == "/start" || message.Text.ToLower() == "help")
                    {
                        await Bot.SendTextMessageAsync(message.From.Id, instruction);

                        Favorites favorites = new Favorites();
                        favorites._id = user_id;
                        favorites.featuredfims = new List<string>();
                        var json = JsonSerializer.Serialize<Favorites>(favorites);
                        var data = new StringContent(json, Encoding.UTF8, "application/json");
                        await client.PostAsync("https://localhost:44393/api/proanimaFilms/", data);
                        status.Add(user_id, "Основное меню");
                    }
                    else if (message.Text == "/movie_search" || message.Text == "Поиск фильма")
                        status.Add(user_id, "/movie_search");
                    else if (message.Text == "/tv_series_search" || message.Text == "Поиск сериала")
                        status.Add(user_id, "/tv_series_search");
                    else if (message.Text == "/search_movies_by_genre" || message.Text == "Поиск фильма по жанру")
                        status.Add(user_id, "/search_movies_by_genre");
                    else if (message.Text == "Популярное")
                        status.Add(user_id, "Популярное");
                    else if(message.Text == "/favorites" || message.Text == "Избранное")
                        status.Add(user_id, "/favorites");
                    else if (message.Text == "Добавить в избранное")
                        status.Add(user_id, "Добавить в избранное");
                    else if (message.Text == "Удалить из избранного")
                        status.Add(user_id, "Удалить из избранного");
                    else if (message.Text == "Основное меню")
                        status.Add(user_id, "Основное меню");
                    else
                    {
                        status.Add(user_id, "Основное меню");
                        await Bot.SendTextMessageAsync(message.From.Id, "Пожалуйста, введите запрос заново или введите правильный запрос!");
                    }
                }
            }

            if (status[user_id] == "Основное меню")
            {
                var menu_replyKeyboard = new ReplyKeyboardMarkup(new[]
                    {
                  new[]
                  {
                    new KeyboardButton("Поиск фильма"),
                    new KeyboardButton("Поиск сериала")
                  },
                  new[]
                  {
                    new KeyboardButton("Поиск фильма по жанру"),
                    new KeyboardButton("Популярное")
                  },
                  new[]
                  {
                    new KeyboardButton("Избранное"),
                  }
                });
                await Bot.SendTextMessageAsync(message.Chat.Id, ". . .", replyMarkup: menu_replyKeyboard);
            }
            
            try
            {
                if (status[user_id] == "/movie_search") //Поиск фильмов
                {
                    if (message.Text == "/movie_search" || message.Text == "Поиск фильма")
                        await Bot.SendTextMessageAsync(message.From.Id, "Введите название фильма, который хотите найти.");

                    else if (message.Text != "/movie_search" || message.Text != "Поиск фильма")
                    {
                        string movie = message.Text.ToLower();
                        HttpResponseMessage response = await client.GetAsync("https://localhost:44393/api/proanimaFilms/" + movie);
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        SearchMovies movies = JsonSerializer.Deserialize<SearchMovies>(responseBody);

                        int max_popularity = 0;
                        for (int i = 1; i < movies.results.Count - 1; i++)
                        {
                            if (movies.results[max_popularity].popularity > movies.results[i].popularity)
                                continue;
                            else
                                max_popularity = i;
                        }

                        string title = $"{movies.results[max_popularity].title}";
                        string overview = $"{movies.results[max_popularity].overview}";
                        string poster_path = $"{image_path}{movies.results[max_popularity].poster_path}";
                        int id = movies.results[max_popularity].id;

                        string[] words_int_the_title = movie.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        string movie_path = $"{basic_movie_path}{id}{"-"}{words_int_the_title}";

                        await Bot.SendTextMessageAsync(message.Chat.Id, title);
                        await Bot.SendPhotoAsync(message.Chat.Id, poster_path, overview);

                        var inlineKeyboard = new InlineKeyboardMarkup(new[]
                        {
                                new[]
                                {
                                   InlineKeyboardButton.WithUrl($"Перейти по ссылке на '{movie}'", movie_path)
                                }
                            });
                        await Bot.SendTextMessageAsync(message.Chat.Id, points, replyMarkup: inlineKeyboard);
                    }
                }

                else if (status[user_id] == "/tv_series_search") //Поиск сериалов
                {
                    if (message.Text == "/tv_series_search" || message.Text == "Поиск сериала")
                        await Bot.SendTextMessageAsync(message.From.Id, "Введите название сериала, который хотите найти.");

                    else if (message.Text != "/tv_series_search" || message.Text != "Поиск сериала")
                    {
                        string tv_series = message.Text.ToLower();
                        HttpResponseMessage response = await client.GetAsync("https://localhost:44393/api/proanimaFilms/tvshow/" + tv_series);
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        SearchTVshows tv_seriess = JsonSerializer.Deserialize<SearchTVshows>(responseBody);

                        int max_popularity = 0;
                        for (int i = 1; i < tv_seriess.results.Count - 1; i++)
                        {
                            if (tv_seriess.results[max_popularity].popularity > tv_seriess.results[i].popularity)
                                continue;
                            else
                                max_popularity = i;
                        }

                        string title = $"{tv_seriess.results[max_popularity].name}";
                        string overview = $"{tv_seriess.results[max_popularity].overview}";
                        string poster_path = $"{image_path}{tv_seriess.results[max_popularity].poster_path}";
                        int id = tv_seriess.results[max_popularity].id;

                        string[] words_int_the_name = tv_series.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        string tv_series_path = $"{basic_tvshows_path}{id}{"-"}{words_int_the_name}";

                        await Bot.SendTextMessageAsync(message.Chat.Id, title);
                        await Bot.SendPhotoAsync(e.Message.Chat.Id, poster_path, overview);

                        var inlineKeyboard = new InlineKeyboardMarkup(new[]
                        {
                                new[]
                                {
                                   InlineKeyboardButton.WithUrl($"Перейти по ссылке на '{tv_series}'", tv_series_path)
                                }
                            });
                        await Bot.SendTextMessageAsync(message.Chat.Id, points, replyMarkup: inlineKeyboard);
                    }
                }
            }
            catch (Exception) { await Bot.SendTextMessageAsync(message.Chat.Id, "Ничего не найдено. Возможно, в введенном Вами запросе допущена ошибка. Пожалуйста, введите запрос заново."); };

            try
            {
                if (status[user_id] == "/search_movies_by_genre") //Поиск фильмов по жанрам
                {
                    if (genre_status.ContainsKey("search_movies_by_genre"))
                    {
                        if (message.Text == "/search_movies_by_genre" || message.Text == "Поиск фильма по жанру")
                            genre_status["search_movies_by_genre"] = "0";
                    }
                    else genre_status.Add("search_movies_by_genre", "0");

                    var replyKeyboard = new ReplyKeyboardMarkup(new[]
                    {
                  new[]
                  {
                    new KeyboardButton("Боевик"),
                    new KeyboardButton("Приключения"),
                    new KeyboardButton("Мультфильм"),
                    new KeyboardButton("Комедия")
                  },
                  new[]
                  {
                    new KeyboardButton("Криминал"),
                    new KeyboardButton("Документальный"),
                    new KeyboardButton("Драма"),
                    new KeyboardButton("Семейный")
                  },
                  new[]
                  {
                    new KeyboardButton("Фэнтези"),
                    new KeyboardButton("История"),
                    new KeyboardButton("Ужасы"),
                    new KeyboardButton("Детектив")
                  },
                  new[]
                  {
                    new KeyboardButton("Мелодрама"),
                    new KeyboardButton("Фантастика"),
                    new KeyboardButton("Триллер"),
                    new KeyboardButton("Военный")
                  },
                  new[]
                  {
                       new KeyboardButton("Основное меню")
                  }
                    });
                    await Bot.SendTextMessageAsync(message.Chat.Id, ". . .", replyMarkup: replyKeyboard);

                    if (message.Text == "Боевик")
                    {
                        genre_status["search_movies_by_genre"] = "Боевик";
                        int_genre = 28;
                        string_genre = genre_status["search_movies_by_genre"];
                    }
                    if (message.Text == "Приключения")
                    {
                        genre_status["search_movies_by_genre"] = "Приключения";
                        int_genre = 12;
                        string_genre = genre_status["search_movies_by_genre"];
                    }
                    if (message.Text == "Мультфильм")
                    {
                        genre_status["search_movies_by_genre"] = "Мультфильм";
                        int_genre = 16;
                        string_genre = genre_status["search_movies_by_genre"];
                    }
                    if (message.Text == "Комедия")
                    {
                        genre_status["search_movies_by_genre"] = "Комедия";
                        int_genre = 35;
                        string_genre = genre_status["search_movies_by_genre"];
                    }
                    if (message.Text == "Криминал")
                    {
                        genre_status["search_movies_by_genre"] = "Криминал";
                        int_genre = 80;
                        string_genre = genre_status["search_movies_by_genre"];
                    }
                    if (message.Text == "Документальный")
                    {
                        genre_status["search_movies_by_genre"] = "Документальный";
                        int_genre = 99;
                        string_genre = genre_status["search_movies_by_genre"];
                    }
                    if (message.Text == "Драма")
                    {
                        genre_status["search_movies_by_genre"] = "Драма";
                        int_genre = 18;
                        string_genre = genre_status["search_movies_by_genre"];
                    }
                    if (message.Text == "Семейный")
                    {
                        genre_status["search_movies_by_genre"] = "Семейный";
                        int_genre = 10751;
                        string_genre = genre_status["search_movies_by_genre"];
                    }
                    if (message.Text == "Фэнтези")
                    {
                        genre_status["search_movies_by_genre"] = "Фэнтези";
                        int_genre = 14;
                        string_genre = genre_status["search_movies_by_genre"];
                    }
                    if (message.Text == "История")
                    {
                        genre_status["search_movies_by_genre"] = "История";
                        int_genre = 36;
                        string_genre = genre_status["search_movies_by_genre"];
                    }
                    if (message.Text == "Ужасы")
                    {
                        genre_status["search_movies_by_genre"] = "Ужасы";
                        int_genre = 27;
                        string_genre = genre_status["search_movies_by_genre"];
                    }
                    if (message.Text == "Детектив")
                    {
                        genre_status["search_movies_by_genre"] = "Детектив";
                        int_genre = 9648;
                        string_genre = genre_status["search_movies_by_genre"];
                    }
                    if (message.Text == "Мелодрама")
                    {
                        genre_status["search_movies_by_genre"] = "Мелодрама";
                        int_genre = 10749;
                        string_genre = genre_status["search_movies_by_genre"];
                    }
                    if (message.Text == "Фантастика")
                    {
                        genre_status["search_movies_by_genre"] = "Фантастика";
                        int_genre = 878;
                        string_genre = genre_status["search_movies_by_genre"];
                    }
                    if (message.Text == "Триллер")
                    {
                        genre_status["search_movies_by_genre"] = "Триллер";
                        int_genre = 53;
                        string_genre = genre_status["search_movies_by_genre"];
                    }
                    if (message.Text == "Военный")
                    {
                        genre_status["search_movies_by_genre"] = "Военный";
                        int_genre = 10752;
                        string_genre = genre_status["search_movies_by_genre"];
                    }
                    Console.WriteLine(user_id + string_genre);

                    if (genre_status["search_movies_by_genre"] != "0" && message.Text == string_genre)
                    {
                        HttpResponseMessage response = await client.GetAsync("https://localhost:44393/api/proanimaFilms/genre/" + int_genre);
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        SearchMoviesByGenre movies_by_genre = JsonSerializer.Deserialize<SearchMoviesByGenre>(responseBody);

                        int n = 1;
                        for (int i = 0; i < movies_by_genre.results.Count; i++, n++)
                        {
                            moviesbygenre[n] = movies_by_genre.results[i].title;
                        }

                        string _moviesbygenre = " ";
                        foreach (KeyValuePair<int, string> keyValue in moviesbygenre)
                        {
                            _moviesbygenre += $"\n {keyValue.Key} - {keyValue.Value} \n";
                        }
                        await Bot.SendTextMessageAsync(message.Chat.Id, _moviesbygenre);
                        await Bot.SendTextMessageAsync(message.Chat.Id, $"Выберите номер фильма, который хотите просмотреть.");
                    }

                    if (message.Text != "/search_movies_by_genre" && message.Text != "Поиск фильма по жанру" && message.Text != string_genre)
                    {
                        int int_number = Convert.ToInt32(message.Text);

                        if (int_number > 0 && int_number <= 20)
                        {
                            foreach (int number in moviesbygenre.Keys)
                            {
                                if (int_number == number)
                                {
                                    HttpResponseMessage response = await client.GetAsync("https://localhost:44393/api/proanimaFilms/" + moviesbygenre[number]);
                                    response.EnsureSuccessStatusCode();
                                    string responseBody = await response.Content.ReadAsStringAsync();
                                    SearchMovies movies = JsonSerializer.Deserialize<SearchMovies>(responseBody);

                                    int max_popularity = 0;
                                    for (int i = 1; i < movies.results.Count - 1; i++)
                                    {
                                        if (movies.results[max_popularity].popularity > movies.results[i].popularity)
                                            continue;
                                        else
                                            max_popularity = i;
                                    }

                                    string title = $"{movies.results[max_popularity].title}";
                                    string overview = $"{movies.results[max_popularity].overview}";
                                    string poster_path = $"{image_path}{movies.results[max_popularity].poster_path}";
                                    int id = movies.results[max_popularity].id;

                                    string[] words_int_the_title = moviesbygenre[number].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    string movie_path = $"{basic_movie_path}{id}{"-"}{words_int_the_title}";

                                    await Bot.SendTextMessageAsync(message.Chat.Id, title);
                                    await Bot.SendPhotoAsync(e.Message.Chat.Id, poster_path, overview);

                                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                                    {
                                new[]
                                {
                                   InlineKeyboardButton.WithUrl($"Перейти по ссылке на '{moviesbygenre[number]}'", movie_path)
                                }
                            });
                                    await Bot.SendTextMessageAsync(message.Chat.Id, points, replyMarkup: inlineKeyboard);
                                }
                            }
                        }
                        else await Bot.SendTextMessageAsync(message.Chat.Id, "Вы ввели номер, которого нет в списке.Попытайтесь заново!");
                    }
                }


                if (status[user_id] == "Популярное")
                {
                    HttpResponseMessage response = await client.GetAsync("https://localhost:44393/api/proanimaFilms/popular");
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    SearchMovies popular_movies = JsonSerializer.Deserialize<SearchMovies>(responseBody);

                    if (message.Text == "Популярное")
                    {
                        await Bot.SendTextMessageAsync(message.Chat.Id, $"Список популярных фильмов. Обновляется каждый день.");

                        string popular = " ";
                        int j = 1;
                        for (int i = 0; i < popular_movies.results.Count; i++, j++)
                        {
                            popular += $"\n {j} - {popular_movies.results[i].title} \n";
                        }
                        await Bot.SendTextMessageAsync(message.Chat.Id, popular);
                        await Bot.SendTextMessageAsync(message.Chat.Id, $"Выберите номер фильма, который хотите просмотреть.");
                    }

                    if (message.Text != "Популярное")
                    {
                        int int_number = Convert.ToInt32(message.Text);

                        if (int_number > 0 && int_number <= 20)
                        {
                            HttpResponseMessage response1 = await client.GetAsync("https://localhost:44393/api/proanimaFilms/" + popular_movies.results[int_number - 1].title);
                            response1.EnsureSuccessStatusCode();
                            string responseBody1 = await response1.Content.ReadAsStringAsync();
                            SearchMovies movies = JsonSerializer.Deserialize<SearchMovies>(responseBody1);

                            int max_popularity = 0;
                            for (int i = 1; i < movies.results.Count - 1; i++)
                            {
                                if (movies.results[max_popularity].popularity > movies.results[i].popularity)
                                    continue;
                                else
                                    max_popularity = i;
                            }

                            string title = $"{movies.results[max_popularity].title}";
                            string overview = $"{movies.results[max_popularity].overview}";
                            string poster_path = $"{image_path}{movies.results[max_popularity].poster_path}";
                            int id = movies.results[max_popularity].id;

                            string[] words_int_the_title = popular_movies.results[int_number - 1].title.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            string movie_path = $"{basic_movie_path}{id}{"-"}{words_int_the_title}";

                            await Bot.SendTextMessageAsync(message.Chat.Id, title);
                            await Bot.SendPhotoAsync(e.Message.Chat.Id, poster_path, overview);

                            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                            {
                                new[]
                                {
                                   InlineKeyboardButton.WithUrl($"Перейти по ссылке на '{popular_movies.results[int_number-1].title}'", movie_path)
                                }
                        });
                            await Bot.SendTextMessageAsync(message.Chat.Id, points, replyMarkup: inlineKeyboard);
                        }
                        else await Bot.SendTextMessageAsync(message.Chat.Id, "Вы ввели номер, которого нет в списке. Попытайтесь заново!");
                    }
                }
            }
            catch (Exception) { await Bot.SendTextMessageAsync(message.Chat.Id, "Введите, пожалуйста, число!");};

            if (status[user_id] == "/favorites") //Вывод избранных фильмов пользователя
            {
                var favorites_replyKeyboard = new ReplyKeyboardMarkup(new[]
                       {
                  new[]
                  {
                    new KeyboardButton("Добавить в избранное"),
                    new KeyboardButton("Избранное"),
                    new KeyboardButton("Удалить из избранного")
                  },
                  new[]
                  {
                    new KeyboardButton("Основное меню")
                  }
                });
                await Bot.SendTextMessageAsync(message.Chat.Id, ". . .", replyMarkup: favorites_replyKeyboard);

                HttpResponseMessage response = await client.GetAsync("https://localhost:44393/api/proanimaFilms/featured/films/" + user_id);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Favorites favorites = JsonSerializer.Deserialize<Favorites>(responseBody);

                if (favorites.featuredfims.Count == 0)
                    await Bot.SendTextMessageAsync(message.Chat.Id, "У вас нет избранных фильмов.");
                else
                {
                    await Bot.SendTextMessageAsync(message.Chat.Id, "Список избранных фильмов:");

                    string _featuredfims = " ";
                    foreach (string film in favorites.featuredfims)
                    {
                        _featuredfims += $"\n {film} \n";
                    }
                    await Bot.SendTextMessageAsync(message.Chat.Id, _featuredfims);
                }
            }

            if (status[user_id] == "Добавить в избранное") //Добавить фильм в избранное
            {
                if (message.Text == "Добавить в избранное")
                    await Bot.SendTextMessageAsync(message.Chat.Id, "Введите название фильма, который хотите добавить в избранное.");

                else
                {
                    HttpResponseMessage response = await client.GetAsync("https://localhost:44393/api/proanimaFilms/featured/films/" + user_id);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Favorites favorites = JsonSerializer.Deserialize<Favorites>(responseBody);

                    favorites.featuredfims.Add(message.Text);
                    var json = JsonSerializer.Serialize<Favorites>(favorites);
                    Console.WriteLine(json);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    await client.PutAsync("https://localhost:44393/api/proanimaFilms/" + user_id, data);
                    await Bot.SendTextMessageAsync(message.Chat.Id, "Готово!");
                    await Bot.SendTextMessageAsync(message.Chat.Id, "Введите название фильма, который также хотите добавить в избранное.");
                }
            }

            if(status[user_id] == "Удалить из избранного") //Удалить фильм из избранного
            {
                if (message.Text == "Удалить из избранного")
                    await Bot.SendTextMessageAsync(message.Chat.Id, "Введите название фильма, который хотите удалить из избранного. Если вы хотите очистить весь список избранных фильмов, введите 'Clear'.");

                else
                {
                    HttpResponseMessage response = await client.GetAsync("https://localhost:44393/api/proanimaFilms/featured/films/" + user_id);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Favorites favorites = JsonSerializer.Deserialize<Favorites>(responseBody);

                    if (message.Text.ToLower() == "clear")
                    {
                        favorites.featuredfims.Clear();
                        var json = JsonSerializer.Serialize<Favorites>(favorites);
                        Console.WriteLine(json);
                        var data = new StringContent(json, Encoding.UTF8, "application/json");
                        await client.PutAsync("https://localhost:44393/api/proanimaFilms/" + user_id, data);
                        await Bot.SendTextMessageAsync(message.Chat.Id, "Список избранных фильмов очищен.");
                        status[user_id] = "Основное меню";
                        return;
                    }

                    if (favorites.featuredfims.Contains(message.Text))
                    {
                        favorites.featuredfims.Remove(message.Text);
                        var json = JsonSerializer.Serialize<Favorites>(favorites);
                        Console.WriteLine(json);
                        var data = new StringContent(json, Encoding.UTF8, "application/json");
                        await client.PutAsync("https://localhost:44393/api/proanimaFilms/" + user_id, data);
                        await Bot.SendTextMessageAsync(message.Chat.Id, "Готово!");
                        await Bot.SendTextMessageAsync(message.Chat.Id, "Возможно, вы хотите удалить еще какой-то фильм? Введите его название.");
                    }

                    else
                        await Bot.SendTextMessageAsync(message.Chat.Id, "Введенное вами название отсутствует в избранном! Возможно, вы допустили ошибку при вводе. Попытайтесь еще раз.");
                }
            }
        }    
    }
}
