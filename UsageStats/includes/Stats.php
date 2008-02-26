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
	
	//Number of sessions, Number of saves, Unique username count
	$query = "SELECT COUNT(s.sessionid) AS nosessions, SUM(s.saves) AS totalsaves FROM sessions s";
	$retval = $db->db_mysql_query($query, 'stats', 'STATS') ;

	while($row = mysqli_fetch_array($retval, MYSQL_ASSOC))
	{
		echo "No of Sessions: {$row['nosessions']}<br />";
		echo "No of Saves: {$row['totalsaves']}<br />";
	}
	
	$query = "SELECT COUNT(DISTINCT u.User) AS usercount FROM lkpUsers u";
	$retval = $db->db_mysql_query($query, 'stats', 'STATS') ;

	while($row = mysqli_fetch_array($retval, MYSQL_ASSOC))
	{
		echo "No of Unique Users: {$row['usercount']}<br />";
	}
	
	//Unique users count (username/wiki)
	/*$query = "";
	$retval = $db->db_mysql_query($query, 'stats', 'STATS') ;

	while($row = mysqli_fetch_array($retval, MYSQL_ASSOC))
	{
		echo "No of : {$row['']}<br />";
	}*/
		
	//Sessions & Saves per sites
	$query = "SELECT COUNT(SessionID) as sessions, l.langcode, l.site, SUM(s.saves) as nosaves FROM sessions s, lkpWikis l WHERE (s.site = l.siteid) GROUP BY s.site";
	$retval = $db->db_mysql_query($query, 'stats', 'STATS') ;

		echo "<table width='25%' border=1>
  <tr>
    <td>Site</td>
    <td>Sessions</td>
	<td>No of Saves</td>
  </tr>";
	
	while($row = mysqli_fetch_array($retval, MYSQL_ASSOC))
	{
		$lang = "{$row['langcode']}";
		$site = "";
		
		if ($lang != "WIKI" && $lang != "CUS")
		{
			$site = $lang.".{$row['site']}";
		}
		else
		{
		$site = "{$row['site']}";
		}
		
		  echo "<tr>
	    <td>$site</td>
	    <td>{$row['sessions']}</td>.
		<td>{$row['nosaves']}</td>
	  </tr>";
	}
	
	echo "</table>";
	
	//Most popular OS of last x days (unique users)
	/*$query = "";
	$retval = $db->db_mysql_query($query, 'stats', 'STATS') ;

	while($row = mysqli_fetch_array($retval, MYSQL_ASSOC))
	{
		echo "No of : {$row['']}<br />";
	}*/
	
	//Number of plugins known
	$query = "SELECT COUNT(DISTINCT PluginID) as pluginno FROM plugins";
	$retval = $db->db_mysql_query($query, 'stats', 'STATS') ;

	while($row = mysqli_fetch_array($retval, MYSQL_ASSOC))
	{
		echo "No of known Plugins: {$row['pluginno']}<br />";
	}
	
	//Number of saves by language (culture)
	/*$query = "";
	$retval = $db->db_mysql_query($query, 'stats', 'STATS') ;

	while($row = mysqli_fetch_array($retval, MYSQL_ASSOC))
	{
		echo "No of : {$row['']}<br />";
	}*/
}
?>