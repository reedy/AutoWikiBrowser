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
// TODO: Let's get this into a single table or use CSS?

function htmlstats(){
	global $db;
	
	php?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en" lang="en" dir="ltr">
<head>
	<title>AutoWikiBrowser Usage Stats</title>
	<meta name="generator" content="AWB UsageStats PHP app" />
	<meta name="copyright" content="<?php echo "\xC2\xA9"; ?> 2008 Stephen Kennedy, Sam Reed" />
</head>
<body>
<h2><a href="http://en.wikipedia.org/wiki/WP:AWB">AutoWikiBrowser</a></h2>
<?php
	
	//Number of sessions, Number of saves,
	$return = $db->no_of_sessions_and_saves();
	echo "No of Sessions: {$return['nosessions']}<br />";
	echo "No of Saves: {$return['totalsaves']}<br />";

	//Unique username count
	$return = $db->unique_username_count();
	echo "No of Unique Users: {$return['usercount']}<br />";
	
	//Unique users count (username/wiki)
	$row = $db->busiest_user();
	echo <<< EOF
<table width='25%' border='1'>
  <tr>
  	<th colspan="3" align="center">User with the most saves</th>
  </tr>
  <tr>
    <th>Site</th>
    <th>LangCode</th>
	<th>No of Saves</th>
  </tr>
  <tr>
  	<td>{$row['Site']}</td>
  	<td>{$row['LangCode']}</td>
  	<td>{$row['SumOfSaves']}</td>
  </tr>
</table>
EOF
		
	//Sessions & Saves per sites
?>
<table width='25%' border='1'>
  <tr>
    <td>Site</td>
    <td>Sessions</td>
	<td>No of Saves</td>
  </tr>
<?php

	$retval = $db->db_mysql_query("SELECT COUNT(SessionID) as sessions, l.langcode, l.site, SUM(s.saves) as nosaves FROM sessions s, lkpWikis l WHERE (s.site = l.siteid) GROUP BY s.site", 'stats', 'STATS');

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
	$retval = $db->db_mysql_query("SELECT o.OS, COUNT(s.os) AS nousers FROM sessions s, lkpOS o WHERE (s.os = o.osid) GROUP BY s.os;", 'stats', 'STATS');
	
			echo "<table width='25%' border='1'>
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
	$return = $db->plugin_count();
	echo "No of known Plugins: {$return['pluginno']}<br />";
	
	//Number of saves by language (culture)
	$retval = $db->db_mysql_query("SELECT language, country, COUNT(culture) AS nocultures FROM sessions s, lkpCultures c WHERE (s.culture = c.CultureID) GROUP BY s.culture", 'stats', 'STATS');

			echo "<table width='25%' border='1'>
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