using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.ReplyMarkups;

using System.Data;
using MySql.Data.MySqlClient;

namespace Telegram.Bot.TIYP
{
	public class Start
	{
		private static readonly TelegramBotClient Bot = new TelegramBotClient("255451843:AAH9aCJiFqundaJfrv2sjO16Mpo8jLEAUrw");
		private static readonly DB.Clients ClientDB =  new DB.Clients();


		public static async void BotOnMessageLocationReceiving(object sender, MessageEventArgs messageEventArgs)
		{
			var message = messageEventArgs.Message;



				if (message.Type == MessageType.TextMessage) {
					if (message.Text.StartsWith ("/start")) {
					var usage = @"Приветствую Вас, " + message.Chat.FirstName + " " + message.Chat.LastName + "!";

					await Bot.SendTextMessageAsync (message.Chat.Id, usage, replyMarkup: new ReplyKeyboardHide ());
					try {
						Console.WriteLine (Program.state [message.Chat.Id]);
					} catch (KeyNotFoundException) {
						ClientDB.AddClient (message.Chat.Id, message.Chat.FirstName, message.Chat.LastName);

						Program.state [message.Chat.Id] = "Location";

						Console.WriteLine (Program.state [message.Chat.Id]);
					}
					if (Program.state [message.Chat.Id] != "Location") 
					{
						Program.state [message.Chat.Id] = "SetReminder";

					}
					}
				}


			//await	Task.Delay (1000); // simulate longer running task
			try {
			if (Program.state [message.Chat.Id] == "Location") 
			{
				if (message.Type != MessageType.LocationMessage)
				{
					await Bot.SendChatActionAsync (message.Chat.Id, ChatAction.Typing);

					string usage = @"Отправьте пожалуйста своё местоположение, чтобы мы знали, какой у Вас часовой пояс.";

					var keyboard = new ReplyKeyboardMarkup (new [] {
						new KeyboardButton ("Location") {
							RequestLocation = true,

						},
					});
					keyboard.ResizeKeyboard = true;
					await	Task.Delay (200); // simulate longer running task

					await Bot.SendTextMessageAsync (message.Chat.Id, usage, replyMarkup: keyboard);
				//
				}
				
				/*if (message.Type == MessageType.LocationMessage)
				{
					ClientDB.AddLocation (message.Chat.Id, message.Location.Longitude, message.Location.Latitude);
					Program.state [message.Chat.Id] = "Phone";
				}*/
			}


			} catch (KeyNotFoundException) {
				Console.WriteLine("EbalA Location");
			}
		}
		public static async void BotOnMessagePhoneNumberReceiving(object sender, MessageEventArgs messageEventArgs)
		{
			try {
			var message = messageEventArgs.Message;
			if (Program.state [message.Chat.Id] == "Location") {
				if (message.Type == MessageType.LocationMessage) {
					ClientDB.AddLocation (message.Chat.Id, message.Location.Longitude, message.Location.Latitude);
					Program.state [message.Chat.Id] = "Phone";
				}
			}

			if (Program.state [message.Chat.Id] == "Phone") {
				if (message.Type != MessageType.ContactMessage) {
					if (message.Type == MessageType.TextMessage) {
						if (message.Text.StartsWith ("Not send")) {
							Program.state [message.Chat.Id] = "Mail";
							Console.WriteLine ("гавно");
						} else {
							string usage = @"Используйте кнопки внизу экрана: Not send или Send number.";

							var keyboard = new ReplyKeyboardMarkup (new [] {
								new KeyboardButton ("Not send") {

								},
								new KeyboardButton ("Send number") {
									RequestContact = true,

								},

							});

							keyboard.ResizeKeyboard = true;
							await Task.Delay (200); // simulate longer running task

							await Bot.SendTextMessageAsync (message.Chat.Id, usage, replyMarkup: keyboard);
						}
					} else {
						string usage = @"Отправьте пожалуйста свой номер телефона, чтобы в дальнейшем Вы могли использовать все функции TIYP.";

						var keyboard = new ReplyKeyboardMarkup (new [] {
							new KeyboardButton ("Not send") {

							},
							new KeyboardButton ("Send number") {
								RequestContact = true,

							},

						});

						keyboard.ResizeKeyboard = true;
						await Task.Delay (200); // simulate longer running task

						await Bot.SendTextMessageAsync (message.Chat.Id, usage, replyMarkup: keyboard);
					}
				} else {

				}
			}
			} catch (KeyNotFoundException) {
				Console.WriteLine("EbalA Phone");
			}
		}
		public static async void BotOnMessageMailReceiving(object sender, MessageEventArgs messageEventArgs)
		{
			try {
			var message = messageEventArgs.Message;
			if (Program.state [message.Chat.Id] == "Phone") {
				if (message.Type == MessageType.ContactMessage) {
					//await Bot.SendTextMessageAsync (message.Chat.Id, "Все Ваши данные будут находиться в строжайшей секретности!", replyMarkup: new ReplyKeyboardHide ());
					ClientDB.AddPhone (message.Chat.Id, message.Contact.PhoneNumber);
					Program.state [message.Chat.Id] = "Mail";
				} else if (message.Type == MessageType.TextMessage) {
					if (message.Text.StartsWith ("Not send")) {
						Program.state [message.Chat.Id] = "Mail";
						Console.WriteLine ("гавно");
					}
				}
			}

			if (Program.state [message.Chat.Id] == "Mail") 
			{
				if (message.Type != MessageType.TextMessage) {
					await Bot.SendChatActionAsync (message.Chat.Id, ChatAction.Typing);

					string usage = @"Отправьте пожалуйста свой e-mail.";

					var keyboard = new ReplyKeyboardMarkup (new [] {
						new KeyboardButton ("Not send e-mail") {
						},
					});
					keyboard.ResizeKeyboard = true;
					await Task.Delay (200); // simulate longer running task

					await Bot.SendTextMessageAsync (message.Chat.Id, usage, replyMarkup: keyboard);
				} else {
					Regex ReMail = new Regex (@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
					MatchCollection matches = ReMail.Matches (message.Text);

					if (matches.Count == 1) {
						Program.state [message.Chat.Id] = "SetReminder";
						await Bot.SendTextMessageAsync (message.Chat.Id, "Спасибо, что начали использовать TIYP!", replyMarkup: new ReplyKeyboardHide ());
						ClientDB.AddMail (message.Chat.Id, message.Text);
						await Task.Delay (500);
						var keyboard = new ReplyKeyboardMarkup (new [] {
							new KeyboardButton ("Новая задача") {
							},
							new KeyboardButton ("Меню") {
							}, 
						});
						keyboard.ResizeKeyboard = true;
						await Bot.SendTextMessageAsync (message.Chat.Id, "Желаете создать новую задачу?", replyMarkup: keyboard);
					} else if (message.Text.StartsWith ("Not send e-mail")) {
						Program.state [message.Chat.Id] = "SetReminder";
						await Bot.SendTextMessageAsync (message.Chat.Id, "Спасибо, что начали использовать TIYP!", replyMarkup: new ReplyKeyboardHide ());
						await Task.Delay (500);
						var keyboard = new ReplyKeyboardMarkup (new [] {
							new KeyboardButton ("Новая задача") {
							},
							new KeyboardButton ("Меню") {
							}, 
						});
						keyboard.ResizeKeyboard = true;
						await Bot.SendTextMessageAsync (message.Chat.Id, "Желаете создать новую задачу?", replyMarkup: keyboard);

					} else {
						await Bot.SendChatActionAsync (message.Chat.Id, ChatAction.Typing);

						string usage = @"Не верный e-mail.";

						var keyboard = new ReplyKeyboardMarkup (new [] {
							new KeyboardButton ("Not send e-mail") {
							},
						});
						keyboard.ResizeKeyboard = true;

						await Bot.SendTextMessageAsync (message.Chat.Id, usage, replyMarkup: keyboard);

					}

				}
					
			}
			} catch (KeyNotFoundException) {
				Console.WriteLine("EbalA Mail");
			}
		}
	}
}


