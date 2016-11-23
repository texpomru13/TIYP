using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace TIYP
{
	public class Work
	{
		protected string connParams = "Server=localhost; Database=TIYP; User ID=root; Password=root; Pooling=false; CharSet=utf8;";

		protected IDbConnection conn;

		protected IDbCommand cmd; // Выполнение команд

		protected IDataReader reader;

		public int Id 
		{ 
			get 
			{
				return 0;
			}
		}

		public int ClientId { get; set;}

		public string Name { get; set; }

	}
}

