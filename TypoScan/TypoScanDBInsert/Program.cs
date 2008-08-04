using System;
using System.Collections.Generic;
using System.Text;

using WikiFunctions;
using MySql.Data.MySqlClient;

namespace TypoScanDBInsert
{
    class Program
    {
        static readonly string insertString = "INSERT INTO articles(title) VALUES";
        static readonly string connStr = "server=mysql.reedyboy.net;user id=;password=;port=3306;database=typoscan;charset=utf8";

        static System.Diagnostics.Stopwatch watch;

        static MySqlConnection conn = null;
        static MySqlCommand command;

        static int count = 0;
        static int totalArticles = 0;

        static List<WikiFunctions.Article> articles;

        static StringBuilder builder;

        static void Main(string[] args)
        {
            try
            {
                watch = System.Diagnostics.Stopwatch.StartNew();
                WikiFunctions.Globals.UnitTestMode = true; //Hack to save faffing with setting up WF for use

                articles = new WikiFunctions.Lists.TextFileListProvider().MakeList("typos.txt");

                conn = new MySqlConnection();
                conn.ConnectionString = connStr;
                conn.Open();

                command = new MySqlCommand();
                command.Connection = conn;

                command.CommandText = "SET NAMES 'utf8'";
                command.ExecuteNonQuery();

                //Import();
                ReImport();

                Console.WriteLine("Time elapsed (s): " + (watch.ElapsedMilliseconds / 1000));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (conn != null)
                    conn.Close();

                Console.ReadLine();
            }

        }

        static void Import(bool reimport)
        {
            builder = new StringBuilder(insertString);

            using (System.IO.StreamReader sr = new System.IO.StreamReader("database.sql", Encoding.UTF8))
            {
                command.CommandText = sr.ReadToEnd();
                sr.Close();
            }

            command.ExecuteNonQuery();

            foreach (WikiFunctions.Article a in articles)
            {
                count++;
                totalArticles++;

                builder.Append("('" + a.Name.Replace("'", "''").Replace("‘", "‘‘").Replace("’", "’’") + "')");

                if (count == 10 || totalArticles == articles.Count)
                {
                    count = 0;

                    command.CommandText = builder.ToString();
                    command.ExecuteNonQuery();

                    builder = new StringBuilder(insertString);
                    Console.WriteLine(totalArticles);
                }
                else
                    builder.Append(", ");
            }
        }

        static void ReImport()
        {
            builder = new StringBuilder();

            for (int i = 0; i < articles.Count; i++)
            {
                count++;
                totalArticles++;

                builder.AppendLine("UPDATE articles SET title = '" + articles[i].Name.Replace("'", "''").Replace("‘", "‘‘").Replace("’", "’’") + "' WHERE (articleid = '" + (i + 1) + "');");

                if (count == 10 || totalArticles == articles.Count)
                {
                    count = 0;

                    command.CommandText = builder.ToString();
                    command.ExecuteNonQuery();

                    builder = new StringBuilder();
                    Console.WriteLine(totalArticles);
                }

                command.ExecuteNonQuery();
            }
            Console.WriteLine("Done");
        }
    }
}
