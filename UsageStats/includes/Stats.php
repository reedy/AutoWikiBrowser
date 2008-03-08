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

// TODO: Posting from AWB debug builds or cron to Wikipedia?

// Return a web page showing stats:
function htmlstats(){
	global $db;
	
	php?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en" lang="en" dir="ltr">
<head>
	<title>AutoWikiBrowser Usage Stats</title>
	<meta name="generator" content="AWB UsageStats PHP app v<?php echo MAJOR.'.'.MINOR; ?>" />
	<meta name="copyright" content="<?php echo "\xC2\xA9"; ?> 2008 Stephen Kennedy, Sam Reed" />
	<style type="text/css">
		BODY, .default  {
			font-size : 12pt;
			font-family : Arial, Courier, Helvetica;
			color : Black;
		}
		
		a:link  {
			color : blue;
			text-decoration : none;
		}
		
		A:visited {
			color : purple;
			text-decoration : none;
		}
		
		a:hover  {
			color : #D79C02;
			text-decoration : underline;
		}
		
		table, caption {
			width : 700px;
		}
		
		table, caption, th, td {
			border-width : 1px;
			border-style : solid;		
		}
		
		caption {
			border-bottom-width: 0px;
			font-weight: bold;
		}
	</style>
	<script src="/res/sorttable.js" type="text/javascript"></script>
</head>
<body>
<h2><a href="http://en.wikipedia.org/wiki/WP:AWB">AutoWikiBrowser</a> Usage Stats</h2>
<p>Statistics on AWB usage since 3 March 2008.</p>
<p>For more information about the AutoWikiBrowser wiki editor, please see our <a href="http://en.wikipedia.org/wiki/WP:AWB">Wikipedia page</a>.</p>
<table>
	<caption>Overview</caption>
<?php
	
	//Number of sessions, Number of saves,
	$row = $db->no_of_sessions_and_saves();
	PrintTableRow('Number of Sessions', $row['nosessions']);
	PrintTableRow('Total Number of Saves', $row['totalsaves']);

	// Number of wikis
	$row = $db->wiki_count();
	PrintTableRow('Number of Wiki Sites', $row['Wikis']);

	// Username count
	$row = $db->username_count();
	PrintTableRow('Number of Usernames Known', $row['usercount']);

	//Unique users count (username/wiki)
	$row = $db->unique_username_count();
	PrintTableRow('Number of Unique Users<sup><a href="#1">1</a></sup>', $row['UniqueUsersCount']);
	
	//Number of plugins known
	$row = $db->plugin_count();
	PrintTableRow('Number of Plugins Known', $row['Plugins']);

	// Number of log entries
	$row = $db->db_mysql_query_single_row('SELECT COUNT(DISTINCT LogID) as LogIDCount FROM log', 'htmlstats', 'Stats'); // note: we'll only display this on this web page, hence doing it here
	PrintTableRow('Number of Log Entries', $row['LogIDCount']);

	// Record count
	$row = $db->record_count();
	PrintTableRow('Total Number of Records in Database', $row['RecordCount']);

	//Sessions & Saves per sites
	echo <<< EOF

</table>
<p/>
<table class="sortable">
	<caption><a name="sites"></a>Sessions &amp; saves per site<sup><a href="#2">2</a></sup></caption>
<thead>
	<tr>
		<th scope="col">Site</th>
		<th scope="col">Number of Sessions</th>
		<th scope="col">Number of Saves</th>
	</tr>
</thead>
EOF;

	$result = $db->sites();
	
	while($row = $result->fetch_assoc())
	{
		$site=BuildWikiHostname($row['LangCode'], $row['Site']);		
		echo <<<EOF

	<tr>
		<td>{$site}</td>
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

</table>
<p/>
<table class="sortable">
	<caption>UI Cultures</caption>
<thead>
	<tr>
		<th scope="col">Language</th>
		<th scope="col">Country</th>
		<th scope="col">Number of Saves</th>
	</tr>
</thead>
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
	$site=BuildWikiHostname($row['LangCode'], $row['Site']);
	echo <<< EOF

</table>
<p/>
<table>
	<caption>User with the most saves<sup><a href="#3">3</a></sup></caption>
<thead>
	<tr>
		<th scope="col">Site</th>
		<th scope="col">LangCode</th>
		<th scope="col">Number of Saves</th>
	</tr>
</thead>
	<tr>
		<td>{$site}</td>
		<td>{$row['LangCode']}</td>
		<td>{$row['SumOfSaves']}</td>
	</tr>
EOF;

	// List of plugins
	echo <<< EOF

</table>
<p/>
<table class="sortable">
	<caption>Known Plugins</caption>
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
<table>
	<caption><a name="Diagnostics"></a>Diagnostics</caption>
	<tr>
		<th align="left" scope="row">Script version</th><td><?php echo MAJOR.'.'.MINOR; ?></td>
	</tr>	
<?php

	// Script errors
	$row = $db->errors();
	PrintTableRow('Submission errors', $row['Errors']);
	$row = $db->errors_fixed();
	PrintTableRow('Submission errors (not fixed)', $row['Errors']);
	
	// Username not recorded
	$row = $db->missing_usernames_count();
	PrintTableRow('Number of sessions where username not recorded', $row['MissingUsernames']);
	echo '<!-- kingboyk: Not sure what\'s happening here, seems to be RU wiki only possibly just one user... do we still have a bug in username gathering or did he perhaps alter the AWB source code? Hmm... -->';
?>

</table>
<p>Note: These statistics are designed to help AWB developers better understand how the program is being used and to prioritise
development tasks. They are <i>indicative only</i> - for example, we don't currently take any account of a user changing
their username or switching to a different wiki mid-session.</p>
<div><small>
<sup><a name="1">1</a></sup>Unique username/wiki/language code<br/>
<sup><a name="2">2</a></sup>Only sites at which AWB has logged 50 or more saves are shown<br/>
<sup><a name="3">3</a></sup>Anonymous (usernames are not revealed)
</small>
<br/></div>
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

</table>
<p/>
<table class="sortable">
<caption>Operating Systems{$headersuffix}</caption>
<thead>
	<tr>
		<th scope="col">OS</th>
		<th scope="col">Number of Sessions</th>
		<th scope="col">Number of Saves</th>
	</tr>
</thead>
EOF;

	while($row = $result->fetch_assoc())
	{
		echo <<< EOF

	<tr>
		<td>{$row['OS']}</td>
		<td>{$row['CountOfSessionID']}</td>
		<td>{$row['SumOfSaves']}</td>
	</tr>
EOF;
	}
	
	$result->close();
}

function BuildWikiHostname($lang, $site) {
	switch ($lang)
	{
		case 'WIK': // Wikia
			break; // should be correct already, just needs A HREF
		case 'CUS': // Custom site, meta, species, or commons
			switch ($site)
			{
				case 'meta':
				case 'species':
				case 'commons':
					$site .= '.wikimedia.org';
				default:
					if (strpos($site, '.') === false) // use of === false per PHP docs
						return "&lt;intranet site&gt;<!--{$site}-->";
					// else already correct domain name
			}
			break;
		case 'sim': // Simple English Wikipedia: "simple" truncated
			return BuildWikiHostname('simple', $site);
		default: // Other Wikimedia sites
			$site = "{$lang}.{$site}.org";
	}
	return "<a href=\"http://{$site}/\">{$site}</a>";
}

function PrintTableRow($header, $data) {
	echo <<<EOF
	
	<tr>
		<th align="left" scope="row">{$header}</th><td>{$data}</td>
	</tr>
EOF;
}
?>