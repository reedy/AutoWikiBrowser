<?php

if ( isset( $_SERVER ) && array_key_exists( 'REQUEST_METHOD', $_SERVER ) ) {
	// require_once('common.php');
	die ( "This is a command-line script\n" );
}

if ( !isset( $argv[1] ) && !isset( $argv[2] ) ) {
	die ( "Parameters: <filename> <wiki>\n" );
}

echo "Connecting to MySQL...\n";
require_once( 'typo-db.php' );
require_once( 'common.php' );

// TODO:Need better parameter handling
$filename = $argv[1];
$siteid = GetOrAddSite( $argv[2] );

echo "Site ID: " . $siteid . "\n";

echo "Reading file {$filename}...\n";
$f = fopen( $filename, 'r' ) or die;

echo "Init complete, importing...\n";

$i = 0;
while ( !feof( $f ) ) {
	$name = trim( fgets( $f ) );
	if ( empty( $name ) ) {
		continue;
	}
	$i++;
	$q = "INSERT INTO articles (title, siteid) VALUES ('" . mysql_escape_string( $name ) . "', '" . $siteid . "')";
	mysql_query( $q ) or die;
	// echo $name . "\n";
	if ( $i % 10 == 0 ) {
		echo $i . "\n';
	}
}

echo "Success!\n";