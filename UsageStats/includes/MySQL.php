<?php
/*
(C) 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/

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

class DB {	
	private $mysqli; /* @var $mysqli mysqli */ // Hint for Zend Studio autocomplete, don't delete
	
	function db_connect() {
		// DEBUG MODE:
		mysqli_report(MYSQLI_REPORT_ALL);
		
		global $GlobalConfig, $mysqli;
		
		$mysqli = new mysqli($GlobalConfig->dbserver, $GlobalConfig->dbuser, $GlobalConfig->dbpass, "awb");
	
		if (!$mysqli) {
		    dead("Connect failed: " . $mysqli->connect_error());
		}
	}
	
	function db_disconnect() {
		global $mysqli;	
		$mysqli->close();
	}
	
	function db_mysql_query($query, $caller, $module = 'MySQL') {
		# by doing it in one routine, is easier to slot in debugging/logging later if need be
		global $mysqli;		
		return $mysqli->query("/* $module:$caller */ $query");
	}
	
	function add_usage_record($VerifyID) {
		// TODO: We should do a log also in case the code is fucked up?
		
		// AWB version
		$versionarray=explode(".", $_POST['Version']);		
		if (count($versionarray) != 4)
			dead("Didn't receive a valid AWB version identifier");
		$versionid = $this->get_or_add_lookup_record('lkpVersions', 'VersionID', "Major={$versionarray[0]}" .
			" AND Minor={$versionarray[1]} AND Build={$versionarray[2]} AND Revision={$versionarray[3]}", 
			'Major, Minor, Build, Revision', 
			"{$versionarray[0]}, {$versionarray[1]}, {$versionarray[2]}, {$versionarray[3]}");
		
		// Wiki and langcode
		switch ($_POST['Wiki']) {
			case "":
				dead("Received an empty sitename string");				
			case "custom":
				$langcode="CUS";
				break;				
			default:
				$langcode=$_POST['Language'];
				break;
		}
		
		// TODO: Here and AWB: Record the custom sitename, if that's considered acceptable to collect
			// we maybe ought to cache some of this stuff, e.g. Wikipedia EN, current AWB version, etc
		$wikiid=$this->get_or_add_lookup_record('lkpWikis', 'SiteID', "Site=\"{$_POST['Wiki']}\" AND ".
			"LangCode=\"{$langcode}\"", 'Site, LangCode', "\"{$_POST['Wiki']}\", \"{$langcode}\"");
		
		// Culture
		$culturearray=explode("-", $_POST['Culture']);
		if (count($culturearray) != 2)
			dead("Didn't receive a valid culture identifier");
		$cultureid = $this->get_or_add_lookup_record('lkpCultures', 'CultureID', 'Language=' .
			"\"{$culturearray[0]}\" AND Country=\"{$culturearray[1]}\"", 'Language, Country',
			"\"{$culturearray[0]}\", \"{$culturearray[1]}\"");
		
		// OS:
		$OSID = $this->get_or_add_lookup_record('lkpOS', 'OSID', "OS=\"{$_POST['OS']}\"", 
			'OS', "\"{$_POST['OS']}\"");
			
		// Framework:
		$frameworkid=$this->get_or_add_lookup_record('lkpFramework', 'FrameworkID', "Framework=\"{$_POST['Framework']}\"",
			'Framework', "\"{$_POST['Framework']}\"");
			
		// Debug:
		switch($_POST['Debug']) {
			case 'Y':
				$debug=1;
				break;
			case 'N':
				$debug=0;
				break;
			default:
				dead("Didn't receive a valid debug property");
		}
			
		// Number of saves:
		if ($_POST['Saves'] == "") dead("No edit counter received");
		
		// Query string:
		$query = "INSERT INTO sessions (DateTime, Version, Debug, Saves, Site, Culture, OS, Framework";
		$query2 = ') SELECT "' . self::get_mysql_utc_stamp() . "\",  {$versionid}, {$debug}, {$_POST['Saves']}, {$wikiid}, ".
			"{$cultureid}, {$OSID}, {$frameworkid}";
			
		// User (may be null):
		if ($_POST['User'] != "") {
			$userid = $this->get_or_add_lookup_record('lkpUsers', 'UserID', "User=\"{$_POST['User']}\"",
				'User', "\"{$_POST['User']}\"");
			$query+=", User"; $query2+=$_POST['User'];
		}

		$result = $this->db_mysql_query($query.$query2, 'add_usage_record') || dead("Query error");
		
		// TODO: Plugins
	}
	
	private function get_or_add_lookup_record($table, $autoid, $lookupquery, $insertfields, $insertvalues) {
		$query = "SELECT {$autoid} FROM {$table} WHERE {$lookupquery}";
	
		$result = $this->db_mysql_query($query, 'get_or_add_lookup_record');  /* @var $result mysqli_result */
		
		if($result->num_rows == 1) {
			$row = $result->fetch_row();
			$retval=$row[0];
		} else {
			global $mysqli;
			$this->db_mysql_query("INSERT INTO {$table} ({$insertfields}) SELECT {$insertvalues}", 
				'get_or_add_lookup_record');
			$retval = $mysqli->insert_id;
		}
		
		$result->free();
		return $retval;
	}
	
	static function get_mysql_utc_stamp() {
		return gmdate("Y-m-d H:i:s", time());
	}
}

?>