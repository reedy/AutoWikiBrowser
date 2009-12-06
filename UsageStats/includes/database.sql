SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";

-- --------------------------------------------------------

--
-- Stand-in structure for view `FriendlyLog`
--
CREATE TABLE IF NOT EXISTS `FriendlyLog` (
`LogID` int(10) unsigned
,`Operation` tinyint(3) unsigned
,`DateTime` datetime
,`SuccessYN` varchar(3)
,`ErrorFixed` varchar(3)
,`Message` longtext
,`SessionID` int(10) unsigned
,`User` varchar(255)
,`Saves` mediumint(8) unsigned
,`ScriptMajor` tinyint(4)
,`ScriptMinor` tinyint(4)
);
-- --------------------------------------------------------

--
-- Stand-in structure for view `FriendlySessions`
--
CREATE TABLE IF NOT EXISTS `FriendlySessions` (
`SessionID` int(10) unsigned
,`DateTime` datetime
,`User` varchar(255)
,`Saves` mediumint(8) unsigned
,`Site` varchar(256)
,`LangCode` varchar(3)
,`Version` varbinary(25)
,`DebugBuild` varchar(3)
,`Plugins` bigint(21)
,`Culture` varchar(5)
);
-- --------------------------------------------------------

--
-- Table structure for table `lkpCultures`
--

