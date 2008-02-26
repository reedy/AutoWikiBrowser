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
	
	//Number of sessions, Number of saves
	$query = "SELECT COUNT(sessionid) AS nosessions, SUM(saves) AS totalsaves FROM sessions";
	$retval = $db->db_mysql_query($query, 'stats', 'STATS') ;

	while($row = mysqli_fetch_array($retval, MYSQL_ASSOC))
	{
		echo "No of Sessions: {$row['nosessions']}<br />";
		echo "No of Saves: {$row['totalsaves']}<br />";
	}
	
	//Unique users count (username/wiki)
	/*$query = "";
	$retval = $db->db_mysql_query($query, 'stats', 'STATS') ;

	while($row = mysqli_fetch_array($retval, MYSQL_ASSOC))
	{
		echo "No of : {$row['']}<br />";
	}*/
		
	//Unique username count
	$query = "SELECT COUNT(DISTINCT User) AS usercount FROM lkpUsers";
	$retval = $db->db_mysql_query($query, 'stats', 'STATS') ;

	while($row = mysqli_fetch_array($retval, MYSQL_ASSOC))
	{
		echo "No of Unique Users: {$row['usercount']}<br />";
	}
	
	//Sessions per sites
	/*$query = "";
	$retval = $db->db_mysql_query($query, 'stats', 'STATS') ;

	while($row = mysqli_fetch_array($retval, MYSQL_ASSOC))
	{
		echo "No of : {$row['']}<br />"
	}*/
	
	//Saves per sites
	/*$query = "";
	$retval = $db->db_mysql_query($query, 'stats', 'STATS') ;

	while($row = mysqli_fetch_array($retval, MYSQL_ASSOC))
	{
		echo "No of : {$row['']}<br />"
	}*/
	
	//Most popular OS of last x days (unique users)
	/*$query = "";
	$retval = $db->db_mysql_query($query, 'stats', 'STATS') ;

	while($row = mysqli_fetch_array($retval, MYSQL_ASSOC))
	{
		echo "No of : {$row['']}<br />"
	}*/
	
	//Number of plugins known
	/*$query = "";
	$retval = $db->db_mysql_query($query, 'stats', 'STATS') ;

	while($row = mysqli_fetch_array($retval, MYSQL_ASSOC))
	{
		echo "No of : {$row['']}<br />"
	}*/
	
	//Number of saves by language (culture)
	/*$query = "";
	$retval = $db->db_mysql_query($query, 'stats', 'STATS') ;

	while($row = mysqli_fetch_array($retval, MYSQL_ASSOC))
	{
		echo "No of : {$row['']}<br />"
	}*/
}
?>