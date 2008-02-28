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

class DB {	
	private $mysqli; /* @var $mysqli mysqli */ // Hint for Zend Studio autocomplete, don't delete
	
	function db_connect() {
		// DEBUG MODE:
		//mysqli_report(MYSQLI_REPORT_ALL);
		
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
		($retval = $mysqli->query("/* $module:$caller */ $query")) || dead("Query error: {$query}\n");
		return $retval;
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
		if ($_POST['Wiki'] == "")
				dead("Received an empty sitename string");		

		// we maybe ought to cache some of this stuff, e.g. Wikipedia EN, current AWB version, etc
		$wikiid=$this->get_or_add_lookup_record('lkpWikis', 'SiteID', "Site=\"{$_POST['Wiki']}\" AND ".
			"LangCode=\"{$_POST['Language']}\"", 'Site, LangCode', "\"{$_POST['Wiki']}\", \"{$_POST['Language']}\"");
		
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
		$query = "INSERT INTO sessions (DateTime, Version, Debug, Saves, Site, Culture, OS, TempKey";
		$query2 = ') SELECT "' . self::get_mysql_utc_stamp() . "\",  {$versionid}, {$debug}, {$_POST['Saves']}, {$wikiid}, ".
			"{$cultureid}, {$OSID}, {$VerifyID}";
			
		// User (may be null):
		if ($_POST['User'] != "") {
			$userid = $this->get_or_add_lookup_record('lkpUsers', 'UserID', "User=\"{$_POST['User']}\"",
				'User', "\"{$_POST['User']}\"");
			$query.=", User"; $query2.=", $userid";
		}

		$this->db_mysql_query($query.$query2, 'add_usage_record');
		global $mysqli;
		$retval = $mysqli->insert_id;
		//$result->free(); // threw an error (and yes I had $result=), perhaps because we added a record and therefore don't actually have a recordset to clear?
		
		// Plugins:
		if ($_POST['PluginCount'] == "") dead("No PluginCount received");
		for ($i = 1; $i <= $_POST['PluginCount']; $i++) { // 1-based
			$pluginname=$_POST["P{$i}N"];
			//echo "P1N: {$_POST["P1N"]}\npluginname var: {$pluginname}\n";
			$pluginid=$this->get_or_add_lookup_record('lkpPlugins', 'PluginID', "Plugin=\"{$pluginname}\"", 
			   'Plugin', "\"{$pluginname}\"");
			
			$versionarray=explode(".", $_POST["P{$i}V"]);		
			if (count($versionarray) != 4)
				dead("Didn't receive a valid AWB version identifier");
				
			$this->db_mysql_query('INSERT INTO plugins (SessionID, PluginID, Major, Minor, Build, Revision) SELECT ' .
			   "{$retval}, {$pluginid}, {$versionarray[0]}, {$versionarray[1]}, {$versionarray[2]}, {$versionarray[3]}",
			   'add_usage_record');
		}
		
		return $retval;
	}
	
	function no_of_sessions_and_saves ()	{
		$retval = $this->db_mysql_query("SELECT COUNT(s.sessionid) AS nosessions, SUM(s.saves) AS totalsaves FROM sessions s", 'stats', 'STATS');
		return mysqli_fetch_array($retval, MYSQL_ASSOC);
	}
	
	function unique_username_count() {
		$retval = $this->db_mysql_query("SELECT COUNT(DISTINCT u.User) AS usercount FROM lkpUsers u", 'stats', 'STATS') ;
		return mysqli_fetch_array($retval, MYSQL_ASSOC);
	}
	
	function plugin_count() {
		$retval = $this->db_mysql_query("SELECT COUNT(DISTINCT PluginID) as pluginno FROM plugins", 'stats', 'STATS') ;
		return mysqli_fetch_array($retval, MYSQL_ASSOC);
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