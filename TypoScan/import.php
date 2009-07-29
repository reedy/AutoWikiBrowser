<?php

if (isset($_SERVER) && array_key_exists('REQUEST_METHOD', $_SERVER))
{
	//require_once('common.php');
	die ('This is a command-line script');
}

echo "Connecting to MySQL...\n";
require_once('typo-db.php');

if (isset($argv[1]))
	$filename = $argv[1];
else
	$filename = 'import.txt';
	
require_once('common.php');

//TODO:Need better parameter handling
if (!isset($argv[2]))
	die ('Site needed\n');

$siteid = GetOrAddSite($argv[2]);
	
echo "Site ID: " . $siteid;

echo "Reading file {$filename}...\n";	
$f = fopen($filename, 'r') or die;

echo "Init complete, importing...\n";

while(!feof($f))
{
	$name = trim(fgets($f));
	if (empty($name))
		continue;
	$q = "INSERT INTO articles (title, siteid) VALUES ('" . mysql_escape_string($name) . "', '" . $siteid . "')";
	mysql_query($q) or die;
	//echo $name . "\n";
}

echo 'Success!';