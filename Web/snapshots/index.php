<?php
//ini_set('display_errors', '1');

?>
<!DOCTYPE html>
<head>
	<title>AutoWikiBrowser SVN Snapshots</title>
	<style type="text/css" media="all">@import "main.css";</style>
	<script src="sorttable.js" type="text/javascript"></script>
</head>
<body> 

<h2>AutoWikiBrowser SVN Snapshots</h2>
Please note that version 4.6.2.0 is a legacy release for old Wiki installations where they have no API.
<br />
This is an unsupported version in most usage. In most cases, you should be using the latest version, which is the stable supported version.
<br /><br />
<table>
<caption>Files</caption>

<?php
$string = '';
foreach ( glob( "*.zip" ) as $zip ) {
  $string = "<tr>
    <td><center><a href=$zip>$zip</a></center></td>
  </tr>" . $string;
} 

echo $string;

?></table>
<br />

<p>If you have any problems with the versions, and have to report bugs, 
please include the numbers following 'rev' in the filename. Thanks!</p>

<br />

<p>Maintained by Reedy.<br />
Any problems, contact him on IRC in #autowikibrowser on <a 
href='irc://irc.freenode.net'> irc.freenode.net</a>
or via en.wikipedia @ <a 
href='http://en.wikipedia.org/wiki/User:Reedy'>User:Reedy</a></p>
</html>
