using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

#if NETSTANDARD1_1
using System.Reflection;
#endif

using Newtonsoft.Json;

using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot
{
	public class DB
	{
		
		public class TIYPDataBase
		{
			protected string connParams = "Server=localhost; Database=TIYP; User ID=root; Password=root; Pooling=false; CharSet=utf8;";

			protected IDbConnection conn;

			protected IDbCommand cmd; // Выполнение команд

			protected IDataReader reader;

			public virtual void SelectTable (string table){}

			public void Runn (string param)
			{
				cmd.CommandText = param;

				reader = cmd.ExecuteReader ();

				reader.Read ();

				reader.Close();
			}


			public void AddUser()
			{
				//вывод значений таблицы в консоль
				/*while (reader.Read())
				{
					Console.WriteLine("Person No " + reader["id"].ToString() +
						", Name: " + reader["id_telebot"] +
						", Birthday: " + reader["e-mail"].ToString() +
						", Address: " + reader["berth_date"] +
						", Passport No: " + reader["first_name"]);
				}*/
				
			}

		}

		/// <summary>
		/// Работа с БД в таблице Clients
		/// </summary>
		public class Clients : TIYPDataBase
		{
			public override void SelectTable(string table)
			{
				conn = new MySqlConnection (connParams);

				try 
				{


					conn.Open ();
					string param =  "select * from Clients WHERE id_telebot = 213628639;"; //"+ table +"

					cmd = conn.CreateCommand (); // Выполнение команд

					cmd.CommandText = param;

					reader = cmd.ExecuteReader ();

					Console.WriteLine( reader.Read ());

				}
			 	catch (MySqlException ex) 
				{
					Console.WriteLine ("Error: " + ex.Message);
				} 
				finally 
				{
					conn.Close ();
				}
				
			}

			public void AddClient(long id_telebot ,string first_name, string last_name)
			{
				conn = new MySqlConnection (connParams);

				try {
					conn.Open ();
					cmd = conn.CreateCommand (); // Выполнение команд
					string param =  "select * from Clients WHERE id_telebot = " + id_telebot +";"; 
					cmd.CommandText = param;
					reader = cmd.ExecuteReader ();

					if(reader.Read() == false)
					{
						reader.Close();
						//cmd.CommandText = "select * from `Clients`;";
						param = "insert into Clients (id_telebot, first_name, last_name, reg_date)" +
							"values ('"+ id_telebot +"','"+ first_name +"', '" + last_name + "', NOW());";

						Runn(param);

						//Console.WriteLine(cmd.CommandText = "select * from `Person`;");
					}
				} catch (MySqlException ex) {
					Console.WriteLine ("Error: " + ex.Message);
				} finally 
				{
					conn.Close ();
				}
			}

			public void AddMail(long id_telebot, string mail)
			{
				conn.Open ();

				cmd = conn.CreateCommand (); // Выполнение команд

				string param = "UPDATE Clients SET e_mail='" + mail + "' WHERE id_telebot = " + id_telebot + ";";

				Runn (param);
			}

			public void AddPhone(long id_telebot, string phoneNumber)
			{
				conn.Open ();

				cmd = conn.CreateCommand (); // Выполнение команд

				string param = "UPDATE Clients SET phoneNumber='" + phoneNumber + "' WHERE id_telebot = " + id_telebot + ";";

				Runn (param);
			}


			public void AddLocation(long id_telebot, float longitude, float latitude)
			{
				try {
					conn = new MySqlConnection (connParams);

					conn.Open ();

					cmd = conn.CreateCommand (); 

					string longi = longitude.ToString();
					string lati = Convert.ToString( latitude);
					lati = lati.Insert(lati.IndexOf(","), ".");
					lati = lati.Replace(",", "");
					longi = longi.Insert(longi.IndexOf(","), ".");
					longi = longi.Replace(",", "");
						
					string param = 
						"UPDATE Clients SET longitude=" + longi + " WHERE id_telebot = " + id_telebot + ";";
					Console.WriteLine(param);
					Runn (param);
					param = "UPDATE Clients SET latitude='" + lati + "' WHERE id_telebot = " + id_telebot + ";";
					Console.WriteLine(param);
					Runn (param);
				}
				catch (MySqlException ex) 
				{
					Console.WriteLine ("Error: " + ex.Message);
				} 
				conn.Close ();
			}


			
		}

		/// <summary>
		/// Работа с БД в таблице Work
		/// </summary>
		public class Work
		{
			
		}

		/// <summary>
		/// Работа с БД в таблице Files_work
		/// </summary>
		public class FilesWork
		{

		}

		/// <summary>
		/// Работа с БД в таблице Child_work
		/// </summary>
		public class ChildWork
		{

		}
	}
}


