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
	

	public class Work
	{
		private static readonly TelegramBotClient Bot = new TelegramBotClient("255451843:AAH9aCJiFqundaJfrv2sjO16Mpo8jLEAUrw");
		private static readonly DB.Work WorktDB =  new DB.Work();
		private static Dictionary<long, DateTime> workTime = new Dictionary<long, DateTime> ();

		/// <summary>
		/// Bots the choose menu or task.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="messageEventArgs">The ${ParameterType} instance containing the event data.</param>
		public static async void BotChooseMenuOrTask(object sender, MessageEventArgs messageEventArgs)
		{
			var message = messageEventArgs.Message;
			try
			{
				if (message.Type == MessageType.TextMessage) {
					if (message.Text.StartsWith ("/start")) {
						if (Program.state [message.Chat.Id] == "Normal") {
							Program.state [message.Chat.Id] = "SetReminder";
						}
					}
				}
				Console.WriteLine (Program.state [message.Chat.Id] + "1234");

				if (Program.state[message.Chat.Id] == "SetReminder")
				{
					if (message.Type == MessageType.TextMessage) {
						if (message.Text.StartsWith ("Новая задача")) {
							Program.state [message.Chat.Id] = "NewTask";
						} else if (message.Text.StartsWith ("Меню")) {
							Program.state [message.Chat.Id] = "Menu";
						} else {
							var keyboard = new ReplyKeyboardMarkup (new [] {
								new KeyboardButton ("Новая задача") {
								},
								new KeyboardButton ("Меню") {
								}, 
							});
							keyboard.ResizeKeyboard = true;
							await Bot.SendTextMessageAsync (message.Chat.Id, "Желаете создать новую задачу?", replyMarkup: keyboard);
						}
					}
				}
			}
			catch (KeyNotFoundException) {
				Console.WriteLine("EbalA MenuOrTask");
			}
		}

		/// <summary>
		/// имя задачи
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="messageEventArgs">The ${ParameterType} instance containing the event data.</param>
		public static async void BotSetTaskName (object sender, MessageEventArgs messageEventArgs)
		{
			try
			{
				var message = messageEventArgs.Message;
				if (message.Type == MessageType.TextMessage) {
					if (Program.state [message.Chat.Id] == "SetReminder") {
						
						if (message.Text.StartsWith ("Новая задача")) {
							Program.state [message.Chat.Id] = "NewTask";
							await Task.Delay (1000);

						}
					
				}
					if (message.Text.StartsWith("/newtask"))
					{
						Program.state [message.Chat.Id] = "NewTask";
					}

					if (Program.state [message.Chat.Id] == "NewTask" ) 
					{
						if (message.Text.StartsWith("/newtask"))
							{
								string usage = @"Send task name.";
								await Bot.SendTextMessageAsync (message.Chat.Id, usage, replyMarkup: new ReplyKeyboardHide ());	
							}
							else
							{
								WorktDB.AddTaskName (message.Chat.Id, message.Text.ToString ());
							}
 					}
			}
			}
			catch (KeyNotFoundException) {
				Console.WriteLine("EbalA TaskName");
			}
		}


		public static async void BotSetTaskDateStart (object sender, MessageEventArgs messageEventArgs)
		{
			try
			{
				var message = messageEventArgs.Message;
				if (Program.state [message.Chat.Id] == "NewTask") 
				{
					if (message.Type == MessageType.TextMessage) 
					{
						if (!message.Text.StartsWith ("/newtask")) 
						{
							Program.state [message.Chat.Id] = "SetDateStart";

						}
					}
				}

				if (Program.state [message.Chat.Id] == "SetDateStart") {
					if (message.Type == MessageType.TextMessage) {
						if (message.Text.StartsWith ("Yes")) {
						Program.state [message.Chat.Id] = "YearAndMonth";
						workTime.Add(message.Chat.Id, new DateTime());
						workTime [message.Chat.Id] = workTime [message.Chat.Id].AddYears (DateTime.Now.Year - 1);
						Console.WriteLine (workTime [message.Chat.Id]);
						} else if (message.Text.StartsWith ("No")) {
							Program.state [message.Chat.Id] = "Normal";
						} else {
							var keyboard = new ReplyKeyboardMarkup (
								               new[] {
									new[] {
										new KeyboardButton ("Yes"),
										new KeyboardButton ("No"),
									}
								});
							keyboard.ResizeKeyboard = true;
							await Bot.SendTextMessageAsync (message.Chat.Id, "Установить дату и напоимнание?", replyMarkup: keyboard);
						}
					}
				}

				if (Program.state [message.Chat.Id] == "YearAndMonth") 
				{
					string[] month = {
						"Январь - January",
						"Февраль - February",
						"Март - March",
						"Апрель - April",
						"Май - May",
						"Июнь - June",
						"Июль - July",
						"Август - August",
						"Сентябрь - September",
						"Октябрь - October",
						"Ноябрь - November",
						"Декабрь - December"
					};

				/*var keybb = new KeyboardButton[12][];

				for (int i = 11; i >= 0; i--) 
				{
					
					keybb [i ] = new[]
					{
						new KeyboardButton (month [i])

							//CallbackData = i.ToString()
						}
					};
				}
				var keyboard = new ReplyKeyboardMarkup(keybb);
				await Bot.SendTextMessageAsync (message.Chat.Id, "Месяца keyboard ",replyMarkup: keyboard);*/

					if (workTime[message.Chat.Id].Year == DateTime.Now.Year)
					{
						var keyb = new InlineKeyboardButton[14 - DateTime.Now.Month][];
						Console.WriteLine (DateTime.Now.Month);

						for (int i = 12; i > 12 - (13 - DateTime.Now.Month) ; i--) 
						{
							//Console.WriteLine (month [i]);
							Console.WriteLine (i - (DateTime.Now.Month - 1));
							keyb [i - (DateTime.Now.Month - 1)] = new[]
							{
								new InlineKeyboardButton (month [i - 1])
								{
									CallbackData = (i - 1).ToString()
								}
							};
						}

						if (workTime[message.Chat.Id].Year == DateTime.Now.Year )
						{
							keyb [0] = new[]
							{
								new InlineKeyboardButton ((DateTime.Now.Year + 1).ToString() + " Year >")
								{
									CallbackData = (DateTime.Now.Year + 1).ToString() 
								}
							};
						}
						else 
						{
							keyb [0] = new[]
							{
								new InlineKeyboardButton ((workTime[message.Chat.Id].Year + 1).ToString() + " Year >")
								{
									CallbackData = (workTime[message.Chat.Id].Year + 1).ToString() 
								},
								new InlineKeyboardButton ("< Year " + (workTime[message.Chat.Id].Year - 1).ToString())
								{
									CallbackData = (workTime[message.Chat.Id].Year + 1).ToString() 
								}
							};
						}

						var keyboard1 = new InlineKeyboardMarkup(keyb);
						await Bot.SendTextMessageAsync (message.Chat.Id, workTime[message.Chat.Id].Year.ToString() + " год",replyMarkup: keyboard1);
						Program.state [message.Chat.Id] = "ChooseMonthOrYear";
					}
					else
					{
						var keyb = new InlineKeyboardButton[13][];

						for (int i = 12; i >= 1; i--) 
						{
							
							keyb [i] = new[]
							{
								new InlineKeyboardButton (month [i - 1])
								{
									CallbackData = (i - 1).ToString() 
								}
							};
						}
					
						if (workTime[message.Chat.Id].Year == DateTime.Now.Year )
						{
							keyb [0] = new[]
							{
								new InlineKeyboardButton ((DateTime.Now.Year + 1).ToString() + " Year >")
								{
									CallbackData = (DateTime.Now.Year + 1).ToString() 
								}
							};
						}
						else 
						{
							keyb [0] = new[]
							{
								new InlineKeyboardButton ((workTime[message.Chat.Id].Year + 1).ToString() + " Year >")
								{
									CallbackData = (workTime[message.Chat.Id].Year + 1).ToString() 
								},
								new InlineKeyboardButton ("< Year " + (workTime[message.Chat.Id].Year - 1).ToString())
								{
									CallbackData = (workTime[message.Chat.Id].Year + 1).ToString() 
								}
							};
						}
					var keyboard1 = new InlineKeyboardMarkup(keyb);
						//await Bot.EditMessageTextAsync (message.Chat.Id, message.MessageId, "Месяца inline", replyMarkup: keyboard1);
						await Bot.SendTextMessageAsync (message.Chat.Id,  workTime[message.Chat.Id].ToString("yyyy") + " год",replyMarkup: keyboard1);
						Program.state [message.Chat.Id] = "ChooseMonthOrYear";
					}


					/*var keybb = new InlineKeyboardButton[13 - DateTime.Now.Month][];
					Console.WriteLine (DateTime.Now.Month);

					for (int i = 11; i > 11 - (13 - DateTime.Now.Month); i--) 
					{
						Console.WriteLine (month [i]);
					Console.WriteLine (i - (DateTime.Now.Month - 1));
					keybb [i - (DateTime.Now.Month - 1)] = new[]
						{
							new InlineKeyboardButton (month [i])
							{
								//CallbackData = i.ToString()
							}
						};
					}
					var keyboard = new InlineKeyboardMarkup(keybb);
					await Bot.SendTextMessageAsync (message.Chat.Id, "Список задач",replyMarkup: keyboard);*/
				}
			}
			catch(Exception ex) {
				Console.WriteLine (ex.Message);
			}
		}

		/*private InlineKeyboardButton[] calend()
		{
			var keyb = new InlineKeyboardButton[7];
		}*/

		public static async void BotDateQueryReceived (object sender, CallbackQueryEventArgs callbackQueryEventArgs)
		{
			var message = callbackQueryEventArgs.CallbackQuery.Message;
			//Program.state[message.Chat.Id] = "Day";
			var callback = callbackQueryEventArgs.CallbackQuery;


			if (Program.state [message.Chat.Id] == "YearAndMonth") 
			{
				if( message.Type == MessageType.TextMessage)
				Program.state [message.Chat.Id] = "ChooseMonthOrYear";
			}

			if (Program.state [message.Chat.Id] == "ChooseMonthOrYear")
			{
				string[] month = {
					"Январь - January",
					"Февраль - February",
					"Март - March",
					"Апрель - April",
					"Май - May",
					"Июнь - June",
					"Июль - July",
					"Август - August",
					"Сентябрь - September",
					"Октябрь - October",
					"Ноябрь - November",
					"Декабрь - December"
				};

			
				if (Convert.ToInt32 (callback.Data) <= 312 && Convert.ToInt32 (callback.Data) >= 12) {
					Console.WriteLine ("Gaveha");
					int day = (Convert.ToInt32 (callback.Data) - 2) / 10;
					if (day > workTime [message.Chat.Id].Day) {
						workTime [message.Chat.Id] = workTime [message.Chat.Id].AddDays (day - workTime [message.Chat.Id].Day);
					} else if (day < workTime [message.Chat.Id].Day) {
						workTime [message.Chat.Id] = workTime [message.Chat.Id].AddDays (day - workTime [message.Chat.Id].Day);
					} else {
						workTime [message.Chat.Id] = workTime [message.Chat.Id].AddDays (0);
					}
					//workTime[message.Chat.Id].AddDays(
					var keyb = new KeyboardButton[24][];
					for (int i = 1; i < 25; i++) {
							keyb[i - 1] = new[]
							{
							new KeyboardButton(i.ToString() + ":")
								{ 
									Text = i.ToString()
								}
							};
						}
					var keyboard = new ReplyKeyboardMarkup (keyb);
					var keyboard1 = new InlineKeyboardMarkup (new[] {
						new[] {
							new InlineKeyboardButton ("Назад") {
								CallbackData = (workTime [message.Chat.Id].Month + 1).ToString ("D")
							}
						}
					});
					await Bot.EditMessageTextAsync (message.Chat.Id, message.MessageId, workTime[message.Chat.Id].ToString("dddd d MMMMM yyyy") + " годa", replyMarkup: keyboard1 );
					await Bot.SendTextMessageAsync (message.Chat.Id, " Установите время", replyMarkup: keyboard, replyToMessageId: message.MessageId);
					}
				
				
				if (Convert.ToInt32 (callback.Data) <= 11 && Convert.ToInt32 (callback.Data) >= 0) {


					if (workTime [message.Chat.Id].Month.CompareTo (Convert.ToInt32 (callback.Data) + 1) == 1) {
						workTime [message.Chat.Id] = workTime [message.Chat.Id].AddMonths (Convert.ToInt32 (callback.Data) + 1 - workTime [message.Chat.Id].Month );
					} else if (workTime [message.Chat.Id].Month.CompareTo (Convert.ToInt32 (callback.Data + 1)) == -1) {
						workTime [message.Chat.Id] = workTime [message.Chat.Id].AddMonths ((Convert.ToInt32 (callback.Data) + 1) - workTime [message.Chat.Id].Month);
					} else if (workTime [message.Chat.Id].Month.CompareTo (Convert.ToInt32 (callback.Data + 1)) == 0) {
						workTime [message.Chat.Id] = workTime [message.Chat.Id].AddMonths (0);
					}

					int index = 0;
					int day = DateTime.DaysInMonth (workTime [message.Chat.Id].Year, workTime [message.Chat.Id].Month);
					if (day == 28 && workTime [message.Chat.Id].DayOfWeek.ToString ("D") == "0") {
						index = 6;
					} else if ((day == 30 && workTime [message.Chat.Id].DayOfWeek.ToString ("D") == "7") || (day == 31 && (workTime [message.Chat.Id].DayOfWeek.ToString ("D") == "6" || workTime [message.Chat.Id].DayOfWeek.ToString ("D") == "7"))){
						index = 8;
					} else {
						index = 7;
					}
					var keyb = new InlineKeyboardButton[index][];
					var keyb6 = new InlineKeyboardButton[7];
					var keyb5 = new InlineKeyboardButton[7];
					var keyb4 = new InlineKeyboardButton[7];
					var keyb3 = new InlineKeyboardButton[7];
					var keyb2 = new InlineKeyboardButton[7];
					var keyb1 = new InlineKeyboardButton[7];
					var keyb0 = new InlineKeyboardButton[7];


					string[] weekDay = {"Su" , "Mo", "Tu", "We", "Th", "Fr", "Sa" };
					//workTime [message.Chat.Id].ToLongDateString ();
					for (int i = 0; i < 7; i++) {
						keyb0[i] = new InlineKeyboardButton (weekDay [i])
						{
							CallbackData =  "-1"
						};
					}
					keyb [0] = keyb0;
					var keyboard1 = new InlineKeyboardMarkup(keyb);
					keyb [index - 1] = new[] {
						new InlineKeyboardButton ("Назад")
						{
							CallbackData = workTime[message.Chat.Id].Year.ToString("D")
						}
					};


					int dy = 1;
					DateTime dt = new DateTime (workTime [message.Chat.Id].Year, workTime [message.Chat.Id].Month, dy);

					for (int j = 0; j < 7; j++) {
						dt = new DateTime (workTime [message.Chat.Id].Year, workTime [message.Chat.Id].Month, dy);
						if (Convert.ToInt32(dt.DayOfWeek.ToString ("D")) == j) {
							keyb1 [j] = new InlineKeyboardButton (dy.ToString ())
							{
								CallbackData = dy.ToString() + "2"
							};
							dy++;
						} else {
							keyb1 [j] = new InlineKeyboardButton (" ")
							{
								CallbackData =  "-1"
							};
							//Console.WriteLine ("KAKA " + i + " " + Convert.ToInt32(dt.DayOfWeek.ToString ("D")) + " " + j + " " + dy);
						}
					}
					keyb [1] = keyb1;
					for (int j = 0; j < 7; j++) {
						dt = new DateTime (workTime [message.Chat.Id].Year, workTime [message.Chat.Id].Month, dy);
						if (Convert.ToInt32(dt.DayOfWeek.ToString ("D")) == j) {
							keyb2 [j] = new InlineKeyboardButton (dy.ToString ())
							{
								CallbackData = dy.ToString() + "2"
							};
							dy++;
						} else {
							keyb2 [j] = new InlineKeyboardButton (" ")
							{
								CallbackData =  "-1"
							};
							//Console.WriteLine ("KAKA " + i + " " + Convert.ToInt32(dt.DayOfWeek.ToString ("2")) + " " + j + " " + dy);
						}
					}
					keyb [2] = keyb2;
					for (int j = 0; j < 7; j++) {
						dt = new DateTime (workTime [message.Chat.Id].Year, workTime [message.Chat.Id].Month, dy);
						if (Convert.ToInt32(dt.DayOfWeek.ToString ("D")) == j) {
							keyb3 [j] = new InlineKeyboardButton (dy.ToString ())
							{
								CallbackData = dy.ToString() + "2"
							};
							dy++;
						} else {
							keyb3 [j] = new InlineKeyboardButton (" ")
							{
								CallbackData =  "-1"
							};
							//Console.WriteLine ("KAKA " + i + " " + Convert.ToInt32(dt.DayOfWeek.ToString ("D")) + " " + j + " " + dy);
						}
					}
					keyb [3] = keyb3;
					for (int j = 0; j < 7; j++) {
						dt = new DateTime (workTime [message.Chat.Id].Year, workTime [message.Chat.Id].Month, dy);
						if (Convert.ToInt32(dt.DayOfWeek.ToString ("D")) == j) {
							keyb4 [j] = new InlineKeyboardButton (dy.ToString ())
							{
								CallbackData = dy.ToString() + "2"
							};
							dy++;
						} else {
							keyb4 [j] = new InlineKeyboardButton (" ")
							{
								CallbackData =  "-1"
							};
							//Console.WriteLine ("KAKA " + i + " " + Convert.ToInt32(dt.DayOfWeek.ToString ("D")) + " " + j + " " + dy);
						}
					}
					keyb [4] = keyb4;
					if (!(day == 28 && workTime [message.Chat.Id].DayOfWeek.ToString ("D") == "0")) {
						for (int j = 0; j < 7; j++) {
							if (dy <= DateTime.DaysInMonth (dt.Year, dt.Month)) {
								dt = new DateTime (workTime [message.Chat.Id].Year, workTime [message.Chat.Id].Month, dy);
							}
							if (Convert.ToInt32 (dt.DayOfWeek.ToString ("D")) == j && dy <= DateTime.DaysInMonth (dt.Year, dt.Month)) {
								keyb5 [j] = new InlineKeyboardButton (dy.ToString ()) {
									CallbackData = dy.ToString () + "2"
								};
								dy++;
							} else {
								keyb5 [j] = new InlineKeyboardButton (" ") {
									CallbackData = "-1"
								};
								//Console.WriteLine ("KAKA " + i + " " + Convert.ToInt32(dt.DayOfWeek.ToString ("D")) + " " + j + " " + dy);
							}
						}
						keyb [5] = keyb5;
					}
					if ((day == 30 && workTime [message.Chat.Id].DayOfWeek.ToString ("D") == "7") || (day == 31 && (workTime [message.Chat.Id].DayOfWeek.ToString ("D") == "6" || workTime [message.Chat.Id].DayOfWeek.ToString ("D") == "7"))) {
						for (int j = 0; j < 7; j++) {
							if (dy <= DateTime.DaysInMonth (dt.Year, dt.Month)) {
								dt = new DateTime (workTime [message.Chat.Id].Year, workTime [message.Chat.Id].Month, dy);
							}
							if (Convert.ToInt32 (dt.DayOfWeek.ToString ("D")) == j && dy <= DateTime.DaysInMonth (dt.Year, dt.Month)) {
								keyb6 [j] = new InlineKeyboardButton (dy.ToString ()) {
									CallbackData = dy.ToString () + "2"
								};
								dy++;
							} else {
								keyb6 [j] = new InlineKeyboardButton (" ") {
									CallbackData = "-1"
								};
								//Console.WriteLine ("KAKA " + i + " " + Convert.ToInt32(dt.DayOfWeek.ToString ("D")) + " " + j + " " + dy);
							}
						}
						keyb [6] = keyb6;
					}

					//keyboard1 = new InlineKeyboardMarkup(keyb);
					//await Bot.AnswerCallbackQueryAsync(callbackQueryEventArgs.CallbackQuery.Id, $"Received workTime [message.Chat.Id].ToString('MMMMM')");
					Console.WriteLine( workTime [message.Chat.Id].ToString("MMMMM"));
					Console.WriteLine( workTime [message.Chat.Id].ToString("yyyy"));
					Console.WriteLine(workTime [message.Chat.Id].Month.CompareTo( Convert.ToInt32(callback.Data)));
					//await Bot.SendTextMessageAsync (message.Chat.Id, "Месяца inline",replyMarkup: keyboard1);
					await Bot.EditMessageTextAsync (message.Chat.Id, message.MessageId, workTime[message.Chat.Id].ToString("yyyy") + " год " + workTime[message.Chat.Id].ToString("MMMMM"), replyMarkup: keyboard1 );

				} else if (Convert.ToInt32 (callback.Data) >= 2016) {
					try
					{
						if (workTime [message.Chat.Id].Year < Convert.ToInt32(callback.Data)) {
							workTime [message.Chat.Id] = workTime [message.Chat.Id].AddYears (1);
						} else if( workTime [message.Chat.Id].Year != 2016 ){
							
							workTime [message.Chat.Id] = workTime [message.Chat.Id].AddYears(-1);
						}

						int index = 0;
						if (workTime [message.Chat.Id].Year == DateTime.Now.Year) {
							index = 14 - DateTime.Now.Month;
						} else {
							index = 13;
						}
						var keyb = new InlineKeyboardButton[index][];

						if (workTime [message.Chat.Id].Year == DateTime.Now.Year) {
							for (int i = 12; i > 12 - (13 - DateTime.Now.Month) ; i--) 
							{
								//Console.WriteLine (month [i]);
								Console.WriteLine (i - (DateTime.Now.Month - 1));
								keyb [i - (DateTime.Now.Month - 1)] = new[]
								{
									new InlineKeyboardButton (month [i - 1])
									{
										CallbackData = (i - 1).ToString()
									}
								};
							}

							if (workTime[message.Chat.Id].Year == DateTime.Now.Year )
							{
								keyb [0] = new[]
								{
									new InlineKeyboardButton ((DateTime.Now.Year + 1).ToString() + " Year >")
									{
										CallbackData = (DateTime.Now.Year + 1).ToString() 
									}
								};
							}
							else 
							{
								keyb [0] = new[]
								{
									new InlineKeyboardButton ((workTime[message.Chat.Id].Year + 1).ToString() + " Year >")
									{
										CallbackData = (workTime[message.Chat.Id].Year + 1).ToString() 
									},
									new InlineKeyboardButton ("< Year " + (workTime[message.Chat.Id].Year - 1).ToString())
									{
										CallbackData = (workTime[message.Chat.Id].Year + 1).ToString() 
									}
								};
							}

						} else {
							for (int i = 12; i >= 1; i--) {

								keyb [i] = new[] {
									new InlineKeyboardButton (month [i - 1]) {
										CallbackData = (i - 1).ToString () 
									}
								};
							}
							if (workTime [message.Chat.Id].Year == DateTime.Now.Year) {
								keyb [0] = new[] {
									new InlineKeyboardButton ((DateTime.Now.Year + 1).ToString () + " Year >") {
										CallbackData = (DateTime.Now.Year + 1).ToString () 
									}
								};
							} else {
								keyb [0] = new[] {
									new InlineKeyboardButton ((workTime [message.Chat.Id].Year + 1).ToString () + " Year >") {
										CallbackData = (workTime [message.Chat.Id].Year + 1).ToString () 
									},
									new InlineKeyboardButton ("< Year " + (workTime [message.Chat.Id].Year - 1).ToString ()) {
										CallbackData = (workTime [message.Chat.Id].Year + 1).ToString () 
									}
								};
							}
						}
							
						if (workTime[message.Chat.Id].Year == DateTime.Now.Year )
						{
							keyb [0] = new[]
							{
								new InlineKeyboardButton ((DateTime.Now.Year + 1).ToString() + " Year >")
								{
									CallbackData = (DateTime.Now.Year + 1).ToString() 
								}
							};
						}
						else 
						{
							keyb [0] = new[]
							{
								new InlineKeyboardButton ("< Year " + (workTime[message.Chat.Id].Year - 1).ToString())
								{
									CallbackData = (workTime[message.Chat.Id].Year - 1).ToString() 
								},
								new InlineKeyboardButton ((workTime[message.Chat.Id].Year + 1).ToString() + " Year >")
								{
									CallbackData = (workTime[message.Chat.Id].Year + 1).ToString() 
								}
							};
						}
						var keyboard1 = new InlineKeyboardMarkup(keyb);
						await Bot.AnswerCallbackQueryAsync(callbackQueryEventArgs.CallbackQuery.Id, $"Received {callbackQueryEventArgs.CallbackQuery.Data}");
							//await Bot.SendTextMessageAsync (message.Chat.Id, "Месяца inline",replyMarkup: keyboard1);

						await Bot.EditMessageTextAsync (message.Chat.Id, message.MessageId, workTime[message.Chat.Id].ToString("yyyy") + " год", replyMarkup: keyboard1);

						Console.WriteLine ("kakaha");
					}
					catch(Exception ex) {
						Console.WriteLine (ex.Message);
					}
				}
				await Bot.AnswerCallbackQueryAsync(callbackQueryEventArgs.CallbackQuery.Id, $"Received {callbackQueryEventArgs.CallbackQuery.Data}");
			}
		}



		/// <summary>
		/// Bots the show task.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="messageEventArgs">The ${ParameterType} instance containing the event data.</param>
		public static async void BotShowTask (object sender, MessageEventArgs messageEventArgs)
		{
			try
			{
				var message = messageEventArgs.Message;
				if (message.Type == MessageType.TextMessage) {
					if (message.Text.StartsWith ("/showtask")) {
						if (Program.state [message.Chat.Id] == "Normal") {
							Program.state [message.Chat.Id] = "ShowTask";

						}
					}
				}

				if (Program.state [message.Chat.Id] == "ShowTask")
					{
					Dictionary<int,string> work = WorktDB.ShowTask(message.Chat.Id);
					var keybb = new InlineKeyboardButton[work.Count][];
					int i = 0;
					foreach (var W in work) 
					{
						keybb[i]  = new[]
						{
							new InlineKeyboardButton(W.Value){
								CallbackData = W.Key.ToString()
							}
						};
						i++;
					}
					var keyboard1 = new InlineKeyboardMarkup(keybb);
					await Bot.SendTextMessageAsync (message.Chat.Id, "Список задач",replyMarkup: keyboard1);
				}

			}
			catch (Exception ex) {
				Console.WriteLine("EbalA ShowTask " + ex.Message);
			}
		}

		public static async void BotTaskParamQueryReceived (object sender, CallbackQueryEventArgs callbackQueryEventArgs)
		{
			var message = callbackQueryEventArgs.CallbackQuery.Message;
			var callback = callbackQueryEventArgs.CallbackQuery;
			if (Program.state [message.Chat.Id] == "TaskParam") 
			{
			}
		}

	}
}

