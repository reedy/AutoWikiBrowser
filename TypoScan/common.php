<?php

	if($_SERVER['QUERY_STRING'] == 'source')
	{
		$path = '/' . $_SERVER['PHP_SELF'];
		$path = substr($path, strrpos($path, '/') + 1);
		header("Content-Type:text/html; charset=utf-8");
		echo "<h1>{$path}</h1><br/>";
		highlight_file($path);
		die;
	}
	
	require_once('Xml.php');
	
	function DisableCaching()
	{
		header('Cache-Control: no-cache, no-store, must-revalidate'); //HTTP/1.1
		header('Expires: Sun, 01 Jul 2005 00:00:00 GMT');
		header('Pragma: no-cache'); //HTTP/1.0
	}

	function Head($title="TypoScan")
	{
		?><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" lang="en" xml:lang="en">
<head>
	<title><?php echo $title; ?></title>
	<style type="text/css" media="all">@import "main.css";</style>
	<script src="sorttable.js" type="text/javascript"></script>
</head>
<body><?php
	}
	
	function Tail()
	{ ?>	<p>
	<span style="float:right;"><a href="?source">View source</a></span>
	<a href="http://validator.w3.org/check?uri=referer"><img
		src="http://www.w3.org/Icons/valid-xhtml10-blue"
		alt="Valid XHTML 1.0 Transitional" height="31" width="88" /></a>
	</p>
</body>
</html><?php
	}
