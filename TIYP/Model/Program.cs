﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
	class Program
	{
		private static readonly TelegramBotClient Bot = new TelegramBotClient("255451843:AAH9aCJiFqundaJfrv2sjO16Mpo8jLEAUrw");
		private static readonly DB.Clients ClientDB =  new DB.Clients();

		public static Dictionary <long, string> state = new Dictionary <long, string>();

		static void Main(string[] args)
		{
			//Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
			Bot.OnMessage += BotOnMessageReceived;
			Bot.OnMessageEdited += BotOnMessageReceived;
			Bot.OnInlineQuery += BotOnInlineQueryReceived;
			Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
			Bot.OnReceiveError += BotOnReceiveError;
			Bot.OnMessage += Start.BotOnMessageLocationReceiving;
			Bot.OnMessage += Start.BotOnMessagePhoneNumberReceiving;
			Bot.OnMessage += Start.BotOnMessageMailReceiving;
			Bot.OnMessage += Work.BotChooseMenuOrTask;
			Bot.OnMessage += Work.BotSetTaskName;
			Bot.OnMessage += Work.BotShowTask;
			Bot.OnCallbackQuery += Work.BotTaskParamQueryReceived;
			Bot.OnMessage += Work.BotSetTaskDateStart;
			Bot.OnCallbackQuery += Work.BotDateQueryReceived;


			state = ClientDB.Client ();
			foreach (var v in state) {
				Console.WriteLine (v);
			}

			//	var me = Bot.GetMeAsync().Result;

			//Console.Title = me.Username;

			//Bot.SendTextMessage(

			Bot.StartReceiving();
			Console.ReadLine();
			Bot.StopReceiving();
		}


		private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
		{
			Debugger.Break();
		}

		private static void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs chosenInlineResultEventArgs)
		{
			Console.WriteLine($"Received choosen inline result: {chosenInlineResultEventArgs.ChosenInlineResult.ResultId}");
		}

		private static async void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs inlineQueryEventArgs)
		{
			InlineQueryResult[] results = {
				new InlineQueryResultLocation
				{

					Id = "1",
					Latitude = 40.7058316f, // displayed result
					Longitude = -74.2581888f,
					Title = "New York",
					InputMessageContent = new InputLocationMessageContent // message if result is selected
					{
						Latitude = 40.7058316f,
						Longitude = -74.2581888f,
					}
				},

				new InlineQueryResultLocation
				{
					Id = "2",
					Longitude = 52.507629f, // displayed result
					Latitude = 13.1449577f,
					Title = "Berlin",
					InputMessageContent = new InputLocationMessageContent // message if result is selected
					{
						Longitude = 52.507629f,
						Latitude = 13.1449577f
					}
				}
			};

			await Bot.AnswerInlineQueryAsync("1", results, isPersonal: true, cacheTime: 0);
		}

		private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
		{
			var message = messageEventArgs.Message;


			if (message.Type == MessageType.TextMessage ) {



				if (message.Text.StartsWith ("/inline")) { // send inline keyboard
					await Bot.SendChatActionAsync (message.Chat.Id, ChatAction.Typing);
					var keyboard = new InlineKeyboardMarkup (new[] {
						new[] { // second row
							new InlineKeyboardButton ("пн"),
							new InlineKeyboardButton ("вт"),
							new InlineKeyboardButton ("ср"),
							new InlineKeyboardButton ("чт"),
							new InlineKeyboardButton ("пт"),
							new InlineKeyboardButton ("сб"),
							new InlineKeyboardButton ("вс"),
						},
						new[] { // second row
							new InlineKeyboardButton ("2.1"),
							new InlineKeyboardButton ("2.2"),
							new InlineKeyboardButton ("2.1"),
							new InlineKeyboardButton ("2.2"),
							new InlineKeyboardButton ("2.1"),
							new InlineKeyboardButton ("2.2"),
							new InlineKeyboardButton ("2.1"),
						},
						new[] { // second row
							new InlineKeyboardButton ("2.1"),
							new InlineKeyboardButton ("2.2"),
							new InlineKeyboardButton ("2.1"),
							new InlineKeyboardButton ("2.2"),
							new InlineKeyboardButton ("2.1"),
							new InlineKeyboardButton ("2.2"),
							new InlineKeyboardButton ("2.1"),
						},
						new[] { // second row
							new InlineKeyboardButton ("2.1"),
							new InlineKeyboardButton ("2.2"),
							new InlineKeyboardButton ("2.1"),
							new InlineKeyboardButton ("2.2"),
							new InlineKeyboardButton ("2.1"),
							new InlineKeyboardButton ("2.2"),
							new InlineKeyboardButton ("2.1"),
						},
						new[] { // second row
							new InlineKeyboardButton ("2.1"),
							new InlineKeyboardButton ("2.2"),
							new InlineKeyboardButton ("2.1"),
							new InlineKeyboardButton ("2.2"),
							new InlineKeyboardButton ("2.1"),
							new InlineKeyboardButton ("2.2"),
							new InlineKeyboardButton ("2.1"),
						},
						new[] { // second row
							new InlineKeyboardButton ("2.1"),
							new InlineKeyboardButton ("2.2"),
							new InlineKeyboardButton ("2.1"),
							new InlineKeyboardButton ("2.2"),
							new InlineKeyboardButton ("2.1"),
							new InlineKeyboardButton (" "),
							new InlineKeyboardButton (" "),
						}
					});


					string[] weekDays = { "Sun asd", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
					var keybb = new InlineKeyboardButton[1][];
					//var keybb1 = new InlineKeyboardButton[weekDays.Length];
					for (int i = 0; i < weekDays.Length; i++) 
					{
						keybb  [0][i] = new InlineKeyboardButton (weekDays [i]);

							/*keybb [i] = new[] {
							
							keybb[i][i]
						};*/
				
					}
					//keybb [0] = keybb1;
					var keyboard1 = new InlineKeyboardMarkup(keybb);
					await Bot.SendTextMessageAsync (message.Chat.Id, "1 "  ,replyMarkup: keyboard1);
					await Task.Delay (5000); // simulate longer running task

					await Bot.SendTextMessageAsync (message.Chat.Id, "Choose the day",
						replyMarkup: keyboard);
				} else if (message.Text.StartsWith ("/keyboard")) { // send custom keyboard
					var keyboard = new ReplyKeyboardMarkup (new[] {
						new [] { // first row
							new KeyboardButton ("1.1"),
							new KeyboardButton ("1.2"),  
						},
						new [] { // last row
							new KeyboardButton ("2.1"),
							new KeyboardButton ("2.2"),  
						}
					});

					await Bot.SendTextMessageAsync (message.Chat.Id, "Choose",
						replyMarkup: keyboard);
				} else if (message.Text.StartsWith ("/photo")) { // send a photo
					await Bot.SendChatActionAsync (message.Chat.Id, ChatAction.UploadPhoto);

					const string file = @"<FilePath>";

					var fileName = file.Split ('\\').Last ();

					using (var fileStream = new FileStream (file, FileMode.Open, FileAccess.Read, FileShare.Read)) {
						var fts = new FileToSend (fileName, fileStream);

						await Bot.SendPhotoAsync (message.Chat.Id, fts, "Nice Picture");
					}
				} else if (message.Text.StartsWith ("/request")) { // request location or contact
					var keyboard = new ReplyKeyboardMarkup (new [] {
						new KeyboardButton ("Location") {
							RequestLocation = true
						},
						new KeyboardButton ("Contact") {
							RequestContact = true
						}, 
					});

					await Bot.SendTextMessageAsync (message.Chat.Id, "Who or Where are you?", replyMarkup: keyboard);
				} else if (message.Text.StartsWith ("/start")) {
					
					/*try
					{
						Console.WriteLine(state [message.Chat.Id]);
					}
					catch(KeyNotFoundException) 
					{
						ClientDB.AddClient (message.Chat.Id, message.Chat.FirstName, message.Chat.LastName);

						state [message.Chat.Id] = "Location";
						
						Console.WriteLine (state [message.Chat.Id]);
					}*/

				} 

				/*else {

					var usage = @"Используйте:
	/inline   - send inline keyboard
	/keyboard - send custom keyboard
	/photo    - send a photo
	/request  - request location or contact
	гавно 
	";
					Console.WriteLine (message.Chat.Id);

					await Bot.SendTextMessageAsync (message.Chat.Id, usage,
						replyMarkup: new ReplyKeyboardHide ());
				}*/
			}
		}
		private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
		{
			var callback = callbackQueryEventArgs.CallbackQuery;
			var keyboard = new InlineKeyboardMarkup (new[] {
				new[] { // first row
					new InlineKeyboardButton ("Berlin") {
						CallbackData = "Berlin123"
					},
					new InlineKeyboardButton ("1.2"),
				},
				new[] { // second row
					new InlineKeyboardButton ("2.1"),
					new InlineKeyboardButton ("2.2"),
				}
			});
			if (callbackQueryEventArgs.CallbackQuery.Data == "Berlin123") 
			{
				var usage = @"Жизнь боль";

				//await Bot.SendTextMessageAsync (callbackQueryEventArgs.CallbackQuery.Message.Chat.Id, usage,	replyMarkup: new ReplyKeyboardHide ());
				Console.WriteLine (callback.Message.MessageId);
				await Bot.EditMessageTextAsync (callback.Message.Chat.Id, callback.Message.MessageId, "ебать колупать, поменялось!", replyMarkup: keyboard);
				//await Bot.EditInlineMessageReplyMarkupAsync (callback.Message.MessageId.ToString(), replyMarkup: keyboard);
			}

			await Bot.AnswerCallbackQueryAsync(callbackQueryEventArgs.CallbackQuery.Id, $"Received {callbackQueryEventArgs.CallbackQuery.Data}");
		}


	}
}


