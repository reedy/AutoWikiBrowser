<?php

if (isset($_SERVER) && array_key_exists('REQUEST_METHOD', $_SERVER)) die ('This is a command-line script');

require_once('typo-db.php');

$conn = mysql_connect($dbserver, $dbuser, $dbpass); 
mysql_select_db($database, $conn) or die;

if (isset($argv[1])) 
	$filename = $argv[1];
else
	$filename = 'import.txt';
	
$f = fopen($filename, 'r') or die;

echo "Init complete, importing...\n";

while(!feof($f))
{
	$name = trim(fgets($f));
	$q = "INSERT INTO articles (title) VALUES ('" . mysql_escape_string($name) . "')";
	mysql_query($q) or die;
	//echo $name . "\n";
}

echo 'Success!';