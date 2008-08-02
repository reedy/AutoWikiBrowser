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
			$user = $_POST['user'];
			
			if (!empty($articles))
			{
				$query = 'UPDATE articles SET finished = 1, checkedin = NOW(), user = "' . $user . '" WHERE articleid IN (' . $articles . ')';
				$result=mysql_query($query) or die ('Error: '.mysql_error());
				
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
		
			break;
	}
	
	mysql_close($conn);
?>