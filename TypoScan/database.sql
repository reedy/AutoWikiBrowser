DROP TABLE IF EXISTS `typoscan`.`articles`;
CREATE IF NOT EXISTS TABLE  `typoscan`.`articles` (
  `articleid` int(10) unsigned NOT NULL auto_increment,
  `title` blob NOT NULL,
  `checkedout` datetime NOT NULL default '0000-00-00 00:00:00',
  `finished` tinyint(1) NOT NULL default '0',
  PRIMARY KEY  (`articleid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;