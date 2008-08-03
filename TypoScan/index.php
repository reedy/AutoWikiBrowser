<?php
	//prevent caching
	header('Cache-Control: no-cache, no-store, must-revalidate'); //HTTP/1.1
	header('Expires: Sun, 01 Jul 2005 00:00:00 GMT');
	header('Pragma: no-cache'); //HTTP/1.0

	$dbserver = 'mysql.reedyboy.net';
	$dbuser = 'typoscan';
	$dbpass = '256iumD3';
	$database = 'typoscan';
	
	$conn=mysql_connect($dbserver, $dbuser, $dbpass); 
	mysql_select_db($database, $conn);
	
	switch($_GET['action'])
	{
		case 'finished':
			header("Content-type: text/html; charset=utf-8"); 
			$articles = $_POST['articles'];
			$skippedarticles = $_POST['skipped'];
			$skippedreason = $_POST['skipreason'];
			$user = $_POST['user'];
			
			$articlesempty = empty($articles);
			$skippedempty = empty($skippedarticles);
			
			if (!$articlesempty || !$skippedempty)
			{
				if (!$articlesempty)
				{
					$query = 'UPDATE articles SET finished = 1, checkedin = NOW(), userid = "' . GetOrAddUser($user) . '" WHERE articleid IN (' . $articles . ')';
					$result=mysql_query($query) or die ('Error: '.mysql_error() . '\nQuery: ' . $query);
				}
				
				if (!$skippedempty)
				{
					//echo $skippedarticles;
					//echo $skippedreason;
					//Deal with Ignored
					
					$skippedarticles = explode(",", $skippedarticles);
					$skippedreason = explode(",", $skippedreason);
					
					for ($i=0; $i < count($skippedarticles); $i++)
					{
						$query = 'UPDATE articles SET skipid = "' . GetOrAddIgnoreReason($skippedreason[$i]) . '", checkedin = NOW(), user = "' . $user . '" WHERE (articleid = "' . $skippedarticles[$i] . '")';
						//echo $query;
					    $result=mysql_query($query) or die ('Error: '.mysql_error() . '\nQuery: ' . $query);
					}
				}
				
				echo "<html><body>Articles Updated</body></html>";
			}
			else
				echo "<html><body>Articles have to be posted to the script</body></html>";
		break;
		
		case 'displayarticles':
			header("Content-type: text/xml; charset=utf-8"); 
		
			$query = 'SELECT articleid, title FROM articles WHERE (checkedout < DATE_SUB(NOW(), INTERVAL 2 HOUR)) AND (finished = 0) LIMIT 100';
			
			$result=mysql_query($query) or die ('Error: '.mysql_error());
			
			$xml_output  = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n";
			$xml_output .= "<articles>\n";

			$array = array();

			while($row = mysql_fetch_assoc($result))
			{
				$array[] = $row['articleid'];
				$therow = $row['title'];
				$xml_output .= "\t<article id='{$row['articleid']}'>";
				// Escaping illegal characters
				$therow = str_replace("&", "&amp;", $therow);
				$therow = str_replace("<", "&lt;", $therow);
				$therow = str_replace(">", "&gt;", $therow);
				$therow = str_replace("\"", "&quot;", $therow);
				$xml_output .= utf8_encode($therow) . "</article>\n";
			}
			
			$query = 'UPDATE articles SET checkedout = NOW() WHERE articleid IN (' . implode(",", $array) . ')';
			$result=mysql_query($query) or die ('Error: '.mysql_error());
			
			$xml_output .= "</articles>";

			echo $xml_output; 
			break;
		
		case 'stats':
		default:
		header("Content-type: text/html; charset=utf-8");
		
			echo'<html>
			<head>
			<title>TypoScan Stats</title>
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
			border-bottom-width : 0px;
			font-weight : bold;
		}
		
		TH.sortable {
			cursor : pointer;
		}
	</style>
		<script src="sorttable.js" type="text/javascript"></script>
	</head>
				<table>
				<caption>Overview</caption>';
			//Number of articles in Database
			$query = "SELECT COUNT(articleid) AS noarticles FROM articles";
			$result=mysql_fetch_array(mysql_query($query));
			$totalArticles = $result['noarticles'];
			PrintTableRow("Number of Articles", $totalArticles);
			
			//Number of currently checked out articles
			$query = "SELECT COUNT(articleid) AS nocheckedout FROM articles WHERE (checkedout >= DATE_SUB(NOW(), INTERVAL 2 HOUR)) AND (finished = 0)";
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
			
			//Number of finished articles
			$query = "SELECT COUNT(articleid) AS nofinished FROM articles WHERE (finished = 1)";
			$result=mysql_fetch_array(mysql_query($query));
			$finishedArticles = $result['nofinished'];
			PrintTableRow("Number of Finished Articles", $finishedArticles);
			
			//Number of ignored articles
			$query = "SELECT COUNT(articleid) as noignored FROM articles WHERE (skipid > 0)";
			$result=mysql_fetch_array(mysql_query($query));
			$unfinishedArticles = $result['noignored'];
			PrintTableRow("Number of Ignored Articles", $unfinishedArticles);
			
			//Number of unfinished articles
			PrintTableRow("Number of Unfinished Articles", ($totalArticles - $finishedArticles));
			
			//Number of users
			$query = "SELECT COUNT(userid) AS nousers FROM users";
			$result=mysql_fetch_array(mysql_query($query));
			PrintTableRow("Number of Users", $result['nousers']);

			echo '</table>
			<p/>';
			
			//Number of finished/ignored by user
			$query = "SELECT SUM(finished) AS edits, SUM(skipid > 0) AS skips, username FROM articles a, users u WHERE (a.userid = u.userid) AND (a.userid > 0) GROUP BY a.userid ORDER BY edits DESC, skips DESC";
		
			echo '<table class="sortable">
	<caption>Edits by User</caption>
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
				echo '<tr><td>'. $row['username'] . '</td><td>' . $row['edits'] . '</td><td>' . $row['skips'] . '</td></tr>';
			}
			
			echo '</table>
			</p>';
			
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
				echo '<tr><td>'. $row['skipreason'] . '</td><td>' . $row['noskips'] . '</td></tr>';
			}

			echo '</table>
			</html>';			
			break;
	}
	
	mysql_close($conn);
	
	function PrintTableRow($header, $data) {
	echo '
	
	<tr>
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
		$query = 'SELECT "' . $selectcol .'"FROM "' . $table .'"WHERE ("' . $wherecolcol.'" = "' . $data .'")';
		$result = mysql_query($query) or die ('Error: '.mysql_error() . '\nQuery: ' . $query);
		
		//echo $query;
			
		if( mysql_num_rows($result) == 1)
		{
			//echo 'Exists:' . mysql_result($result, 0);
			return mysql_result($result, 0);
		}
		else
		{
			$query = 'INSERT INTO "' . $table. '"("' . $wherecol.'") VALUES("' . $data .'")';
			$result = mysql_query($query) or die ('Error: '.mysql_error() . '\nQuery: ' . $query);;
			
			//echo 'Inserted: ' . mysql_result($result, 0);
			return mysql_result($result, 0);
		}
	}
?>
