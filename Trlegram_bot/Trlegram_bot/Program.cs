using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Trlegram_bot
{
    class Program
    {
        private static Dictionary<string, string> DataBase;

        private static Currency currencyApi;

        static void Main(string[] args)
        {
            currencyApi = new Currency();
            currencyApi.download();

            var Data = File.ReadAllText("DataBase.json");  //читаем данные из JSON файла
            DataBase = JsonConvert.DeserializeObject<Dictionary<string, string>>(Data); //перекодируем из JSON в словарь

            var Api = new TelegramAPI(); //экземпляр класса для взаимодействия с АРI телеграмма

            for (; ; )
            {
                var updates = Api.getUpdates(); //получили апдейты
                foreach (var update in updates) 
                {
                    if (update.message == null || update.update_id == null || update.message.text == null)
                    {
                        continue; //если сообщение не текстовое или произошла проблема - игнорим
                    }
                    var answer = answerQuestion(update.message.text); //получаем ответ на сообщение
                    //отправляем ответ пользователю, обращаясь к нему по имени
                    var message = $"Dear {update.message.chat.first_name}, {answer}"; 
                    Api.sendMessage(message, update.message.chat.id); //отправляем сообщение в чат номер update.message.chat.id
                }
            }            
        }

        private static string answerQuestion(string Question) //метод получения ответа на сообщение пользователя
        {
            var Answers = new List<string>(); //создаем массив ответов

            var UserQuestion = Question.ToLower(); //читаем и сразу приводим к нижнему регистру

            foreach (var Entry in DataBase)  //перебираем все элементы словаря
            {
                if (UserQuestion.Contains(Entry.Key))  //Contains - содержит
                {
                    Answers.Add(Entry.Value);
                }
            }

            if (UserQuestion.Contains("tell me a quote")) //рассказать афоризм
            {
                var forismatic = new Forismatic();
                Answers.Add(forismatic.getRandom());
            }

            if (UserQuestion.Contains("what day is it"))
            {
                Answers.Add($"Today: {DateTime.Now.ToString("dd.MM.yyyy")}."); //закидываем данные в формате дд.мм.гггг
            }

            if (UserQuestion.Contains("what time is it"))
            {
                Answers.Add($"Current time: {DateTime.Now.ToString("HH:mm:ss")}."); //закидываем данные в формате чч:мм:сс
            }

            if (UserQuestion.StartsWith("what is exchange rate for "))
            {
                var code = UserQuestion.Substring(UserQuestion.Length - 3); //последние три символа
                Answers.Add(currencyApi.getRate(code.ToUpper()));
            }

            if (Answers.Count == 0)
            {
                return ("i'm sorry, i don't understand :( Repeat, please.");
            }
            else
            {
                string ans = string.Join(" ", Answers);    //собираем получненые ответы в единую строку, разделяя их пробелами
                //для красоты у первого элемента заменяем первый символ с заглавного на строчный
                ans = ans.Remove(0, 1).Insert(0, ans[0].ToString().ToLower());
                return (ans);   //возвращаем ответ
            }
        }
    }
}
