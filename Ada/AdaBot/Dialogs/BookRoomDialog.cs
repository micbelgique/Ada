using AdaSDK;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Bot.Connector;
using System.Configuration;
using TechOfficeSDK;
using TechOfficeSDK.Models;

namespace AdaBot.Dialogs
{
    [Serializable]
    public class BookRoomDialog : IDialog<object>
    {
        [NonSerialized]
        TechOfficeClient _client;

        public BookRoomDialog(LuisResult result)
        {
            _client = new TechOfficeClient() { WebAppUrl = $"{ ConfigurationManager.AppSettings["TechOfficeAppUrl"] }" };
        }

        public async Task StartAsync(IDialogContext context)
        {
            var rooms = await _client.GetRooms();
            await DisplayRooms(context, rooms);
        }
        
        private async Task DisplayRooms(IDialogContext context, IEnumerable<Room> rooms)
        {
            var msg = context.MakeMessage();
            msg.Text = "Voici les salles disponibles";
            msg.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            msg.Attachments = BuildRoomCards(rooms).Select(x => x.ToAttachment()).ToList();
            await context.PostAsync(msg);
            context.Wait(BookRoomCallback);
        }

        private async Task DisplayRoomDetails(IDialogContext context, Room room)
        {
            var msg = context.MakeMessage();
            msg.Text = "Vous avez réservé la salle !";
            await context.PostAsync(msg);
        }

        #region CallBacks
        private async Task BookRoomCallback(IDialogContext context, IAwaitable<IMessageActivity> result)
        { 
            var activity = await result;
            try
            {
                var action = OrderButtonAction.FromJson(activity.Text);
                Room room;
                string msg;

                switch (action.Type)
                {
                    case 2: 
                        _client = new TechOfficeClient() { WebAppUrl = $"{ ConfigurationManager.AppSettings["TechOfficeAppUrl"] }" };
                        room = await _client.GetRoom(action.Value);

                        // On inverse le statut de la room et on la poste
                        room.DoUndo();
                        await _client.PutRoom(room);

                        msg = $"La salle est maintenant {room.displayStatus()}";
                        await context.PostAsync($"{msg}");
                        break;
                    case 3:
                        _client = new TechOfficeClient() { WebAppUrl = $"{ ConfigurationManager.AppSettings["TechOfficeAppUrl"] }" };
                        room = await _client.GetRoom(action.Value);

                        msg = $"Cette salle d'une capacité de {room.Capacity} personnes est actuellement {room.displayStatus()}.";
                        
                        await context.PostAsync($"{msg}");
                        break;
                }
                context.Done(true);
            }
            catch (Exception)
            {
                // todo: ewww
            }
        }

        #endregion

        #region Cards

        private List<HeroCard> BuildRoomCards(IEnumerable<Room> rooms)
        {
            var cards = new List<HeroCard>();
            foreach (var room in rooms)
            {
                var image = new CardImage($"{new Uri(ConfigurationManager.AppSettings["TechOfficeAppUrl"])}{room.Picture}");
                var buttonBook = new CardAction
                {
                    Value = new OrderButtonAction { Type = 2, Value = room.Id }.ToJson(),
                    Title = "Reserver",
                    Type = ActionTypes.PostBack
                };
                var buttonDetail = new CardAction
                {
                    Value = new OrderButtonAction { Type = 3, Value = room.Id }.ToJson(),
                    Title = "Detail",
                    Type = ActionTypes.PostBack
                };
                cards.Add(new HeroCard
                {
                    Title = room.Name,
                    Text = $"Capacité : {room.Capacity} personnes | statut : {room.displayStatus()}",
                    Buttons = new List<CardAction> { buttonBook, buttonDetail },
                    Images = new List<CardImage> { image }
                });
            }

            return cards;
        }

        #endregion

    }

    internal class OrderButtonAction
    {
        public int Type { get; set; }
        public int Value { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static OrderButtonAction FromJson(string json)
        {
            return JsonConvert.DeserializeObject<OrderButtonAction>(json);
        }

    }

}