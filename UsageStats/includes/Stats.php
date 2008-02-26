<?php
/*
Some queries we might want on a stats page:
* Number of sessions
* Number of saves
* Unique users count (username/wiki)
* Unique username count
* Sessions per site
* Saves per sites
* Most popular OS of last x days (unique users)
* Number of plugins known
* Number of saves by language (culture)
*/

// TODO: Queries. Web-viewable stats when no data is POSTed. Posting from AWB debug builds or cron to Wikipedia.
	require_once("MySQL.php");

function stats(){
	$db=new DB();
	$db->db_connect();
		
	$retval = $db->db_mysql_query("SELECT COUNT(sessionid) AS nosessions, SUM(saves) AS totalsaves FROM sessions", 'stats', 'STATS') ;

	while($row = mysqli_fetch_array($retval, MYSQL_ASSOC))
	{
		echo "No of Sessions: {$row['nosessions']}<br />";

		echo "No of Saves: {$row['totalsaves']}<br />";
	} 
}

?>