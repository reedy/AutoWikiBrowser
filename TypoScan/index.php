<?php
/*
* Copyright (C) 2008 Sam Reed & Max Semenik
*
* This program is free software; you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation; either version 2 of the License, or
* (at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License along
* with this program; if not, write to the Free Software Foundation, Inc.,
* 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
* http://www.gnu.org/copyleft/gpl.html
*/
	
	require_once('common.php');
	
	//prevent caching
	DisableCaching();
	
	/* Manually create file 'typo-db.php' using the following boilerplate:
<?php
	$dbserver = '';
	$dbuser = '';
	$dbpass = '';
	$database = '';
	
	$conn=mysql_connect($dbserver, $dbuser, $dbpass); 
	mysql_select_db($database, $conn);	
	**/
	require_once('typo-db.php');
	
	$query = "SET NAMES 'utf8'";
	$result=mysql_query($query) or die ('Error: ' . htmlspecialchars(mysql_error()) . '\nQuery: ' . $query);
	
	if (!isset($_GET['action'])) $_GET['action'] = '';
	
	switch($_GET['action'])
	{
		case 'finished':
			header("Content-type: text/xml; charset=utf-8");
			PostRequired();
			$articles = @$_POST['articles'];
			$skippedarticles = @$_POST['skipped'];
			$skippedreason = @$_POST['skipreason'];
			$user = @$_POST['user'];
			$wiki = @$_GET['wiki'];
			
			$articlesempty = empty($articles);
			$skippedempty = empty($skippedarticles);
			
			if ((!$articlesempty || !$skippedempty) && $user && $wiki)
			{
				$userid = GetOrAddUser($user);
				
				if ($wiki != 'en.wikipedia.org') ReturnError('This project is not supported', 'project');

				if (!$articlesempty)
				{
					$query = 'UPDATE articles SET finished = 1, checkedin = NOW(), userid = "' . $userid . '" WHERE articleid IN (' . $articles . ')';
					$result=mysql_query($query) or die ('Error: '.mysql_error() . '\nQuery: ' . $query);
				}
				
				if (!$skippedempty)
				{
					//echo 'sa: ' . $skippedarticles;
					//echo 'sr: ' . $skippedreason;
					//Deal with Ignored
					
					$skippedarticles = explode(",", $skippedarticles);
					$skippedreason = explode(",", $skippedreason);
					
					//echo 'sa size: ' . count($skippedarticles);
									
					for ($i=0; $i < count($skippedarticles); $i++)
					{
						$query = 'UPDATE articles SET skipid = "' . GetOrAddIgnoreReason($skippedreason[$i]) . '", checkedin = NOW(), userid = "' . $userid . '" WHERE (articleid = "' . $skippedarticles[$i] . '")';
						//echo $query;
					    $result=mysql_query($query) or die ('Error: '.mysql_error() . '\nQuery: ' . $query);
					}
				}
				echo Xml::XmlHeader() . Xml::element('operation', array('status' => 'success'), 'Articles marked as processed');
			}
			else
			{
				ReturnError('Request misses essential data', 'request');
			}
		break;
		
		case 'displayarticles':
			header("Content-type: text/xml; charset=utf-8"); 
		
			$wiki = @$_GET['wiki'];
			if ($wiki != 'en.wikipedia.org') ReturnError('This project is not supported', 'project');

			$query = 'SELECT articleid, title FROM articles WHERE (checkedout < DATE_SUB(NOW(), INTERVAL 2 HOUR)) AND (userid = 0) LIMIT 100';
			
			$result=mysql_query($query) or die ('Error: '.mysql_error());
			
			$xml_output  = Xml::XmlHeader() . "\n";

			$array = array();

			$xml_output .= Xml::openElement('articles');
			while($row = mysql_fetch_assoc($result))
			{
				$array[] = $row['articleid'];
				$therow = $row['title'];
				$xml_output .= "\t" . Xml::element('article', array('id' => $row['articleid']), $therow);
			}
			$xml_output .= Xml::closeElement('articles');
			
			$query = 'UPDATE articles SET checkedout = NOW() WHERE articleid IN (' . implode(",", $array) . ')';
			$result=mysql_query($query) or die ('Error: '.mysql_error());
			
			echo $xml_output; 
			break;
		
		case 'stats':
		default:
		header("Content-type: text/html; charset=utf-8");
		
			Head('TypoScan - Stats');
			echo'<h2><a href="http://en.wikipedia.org/wiki/Wikipedia:TypoScan">TypoScan</a> Stats</h2>
		<table>
		<caption>Overview</caption>';
			//Number of articles in Database
			$query = "SELECT COUNT(articleid) AS noarticles FROM articles";
			$result=mysql_fetch_array(mysql_query($query));
			$totalArticles = $result['noarticles'];
			PrintTableRow("Number of Articles", $totalArticles);
			
			//Number of finished articles
			$query = "SELECT COUNT(articleid) AS nofinished FROM articles WHERE (finished = 1)";
			$result=mysql_fetch_array(mysql_query($query));
			$finishedArticles = $result['nofinished'];
			PrintTableRow("Number of Finished Articles", $finishedArticles);
			
			//Number of ignored articles
			$query = "SELECT COUNT(articleid) as noignored FROM articles WHERE (skipid > 0)";
			$result=mysql_fetch_array(mysql_query($query));
			$ignoredArticles = $result['noignored'];
			PrintTableRow("Number of Ignored Articles", $ignoredArticles);
			
			//Number of touched articles
			PrintTableRow("Number of Touched Articles", ($finishedArticles + $ignoredArticles));
			
			//Number of untouched articles
			PrintTableRow("Number of Untouched Articles", ($totalArticles - $finishedArticles - $ignoredArticles));
			
			//Percentage Completion
			PrintTableRow("Percentage Completion", 
				($totalArticles ? round( ((($finishedArticles + $ignoredArticles)/$totalArticles) * 100),2) : '0') .'%');
			
			//Number of currently checked out articles
			$query = "SELECT COUNT(articleid) AS nocheckedout FROM articles WHERE (checkedout >= DATE_SUB(NOW(), INTERVAL 2 HOUR)) AND (userid = 0)";
			$result=mysql_fetch_array(mysql_query($query));
			PrintTableRow("Number of Currently Checked Out Articles", $result['nocheckedout']);
			
			//Number of never checked out articles
			$query = "SELECT COUNT(articleid) AS nonevercheckedout FROM articles WHERE (checkedout = '0000-00-00 00:00:00')";
			$result=mysql_fetch_array(mysql_query($query));
			PrintTableRow("Number of Never Checked Out Articles", $result['nonevercheckedout']);
			
			//Number of ever checked out articles
			$query = "SELECT COUNT(articleid) AS noevercheckedout FROM articles WHERE (checkedout > '0000-00-00 00:00:00')";
			$result=mysql_fetch_array(mysql_query($query));
			PrintTableRow("Number of Ever Checked Out Articles", $result['noevercheckedout']);
				
			//Number of users
			$query = "SELECT COUNT(userid) AS nousers FROM users";
			$result=mysql_fetch_array(mysql_query($query));
			PrintTableRow("Number of Users", $result['nousers']);

			echo '</table>
			<p/>';
			
			//Number of finished/ignored by user
			$query = "SELECT SUM(finished) AS edits, SUM(skipid > 0) AS skips, username FROM articles a, users u WHERE (a.userid = u.userid) AND (a.userid > 0) GROUP BY a.userid ORDER BY edits DESC, skips DESC";
		
			echo '<table class="sortable">
	<caption>User Stats</caption>
<thead>
	<tr>
		<th scope="col" class="sortable">User</th>
		<th scope="col" class="sortable">Number of Saved Articles</th>
		<th scope="col" class="sortable">Number of Skipped Articles</th>
	</tr>
</thead>';
	
			$result=mysql_query($query);
			
			while($row = mysql_fetch_assoc($result))
			{
				echo '<tr><td>'. htmlspecialchars($row['username']) . '</td><td>' . $row['edits'] . '</td><td>' . $row['skips'] . '</td></tr>';
			}
			
			echo '</table>
			<p/>';
			
			//No of Ignores per reason
			$query = "SELECT COUNT(a.skipid) AS noskips, s.skipreason FROM articles a, skippedreason s WHERE (a.skipid = s.skipid) AND (a.skipid > 0) GROUP BY a.skipid ORDER BY noskips DESC";
			
			echo '<table class="sortable">
	<caption>Skipped per Reason</caption>
<thead>
	<tr>
		<th scope="col" class="sortable">Reason</th>
		<th scope="col" class="sortable">Number of Skipped</th>
	</tr>
</thead>';

			$result=mysql_query($query);
			
			while($row = mysql_fetch_assoc($result))
			{
				echo '<tr><td>'. htmlspecialchars($row['skipreason']) . '</td><td>' . $row['noskips'] . '</td></tr>';
			}

			echo '</table>
';
			Tail();
			break;
	}
	
	mysql_close($conn);
	
	function PrintTableRow($header, $data) {
	echo '	<tr>
		<th align="left" scope="row">'.$header.'</th><td>'.$data.'</td>
	</tr>
';
}

	function GetOrAddIgnoreReason($reason)
	{
		return GetOrAdd($reason, 'skipid', 'skipreason', 'skippedreason');
	}
	
	function GetOrAddUser($user)
	{
		return GetOrAdd($user, 'userid', 'username', 'users');
	}
	
	function GetOrAdd($data, $selectcol, $wherecol, $table)
	{
		$query = 'SELECT ' . $selectcol . ' FROM `' . $table .'` WHERE (' . $wherecol . ' = "' . mysql_escape_string($data) .'")';
		$result = mysql_query($query) or die ('Error: '.mysql_error() . '\nQuery: ' . $query);
		
		//echo $query;
			
		if( mysql_num_rows($result) == 1)
		{
			//echo 'Exists:' . mysql_result($result, 0);
			return mysql_result($result, 0);
		}
		else
		{
			$query = 'INSERT INTO ' . $table. '(' . $wherecol. ') VALUES("' . mysql_escape_string($data) .'")';
			$result = mysql_query($query) or die ('Error: '.mysql_error() . '\nQuery: ' . $query);;
			
			//echo 'Inserted: ' . mysql_result($result, 0);
			return GetOrAdd($data, $selectcol, $wherecol, $table);
		}
	}
	
	function PostRequired()
	{
		if ($_SERVER['REQUEST_METHOD'] != 'POST') ReturnError('method', 'This operation requires data to be posted');
	}
	
	function ReturnError(string $message, $error = false)
	{
		$attribs = array(array('status' => 'failed'));
		if ($error)
			$attribs['error'] = $error;

		echo Xml::XmlHeader() . Xml::element('operation', $attribs,  $message);
		die;
	}
	
	function RemoveNonInts($arr)
	{
		return array_filter($arr, 'is_int');
	}

