DROP TABLE IF EXISTS `articles`;
CREATE TABLE  `articles` (
  `articleid` int(10) unsigned NOT NULL auto_increment,
  `title` blob NOT NULL,
  `checkedout` datetime NOT NULL default '0000-00-00 00:00:00',
  `finished` tinyint(1) NOT NULL default '0',
  `skipid` int(10) NOT NULL default '0',
  `userid` int(10) unsigned NOT NULL default '0',
  `checkedin` datetime NOT NULL default '0000-00-00 00:00:00',
  `siteid` int(10) unsigned NOT NULL default '0',
  PRIMARY KEY  (`articleid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `skippedreason`;
CREATE TABLE `skippedreason` (
  `skipid` int(10) unsigned NOT NULL auto_increment,
  `skipreason` varchar(50) default NULL,
  PRIMARY KEY  (`skipid`)
)ENGINE=InnoDB DEFAULT CHARSET=utf8;

INSERT INTO `skippedreason`(`skipreason`) VALUES ('Clicked ignore'), ('No change'), ('Non-existent page'), ('No typo fixes');

DROP TABLE IF EXISTS `users`;
CREATE TABLE `users` (
  `userid` int(10) unsigned NOT NULL auto_increment,
  `username` varchar(50) default NULL,
  PRIMARY KEY (`userid`)
)ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `site`;
CREATE TABLE `site` (
  `siteid` int(10) unsigned NOT NULL auto_increment,
  `address` varchar(50) NOT NULL,
   PRIMARY KEY (`siteid`)
)ENGINE=InnoDB DEFAULT CHARSET=utf8;

INSERT INTO `site`(`address`) VALUES ('en.wikipedia.org');