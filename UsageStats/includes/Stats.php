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
* Username count
* Sessions per site
* Saves per sites
* Most popular OS of last x days (unique users)
* Number of plugins known
* Number of saves by language (culture)
*/

// TODO: Posting from AWB debug builds or cron to Wikipedia.
// TODO: Add count of wikis

// Return a web page showing stats:
function htmlstats(){
	global $db;
	
	php?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en" lang="en" dir="ltr">
<head>
	<title>AutoWikiBrowser Usage Stats</title>
	<meta name="generator" content="AWB UsageStats PHP app" />
	<meta name="copyright" content="<?php echo "\xC2\xA9"; ?> 2008 Stephen Kennedy, Sam Reed" />
	<style type="text/css">
		BODY  {
			font-size : 12pt;
			font-family : Arial, Courier, Helvetica;
			color : Black;
		}
		
		.default  {
			font-size : 12pt;
			font-family : Arial, Courier, Helvetica;
			color : Black;
		}
		
		a:link  {
			color : blue;
			text-decoration : none;
		}
		
		A:visited {
			color: purple;
			text-decoration : none;
		}
		
		a:hover  {
			color : #D79C02;
			text-decoration : underline;
		}
	</style>
</head>
<body>
<h2><a href="http://en.wikipedia.org/wiki/WP:AWB">AutoWikiBrowser</a> Usage Stats</h2>
Statistics on AWB usage since 3 March 2008.
<p/>For more information about the AutoWikiBrowser wiki editor, please see our <a href="http://en.wikipedia.org/wiki/WP:AWB">Wikipedia page</a>.
<p/>
<table border="1">
<?php
	
	//Number of sessions, Number of saves,
	$row = $db->no_of_sessions_and_saves();	
	echo <<<EOF
	<tr>
		<th align="left">Number of Sessions</th><td>{$row['nosessions']}</td>
	</tr>
	<tr>
		<th align="left">Total Number of Saves</th><td>{$row['totalsaves']}</td>
	</tr>
EOF;

	// Username count
	$row = $db->username_count();	
	echo <<<EOF
	
	<tr>
		<th align="left">Number of Usernames Known</th><td>{$row['usercount']}</td>
	</tr>
EOF;

	//Unique users count (username/wiki)
	$row = $db->unique_username_count();	
	echo <<<EOF
	
	<tr>
		<th align="left">Number of Unique Users<sup><a href="#1">1</a></sup></th><td>{$row['UniqueUsersCount']}</td>
	</tr>
EOF;
	
	//Number of plugins known
	$row = $db->plugin_count();
	echo <<<EOF
	
	<tr>
		<th align="left">Number of Known Plugins</th><td>{$row['pluginno']}</td>
	</tr>
EOF;

	// Number of log entries
	$row = $db->db_mysql_query_single_row('SELECT COUNT(DISTINCT LogID) as LogIDCount FROM log', 'htmlstats', 'Stats'); // note: we'll only display this on this web page, hence doing it here
	echo <<<EOF
	
	<tr>
		<th align="left">Number of Log Entries</th><td>{$row['LogIDCount']}</td>
	</tr>
EOF;

	// Record count
	$row = $db->record_count();
	echo <<< EOF

	<tr>
		<th align="left">Total Number of Records in Database</th><td>{$row['RecordCount']}</td>
	</tr>
EOF;

	//Sessions & Saves per sites
	echo <<< EOF

</table>
<p/>
<table border="1">
  <tr>
  	<th colspan="3" align="center">Sessions &amp; saves per site</th>
  </tr>
  <tr>
    <th>Site</th>
    <th>Sessions</th>
	<th>No of Saves</th>
  </tr>
EOF;

	$result = $db->sites();
	
	while($row = $result->fetch_assoc())
	{
		$site=BuildWikiHostname($row['LangCode'], $row['Site']);		
		echo <<<EOF

	<tr>
		<td><a href="http://{$site}/">{$site}</a></td>
		<td>{$row['CountOfSessionID']}</td>
		<td>{$row['SumOfSaves']}</td>
	</tr>
EOF;
	}
		  
	$result->close();
			
	//OS Stats
	OS_XHTML($db->OSs(), '');
	OS_XHTML($db->OSs(true), ' (last 30 days)');
	
	//Number of saves by language (culture)
	echo <<< EOF

  <tr>
  	<th colspan="3" align="center">UI Cultures</th>
  </tr>
	<tr>
		<th>Language</th>
		<th>Country</th>
		<th>Number of Saves</th>
	</tr>
EOF;
	$result = $db->cultures();
	
	while($row = $result->fetch_assoc()) {
		  echo <<< EOF

	<tr>
		<td>{$row['Language']}</td>
		<td>{$row['Country']}</td>
		<td>{$row['SumOfSaves']}</td>
	</tr>
EOF;
	}
	
	$result->close();
	
	//User with the most saves
	$row = $db->busiest_user();
	echo <<< EOF

	<tr>
		<th colspan="3" align="center">User with the most saves<sup><a href="#2">2</a></sup></th>
	</tr>
	<tr>
		<th>Site</th>
		<th>LangCode</th>
		<th>Number of Saves</th>
	</tr>
	<tr>
		<td>{$row['Site']}</td>
		<td>{$row['LangCode']}</td>
		<td>{$row['SumOfSaves']}</td>
	</tr>
EOF;

	// List of plugins
	echo <<< EOF

	<tr>
		<th colspan="3" align="center">Known Plugins</th>
	</tr>
EOF;

	$result = $db->plugins();
	
	while ($row = $result->fetch_assoc()) {
		echo <<< EOF

	<tr>
		<td colspan="3" align="left">{$row['Plugin']}</td>
	</tr>
EOF;
	}
	
	$result->close();
?>

</table>
<p/>
<small>
<sup><a name="1">1</a></sup>Unique username/wiki/language code<br/>
<sup><a name="2">2</a></sup>Anonymous (usernames are not revealed)
</small>
<br/>
<hr/>
<p>
<a href="http://validator.w3.org/check?uri=referer"><img
    src="http://www.w3.org/Icons/valid-xhtml10"
    alt="Valid XHTML 1.0 Transitional" height="31" width="88" /></a>
<a href="http://www.php.net/"><img src="/res/php5-power-micro.png" alt="Powered by PHP 5" height="15" width="80" /></a>
</p>
</body>
</html>
<?php
}

// Helper routines:
function OS_XHTML($result, $headersuffix) {
	echo <<< EOF

  <tr>
  	<th colspan="3" align="center">Operating Systems{$headersuffix}</th>
  </tr>
	<tr>
		<th>OS</th>
		<th>Number of Saves</th>
		<th>Number of Sessions</th>
	</tr>
EOF;

	while($row = $result->fetch_assoc())
	{
		echo <<< EOF

	<tr>
		<td>{$row['OS']}</td>
		<td>{$row['SumOfSaves']}</td>
		<td>{$row['CountOfSessionID']}</td>
	</tr>
EOF;
	}
	
	$result->close();
}

function BuildWikiHostname($lang, $site) {
	switch ($lang)
	{
		case 'WIK':
			return $site;
		case 'CUS':
			switch ($site)
			{
				case 'meta':
				case 'species':
				case 'commons':
					$site .= ".wikimedia.org";
			}
			return $site;
		default:
			return "{$lang}.{$site}.org";
	}	
}
?>