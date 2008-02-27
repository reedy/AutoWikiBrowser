<?php
/*
(C) 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
(C) 2008 Sam Reed

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

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

// TODO: Posting from AWB debug builds or cron to Wikipedia.

/* TODO: this is a fucking mess! remember encapsulation: put all the queries into functions in the mysql object, so we
can more easily modify them, so that wiki/XML can reuse them, and to make this more of an output-only routine */

function htmlstats(){
	global $db;
	
	php?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en" lang="en" dir="ltr">
<head>
	<title>AutoWikiBrowser Usage Stats</title>
	<meta name="generator" content="AWB UsageStats PHP app" />
	<meta name="copyright" content="<?php utf8_encode('©'); // TODO: How the fuck do you get a (c) symbol in UTF8?! ?> 2008 Stephen Kennedy, Sam Reed" />
</head>
<body>
<h2><a href="http://en.wikipedia.org/wiki/WP:AWB">AutoWikiBrowser</a></h2>
<?php
	
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
	    <td>{$row['sessions']}</td>
		<td>{$row['nosaves']}</td>
	  </tr>";
	}
	
	echo "</table>";
	
	//OS Stats
	$query = "SELECT o.OS, COUNT(s.os) AS nousers FROM sessions s, lkpOS o WHERE (s.os = o.osid) GROUP BY s.os;";
	$retval = $db->db_mysql_query($query, 'stats', 'STATS') ;
	
			echo "<table width='25%' border=1>
  <tr>
    <td>OS</td>
    <td>Number of Users</td>
  </tr>";

	while($row = mysqli_fetch_array($retval, MYSQL_ASSOC))
	{
	    echo "<td>{$row['OS']}</td>
		<td>{$row['nousers']}</td>
	  </tr>";
	}
	
	echo "</table>";
	
	//Number of plugins known
	$query = "SELECT COUNT(DISTINCT PluginID) as pluginno FROM plugins";
	$retval = $db->db_mysql_query($query, 'stats', 'STATS') ;

	while($row = mysqli_fetch_array($retval, MYSQL_ASSOC))
	{
		echo "No of known Plugins: {$row['pluginno']}<br />";
	}
	
	//Number of saves by language (culture)
	$query = "SELECT language, country, COUNT(culture) AS nocultures FROM sessions s, lkpCultures c WHERE (s.culture = c.CultureID) GROUP BY s.culture";
	$retval = $db->db_mysql_query($query, 'stats', 'STATS') ;

			echo "<table width='25%' border=1>
  <tr>
    <td>Country</td>
    <td>Language</td>
	<td>Number</td>
  </tr>";
	
	while($row = mysqli_fetch_array($retval, MYSQL_ASSOC))
	{
		  echo "<tr>
	    <td>{$row['country']}</td>
	    <td>{$row['language']}</td>
		<td>{$row['nocultures']}</td>
	  </tr>";
	}
	
?>
</table>
</body>
</html>
<?php
}
?>