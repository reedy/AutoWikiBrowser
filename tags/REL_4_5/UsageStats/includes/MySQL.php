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
	private $logID = 0;
	
	// database connection
	function db_connect() {
		// DEBUG MODE:
		//mysqli_report(MYSQLI_REPORT_ALL);
		
		global $GlobalConfig, $mysqli;
		
		$mysqli = new mysqli($GlobalConfig->dbserver, $GlobalConfig->dbuser, $GlobalConfig->dbpass, "awb");
	
		if (!$mysqli) {
		    dead("Connect failed: " . $mysqli->connect_error());
		} else {
			$this->db_mysql_query('SET NAMES utf8', 'db_connect');
		}
	}
	
	function db_disconnect() {
		global $mysqli;	
		$mysqli->close();
	}
	
	// logging	
	function init_log($operation) {
		global $mysqli, $logID;
		
		$this->db_mysql_query('INSERT INTO log (DateTime, Operation, POST, ScriptMajor, ScriptMinor) SELECT "' . self::get_mysql_utc_stamp() .
			"\", {$operation}, \"".print_r($_POST, true).'", '.MAJOR.', '.MINOR, 'init_log');
		$logID = $mysqli->insert_id;
	}
	
	private function log_success($sessionID) {
		global $logID;		
		($logID != 0) && $this->db_mysql_query("UPDATE awb.log SET SuccessYN=1, SessionID={$sessionID} 
		WHERE log.LogID={$logID} LIMIT 1", 'log_success');
	}
	
	function log_error($msg) {	
		global $logID;
		($logID != 0) &&
			$this->db_mysql_query("UPDATE awb.log SET SuccessYN=0, Message='{$msg}' WHERE log.LogID={$logID} LIMIT 1", 'log_error');
	}
	
	// querying
	function db_mysql_query($query, $caller, $module = 'MySQL') {
		# by doing it in one routine, is easier to slot in debugging/logging later if need be
		global $mysqli;
		($retval = $mysqli->query("/* $module:$caller */ $query")) || dead("Query error: {$query}\n");
		return $retval;
	}
	
	function db_mysql_query_single_row($query, $caller, $module = 'MySQL') {
		$result = $this->db_mysql_query($query, $caller, $module);
		$retval = $result->fetch_assoc();
		$result->close();
		return $retval;
	}
	
	// add/update usage stats
	function add_usage_record($VerifyID) {
		global $mysqli;
		
		// AWB version
		$versionarray=explode(".", $_POST['Version']);		
		if (count($versionarray) != 4)
			dead("Didn't receive a valid AWB version identifier");
		$versionid = $this->get_or_add_lookup_record('lkpVersions', 'VersionID', "Major={$versionarray[0]}" .
			" AND Minor={$versionarray[1]} AND Build={$versionarray[2]} AND Revision={$versionarray[3]}", 
			'Major, Minor, Build, Revision', 
			"{$versionarray[0]}, {$versionarray[1]}, {$versionarray[2]}, {$versionarray[3]}");
		
		// Wiki and langcode
		switch ($_POST['Wiki'])
		{
			case '':
				dead("Received an empty sitename string");
			case 'wowwiki.com':
				$wiki='www.wowwiki.com';
				break;
			case 'localhost':
			case '127.0.0.1':
				$wiki='localhost';
				break;
			default:
				$wiki=$_POST['Wiki'];
		}
				
		switch ($wiki)
		{
			case 'species':
			case 'commons':
			case 'meta':
				$langcode='CUS'; /* HACK! TODO: Once the next compulsory AWB release after 4.3 is out, this can be replaced with $_POST['Language'],
		as AWB has been fixed to send "CUS" for these sites */
				break;		
			default:
				if ($_POST['Language'] == "simple")
					$langcode = "sim";
				else		
					$langcode = $_POST['Language'];
		}

		// we maybe ought to cache some of this stuff, e.g. Wikipedia EN, current AWB version, etc
		$wikiid=$this->get_or_add_lookup_record('lkpWikis', 'SiteID', "Site=\"{$wiki}\" AND ".
			"LangCode=\"{$langcode}\"", 'Site, LangCode', "\"{$wiki}\", \"{$langcode}\"");
		
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
		$query = 'INSERT INTO sessions (DateTime, Version, Debug, Saves, Site, Culture, OS, TempKey';
		$query2 = ') SELECT "' . self::get_mysql_utc_stamp() . "\",  {$versionid}, {$debug}, {$_POST['Saves']}, {$wikiid}, ".
			"{$cultureid}, {$OSID}, {$VerifyID}";
			
		// User (may be null):
		if ($_POST['User'] != "") {
			$userid = $this->get_or_add_userid();
			$query.=", User"; $query2.=", $userid";
		}

		$this->db_mysql_query($query.$query2, 'add_usage_record');
		$sessionid = $mysqli->insert_id;
		//$result->free(); // threw an error (and yes I had $result=), perhaps because we added a record and therefore don't actually have a recordset to clear?
		
		// Plugins:
		$this->GetPluginsData($sessionid);
				
		$this->log_success($sessionid);
		return $sessionid;
	}
	
	function update_usage_record() {
		$this->init_log(2);
		$this->verify_repeat_caller();
		
		// User (will usually be null):
		if ($_POST['User'] != "") {
			$userid = $this->get_or_add_userid();
			$this->db_mysql_query("UPDATE sessions SET User={$userid} WHERE sessions.SessionID = {$_POST['RecordID']} LIMIT 1",
				'update_usage_record');
		}
		
		($_POST['Saves'] > 0) && 
			$this->db_mysql_query("UPDATE sessions SET Saves = {$_POST['Saves']} WHERE sessions.SessionID = {$_POST['RecordID']} LIMIT 1",
			'update_usage_record'); // Saves can be empty if the only change was loading a new plugin
		$this->GetPluginsData($_POST['RecordID']);
		$this->log_success($_POST['RecordID']);
	}
	
	// helper routines
	private function get_or_add_userid() {
		return $this->get_or_add_lookup_record('lkpUsers', 'UserID', "User=\"{$_POST['User']}\"", 'User', "\"{$_POST['User']}\"");
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
	
	private function GetPluginsData($session) {
		if ($_POST['PluginCount'] == "") dead("No PluginCount received");
		
		for ($i = 1; $i <= $_POST['PluginCount']; $i++) { // 1-based
			$pluginname=$_POST["P{$i}N"];
			$plugintype=$_POST["P{$i}T"];
			$pluginid=$this->get_or_add_lookup_record('lkpPlugins', 'PluginID', "Plugin=\"{$pluginname}\"", 
			   'Plugin, PluginType', "\"{$pluginname}\", {$plugintype}");
			
			$versionarray=explode(".", $_POST["P{$i}V"]);		
			if (count($versionarray) != 4)
				dead("Didn't receive a valid AWB version identifier");
				
			$this->db_mysql_query('INSERT INTO plugins (SessionID, PluginID, Major, Minor, Build, Revision) SELECT ' .
			   "{$session}, {$pluginid}, {$versionarray[0]}, {$versionarray[1]}, {$versionarray[2]}, {$versionarray[3]}",
			   'add_usage_record');
		}		
	}
	
	private function verify_repeat_caller() {
		$row = $this->db_mysql_query_single_row("SELECT Count(sessions.SessionID) AS Count FROM sessions GROUP BY 
			sessions.SessionID, sessions.TempKey HAVING (((sessions.SessionID)={$_POST['RecordID']}) AND
			((sessions.TempKey)={$_POST['Verify']}))", 'verify_repeat_caller');
		($row['Count'] == '1') || dead("Client verification failed!");
	}
	
	private static function get_mysql_utc_stamp() {
		return gmdate("Y-m-d H:i:s", time());
	}	
	
	// reusable queries:
	function no_of_sessions_and_saves ()	{
		return $this->db_mysql_query_single_row('SELECT COUNT(s.sessionid) AS nosessions, IFNULL(SUM(s.saves),0) AS totalsaves FROM sessions s', 'no_of_sessions_and_saves');
	}
	
	function username_count() {
		return $this->db_mysql_query_single_row('SELECT Count(*) AS usercount FROM lkpUsers WHERE ((Not (lkpUsers.User)="<Withheld>"))', 'username_count') ;
	}
	
	function unique_username_count() {
		return $this->db_mysql_query_single_row('SELECT Count(*) AS UniqueUsersCount
FROM (SELECT sessions.User
FROM (sessions INNER JOIN lkpUsers ON sessions.User = lkpUsers.UserID) INNER JOIN lkpWikis ON sessions.Site = lkpWikis.SiteID
WHERE ((Not (lkpUsers.User)="<Withheld>"))
GROUP BY sessions.User, lkpWikis.Site, lkpWikis.LangCode) AS UniqueUsers', 'unique_username_count');
	}
	
	function plugin_count() {
		return $this->db_mysql_query_single_row(' SELECT COUNT( * ) AS Plugins FROM lkpPlugins', 'plugin_count') ;
	}
	
	function wiki_count() {
		return $this->db_mysql_query_single_row(' SELECT COUNT( * ) AS Wikis FROM lkpWikis', 'wiki_count') ;
	}
	
	function sites() {
		return $this->db_mysql_query('SELECT Count(sessions.SessionID) AS CountOfSessionID, Sum(sessions.Saves) AS SumOfSaves, lkpWikis.Site, lkpWikis.LangCode
FROM sessions INNER JOIN lkpWikis ON sessions.Site = lkpWikis.SiteID
GROUP BY lkpWikis.Site, lkpWikis.LangCode
HAVING (((Sum(sessions.Saves))>49))
ORDER BY Sum(sessions.Saves) DESC , lkpWikis.Site, lkpWikis.LangCode', 'sites');
	}
	
	function OSs($timelimited = false) {
		$query='SELECT lkpOS.OS, Sum( sessions.Saves ) AS SumOfSaves, Count( sessions.SessionID ) AS CountOfSessionID
			FROM sessions INNER JOIN lkpOS ON sessions.OS = lkpOS.OSID ';
		($timelimited) && $query .= 'WHERE DATE_SUB( CURDATE( ) , INTERVAL 30 DAY ) <= sessions.DateTime ';
		$query .= 'GROUP BY sessions.OS, lkpOS.OS	ORDER BY lkpOS.OS';
		return $this->db_mysql_query($query, 'OSs');
	}
	
	function cultures() {
		return $this->db_mysql_query('SELECT lkpCultures.Language, lkpCultures.Country, Sum(sessions.Saves) AS SumOfSaves
FROM sessions INNER JOIN lkpCultures ON sessions.Culture = lkpCultures.CultureID
GROUP BY lkpCultures.Language, lkpCultures.Country
ORDER BY lkpCultures.Language, lkpCultures.Country', 'cultures');
	}
		
	function plugins() {
		return $this->db_mysql_query('SELECT lkpPlugins.Plugin, lkpPlugins.PluginType FROM lkpPlugins GROUP BY lkpPlugins.Plugin ORDER BY lkpPlugins.Plugin', 'plugins');
	}
	
	function busiest_user() {
		return $this->db_mysql_query_single_row('SELECT lkpWikis.Site, Sum(sessions.Saves) AS SumOfSaves,
(CASE lkpWikis.LangCode WHEN "CUS" THEN "N/A" WHEN "WIK" THEN "N/A" ELSE lkpWikis.LangCode END) AS LangCode
FROM (sessions INNER JOIN lkpUsers ON sessions.User = lkpUsers.UserID) INNER JOIN lkpWikis ON sessions.Site = lkpWikis.SiteID
WHERE ((Not (lkpUsers.User)="<Withheld>"))
GROUP BY lkpWikis.Site, lkpWikis.LangCode, sessions.User
ORDER BY Sum(sessions.Saves) DESC LIMIT 1', 'busiest_user');
	}
	
	function record_count() {
		return $this->db_mysql_query_single_row('SELECT SUM(Counts.Count) AS RecordCount FROM(
			SELECT COUNT(*) AS Count FROM sessions
			UNION ALL SELECT COUNT(*) FROM log
			UNION ALL SELECT COUNT(*) FROM plugins
			UNION ALL SELECT COUNT(*) FROM lkpCultures
			UNION ALL SELECT COUNT(*) FROM lkpOS
			UNION ALL SELECT COUNT(*) FROM lkpPlugins
			UNION ALL SELECT COUNT(*) FROM lkpUsers
			UNION ALL SELECT COUNT(*) FROM lkpVersions
			UNION ALL SELECT COUNT(*) FROM lkpWikis
		) AS Counts', 'record_count');
	}
	
	function errors() {
		return $this->db_mysql_query_single_row('SELECT Count(*) AS Errors FROM log WHERE (((log.SuccessYN)=0))', 'errors');
	}
	
	function errors_fixed() {
		return $this->db_mysql_query_single_row('SELECT Count(*) AS Errors FROM log WHERE (((log.SuccessYN)=0) AND (log.ErrorFixed=0))', 'errors_fixed');
	}
	
	function missing_usernames_count() {
		return $this->db_mysql_query_single_row('SELECT Count(sessions.SessionID) AS MissingUsernames 
		FROM sessions WHERE (((sessions.User) Is Null))', 'missing_usernames_count');
	}
}

?>