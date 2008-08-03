using System;
using System.Collections.Generic;
using System.Text;

using WikiFunctions;
using MySql.Data.MySqlClient;

namespace TypoScanDBInsert
{
    class Program
    {
        static readonly string insertString = "INSERT INTO typoscan(title) VALUES";
        static readonly string connStr = "server=mysql.reedyboy.net;user id=;password=;port=3306;database=typoscan";

        static System.Diagnostics.Stopwatch watch;
        static void Main(string[] args)
        {
            WikiFunctions.Globals.UnitTestMode = true; //Hack to save faffing with setting up WF for use

            MySqlConnection conn = null;
            MySqlCommand command;
            try
            {
                watch = System.Diagnostics.Stopwatch.StartNew();

                StringBuilder builder = new StringBuilder(insertString);
                int count = 0;
                int totalArticles = 0;

                List<WikiFunctions.Article> articles = new WikiFunctions.Lists.TextFileListProvider().MakeList("typos.txt");

                conn = new MySqlConnection();
                conn.ConnectionString = connStr;
                conn.Open();

                command = new MySqlCommand();
                command.Connection = conn;
                command.CommandText = @"DROP TABLE IF EXISTS `typoscan`.`articles`;
CREATE TABLE  `typoscan`.`articles` (
  `articleid` int(10) unsigned NOT NULL auto_increment,
  `title` blob NOT NULL,
  `checkedout` datetime NOT NULL default '0000-00-00 00:00:00',
  `finished` tinyint(1) NOT NULL default '0',
  `skipid` int(10) NOT NULL default '0',
  `user` varchar(50) default NULL,
   `checkedin` datetime NOT NULL default '0000-00-00 00:00:00',
  PRIMARY KEY  (`articleid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `typoscan`.`skippedreason`;
CREATE TABLE `typoscan`.`skippedreason` (
  `skipid` int(10) unsigned NOT NULL auto_increment,
  `skipreason` varchar(50) default NULL,
  PRIMARY KEY  (`skipid`)
)ENGINE=InnoDB DEFAULT CHARSET=utf8;

INSERT INTO `typoscan`.`skippedreason`(`skipreason`) VALUES ('Clicked ignore'), ('No change'), ('Non-existent page'), ('No typo fixes');"; //probably should be read from file
                command.ExecuteNonQuery();

                foreach (WikiFunctions.Article a in articles)
                {
                    count++;
                    totalArticles++;

                    builder.Append("('" + a.Name.Replace("'", "''").Replace("‘", "‘‘").Replace("’", "’’") + "')");

                    if (count == 10 || totalArticles == articles.Count)
                    {
                        count = 0;

                        command = new MySqlCommand();
                        command.Connection = conn;
                        command.CommandText = builder.ToString();
                        command.ExecuteNonQuery();

                        builder = new StringBuilder(insertString);
                        Console.WriteLine(totalArticles);
                    }
                    else
                        builder.Append(", ");
                }

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
    }
}
