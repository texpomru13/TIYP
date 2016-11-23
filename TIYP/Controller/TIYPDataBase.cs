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

			public Dictionary<long, string> Client()
			{
				Dictionary <long, string> statiment = new Dictionary <long, string>();
				try 
				{
					conn = new MySqlConnection (connParams);

					conn.Open ();
					string param =  "select id_telebot from Clients;"; //"+ table +"

					cmd = conn.CreateCommand (); // Выполнение команд

					cmd.CommandText = param;

					reader = cmd.ExecuteReader ();

					while(reader.Read ())
					{
						long id_telebot = Convert.ToInt32(reader["id_telebot"].ToString());
						statiment.Add(id_telebot, "Normal");
					}

				}
				catch (MySqlException ex) 
				{
					Console.WriteLine ("Error: " + ex.Message);
				} 
				finally 
				{
					conn.Close ();
				}

				return statiment;
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
			

			public void AddClient(long id_telebot ,string first_name, string last_name)
			{

				try {
					conn = new MySqlConnection (connParams);
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
				try
				{
					conn = new MySqlConnection (connParams);

					conn.Open ();

					cmd = conn.CreateCommand (); // Выполнение команд

					string param = "UPDATE Clients SET e_mail='" + mail + "' WHERE id_telebot = " + id_telebot + ";";

					Runn (param);
				}
				catch (MySqlException ex) 
				{
					Console.WriteLine ("Error: " + ex.Message);
				} 
			}

			public void AddPhone(long id_telebot, string phoneNumber)
			{
				try
				{
					conn = new MySqlConnection (connParams);


					conn.Open ();

					cmd = conn.CreateCommand (); // Выполнение команд

					string param = "UPDATE Clients SET phone_number='" + phoneNumber + "' WHERE id_telebot = " + id_telebot + ";";

					Runn (param);
				}
				catch (MySqlException ex) 
				{
					Console.WriteLine ("Error: " + ex.Message);
				} 
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
		public class Work : TIYPDataBase
		{
			public void AddTaskName (long id_telebot, string taskname)
			{
				conn = new MySqlConnection (connParams);

				conn.Open ();
				string param =  "select id from Clients WHERE id_telebot = " + id_telebot + ";"; //"+ table +"

				cmd = conn.CreateCommand (); // Выполнение команд

				cmd.CommandText = param;

				reader = cmd.ExecuteReader ();
				int id = 0;
				while(reader.Read ())
				{
					id = Convert.ToInt32( reader["id"]);
				}
				reader.Close ();
				param = 
					"insert into Work (client_id, name)" +
					"values ('"+ id +"','"+ taskname +"');";

				Runn(param);
			}


			public Dictionary<int, string> ShowTask (long id_telebot)
			{
				conn = new MySqlConnection (connParams);

				conn.Open ();
				string param =  "select id from Clients WHERE id_telebot = " + id_telebot + ";"; //"+ table +"

				cmd = conn.CreateCommand (); // Выполнение команд

				cmd.CommandText = param;

				reader = cmd.ExecuteReader ();
				int id = 0;
				while(reader.Read ())
				{
					id = Convert.ToInt32( reader["id"]);
				}
				reader.Close ();
				param = 
					"Select name, id from Work where client_id = " + id + ";";

				cmd.CommandText = param;

				reader = cmd.ExecuteReader ();


				int i = 0;

				Dictionary<int, string> name = new Dictionary<int,string>();
				while(reader.Read ())
				{
					name.Add(Convert.ToInt32(reader["id"]), reader ["name"].ToString ());
					i++;
				}
				return name;


			}
		}

		/// <summary>
		/// Работа с БД в таблице Files_work
		/// </summary>
		public class FilesWork : TIYPDataBase
		{

		}

		/// <summary>
		/// Работа с БД в таблице Child_work
		/// </summary>
		public class ChildWork : TIYPDataBase
		{

		}
	}
}


