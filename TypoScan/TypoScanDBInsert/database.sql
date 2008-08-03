DROP TABLE IF EXISTS `typoscan`.`articles`;
CREATE TABLE  `typoscan`.`articles` (
  `articleid` int(10) unsigned NOT NULL auto_increment,
  `title` blob NOT NULL,
  `checkedout` datetime NOT NULL default '0000-00-00 00:00:00',
  `finished` tinyint(1) NOT NULL default '0',
  `skipid` int(10) NOT NULL default '0',
  `userid` int(10) unsigned NOT NULL default '0',
  `checkedin` datetime NOT NULL default '0000-00-00 00:00:00',
  PRIMARY KEY  (`articleid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `typoscan`.`skippedreason`;
CREATE TABLE `typoscan`.`skippedreason` (
  `skipid` int(10) unsigned NOT NULL auto_increment,
  `skipreason` varchar(50) default NULL,
  PRIMARY KEY  (`skipid`)
)ENGINE=InnoDB DEFAULT CHARSET=utf8;

INSERT INTO `typoscan`.`skippedreason`(`skipreason`) VALUES ('Clicked ignore'), ('No change'), ('Non-existent page'), ('No typo fixes');

DROP TABLE IF EXISTS `typoscan`.`users`;
CREATE TABLE `typoscan`.`users` (
  `userid` int(10) unsigned NOT NULL auto_increment,
  `username` varchar(50) default NULL,
  PRIMARY KEY (`userid`)
)ENGINE=InnoDB DEFAULT CHARSET=utf8;