/*"message":{"message_id":632,
 * "from":{"id":213628639,"first_name":"Valentin","last_name":"Zykov"},
 * "chat":{"id":213628639,"first_name":"Valentin","last_name":"Zykov","type":"private"},
 * "date":1478261672,
 * "location":{"latitude":56.869359,"longitude":60.545133}}},
 * 
 * {"update_id":963258170,
 * "message":{"message_id":633,
 * "from":{"id":213628639,"first_name":"Valentin","last_name":"Zykov"},
 * "chat":{"id":213628639,"first_name":"Valentin","last_name":"Zykov","type":"private"},
 * "date":1478261679,
 * "photo":[{"file_id":"AgADAgADfqgxG9-2uwz8igbgc0-Z414OSw0ABOq7tnfjancrwxsBAAEC","file_size":1552,"width":90,"height":49},{"file_id":"AgADAgADfqgxG9-2uwz8igbgc0-Z414OSw0ABC0St2MBe0eXwhsBAAEC","file_size":19881,"width":320,"height":174},{"file_id":"AgADAgADfqgxG9-2uwz8igbgc0-Z414OSw0ABGX0s-ZazalpxBsBAAEC","file_size":85716,"width":800,"height":436},{"file_id":"AgADAgADfqgxG9-2uwz8igbgc0-Z414OSw0ABMrogAGojlX_wRsBAAEC","file_size":138529,"width":1280,"height":697}]}},{"update_id":963258171,
 * 
 * "message":{"message_id":634,
 * "from":{"id":213628639,"first_name":"Valentin","last_name":"Zykov"},
 * "chat":{"id":213628639,"first_name":"Valentin","last_name":"Zykov","type":"private"},
 * "date":1478261704,
 * "text":"sdf"}}]}
 */