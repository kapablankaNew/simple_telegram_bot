using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using RestSharp;


namespace Trlegram_bot
{
    public class TelegramAPI
    {
        public class API_Result  //результат апи запроса - массив данных о полученных в чатах сообщениях
        {
            public Update[] result { get; set; }
        }

        public class Update   //основные параметры каждого апдейта - номер апдейта (int? - чтобы был null) и сообщение
        {
            public int? update_id { get; set; }
            public Message message { get; set; }
        }

        public class Message  //параметры сообщения - его текст, а также данные о чате, из которого оно получено
        {
            public Chat chat { get; set; }
            public string text { get; set; }
        }

        public class Chat  //параметры чата - его id и ник человека, с которым ведется диалог
        {
            public int id { get; set; }
            public string first_name { get; set; }
        }


        private int lastUpdateId = 0;  //чтобы не отвечать на одни и те же сообщения по нескольку раз, записываем номер последнего сообщения

        public TelegramAPI() {}

        //создаем объект, который будем общаться по заданному УРЛ
        RestClient RC = new RestClient();
        //адрес для запросов с секретным ключом
        const string API_URL = "https://api.telegram.org/bot" + SecretKEY.API_KEY + "/";

        public void sendMessage(string text, int chat_id)
        {
            sendApiRequest("sendMessage", $"chat_id={chat_id}&text={text}");  //отправляем в чат номер chat_id текстовое сообщение
        }

        public Update[] getUpdates()  //запрашиваем у сервера список апдейтов и возвращаем их единым массивом
        {
            //получаем от сервера массив апдейтов в json начиная с lastUpdateId
            var json = sendApiRequest("getUpdates",$"offset={lastUpdateId}");
            //конвертим результат из json в массив result-ов
            var apiResult = JsonConvert.DeserializeObject<API_Result>(json);
            foreach (var update in apiResult.result) 
            {
                if (update.message == null || update.update_id == null || update.message.text == null)
                {
                    continue; //если сообщение не текстовое, или произошли проблемы с его получением - просто игнорим
                }
                else
                {
                    lastUpdateId = (int)update.update_id + 1; //иначе меняем номер последнего апдейта, и на следующей итерации читаем далее
                }
            }
            return apiResult.result;
        }

        private string sendApiRequest(string ApiMethod, string Params)
        {
            //формируем ссылку
            var URL = API_URL + ApiMethod + "?" + Params;
            //готовим объект запроса
            var Request = new RestRequest(URL);
            //выполняем запрос
            var Response = RC.Get(Request);
            //возвращаем полученные данные
            return Response.Content;
        }
    }
}