CREATE TABLE IF NOT EXISTS `lkpCultures` (
  `CultureID` smallint(5) unsigned NOT NULL auto_increment,
  `Language` varchar(2) NOT NULL,
  `Country` varchar(2) NOT NULL,
  UNIQUE KEY `Unique` (`Language`,`Country`),
  KEY `CultureID` (`CultureID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `lkpOS`
--

CREATE TABLE IF NOT EXISTS `lkpOS` (
  `OSID` mediumint(8) unsigned NOT NULL auto_increment,
  `OS` varchar(100) NOT NULL,
  PRIMARY KEY  (`OSID`),
  UNIQUE KEY `OS` (`OS`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `lkpPlugins`
--

CREATE TABLE IF NOT EXISTS `lkpPlugins` (
  `PluginID` mediumint(8) unsigned NOT NULL auto_increment,
  `Plugin` varchar(255) character set utf8 NOT NULL,
  `PluginType` smallint(5) unsigned NOT NULL default '0',
  PRIMARY KEY  (`PluginID`),
  UNIQUE KEY `Plugin` (`Plugin`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `lkpUsers`
--

CREATE TABLE IF NOT EXISTS `lkpUsers` (
  `UserID` mediumint(8) unsigned NOT NULL auto_increment,
  `User` varchar(255) character set utf8 NOT NULL,
  PRIMARY KEY  (`UserID`),
  UNIQUE KEY `User` (`User`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `lkpVersions`
--

CREATE TABLE IF NOT EXISTS `lkpVersions` (
  `VersionID` mediumint(8) unsigned NOT NULL auto_increment,
  `Major` tinyint(3) unsigned NOT NULL,
  `Minor` smallint(5) unsigned NOT NULL,
  `Build` smallint(5) unsigned NOT NULL,
  `Revision` smallint(5) unsigned NOT NULL,
  PRIMARY KEY  (`VersionID`),
  UNIQUE KEY `Unique` (`Major`,`Minor`,`Build`,`Revision`),
  KEY `Major` (`Major`),
  KEY `Minor` (`Minor`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `lkpWikis`
--

CREATE TABLE IF NOT EXISTS `lkpWikis` (
  `SiteID` mediumint(8) unsigned NOT NULL auto_increment,
  `Site` varchar(256) NOT NULL,
  `LangCode` varchar(3) NOT NULL COMMENT '2 letter language code, or CUS for custom, WIK for Wikia',
  PRIMARY KEY  (`SiteID`),
  UNIQUE KEY `Unique` (`Site`,`LangCode`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `log`
--

CREATE TABLE IF NOT EXISTS `log` (
  `LogID` int(10) unsigned NOT NULL auto_increment,
  `Operation` tinyint(3) unsigned NOT NULL COMMENT '1=First contact, 2=Subsequent , 3=Test',
  `DateTime` datetime NOT NULL,
  `ScriptMajor` tinyint(4) NOT NULL,
  `ScriptMinor` tinyint(4) NOT NULL,
  `POST` text character set utf8 COMMENT 'The _POST variable',
  `SuccessYN` tinyint(3) unsigned NOT NULL default '0' COMMENT '0=no, 1=yes',
  `ErrorFixed` tinyint(4) NOT NULL default '0' COMMENT '0=error not fixed or no error,1=fixed',
  `Message` text character set utf8,
  `SessionID` int(10) unsigned default NULL,
  PRIMARY KEY  (`LogID`),
  KEY `SessionID` (`SessionID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COMMENT='Log requests, in case something goes wrong';

-- --------------------------------------------------------

--
-- Table structure for table `plugins`
--

CREATE TABLE IF NOT EXISTS `plugins` (
  `SessionID` int(10) unsigned NOT NULL,
  `PluginID` mediumint(8) unsigned NOT NULL,
  `Major` tinyint(3) unsigned NOT NULL,
  `Minor` smallint(5) unsigned NOT NULL,
  `Build` smallint(5) unsigned NOT NULL,
  `Revision` smallint(5) unsigned NOT NULL,
  PRIMARY KEY  (`SessionID`,`PluginID`),
  KEY `Major` (`Major`),
  KEY `Minor` (`Minor`),
  KEY `PluginID` (`PluginID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `sessions`
--

CREATE TABLE IF NOT EXISTS `sessions` (
  `SessionID` int(10) unsigned NOT NULL auto_increment,
  `TempKey` tinyint(3) unsigned NOT NULL COMMENT 'Key for AWB to update record',
  `DateTime` datetime NOT NULL,
  `Version` mediumint(8) unsigned NOT NULL,
  `Debug` tinyint(3) unsigned NOT NULL,
  `Saves` mediumint(8) unsigned NOT NULL,
  `Site` mediumint(8) unsigned NOT NULL,
  `Culture` smallint(5) unsigned NOT NULL,
  `User` mediumint(8) unsigned default NULL,
  `OS` mediumint(8) unsigned NOT NULL,
  PRIMARY KEY  (`SessionID`),
  KEY `Site` (`Site`),
  KEY `Culture` (`Culture`),
  KEY `User` (`User`),
  KEY `OS` (`OS`),
  KEY `Version` (`Version`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Structure for view `FriendlyLog`
--
DROP TABLE IF EXISTS `FriendlyLog`;

CREATE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `awb`.`FriendlyLog` AS select `awb`.`log`.`LogID` AS `LogID`,`awb`.`log`.`Operation` AS `Operation`,`awb`.`log`.`DateTime` AS `DateTime`,if(`awb`.`log`.`SuccessYN`,_utf8'Yes',_utf8'No') AS `SuccessYN`,if(`awb`.`log`.`ErrorFixed`,_utf8'Yes',_utf8'No') AS `ErrorFixed`,ifnull(`awb`.`log`.`Message`,_utf8'') AS `Message`,`awb`.`log`.`SessionID` AS `SessionID`,ifnull(`awb`.`lkpUsers`.`User`,_utf8'<Not recorded>') AS `User`,`awb`.`sessions`.`Saves` AS `Saves`,`awb`.`log`.`ScriptMajor` AS `ScriptMajor`,`awb`.`log`.`ScriptMinor` AS `ScriptMinor` from (`awb`.`log` left join (`awb`.`sessions` left join `awb`.`lkpUsers` on((`awb`.`sessions`.`User` = `awb`.`lkpUsers`.`UserID`))) on((`awb`.`log`.`SessionID` = `awb`.`sessions`.`SessionID`))) order by `awb`.`log`.`LogID`;

-- --------------------------------------------------------

--
-- Structure for view `FriendlySessions`
--
DROP TABLE IF EXISTS `FriendlySessions`;

CREATE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `awb`.`FriendlySessions` AS select `awb`.`sessions`.`SessionID` AS `SessionID`,`awb`.`sessions`.`DateTime` AS `DateTime`,ifnull(`awb`.`lkpUsers`.`User`,_utf8'<Not recorded>') AS `User`,`awb`.`sessions`.`Saves` AS `Saves`,`awb`.`lkpWikis`.`Site` AS `Site`,`awb`.`lkpWikis`.`LangCode` AS `LangCode`,concat(`awb`.`lkpVersions`.`Major`,_utf8'.',`awb`.`lkpVersions`.`Minor`,_utf8'.',`awb`.`lkpVersions`.`Build`,_utf8'.',`awb`.`lkpVersions`.`Revision`) AS `Version`,if(`awb`.`sessions`.`Debug`,_utf8'Yes',_utf8'No') AS `DebugBuild`,count(`awb`.`plugins`.`PluginID`) AS `Plugins`,concat(`awb`.`lkpCultures`.`Language`,_latin1'-',`awb`.`lkpCultures`.`Country`) AS `Culture` from (((((`awb`.`sessions` left join `awb`.`plugins` on((`awb`.`sessions`.`SessionID` = `awb`.`plugins`.`SessionID`))) left join `awb`.`lkpVersions` on((`awb`.`sessions`.`Version` = `awb`.`lkpVersions`.`VersionID`))) left join `awb`.`lkpUsers` on((`awb`.`sessions`.`User` = `awb`.`lkpUsers`.`UserID`))) left join `awb`.`lkpWikis` on((`awb`.`sessions`.`Site` = `awb`.`lkpWikis`.`SiteID`))) left join `awb`.`lkpCultures` on((`awb`.`sessions`.`Culture` = `awb`.`lkpCultures`.`CultureID`))) group by `awb`.`sessions`.`SessionID`,`awb`.`sessions`.`DateTime`,`awb`.`lkpUsers`.`User`,`awb`.`sessions`.`Saves`,`awb`.`lkpWikis`.`Site`,`awb`.`lkpWikis`.`LangCode`,`awb`.`lkpVersions`.`Major`,`awb`.`lkpVersions`.`Minor`,`awb`.`lkpVersions`.`Build`,`awb`.`lkpVersions`.`Revision`,`awb`.`sessions`.`Debug`,`awb`.`lkpCultures`.`Language`,`awb`.`lkpCultures`.`Country` order by `awb`.`sessions`.`SessionID`;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `log`
--
ALTER TABLE `log`
  ADD CONSTRAINT `log_ibfk_1` FOREIGN KEY (`SessionID`) REFERENCES `sessions` (`SessionID`) ON DELETE CASCADE;

--
-- Constraints for table `plugins`
--
ALTER TABLE `plugins`
  ADD CONSTRAINT `plugins_ibfk_3` FOREIGN KEY (`SessionID`) REFERENCES `sessions` (`SessionID`) ON DELETE CASCADE,
  ADD CONSTRAINT `plugins_ibfk_4` FOREIGN KEY (`PluginID`) REFERENCES `lkpPlugins` (`PluginID`);

--
-- Constraints for table `sessions`
--
ALTER TABLE `sessions`
  ADD CONSTRAINT `sessions_ibfk_1` FOREIGN KEY (`Version`) REFERENCES `lkpVersions` (`VersionID`),
  ADD CONSTRAINT `sessions_ibfk_2` FOREIGN KEY (`Site`) REFERENCES `lkpWikis` (`SiteID`),
  ADD CONSTRAINT `sessions_ibfk_3` FOREIGN KEY (`Culture`) REFERENCES `lkpCultures` (`CultureID`),
  ADD CONSTRAINT `sessions_ibfk_4` FOREIGN KEY (`User`) REFERENCES `lkpUsers` (`UserID`),
  ADD CONSTRAINT `sessions_ibfk_5` FOREIGN KEY (`OS`) REFERENCES `lkpOS` (`OSID`);
